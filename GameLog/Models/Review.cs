using System.ComponentModel.DataAnnotations;

namespace GameLog.Models;

public class Review
{
    public int Id { get; set; }

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;

    [Range(1, 10)]
    public int Rating { get; set; }

    [MaxLength(2000)]
    public string? Text { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<ReviewComment> Comments { get; set; } = new List<ReviewComment>();
}
