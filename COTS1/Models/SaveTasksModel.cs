using COTS1.Data;

namespace COTS1.Models
{
    public class SaveTasksModel
    {
        public int TaskId { get; set; }

        public string Title { get; set; } = null!;
        public string? Note { get; set; } = null!;

        public string? Description { get; set; }

        public DateTime DueDate { get; set; }

        public string? Priority { get; set; }

        public string? Status { get; set; }

        public int? AssignedTo { get; set; }

        public int? CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }
        public virtual ICollection<Subtask> Subtasks { get; set; } = new List<Subtask>();

        public virtual ICollection<TaskNotification> TaskNotifications { get; set; } = new List<TaskNotification>();
    }
}
