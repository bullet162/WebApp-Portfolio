using System.ComponentModel.DataAnnotations;

namespace BlazorPortfolio.Models;

public enum EnrichmentStatus { NotEnriched, Pending, Ready, Approved, Rejected, Failed }

public class DeveloperProfileEnrichment
{
    public int Id { get; set; }

    // Links to the original request
    public int CollaborationRequestId { get; set; }
    public CollaborationRequest? CollaborationRequest { get; set; }

    public EnrichmentStatus Status { get; set; } = EnrichmentStatus.NotEnriched;

    [MaxLength(200)]
    public string? GeneratedHeadline { get; set; }

    [MaxLength(1000)]
    public string? GeneratedSummary { get; set; }

    public string? GeneratedSkillsJson { get; set; }
    
    public string? GeneratedProjectHighlightsJson { get; set; }
    
    public string? GeneratedCollaborationInterestsJson { get; set; }

    public string? SourceCitationsJson { get; set; }

    public double ConfidenceScore { get; set; }

    [MaxLength(100)]
    public string? ModelUsed { get; set; }

    [MaxLength(50)]
    public string? PromptVersion { get; set; }

    public DateTime? GeneratedAt { get; set; }
    
    public DateTime? ApprovedAt { get; set; }
    
    public DateTime? RejectedAt { get; set; }

    [MaxLength(100)]
    public string? ApprovedBy { get; set; }

    public string? ErrorMessage { get; set; }
}
