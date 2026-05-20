using System;
using System.ComponentModel.DataAnnotations;

namespace BlazorPortfolio.Models;

public class ResumeFile
{
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string OriginalFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    public string StoredFileName { get; set; } = string.Empty;

    [Required]
    [MaxLength(2048)]
    public string FileUrl { get; set; } = string.Empty;

    [MaxLength(255)]
    public string? StorageKey { get; set; }

    [Required]
    [MaxLength(100)]
    public string ContentType { get; set; } = "application/pdf";

    public long FileSizeBytes { get; set; }

    public bool IsActive { get; set; }

    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
