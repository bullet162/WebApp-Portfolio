using BlazorPortfolio.Data;
using Microsoft.EntityFrameworkCore;

namespace BlazorPortfolio.Services;

/// <summary>
/// Admin session backed by DB credentials (hashed with BCrypt).
/// </summary>
public class AdminAuthService(IDbContextFactory<AppDbContext> dbFactory)
{
    private bool _isAuthenticated;

    public bool IsAuthenticated => _isAuthenticated;

    public async Task<bool> LoginAsync(string username, string password)
    {
        await using var db = await dbFactory.CreateDbContextAsync();
        var user = await db.AdminUsers.FirstOrDefaultAsync(u => u.Username == username);
        if (user is null) return false;
        _isAuthenticated = BCrypt.Net.BCrypt.Verify(password, user.PasswordHash);
        return _isAuthenticated;
    }

    public void Logout() => _isAuthenticated = false;
}
