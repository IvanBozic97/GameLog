using System.ComponentModel.DataAnnotations;

namespace GameLog.Models;

public class GameEditViewModel
{
    public int Id { get; set; }

    [Required, MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    public int? ReleaseYear { get; set; }
    public string? ImagePath { get; set; }

    public List<int> SelectedGenreIds { get; set; } = new();
    public List<Genre> AllGenres { get; set; } = new();

    public List<int> SelectedPlatformIds { get; set; } = new();
    public List<Platform> AllPlatforms { get; set; } = new();

}
