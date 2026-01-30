using System.ComponentModel.DataAnnotations;

namespace GameLog.Models;

public enum ReportTargetType
{
    Review = 0,
    Comment = 1
}

public class Report
{
    public int Id { get; set; }

    [Required]
    public string ReporterUserId { get; set; } = string.Empty;

    public ReportTargetType TargetType { get; set; }

    public int TargetId { get; set; }

    [Required, MaxLength(200)]
    public string Reason { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsResolved { get; set; } = false;

    public string? ResolvedByUserId { get; set; }
}
