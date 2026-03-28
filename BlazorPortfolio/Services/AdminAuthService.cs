using BlazorPortfolio.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorPortfolio.Services;

/// <summary>
/// Admin session backed by DB credentials (BCrypt) + server-side session cookie.
/// Auth state survives circuit reconnects and page reloads.
/// </summary>
public class AdminAuthService(
    IDbContextFactory<AppDbContext> dbFactory,
    IHttpContextAccessor httpContextAccessor)
{
    private const string SessionKey = "admin_authenticated";

    public bool IsAuthenticated =>
        httpContextAccessor.HttpContext?.Session.GetString(SessionKey) == "1";

    public async Task<bool> LoginAsync(string username, string password)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var user = await db.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return false;

        if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash)) return false;

        httpContextAccessor.HttpContext?.Session.SetString(SessionKey, "1");
        return true;
    }

    public void Logout()
    {
        httpContextAccessor.HttpContext?.Session.Clear();
    }
}
