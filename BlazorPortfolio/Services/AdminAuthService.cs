using BlazorPortfolio.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorPortfolio.Services;

/// <summary>
/// Admin session backed by DB credentials (BCrypt) + a simple auth cookie
/// so login state survives Blazor circuit reconnects.
/// </summary>
public class AdminAuthService(
    IDbContextFactory<AppDbContext> dbFactory,
    IHttpContextAccessor httpContextAccessor)
{
    private const string CookieName = "admin_auth";

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.Request.Cookies.ContainsKey(CookieName) == true;

    public async Task<bool> LoginAsync(string username, string password)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var user = await db.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return false;
        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return false;

        httpContextAccessor.HttpContext?.Response.Cookies.Append(CookieName, "1", new CookieOptions
        {
            HttpOnly = true,
            Secure = true,
            SameSite = SameSiteMode.Lax,
            Expires = DateTimeOffset.UtcNow.AddHours(8)
        });

        return true;
    }

    public void Logout()
    {
        httpContextAccessor.HttpContext?.Response.Cookies.Delete(CookieName);
    }
}
