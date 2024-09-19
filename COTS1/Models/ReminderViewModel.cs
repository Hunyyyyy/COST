using COTS1.Data;

namespace COTS1.Models
{
    public class ReminderViewModel
    {
        public int ReminderId { get; set; } // ID của nhắc nhở

        public int ProjectId { get; set; } // ID của dự án

        public int UserId { get; set; } // ID của người dùng liên quan đến nhắc nhở

        public string ReminderContent { get; set; } = null!; // Nội dung nhắc nhở

        public DateTime ReminderDate { get; set; } // Ngày nhắc nhở

        public bool? IsAcknowledged { get; set; } // Trạng thái đã nhận biết nhắc nhở

        public DateTime? CreatedAt { get; set; } // Ngày tạo nhắc nhở

        public virtual Project Project { get; set; } = null!; // Đối tượng Project liên quan đến nhắc nhở

        public virtual User User { get; set; } = null!; // Đối tượng User liên quan đến nhắc nhở

        // Tính toán liên quan đến thời gian và trạng thái
        public int DaysRemaining { get; set; } // Số ngày còn lại cho đến ngày nhắc nhở
        public string ProjectName { get; set; } = null!; // Tên dự án liên quan đến nhắc nhở
        public string Status { get; set; } = null!; // Trạng thái nhắc nhở (Còn thời gian / Đã quá hạn)

        // Danh sách các nhắc nhở nhiệm vụ (có thể chứa nhiều nhiệm vụ liên quan đến nhắc nhở này)
        public List<TaskReminderViewModel> TaskReminders { get; set; } = new List<TaskReminderViewModel>();
    }

    // Mô hình cho nhiệm vụ liên quan đến nhắc nhở
    public class TaskReminderViewModel
    {
        public int TaskId { get; set; }
        public string TaskTitle { get; set; } = null!; // Tiêu đề của nhiệm vụ

        public int DaysRemaining { get; set; } // Số ngày còn lại cho nhiệm vụ

        public string TaskStatus { get; set; } = null!; // Trạng thái của nhiệm vụ (Còn thời gian / Đã quá hạn)
    }
}
