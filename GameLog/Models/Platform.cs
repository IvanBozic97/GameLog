using System.ComponentModel.DataAnnotations;

namespace GameLog.Models;

public class Platform
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    public ICollection<GamePlatform> GamePlatforms { get; set; } = new List<GamePlatform>();
}
