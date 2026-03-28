namespace BlazorPortfolio.Services;

/// <summary>
/// Simple in-memory admin session. Credentials are stored in appsettings.json.
/// </summary>
public class AdminAuthService
{
    private bool _isAuthenticated;

    public bool IsAuthenticated => _isAuthenticated;

    public bool Login(string username, string password, IConfiguration config)
    {
        var adminUser = config["Admin:Username"];
        var adminPass = config["Admin:Password"];
        _isAuthenticated = username == adminUser && password == adminPass;
        return _isAuthenticated;
    }

    public void Logout() => _isAuthenticated = false;
}
