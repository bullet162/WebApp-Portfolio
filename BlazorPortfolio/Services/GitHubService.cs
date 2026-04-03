using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

namespace BlazorPortfolio.Services;

public class GitHubPinnedRepo
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string? HomepageUrl { get; set; }
    public string PrimaryLanguage { get; set; } = string.Empty;
    public string LanguageColor { get; set; } = "#ccc";
    public int Stars { get; set; }
    public int Forks { get; set; }
    public List<string> Topics { get; set; } = [];
    public int OpenIssues { get; set; }
    public string UpdatedAt { get; set; } = string.Empty;
}

public class GitHubContributionDay
{
    public int Count { get; set; }
    public string Date { get; set; } = string.Empty;
    public string Color { get; set; } = "#161b22";
    public int WeekIndex { get; set; }
    public int DayOfWeek { get; set; }
}

public class GitHubContributionWeek
{
    public List<GitHubContributionDay> Days { get; set; } = [];
}

public class GitHubProfile
{
    public string Login { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Bio { get; set; }
    public string? AvatarUrl { get; set; }
    public int PublicRepos { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
}

public class GitHubService(IConfiguration config, ILogger<GitHubService> logger)
{
    private readonly string? _token = config["GitHub:Token"];

    private HttpClient CreateClient()
    {
        var client = new HttpClient();
        client.DefaultRequestHeaders.UserAgent.ParseAdd("BlazorPortfolio/1.0");
        client.Timeout = TimeSpan.FromSeconds(30);
        if (!string.IsNullOrWhiteSpace(_token))
            client.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _token);
        return client;
    }

    // ── Public profile (REST — no token needed) ───────────────────────────────
    public async Task<GitHubProfile?> GetProfileAsync(string username)
    {
        try
        {
            using var client = CreateClient();
            var resp = await client.GetAsync($"https://api.github.com/users/{username}");
            if (!resp.IsSuccessStatusCode)
            {
                logger.LogWarning("GitHub profile fetch returned {Status}", resp.StatusCode);
                return null;
            }
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var r = doc.RootElement;
            return new GitHubProfile
            {
                Login       = r.Str("login") ?? username,
                Name        = r.Str("name") ?? username,
                Bio         = r.Str("bio"),
                AvatarUrl   = r.Str("avatar_url"),
                PublicRepos = r.Int("public_repos"),
                Followers   = r.Int("followers"),
                Following   = r.Int("following"),
            };
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "GetProfileAsync failed for {Username}", username);
            return null;
        }
    }

    // ── Pinned repos — GraphQL if token present, REST fallback otherwise ───────
    public async Task<List<GitHubPinnedRepo>> GetPinnedReposAsync(string username)
    {
        if (!string.IsNullOrWhiteSpace(_token))
        {
            var pinned = await FetchPinnedGraphQL(username);
            if (pinned.Count > 0) return pinned;
        }
        // Always fall back to REST so something shows even without a token
        return await GetTopReposFallbackAsync(username);
    }

    private async Task<List<GitHubPinnedRepo>> FetchPinnedGraphQL(string username)
    {
        var gql = new
        {
            query = """
                query($login: String!) {
                  user(login: $login) {
                    pinnedItems(first: 6, types: REPOSITORY) {
                      nodes {
                        ... on Repository {
                          name description url homepageUrl
                          isPrivate isArchived
                          stargazerCount forkCount
                          updatedAt
                          primaryLanguage { name color }
                          repositoryTopics(first: 5) { nodes { topic { name } } }
                        }
                      }
                    }
                  }
                }
                """,
            variables = new { login = username }
        };
        try
        {
            using var client = CreateClient();
            var body = new StringContent(JsonSerializer.Serialize(gql), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("https://api.github.com/graphql", body);
            if (!resp.IsSuccessStatusCode) return [];

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            if (!doc.RootElement.TryGetProperty("data", out var data) ||
                !data.TryGetProperty("user", out var user) ||
                user.ValueKind == JsonValueKind.Null) return [];

            var nodes = user.GetProperty("pinnedItems").GetProperty("nodes");
            var result = new List<GitHubPinnedRepo>();
            foreach (var node in nodes.EnumerateArray())
            {
                if (node.Bool("isPrivate") || node.Bool("isArchived")) continue;
                var lang = node.TryGetProperty("primaryLanguage", out var pl) &&
                           pl.ValueKind == JsonValueKind.Object ? pl : (JsonElement?)null;
                var topics = new List<string>();
                if (node.TryGetProperty("repositoryTopics", out var rt))
                    foreach (var t in rt.GetProperty("nodes").EnumerateArray())
                        topics.Add(t.GetProperty("topic").GetProperty("name").GetString() ?? "");
                result.Add(new GitHubPinnedRepo
                {
                    Name            = node.Str("name") ?? "",
                    Description     = node.Str("description") ?? "",
                    Url             = node.Str("url") ?? "",
                    HomepageUrl     = node.Str("homepageUrl"),
                    Stars           = node.Int("stargazerCount"),
                    Forks           = node.Int("forkCount"),
                    UpdatedAt       = node.Str("updatedAt") ?? "",
                    PrimaryLanguage = lang?.Str("name") ?? "",
                    LanguageColor   = lang?.Str("color") ?? "#ccc",
                    Topics          = topics
                });
            }
            return result;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "GraphQL pinned repos failed for {Username}", username);
            return [];
        }
    }

    // ── REST fallback: top starred public non-fork repos ─────────────────────
    private async Task<List<GitHubPinnedRepo>> GetTopReposFallbackAsync(string username)
    {
        try
        {
            using var client = CreateClient();
            var resp = await client.GetAsync(
                $"https://api.github.com/users/{username}/repos?sort=stars&per_page=30&type=public");
            if (!resp.IsSuccessStatusCode)
            {
                logger.LogWarning("REST repos fetch returned {Status}", resp.StatusCode);
                return [];
            }
            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            var result = new List<GitHubPinnedRepo>();
            foreach (var repo in doc.RootElement.EnumerateArray())
            {
                if (repo.Bool("fork") || repo.Bool("archived") || repo.Bool("private")) continue;
                result.Add(MapRestRepo(repo));
                if (result.Count >= 6) break;
            }
            logger.LogInformation("REST fallback returned {Count} repos for {Username}", result.Count, username);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "REST fallback failed for {Username}", username);
            return [];
        }
    }

    // ── All public repos ──────────────────────────────────────────────────────
    public async Task<List<GitHubPinnedRepo>> GetAllPublicReposAsync(string username)
    {
        try
        {
            using var client = CreateClient();
            var result = new List<GitHubPinnedRepo>();
            int page = 1;
            while (result.Count < 200)
            {
                var resp = await client.GetAsync(
                    $"https://api.github.com/users/{username}/repos?sort=updated&per_page=30&page={page}&type=public");
                if (!resp.IsSuccessStatusCode) break;
                using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
                var arr = doc.RootElement;
                if (arr.GetArrayLength() == 0) break;
                foreach (var repo in arr.EnumerateArray())
                {
                    if (repo.Bool("fork") || repo.Bool("archived") || repo.Bool("private")) continue;
                    result.Add(MapRestRepo(repo));
                }
                page++;
            }
            logger.LogInformation("GetAllPublicRepos returned {Count} repos for {Username}", result.Count, username);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "GetAllPublicReposAsync failed for {Username}", username);
            return [];
        }
    }

    private static GitHubPinnedRepo MapRestRepo(JsonElement repo) => new()
    {
        Name            = repo.Str("name") ?? "",
        Description     = repo.Str("description") ?? "",
        Url             = repo.Str("html_url") ?? "",
        HomepageUrl     = repo.Str("homepage"),
        Stars           = repo.Int("stargazers_count"),
        Forks           = repo.Int("forks_count"),
        OpenIssues      = repo.Int("open_issues_count"),
        UpdatedAt       = repo.Str("updated_at") ?? "",
        PrimaryLanguage = repo.Str("language") ?? "",
        LanguageColor   = "#ccc",
    };

    // ── Contribution calendar (GraphQL — requires token) ──────────────────────
    public async Task<(List<GitHubContributionWeek> Weeks, int Total)> GetContributionsAsync(string username)
    {
        if (string.IsNullOrWhiteSpace(_token))
        {
            logger.LogInformation("No GitHub token — contribution graph skipped");
            return ([], 0);
        }
        var now = DateTime.UtcNow;
        var gql = new
        {
            query = """
                query($login: String!, $from: DateTime!, $to: DateTime!) {
                  user(login: $login) {
                    contributionsCollection(from: $from, to: $to) {
                      contributionCalendar {
                        totalContributions
                        weeks {
                          contributionDays {
                            contributionCount date color weekday
                          }
                        }
                      }
                    }
                  }
                }
                """,
            variables = new
            {
                login = username,
                from  = now.AddDays(-364).ToString("yyyy-MM-ddT00:00:00Z"),
                to    = now.ToString("yyyy-MM-ddT23:59:59Z")
            }
        };
        try
        {
            using var client = CreateClient();
            var body = new StringContent(JsonSerializer.Serialize(gql), Encoding.UTF8, "application/json");
            var resp = await client.PostAsync("https://api.github.com/graphql", body);
            if (!resp.IsSuccessStatusCode) return ([], 0);

            using var doc = JsonDocument.Parse(await resp.Content.ReadAsStringAsync());
            if (!doc.RootElement.TryGetProperty("data", out var data) ||
                !data.TryGetProperty("user", out var user) ||
                user.ValueKind == JsonValueKind.Null) return ([], 0);

            var cal   = user.GetProperty("contributionsCollection").GetProperty("contributionCalendar");
            var total = cal.Int("totalContributions");
            var weeks = new List<GitHubContributionWeek>();
            int wi = 0;
            foreach (var week in cal.GetProperty("weeks").EnumerateArray())
            {
                var w = new GitHubContributionWeek();
                foreach (var day in week.GetProperty("contributionDays").EnumerateArray())
                    w.Days.Add(new GitHubContributionDay
                    {
                        Count     = day.Int("contributionCount"),
                        Date      = day.Str("date") ?? "",
                        Color     = day.Str("color") ?? "#161b22",
                        WeekIndex = wi,
                        DayOfWeek = day.Int("weekday"),
                    });
                weeks.Add(w);
                wi++;
            }
            return (weeks, total);
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "GetContributionsAsync failed for {Username}", username);
            return ([], 0);
        }
    }
}

// ── JsonElement extension helpers ─────────────────────────────────────────────
internal static class JsonElementExt
{
    public static string? Str(this JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.String
            ? v.GetString() : null;

    public static string? Str(this JsonElement? el, string prop) =>
        el.HasValue ? el.Value.Str(prop) : null;

    public static int Int(this JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.Number
            ? v.GetInt32() : 0;

    public static bool Bool(this JsonElement el, string prop) =>
        el.TryGetProperty(prop, out var v) && v.ValueKind == JsonValueKind.True;
}
