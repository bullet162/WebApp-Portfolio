namespace BlazorPortfolio.Models;

public class SiteProfile
{
    public int Id { get; set; }
    public string OwnerName { get; set; } = "Your Name";
    public string Tagline { get; set; } = "Full Stack Developer · .NET · Blazor · Cloud";
    public string Eyebrow { get; set; } = "👋 Welcome to my portfolio";
    public string HeroCta { get; set; } = "Hire Me";
    public string HeroSecondaryCta { get; set; } = "View Projects";
    public string ContactBlurb { get; set; } = "Have an opportunity or just want to say hi? Fill out the form and I'll get back to you.";
    public string? Email { get; set; }
    public string? Location { get; set; }
    public string? GitHubUrl { get; set; }
    public string? LinkedInUrl { get; set; }
    public string? HeroBio { get; set; }
    public string? AvatarUrl { get; set; }
    public string? GitHubUsername { get; set; }
    public string? ResumeUrl { get; set; }
}
