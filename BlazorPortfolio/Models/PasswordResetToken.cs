namespace BlazorPortfolio.Models;

public class PasswordResetToken
{
    public int Id { get; set; }
    public int AdminUserId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool Used { get; set; }
}
