namespace GameLog.Models;

public class ReviewDisplayViewModel
{
    public int Id { get; set; }

    public string UserId { get; set; } = "";

    public int Rating { get; set; }

    public string? Text { get; set; }

    public DateTime CreatedAt { get; set; }

    public string DisplayName { get; set; } = "User";

    public string AvatarFileName { get; set; } = "default-avatar.jpg";

    public List<ReviewCommentDisplayViewModel> Comments { get; set; } = new();
}
