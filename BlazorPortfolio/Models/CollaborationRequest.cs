using System.ComponentModel.DataAnnotations;

namespace BlazorPortfolio.Models;

public enum CollaborationStatus { Pending, Approved, Rejected, Flagged }

public class CollaborationRequest
{
    public int Id { get; set; }

    [Required(ErrorMessage = "First name is required.")]
    [MaxLength(50, ErrorMessage = "First name must be 50 characters or less.")]
    [RegularExpression(@"^[\p{L}\s'\-]+$", ErrorMessage = "First name contains invalid characters.")]
    public string FirstName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Last name is required.")]
    [MaxLength(50, ErrorMessage = "Last name must be 50 characters or less.")]
    [RegularExpression(@"^[\p{L}\s'\-]+$", ErrorMessage = "Last name contains invalid characters.")]
    public string LastName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Enter a valid email address.")]
    [MaxLength(200)]
    public string? Email { get; set; }

    [Url(ErrorMessage = "Enter a valid URL.")]
    [MaxLength(500)]
    public string? PortfolioUrl { get; set; }

    [MaxLength(500, ErrorMessage = "Message must be 500 characters or less.")]
    public string? Message { get; set; }

    public CollaborationStatus Status { get; set; } = CollaborationStatus.Pending;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ReviewedAt { get; set; }
}
