namespace COTS1.Models
{
    public class Assigned_And_Suptask_Model

    {
        public int TaskId { get; set; }
        public List<SubtaskViewModel> Subtasks { get; set; }
        public List<int> AssignedSubtaskIds { get; set; }
    }
}