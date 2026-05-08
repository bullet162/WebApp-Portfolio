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

    // --- Developer Network Additions ---
    
    public bool IsVisible { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public bool IsVerified { get; set; } = false;
    public bool OpenToCollaborate { get; set; } = false;
    
    [MaxLength(100)]
    public string? PublicSlug { get; set; }
    
    [MaxLength(100)]
    public string? RoleTitle { get; set; }
    
    [MaxLength(50)]
    public string? ConnectionType { get; set; }
    
    [MaxLength(200)]
    public string? Tags { get; set; }
    
    [MaxLength(500)]
    public string? GitHubUrl { get; set; }
    
    [MaxLength(500)]
    public string? LinkedInUrl { get; set; }
    
    public int DisplayOrder { get; set; }
    
    public bool AiEnrichmentConsent { get; set; }
    public DateTime? AiEnrichmentConsentAt { get; set; }
    public DateTime? LastAiEnrichmentAt { get; set; }
    
    [MaxLength(255)]
    public string? EditTokenHash { get; set; }
    public DateTime? EditTokenExpiresAt { get; set; }
    public DateTime? EditTokenUsedAt { get; set; }
    public DateTime? LastEditRequestedAt { get; set; }
    public DateTime? LastEditedAt { get; set; }
    
    public bool BadgeEnabled { get; set; }
    public bool ReciprocalLinkVerified { get; set; }
    public DateTime? ReciprocalLinkVerifiedAt { get; set; }
    public DateTime? ReciprocalLinkLastCheckedAt { get; set; }
    
    [MaxLength(500)]
    public string? ReciprocalLinkCheckError { get; set; }
    
    [MaxLength(100)]
    public string? ReviewedBy { get; set; }
    
    public string? ReviewNote { get; set; }
    
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
