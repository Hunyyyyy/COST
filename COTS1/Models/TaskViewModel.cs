namespace COTS1.Models
{
    public class TaskViewModel
    {
        public string Title { get; set; }
        public List<string> Description { get; set; }
        public DateTime DueDate { get; set; }
        public string Priority { get; set; }
        public string Recipients { get; set; }
        public string Status { get; set; }
        public string Receives { get; set; }
        public int Progress { get; set; }
    }

}
