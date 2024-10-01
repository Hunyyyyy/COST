namespace COTS1.Models
{
    public class SubmittedSubtaskViewModel
    {
        public int SubmissionId { get; set; }
        public int SubtaskId { get; set; }
        public int TaskId { get; set; }
        public int ProjectId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string ApprovedBy { get; set; }
        public string Subtask { get; set; }
        public string Task { get; set; }
        public string Project { get; set; }
        public string Notes { get; set; }
        public string FilePath { get; set; }
        public DateTime SubmittedAt { get; set; }
        public string Status { get; set; }
        public string SubtaskTitle { get; set; }
        public string SubtaskDescription { get; set; }
    }
}