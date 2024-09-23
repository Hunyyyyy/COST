namespace COTS1.Models
{
    public class TrackProgressViewModel
    {
        public List<AssignedSubtasksModel> ReceivedTasks { get; set; }
        public Dictionary<int, int> TaskProgress { get; set; }
        public Dictionary<int, double> SubtaskProgress { get; set; }
        public Dictionary<int, int> ProjectProgress { get; set; }
    }
}