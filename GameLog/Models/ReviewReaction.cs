using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GameLog.Models;

public class ReviewReaction
{
    public int Id { get; set; }

    [Required]
    public int ReviewId { get; set; }
    public Review Review { get; set; } = null!;

    [Required]
    public string UserId { get; set; } = string.Empty;
    public IdentityUser User { get; set; } = null!;

    public bool IsLike { get; set; }
}
