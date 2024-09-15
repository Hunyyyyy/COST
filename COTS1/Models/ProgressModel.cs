namespace COTS1.Models
{
    public class ProgressModel
    {
        public class ProjectProgressTrackViewModel
        {
            public ProjectProgressViewModel ProjectProgress { get; set; }
            public List<TaskProgressViewModel> TaskProgress { get; set; }
            public List<SubtaskProgressViewModel> SubtaskProgress { get; set; }
        }

        public class ProjectProgressViewModel
        {
            public int ProjectId { get; set; }
            public decimal Progress { get; set; }
            public DateTime LastUpdatedAt { get; set; }
        }

        public class TaskProgressViewModel
        {
            public int TaskId { get; set; }
            public string Title { get; set; }
            public DateTime DueDate { get; set; }
            public int AssignedTo { get; set; }
            public decimal Progress { get; set; }
            public DateTime LastUpdatedAt { get; set; }
        }

        public class SubtaskProgressViewModel
        {
            public int SubtaskId { get; set; }
            public string Title { get; set; }
            public DateTime DueDate { get; set; }
            public int AssignedTo { get; set; }
            public decimal Progress { get; set; }
            public DateTime LastUpdatedAt { get; set; }
            public int TaskId { get; set; }
        }


    }
}
