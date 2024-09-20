namespace COTS1.Models
{
    public class AssignedSubtasksModel
    {
        public int? SubtaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public DateTime? AssignedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public bool IsSubmitted { get; set; }
    }
}