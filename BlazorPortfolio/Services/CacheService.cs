using Microsoft.Extensions.Caching.Memory;

namespace BlazorPortfolio.Services;

/// <summary>
/// Server-side IMemoryCache wrapper. Replaces the old JS sessionStorage approach
/// so cache hits actually reduce Neon DB round-trips.
/// </summary>
public class CacheService(IMemoryCache cache)
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

    public Task<T?> GetAsync<T>(string key)
    {
        cache.TryGetValue(key, out T? value);
        return Task.FromResult(value);
    }

    public Task SetAsync<T>(string key, T data)
    {
        cache.Set(key, data, Ttl);
        return Task.CompletedTask;
    }

    public Task RemoveAsync(string key)
    {
        cache.Remove(key);
        return Task.CompletedTask;
    }

    public static class Keys
    {
        public const string Experiences          = "portfolio_experiences";
        public const string Skills               = "portfolio_skills";
        public const string Projects             = "portfolio_projects";
        public const string Profile              = "portfolio_profile";
        public const string GitHubProfile        = "gh_profile";
        public const string GitHubPinned         = "gh_pinned";
        public const string GitHubContributions  = "gh_contributions";
        public const string GitHubContribTotal   = "gh_contrib_total";
        public const string GitHubAllRepos       = "gh_all_repos";
    }
}
