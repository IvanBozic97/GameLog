using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GameLog.Models;

public class ReviewComment
{
    public int Id { get; set; }

    [Required]
    public int ReviewId { get; set; }

    public Review Review { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;

    public IdentityUser? User { get; set; }

    [Required, MaxLength(1000)]
    public string Text { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
