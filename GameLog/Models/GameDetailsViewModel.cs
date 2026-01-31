using System.ComponentModel.DataAnnotations;

namespace GameLog.Models;

public class GameDetailsViewModel
{
    public Game Game { get; set; } = null!;
    public List<Review> Reviews { get; set; } = new();

    // form for adding a review
    [Range(1, 10)]
    public int Rating { get; set; } = 8;

    [MaxLength(2000)]
    public string? Text { get; set; }
}
