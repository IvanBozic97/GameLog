using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace GameLog.Models;

public class UserProfile
{
    [Key]
    public string UserId { get; set; } = string.Empty;

    public IdentityUser User { get; set; } = null!;

    [MaxLength(100)]
    public string AvatarFileName { get; set; } = "default-avatar.jpg";

    [Required, MaxLength(30)]
    public string DisplayName { get; set; } = string.Empty;
}
