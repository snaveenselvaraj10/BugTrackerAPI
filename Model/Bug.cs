namespace BugTrackerAPI.Model
{
    public class Bug
    {
        public int BugId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Severity { get; set; }
        public string Status { get; set; }
        public int? AssignedTo { get; set; }
    }
}
