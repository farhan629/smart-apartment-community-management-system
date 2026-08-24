namespace ComplaintMaintenanceService.Application.Features.Comments.DTOs;

public class CreateCommentRequestDto
{
    public string CommentText { get; set; } = string.Empty;
    public int? StaffRating { get; set; }
}

public class CommentResponseDto
{
    public Guid CommentId { get; set; }
    public Guid ComplaintId { get; set; }
    public Guid CommentedBy { get; set; }
    public string CommentText { get; set; } = string.Empty;
    public int? StaffRating { get; set; }
    public DateTime CreatedAt { get; set; }
}
