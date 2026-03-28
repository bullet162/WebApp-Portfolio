namespace BlazorPortfolio.Models;

public class Skill
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g. "Frontend", "Backend", "Tools"
    public int Proficiency { get; set; } = 80; // 0-100
}
