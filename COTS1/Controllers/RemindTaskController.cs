using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace COTS1.Controllers
{
    public class RemindTaskController : Controller
    {
        private readonly TestNhiemVuContext _context;

        public RemindTaskController(TestNhiemVuContext context)
        {
            _context = context;
        }

        public IActionResult RemindTask()
        {
            // Lấy danh sách các nhắc nhở từ cơ sở dữ liệu và nối với bảng Projects để lấy tên dự án
            var reminders = _context.Reminders
                .Include(r => r.Project)
                .Include(r => r.User)// Include bảng Projects để lấy dữ liệu từ đó
                .ToList();

            // Tính toán trạng thái và số ngày còn lại cho mỗi nhắc nhở
            var reminderViewModels = reminders.Select(r => new ReminderViewModel
            {
                ReminderId = r.ReminderId,
                ProjectName = r.Project.ProjectName,
                ReminderContent = r.ReminderContent,
                ReminderDate = r.ReminderDate,
                DaysRemaining = (r.ReminderDate - DateTime.Now).Days,
                Status = (r.ReminderDate < DateTime.Now) ? "Đã quá hạn" : "Còn thời gian",

                // Lấy danh sách nhiệm vụ liên quan đến dự án
                TaskReminders = _context.SaveTasksReminders
               .Where(t => t.ProjectId == r.ProjectId)
               .Select(t => new TaskReminderViewModel
               {
                   TaskTitle = t.Title,
                   DaysRemaining = (t.DueDate - DateTime.Now).Days,
                   TaskStatus = (t.DueDate < DateTime.Now) ? "Đã quá hạn" : "Còn thời gian"
               })
               .ToList()
            }).ToList();
            

            return View(reminderViewModels);
        }
        
    }
}
