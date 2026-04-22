namespace BugTrackerAPI.DTOs
{
    public class CreateBugDto
    {
        public required string Title { get; set; }
        public required string Description { get; set; }
        public required string Severity { get; set; }
        public int CreatedBy { get; set; }
    }
}
