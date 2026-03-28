using Microsoft.JSInterop;
using System.Text.Json;

namespace BlazorPortfolio.Services;

public class CacheService(IJSRuntime js)
{
    private static readonly TimeSpan Ttl = TimeSpan.FromMinutes(30);

    private record CacheEntry<T>(T Data, DateTime StoredAt);

    public async Task<T?> GetAsync<T>(string key)
    {
        try
        {
            var raw = await js.InvokeAsync<string?>("portfolioCache.get", key);
            if (raw is null) return default;
            var entry = JsonSerializer.Deserialize<CacheEntry<T>>(raw);
            if (entry is null || DateTime.UtcNow - entry.StoredAt > Ttl) return default;
            return entry.Data;
        }
        catch { return default; }
    }

    public async Task SetAsync<T>(string key, T data)
    {
        try
        {
            var entry = new CacheEntry<T>(data, DateTime.UtcNow);
            var json = JsonSerializer.Serialize(entry);
            await js.InvokeVoidAsync("portfolioCache.set", key, json);
        }
        catch { }
    }

    public async Task RemoveAsync(string key)
    {
        try { await js.InvokeVoidAsync("portfolioCache.remove", key); }
        catch { }
    }

    public static class Keys
    {
        public const string Experiences = "portfolio_experiences";
        public const string Skills      = "portfolio_skills";
        public const string Projects    = "portfolio_projects";
        public const string Profile     = "portfolio_profile";
    }
}
