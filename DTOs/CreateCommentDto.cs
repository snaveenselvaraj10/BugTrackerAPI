namespace BugTrackerAPI.DTOs
{
    public class CreateCommentDto
    {
        public required int BugId { get; set; }
        public required string CommentText { get; set; }
        public int? ParentCommentId { get; set; }
    }
}
