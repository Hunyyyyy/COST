using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace COTS1.Controllers
{
    public class ListTaskController : Controller
    {
        private readonly TestNhiemVuContext db;

        public ListTaskController(TestNhiemVuContext db)
        {
            this.db = db;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
            var tasks = await db.Tasks
                .Where(t => t.AssignedTo == currentUserId) // Lọc theo trưởng nhóm đã nhận nhiệm vụ
                .Select(t=>new TaskViewModel {
                    Title = t.Title,
                    TaskId = t.TaskId,
                    DueDate = t.DueDate,
                    SentDay = t.CreatedAt ?? DateTime.Now,
                    Priority = t.Priority,
                    Status = t.Status,
                    Notes = t.Note,
                    Recipients = t.AssignedToNavigation.Email, // Nếu bạn có Include trên AssignedTo
                    Sender = t.CreatedByNavigation.FullName
                })
                .ToListAsync();

           

            return View(tasks);
        }
        public async Task<IActionResult> Subtasks(int taskID)
        {
            var subtasks = await db.Subtasks
                .Where(st => st.TaskId == taskID)
                .ToListAsync();
            var subtaskViewModels = subtasks.Select(st => new SubtaskViewModel
            {
                SubtaskId = st.SubtaskId,
                Title = st.Title,
                Description = st.Description,
                Status = st.Status
            }).ToList();

            // Trả về view và truyền danh sách công việc con
            return View(subtaskViewModels);
        }
    }
}
