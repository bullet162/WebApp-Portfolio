using System.ComponentModel.DataAnnotations;

namespace BlazorPortfolio.Models;

public class HiringMessage
{
    public int Id { get; set; }
    public string? Name { get; set; }

    [Required(ErrorMessage = "Email is required.")]
    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Subject is required.")]
    [MinLength(3, ErrorMessage = "Subject must be at least 3 characters.")]
    public string Subject { get; set; } = string.Empty;

    public string? Message { get; set; }
    public DateTime SentAt { get; set; } = DateTime.UtcNow;
    public bool IsRead { get; set; }
}
