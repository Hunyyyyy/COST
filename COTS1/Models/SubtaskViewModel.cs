namespace COTS1.Models
{
    public class SubtaskViewModel
    {
        public int SubtaskId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public int AssignedTo { get; set; }
        public int Progress { get; set; }
    }
}
