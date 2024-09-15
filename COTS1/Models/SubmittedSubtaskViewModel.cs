namespace COTS1.Models
{
    public class SubmittedSubtaskViewModel
    {
        public int SubmissionId { get; set; }
        public int SubtaskId { get; set; }
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; }
        public string SubtaskTitle { get; set; }
        public string SubtaskDescription { get; set; }
    }
}
