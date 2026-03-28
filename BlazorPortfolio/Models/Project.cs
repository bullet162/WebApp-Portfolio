namespace BlazorPortfolio.Models;

public class Project
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string TechStack { get; set; } = string.Empty;
    public string? LiveUrl { get; set; }
    public string? RepoUrl { get; set; }
    public int SortOrder { get; set; }
}
