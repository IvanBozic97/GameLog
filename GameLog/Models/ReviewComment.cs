using System.ComponentModel.DataAnnotations;

namespace GameLog.Models;

public class ReviewComment
{
    public int Id { get; set; }

    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required, MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
