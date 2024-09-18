namespace COTS1.Models
{
    public class RecivedTask_And_SubmittedSubtask_Model
    {
        public List<AssignedSubtasksModel> ReceivedTasks { get; set; }
        public List<SubmittedSubtaskViewModel> SubmittedSubtasks { get; set; }
        public IEnumerable<SubmittedSubtaskViewModel> ApprovedSubtasks { get; set; } // New
        public IEnumerable<SubmittedSubtaskViewModel> RejectedSubtasks { get; set; }
    }
}
