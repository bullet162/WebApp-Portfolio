using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using BlazorPortfolio.Models;
using Microsoft.EntityFrameworkCore;
using BlazorPortfolio.Data;
using System.Net.Http.Headers;

using Microsoft.Extensions.Caching.Memory;

namespace BlazorPortfolio.Services;

public class GeminiService(
    IConfiguration config, 
    ILogger<GeminiService> logger, 
    HttpClient http,
    IMemoryCache cache)
{
    private readonly string? _apiKey = config["Gemini:ApiKey"];
    private readonly string _model = config["Gemini:Model"] ?? "gemini-2.5-flash";
    private readonly bool _enabled = config.GetValue<bool>("AiEnrichment:Enabled", true);
    private readonly bool _requireConsent = config.GetValue<bool>("AiEnrichment:RequireConsent", true);
    private readonly bool _requireAdminApproval = config.GetValue<bool>("AiEnrichment:RequireAdminApproval", true);
    private readonly bool _enableModelDiscovery = config.GetValue<bool>("Gemini:EnableModelDiscovery", true);
    private readonly int _modelDiscoveryCacheHours = config.GetValue<int>("Gemini:ModelDiscoveryCacheHours", 12);
    private readonly int _maxModelFallbackAttempts = config.GetValue<int>("Gemini:MaxModelFallbackAttempts", 5);


    public async Task<DeveloperProfileEnrichment> EnrichProfileAsync(CollaborationRequest req, Action<string>? onProgress = null, CancellationToken ct = default)
    {
        var enrichment = new DeveloperProfileEnrichment
        {
            CollaborationRequestId = req.Id,
            Status = EnrichmentStatus.Pending,
            ModelUsed = _model,
            PromptVersion = "v1.0"
        };

        if (!_enabled)
        {
            enrichment.Status = EnrichmentStatus.Failed;
            enrichment.ErrorMessage = "AI Enrichment is currently disabled in configuration.";
            return enrichment;
        }

        if (_requireConsent && !req.AiEnrichmentConsent)
        {
            enrichment.Status = EnrichmentStatus.Failed;
            enrichment.ErrorMessage = "User did not grant consent for AI enrichment.";
            return enrichment;
        }

        if (string.IsNullOrEmpty(_apiKey))
        {
            enrichment.Status = EnrichmentStatus.Failed;
            enrichment.ErrorMessage = "Gemini API key is missing.";
            return enrichment;
        }

        try
        {
            // 1. Gather context from public links
            logger.LogInformation("Enrichment: gathering context from URLs...");
            onProgress?.Invoke("Gathering context from Portfolio & GitHub URLs...");
            var context = await GatherContextAsync(req, onProgress, ct);
            
            // 2. Build Prompt
            var prompt = BuildPrompt(req, context);
            logger.LogInformation("Enrichment: prompt built ({Len} chars). Getting models...", prompt.Length);
            onProgress?.Invoke($"Context gathered. Built {prompt.Length}-char prompt. Getting models...");

            // 3. Call Gemini with Fallbacks
            var modelsToTry = await GetModelsToTryAsync(onProgress);
            onProgress?.Invoke($"Starting generation with {modelsToTry.Count} possible models...");
            GeminiResponseDto? result = null;
            string? errorMsg = null;
            
            foreach (var model in modelsToTry)
            {
                try
                {
                    onProgress?.Invoke($"Calling model: {model}...");
                    result = await CallGeminiAsync(prompt, model, ct);
                    if (result != null)
                    {
                        enrichment.ModelUsed = model;
                        break;
                    }
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.Unauthorized || ex.StatusCode == System.Net.HttpStatusCode.Forbidden)
                {
                    logger.LogError("Gemini API authentication failed (401/403). Cannot proceed with fallbacks.");
                    errorMsg = "AI enrichment failed due to authentication issue. Check API key.";
                    break; // Fail fast
                }
                catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound || ex.StatusCode == System.Net.HttpStatusCode.BadRequest)
                {
                    logger.LogWarning("Gemini model '{Model}' failed ({StatusCode}). {Error}", model, ex.StatusCode, ex.Message);
                    onProgress?.Invoke($"Model {model} failed: {ex.Message}. Trying next...");
                    // Continue to next model
                }
                catch (Exception ex)
                {
                    var msg = ex.Message.Length > 150 ? ex.Message.Substring(0, 150) + "..." : ex.Message;
                    onProgress?.Invoke($"Model {model} failed unexpectedly: {msg}");
                    logger.LogWarning(ex, "Gemini model '{Model}' failed unexpectedly. Trying next fallback...", model);
                }
            }
            
            if (result == null)
            {
                enrichment.Status = EnrichmentStatus.Failed;
                enrichment.ErrorMessage = errorMsg ?? "AI enrichment failed after trying available Gemini text-generation models. Check API key, quota, or model availability.";
                onProgress?.Invoke("FAILED: " + enrichment.ErrorMessage);
            }
            else
            {
                enrichment.Status = EnrichmentStatus.Ready;
                enrichment.GeneratedHeadline = result.Headline;
                enrichment.GeneratedSummary = result.Summary;
                enrichment.GeneratedSkillsJson = JsonSerializer.Serialize(result.Skills);
                enrichment.GeneratedProjectHighlightsJson = JsonSerializer.Serialize(result.ProjectHighlights);
                enrichment.GeneratedCollaborationInterestsJson = JsonSerializer.Serialize(result.CollaborationInterests);
                enrichment.ConfidenceScore = result.OverallConfidence;
                enrichment.GeneratedAt = DateTime.UtcNow;
                enrichment.ErrorMessage = string.Join(" | ", result.Warnings);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to enrich profile for {ReqId}", req.Id);
            enrichment.Status = EnrichmentStatus.Failed;
            enrichment.ErrorMessage = ex.Message;
        }

        return enrichment;
    }

    private async Task<string> GatherContextAsync(CollaborationRequest req, Action<string>? onProgress = null, CancellationToken ct = default)
    {
        var sb = new StringBuilder();
        // Create a timeout for the entire gathering process, linked to the main cancellation token
        using var gatheringCts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        gatheringCts.CancelAfter(TimeSpan.FromSeconds(8)); 
        
        var tasks = new List<Task<(string Label, string? Content)>>();

        if (!string.IsNullOrEmpty(req.PortfolioUrl))
        {
            onProgress?.Invoke("-> Fetching Portfolio: " + req.PortfolioUrl);
            tasks.Add(FetchLabeledAsync("Portfolio", req.PortfolioUrl, gatheringCts.Token));
        }
        if (!string.IsNullOrEmpty(req.GitHubUrl))
        {
            onProgress?.Invoke("-> Fetching GitHub: " + req.GitHubUrl);
            tasks.Add(FetchLabeledAsync("GitHub", req.GitHubUrl, gatheringCts.Token));
        }

        if (!tasks.Any()) return "";

        try
        {
            var results = await Task.WhenAll(tasks);
            foreach (var (label, content) in results)
            {
                if (!string.IsNullOrEmpty(content))
                {
                    onProgress?.Invoke($"✓ Successfully fetched context from {label}.");
                    sb.AppendLine($"{label}: {content}");
                }
                else
                {
                    onProgress?.Invoke($"⚠ No usable content from {label}.");
                }
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("Context gathering timed out — proceeding with form data only.");
            onProgress?.Invoke("⚠ Context gathering timed out or was canceled.");
        }

        return sb.ToString();
    }

    private async Task<(string Label, string? Content)> FetchLabeledAsync(string label, string url, CancellationToken ct)
    {
        try
        {
            var content = await SafeFetchUrlAsync(url, ct);
            return (label, content);
        }
        catch { return (label, null); }
    }

    private async Task<string?> SafeFetchUrlAsync(string url, CancellationToken ct = default)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri)) return null;
        if (uri.Scheme != "http" && uri.Scheme != "https") return null;
        
        // Reject private/internal IPs
        if (uri.IsLoopback || uri.Host.Equals("localhost", StringComparison.OrdinalIgnoreCase)) return null;

        try
        {
            using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
            cts.CancelAfter(TimeSpan.FromSeconds(5));
            var request = new HttpRequestMessage(HttpMethod.Get, url);
            request.Headers.UserAgent.ParseAdd("Mozilla/5.0 (compatible; JhersonPortfolioBot/1.0)");
            
            using var response = await http.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cts.Token);
            if (!response.IsSuccessStatusCode) return null;

            var contentType = response.Content.Headers.ContentType?.MediaType;
            if (contentType == null || !contentType.Contains("html") && !contentType.Contains("text")) return null;

            // Limit response size (128KB — we only need 500 chars after strip)
            var bytes = await response.Content.ReadAsByteArrayAsync(cts.Token);
            if (bytes.Length > 128 * 1024) bytes = bytes.Take(128 * 1024).ToArray();

            var html = System.Text.Encoding.UTF8.GetString(bytes);
            
            // Basic Sanitization: Strip scripts and styles
            html = Regex.Replace(html, @"<(script|style|svg|img|iframe|link).*?>.*?</\1>", "", RegexOptions.Singleline | RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<.*?>", " "); // Strip remaining tags
            html = Regex.Replace(html, @"\s+", " "); // Collapse whitespace
            
            return html.Length > 800 ? html.Substring(0, 800) : html;
        }
        catch (Exception ex)
        {
            logger.LogWarning("Failed to fetch context from {Url}: {Msg}", url, ex.Message);
            return null;
        }
    }

    private string BuildPrompt(CollaborationRequest req, string context)
    {
        return $$"""
        Generate a developer profile for {{req.FirstName}} {{req.LastName}} ({{req.RoleTitle}}).
        Bio: {{req.Message}}
        Links: {{req.PortfolioUrl}}, {{req.GitHubUrl}}
        Context: {{context}}

        Return ONLY this JSON:
        {"headline":"max 80 chars","summary":"max 300 chars","skills":[{"name":"str","confidence":0.0}],"projectHighlights":[{"title":"str","description":"short"}],"collaborationInterests":["str"],"overallConfidence":0.0,"warnings":["str"]}

        Rules: max 5 skills, max 2 projects, max 2 interests. Be concise. No private info.
        """;
    }

    private async Task<List<string>> GetModelsToTryAsync(Action<string>? onProgress = null)
    {
        var models = new List<string>();
        var normalizedConfigured = _model.Replace("models/", "");
        
        // 1. Always try the configured model first
        models.Add(normalizedConfigured);
        
        // 2. Discover ALL available models via API
        if (_enableModelDiscovery)
        {
            try 
            {
                var discovered = await GetAvailableGenerateContentModelsAsync();
                if (discovered != null && discovered.Any())
                {
                    onProgress?.Invoke($"✓ Discovered {discovered.Count} models via API. Prioritizing best versions...");
                    
                    // Sort discovered models: 2.0 > 1.5-flash > 1.5-pro > others
                    var sorted = discovered
                        .OrderByDescending(m => m == normalizedConfigured)
                        .ThenByDescending(m => m.Contains("2.0"))
                        .ThenByDescending(m => m.Contains("flash") && !m.Contains("exp") && !m.Contains("preview"))
                        .ThenByDescending(m => m.Contains("pro") && !m.Contains("exp") && !m.Contains("preview"))
                        .ToList();

                    foreach (var d in sorted)
                    {
                        if (!models.Contains(d)) models.Add(d);
                    }
                }
                else
                {
                    onProgress?.Invoke("⚠ No models found via API discovery.");
                }
            }
            catch (Exception ex)
            {
                onProgress?.Invoke($"⚠ Discovery failed: {ex.Message.Substring(0, Math.Min(ex.Message.Length, 60))}...");
            }
        }

        // 3. Hardcoded safe fallbacks if discovery is empty or disabled
        var safeFallbacks = new[] { "gemini-2.0-flash-exp", "gemini-1.5-flash", "gemini-1.5-pro" };
        foreach (var f in safeFallbacks)
        {
            if (!models.Contains(f)) models.Add(f);
        }
        
        // Limit to max attempts to avoid infinite looping
        var finalSelection = models.Distinct().Take(_maxModelFallbackAttempts).ToList();
        logger.LogInformation("Enrichment fallback chain: {Models}", string.Join(" -> ", finalSelection));
        return finalSelection;
    }

    public async Task<List<string>?> GetAvailableGenerateContentModelsAsync(bool forceRefresh = false)
    {
        const string cacheKey = "Gemini:GenerateContentModels";
        
        if (!forceRefresh && cache.TryGetValue(cacheKey, out List<string>? cachedModels))
        {
            return cachedModels;
        }

        if (string.IsNullOrEmpty(_apiKey)) return null;

        var url = $"https://generativelanguage.googleapis.com/v1beta/models?key={_apiKey}";
        
        try
        {
            using var discoveryCts = new CancellationTokenSource(TimeSpan.FromSeconds(10));
            var response = await http.GetAsync(url, discoveryCts.Token);
            if (!response.IsSuccessStatusCode)
            {
                // Do not log the url since it contains the API key
                logger.LogWarning("Gemini model discovery request failed with status {Status}.", response.StatusCode);
                return null;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var modelsArray = doc.RootElement.GetProperty("models").EnumerateArray();
            
            var discovered = new List<string>();
            foreach (var modelElement in modelsArray)
            {
                var methods = modelElement.GetProperty("supportedGenerationMethods").EnumerateArray()
                                          .Select(x => x.GetString()).ToList();
                if (methods.Contains("generateContent"))
                {
                    var name = modelElement.GetProperty("name").GetString() ?? "";
                    name = name.Replace("models/", ""); // Normalize
                    discovered.Add(name);
                }
            }

            var sorted = discovered
                .OrderByDescending(m => m == _model.Replace("models/", ""))
                .ThenByDescending(m => m.Contains("flash") && !m.Contains("exp") && !m.Contains("preview"))
                .ThenByDescending(m => m.Contains("pro") && !m.Contains("exp") && !m.Contains("preview"))
                .ToList();

            cache.Set(cacheKey, sorted, TimeSpan.FromHours(_modelDiscoveryCacheHours));
            return sorted;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Exception during Gemini model discovery.");
            return null;
        }
    }

    private async Task<GeminiResponseDto?> CallGeminiAsync(string prompt, string modelName, CancellationToken ct = default)
    {
        var normalizedModel = modelName.StartsWith("models/") ? modelName : $"models/{modelName}";
        var url = $"https://generativelanguage.googleapis.com/v1beta/{normalizedModel}:generateContent?key={_apiKey}";
        
        var payload = new
        {
            contents = new[] { new { parts = new[] { new { text = prompt } } } },
            generationConfig = new { responseMimeType = "application/json", maxOutputTokens = 1024 }
        };

        logger.LogInformation("Calling Gemini model '{Model}' for content generation...", modelName);
        
        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(TimeSpan.FromSeconds(45));
        var response = await http.PostAsJsonAsync(url, payload, cts.Token);
        if (!response.IsSuccessStatusCode)
        {
            var errorBody = await response.Content.ReadAsStringAsync(cts.Token);
            logger.LogWarning("Gemini generation request failed for model '{Model}' with status {Status}. Response: {Body}", modelName, response.StatusCode, errorBody);
            throw new HttpRequestException($"Gemini API error {response.StatusCode}: {errorBody}", null, response.StatusCode);
        }

        var json = await response.Content.ReadAsStringAsync(cts.Token);
        using var doc = JsonDocument.Parse(json);
        var contentText = doc.RootElement.GetProperty("candidates")[0].GetProperty("content").GetProperty("parts")[0].GetProperty("text").GetString();

        logger.LogInformation("Gemini model '{Model}' responded successfully.", modelName);
        return JsonSerializer.Deserialize<GeminiResponseDto>(contentText ?? "{}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }

    private class GeminiResponseDto
    {
        public string Headline { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public List<SkillDto> Skills { get; set; } = new();
        public List<ProjectDto> ProjectHighlights { get; set; } = new();
        public List<string> CollaborationInterests { get; set; } = new();
        public double OverallConfidence { get; set; }
        public List<string> Warnings { get; set; } = new();
    }

    private class SkillDto { public string Name { get; set; } = string.Empty; public double Confidence { get; set; } }
    private class ProjectDto { public string Title { get; set; } = string.Empty; public string Description { get; set; } = string.Empty; }
}
