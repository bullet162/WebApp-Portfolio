using System.ComponentModel.DataAnnotations;

namespace BlazorPortfolio.Models;

public enum RevisionStatus { Pending, Approved, Rejected }

public class DeveloperNetworkProfileRevision
{
    public int Id { get; set; }
    
    // Links to the original request
    public int CollaborationRequestId { get; set; }
    public CollaborationRequest? CollaborationRequest { get; set; }

    [MaxLength(50)]
    public string? ProposedFirstName { get; set; }

    [MaxLength(50)]
    public string? ProposedLastName { get; set; }

    [MaxLength(500)]
    public string? ProposedPortfolioUrl { get; set; }

    [MaxLength(500)]
    public string? ProposedMessage { get; set; }

    [MaxLength(100)]
    public string? ProposedRoleTitle { get; set; }

    [MaxLength(50)]
    public string? ProposedConnectionType { get; set; }

    [MaxLength(200)]
    public string? ProposedTags { get; set; }

    [MaxLength(500)]
    public string? ProposedGitHubUrl { get; set; }

    [MaxLength(500)]
    public string? ProposedLinkedInUrl { get; set; }

    public bool? ProposedOpenToCollaborate { get; set; }

    public RevisionStatus Status { get; set; } = RevisionStatus.Pending;

    public DateTime SubmittedAt { get; set; } = DateTime.UtcNow;

    public DateTime? ReviewedAt { get; set; }

    [MaxLength(100)]
    public string? ReviewedBy { get; set; }

    public string? ReviewNote { get; set; }
}
