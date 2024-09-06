namespace COTS1.Models
{
    public class EmailNotificationModel
    {
        public string Subject { get; set; }
        public string Message { get; set; }
        public List<string> Recipients { get; set; } // Danh sách email của các nhân viên
    }
}
