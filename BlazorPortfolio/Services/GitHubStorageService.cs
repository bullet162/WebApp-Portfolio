using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace BlazorPortfolio.Services;

public class GitHubStorageService
{
    private readonly IConfiguration _config;
    private readonly ILogger<GitHubStorageService> _logger;

    private readonly string? _owner;
    private readonly string? _repo;
    private readonly string? _branch;
    private readonly string? _basePath;
    private readonly string? _token;

    public GitHubStorageService(IConfiguration config, ILogger<GitHubStorageService> logger)
    {
        _config = config;
        _logger = logger;

        _owner = _config["GitHubStorage:Owner"] ?? "Jherson-Aguto";
        _repo = _config["GitHubStorage:Repo"] ?? "WebApp-Portfolio";
        _branch = _config["GitHubStorage:Branch"] ?? "main";
        _basePath = _config["GitHubStorage:BasePath"] ?? "public-assets/resume";

        // Read token from environment variables, secure settings, or fallback to general GitHub config
        _token = Environment.GetEnvironmentVariable("GITHUB_STORAGE_TOKEN") 
                 ?? _config["GITHUB_STORAGE_TOKEN"] 
                 ?? _config["GitHub:Resume"]
                 ?? _config["GitHub:Token"];
    }

    public bool IsConfigured =>
        !string.IsNullOrWhiteSpace(_owner) &&
        !string.IsNullOrWhiteSpace(_repo) &&
        !string.IsNullOrWhiteSpace(_token);

    public string? Owner => _owner;
    public string? Repo => _repo;
    public string? Branch => _branch;
    public string? BasePath => _basePath;

    /// <summary>
    /// Uploads a file's byte array to the configured GitHub repository.
    /// Returns the raw githubusercontent URL on success.
    /// </summary>
    public async Task<string> UploadResumeAsync(byte[] fileBytes, string safeFileName)
    {
        if (!IsConfigured)
        {
            throw new InvalidOperationException(
                "GitHub Storage is not fully configured. Please configure GitHubStorage:Owner, GitHubStorage:Repo, and the GITHUB_STORAGE_TOKEN environment variable."
            );
        }

        // Clean path formatting: remove leading/trailing slashes
        var cleanedBasePath = _basePath?.Trim('/') ?? "public-assets/resume";
        var path = $"{cleanedBasePath}/{safeFileName}";

        var url = $"https://api.github.com/repos/{_owner}/{_repo}/contents/{path}";
        var base64Content = Convert.ToBase64String(fileBytes);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("BlazorPortfolio/1.0");
        client.DefaultRequestHeaders.Accept.ParseAdd("application/vnd.github+json");
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        
        // Add API Version header as recommended by GitHub
        client.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");

        var payload = new
        {
            message = $"Upload resume: {safeFileName}",
            content = base64Content,
            branch = _branch
        };

        var json = JsonSerializer.Serialize(payload);
        using var requestContent = new StringContent(json, Encoding.UTF8, "application/json");

        _logger.LogInformation("Uploading resume to GitHub repository {Owner}/{Repo} at path: {Path}", _owner, _repo, path);

        var response = await client.PutAsync(url, requestContent);
        var body = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            // Security safeguard: Do not log the token or authorization headers!
            _logger.LogError("GitHub file upload failed. Status Code: {StatusCode}. Response Body: {Body}", response.StatusCode, body);
            throw new Exception($"GitHub upload failed with status code: {response.StatusCode}");
        }

        _logger.LogInformation("Successfully uploaded resume to GitHub.");

        // Return the raw CDN URL for public embedding & download
        return $"https://raw.githubusercontent.com/{_owner}/{_repo}/{_branch}/{path}";
    }
}
