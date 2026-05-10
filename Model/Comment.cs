namespace BugTrackerAPI.Model
{
    public class Comment
    {
        public int CommentId { get; set; }
        public int BugId { get; set; }
        public string CommentText { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public int? ParentCommentId { get; set; }
        public bool IsDeleted { get; set; }
        public bool Edited { get; set; }
        public DateTime? EditedDate { get; set; }
    }
}
