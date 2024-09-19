namespace COTS1.Models
{
    public class ProjectDetailsViewModel
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
    }

    public class TaskDetailsViewModel
    {
        public int TaskId { get; set; }
        public string Title { get; set; }
        public int Progress { get; set; }
        public DateTime? DueDate { get; set; }
        public List<SubtaskViewModel> Subtasks { get; set; }
    }

    public class SubtaskDetailsViewModel
    {
        public int SubtaskId { get; set; }
        public string Title { get; set; }
        public int AssignedTo { get; set; }
        public int Progress { get; set; }
    }
}