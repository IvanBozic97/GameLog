namespace GameLog.Models;

public class ReviewCommentDisplayViewModel
{
    public int Id { get; set; }
    public int ReviewId { get; set; }

    public string UserId { get; set; } = string.Empty;

    public string DisplayName { get; set; } = "User";
    public string AvatarFileName { get; set; } = "default-avatar.jpg";

    public string Text { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
