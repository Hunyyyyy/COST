namespace COTS1.Models
{
    public class TaskViewModel
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public List<string> Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public string Recipients { get; set; }
        public string Status { get; set; }
        public string Receives { get; set; }
        public int Progress { get; set; }
        public List<SubtaskViewModel> Subtasks { get; set; } = new List<SubtaskViewModel>();
    }
    public class SubtaskViewModel
    {
        public int SubtaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
    }

}
