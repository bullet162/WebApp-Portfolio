using Resend;

namespace BlazorPortfolio.Services;

public class EmailService(IConfiguration config)
{
    public async Task SendPasswordResetAsync(string toEmail, string resetLink)
    {
        var allowed = (config["Resend:AllowedRecipients"] ?? "")
            .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (!allowed.Contains(toEmail, StringComparer.OrdinalIgnoreCase))
            return;

        IResend resend = ResendClient.Create(config["Resend:ApiKey"]!);

        await resend.EmailSendAsync(new EmailMessage
        {
            From = "onboarding@resend.dev",
            To = toEmail,
            Subject = "Reset your Portfolio Admin password",
            HtmlBody = $"""
                <div style="font-family: sans-serif; max-width: 600px; margin: auto;">
                    <h2>Password Reset Request</h2>
                    <p>We received a request to reset your admin password. Click the button below to proceed:</p>
                    <a href="{resetLink}" style="background-color: #000; color: #fff; padding: 12px 24px; text-decoration: none; border-radius: 5px; display: inline-block;">
                        Reset Password
                    </a>
                    <p style="margin-top: 20px; font-size: 12px; color: #666;">
                        This link will expire in <strong>30 minutes</strong>. If you didn't request this, no further action is needed.
                    </p>
                </div>
                """
        });
    }
}
