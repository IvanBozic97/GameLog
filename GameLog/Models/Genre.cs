using System.ComponentModel.DataAnnotations;

namespace GameLog.Models;

public class Genre
{
    public int Id { get; set; }

    [Required, MaxLength(80)]
    public string Name { get; set; } = string.Empty;

    public ICollection<GameGenre> GameGenres { get; set; } = new List<GameGenre>();
}
