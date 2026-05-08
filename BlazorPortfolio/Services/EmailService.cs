using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Resend;
using BlazorPortfolio.Models;
using System.IO;

namespace BlazorPortfolio.Services;

public class EmailService(IConfiguration config, ILogger<EmailService> logger, IWebHostEnvironment env)
{
    private readonly string _templatePath = Path.Combine(env.ContentRootPath, "Templates", "Emails");

    // ── SMTP Methods (Gmail) ────────────────────────────────────────────────
    
    public async Task SendNetworkRequestReceivedAsync(CollaborationRequest req)
    {
        var placeholders = new Dictionary<string, string>
        {
            { "DeveloperName", req.FirstName },
            { "NetworkUrl", "https://jhersonaguto.dev/#network" },
            { "Year", DateTime.UtcNow.Year.ToString() }
        };

        var html = await LoadTemplateAsync("developer-network-request-received.html", placeholders);
        await SendSmtpEmailAsync(req.Email, "We've received your Developer Network request!", html);
    }

    public async Task SendAdminNewRequestAlertAsync(CollaborationRequest req)
    {
        var adminEmail = config["Smtp:AdminEmail"];
        if (string.IsNullOrEmpty(adminEmail)) return;

        var placeholders = new Dictionary<string, string>
        {
            { "DeveloperName", $"{req.FirstName} {req.LastName}" },
            { "Email", req.Email ?? "N/A" },
            { "RoleTitle", req.RoleTitle ?? "N/A" },
            { "PortfolioUrl", req.PortfolioUrl ?? "N/A" },
            { "GitHubUrl", req.GitHubUrl ?? "N/A" },
            { "Message", req.Message ?? "N/A" },
            { "AdminUrl", "https://jhersonaguto.dev/admin/connections" }
        };

        var html = await LoadTemplateAsync("developer-network-admin-notification.html", placeholders);
        await SendSmtpEmailAsync(adminEmail, $"[Admin] New Network Request: {req.FirstName}", html);
    }

    public async Task SendDeveloperApprovedAsync(CollaborationRequest req)
    {
        if (string.IsNullOrEmpty(req.Email)) return;

        var profileUrl = !string.IsNullOrEmpty(req.PublicSlug) 
            ? $"https://jhersonaguto.dev/network/{req.PublicSlug}"
            : "https://jhersonaguto.dev/#network";

        var placeholders = new Dictionary<string, string>
        {
            { "DeveloperName", req.FirstName },
            { "ProfileUrl", profileUrl },
            { "NetworkUrl", "https://jhersonaguto.dev/#network" }
        };

        var html = await LoadTemplateAsync("developer-network-approved.html", placeholders);
        await SendSmtpEmailAsync(req.Email, "Your Developer Network profile is live!", html);
    }

    public async Task SendDeveloperRejectedAsync(CollaborationRequest req)
    {
        if (string.IsNullOrEmpty(req.Email)) return;

        var placeholders = new Dictionary<string, string>
        {
            { "DeveloperName", req.FirstName }
        };

        var html = await LoadTemplateAsync("developer-network-rejected.html", placeholders);
        await SendSmtpEmailAsync(req.Email, "Update regarding your Developer Network request", html);
    }

    private async Task<string> LoadTemplateAsync(string filename, Dictionary<string, string> placeholders)
    {
        try
        {
            var path = Path.Combine(_templatePath, filename);
            if (!File.Exists(path))
            {
                logger.LogWarning("Email template not found: {Path}. Falling back to basic text.", path);
                return "Template missing. Please check admin panel.";
            }

            var html = await File.ReadAllTextAsync(path);
            foreach (var kv in placeholders)
            {
                html = html.Replace($"{{{{{kv.Key}}}}}", kv.Value ?? "");
            }
            return html;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error loading email template {Filename}", filename);
            return "Error loading template.";
        }
    }

    private async Task SendSmtpEmailAsync(string? toEmail, string subject, string htmlBody)
    {
        if (string.IsNullOrEmpty(toEmail)) return;
        if (config["Smtp:NotificationsEnabled"]?.ToLower() != "true") return;

        try
        {
            var host = config["Smtp:Host"];
            var portStr = config["Smtp:Port"];
            var port = string.IsNullOrEmpty(portStr) ? 587 : int.Parse(portStr);
            var user = config["Smtp:Username"];
            var pass = config["Smtp:AppPassword"];
            var fromName = config["Smtp:FromName"] ?? "Jherson Aguto";
            var fromEmail = config["Smtp:FromEmail"];

            if (string.IsNullOrEmpty(host) || string.IsNullOrEmpty(user) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(fromEmail))
            {
                logger.LogWarning("SMTP is not fully configured (Host, User, Pass, or FromEmail missing). Skipping email to {Email}", toEmail);
                return;
            }

            var message = new MimeMessage();
            message.From.Add(new MailboxAddress(fromName, fromEmail));
            message.To.Add(new MailboxAddress(toEmail, toEmail));
            message.Subject = subject;

            var bodyBuilder = new BodyBuilder { HtmlBody = htmlBody };
            message.Body = bodyBuilder.ToMessageBody();

            var enableSsl = config["Smtp:EnableSsl"]?.ToLower() == "true";
            var socketOptions = enableSsl ? (port == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTls) : SecureSocketOptions.None;

            using var client = new SmtpClient();
            await client.ConnectAsync(host, port, socketOptions);
            await client.AuthenticateAsync(user, pass);
            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            logger.LogInformation("SMTP email sent to {Email} with subject: {Subject}", toEmail, subject);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to send SMTP email to {Email}", toEmail);
        }
    }

    public async Task SendContactFormMessageAsync(string senderName, string senderEmail, string message)
    {
        var toEmail = "dalisayfernando4@gmail.com";
        var subject = $"New Contact Message from {senderName}";
        
        // A simple HTML template for the recruiter message
        var html = $@"
            <div style='font-family: sans-serif; max-width: 600px; margin: 0 auto; padding: 20px; border: 1px solid #eee; border-radius: 8px;'>
                <h2 style='color: #00e5ff; margin-top: 0;'>New Contact Message</h2>
                <p style='color: #555; font-size: 14px;'>You have received a new message from your portfolio contact form.</p>
                <div style='background: #f9f9f9; padding: 15px; border-radius: 6px; margin-bottom: 20px;'>
                    <p style='margin: 0 0 10px 0;'><strong>Name:</strong> {System.Net.WebUtility.HtmlEncode(senderName)}</p>
                    <p style='margin: 0;'><strong>Email:</strong> <a href='mailto:{System.Net.WebUtility.HtmlEncode(senderEmail)}'>{System.Net.WebUtility.HtmlEncode(senderEmail)}</a></p>
                </div>
                <h3 style='margin-bottom: 10px; font-size: 16px; color: #333;'>Message:</h3>
                <div style='background: #fff; padding: 15px; border: 1px solid #e0e0e0; border-radius: 6px; white-space: pre-wrap; font-size: 15px; line-height: 1.5; color: #444;'>
                    {System.Net.WebUtility.HtmlEncode(message)}
                </div>
            </div>";

        await SendSmtpEmailAsync(toEmail, subject, html);
    }

    // ── Resend Methods (Legacy/Backup) ──────────────────────────────────────
    
    public async Task SendPasswordResetAsync(string toEmail, string resetLink)
    {
        var apiKey = config["Resend:ApiKey"];
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            logger.LogWarning("Resend:ApiKey is not configured — falling back to logs.");
            logger.LogInformation("Password Reset Link for {Email}: {Link}", toEmail, resetLink);
            return;
        }

        try {
            var overrideTo = config["Resend:OverrideTo"];
            var actualTo   = !string.IsNullOrWhiteSpace(overrideTo) ? overrideTo : toEmail;
            var from = config["Resend:FromAddress"] ?? "onboarding@resend.dev";

            IResend resend = ResendClient.Create(apiKey);

            await resend.EmailSendAsync(new EmailMessage
            {
                From     = from,
                To       = actualTo,
                Subject  = "Reset your Portfolio Admin password",
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
            logger.LogInformation("Password reset email sent via Resend.");
        } catch (Exception ex) {
            logger.LogError(ex, "Resend failed. Link: {Link}", resetLink);
        }
    }
}
