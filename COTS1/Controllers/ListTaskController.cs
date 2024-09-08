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
            
            // Truy vấn lấy tất cả các tasks từ database, bao gồm cả subtasks
            var tasks = await db.Tasks
                .Include(t => t.Subtasks) // Bao gồm cả subtasks
                .ToListAsync();

            // Tạo danh sách các TaskViewModel
            var subtasks = tasks.SelectMany(task => task.Subtasks)
          .Select(subtask => new SubtaskViewModel
          {
              SubtaskId = subtask.SubtaskId,
              Title = "",
              Description = subtask.Description,
              Status = subtask.Status
          }).ToList();

            // Trả về view và truyền danh sách nhiệm vụ vào
            return View(subtasks);
        }
    }
    }
