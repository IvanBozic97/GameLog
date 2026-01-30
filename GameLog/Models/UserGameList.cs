using System.ComponentModel.DataAnnotations;

namespace GameLog.Models;

public enum GameStatus
{
    WantToPlay = 0,
    Playing = 1,
    Played = 2
}

public class UserGameList
{
    public int Id { get; set; }

    [Required]
    public string UserId { get; set; } = string.Empty;

    public int GameId { get; set; }
    public Game Game { get; set; } = null!;

    public GameStatus Status { get; set; }

    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
}
