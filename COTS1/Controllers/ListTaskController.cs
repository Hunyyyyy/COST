using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Linq;
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
            if (currentUserId == null)
            {
                TempData["ErrorMessageFromListTask"] = "Người dùng không hợp lệ.";
                return RedirectToAction("Login", "Login"); 
            }

     
            var projectIds = await db.ProjectUsers
                .Where(pu => pu.UserId == currentUserId)
                .Select(pu => pu.ProjectId)
                .ToListAsync();

            if (!projectIds.Any())
            {
                TempData["ErrorMessageFromListTask"] = "Bạn không thuộc dự án nào.";
                return View(new List<SaveTasksModel>()); 
            }

            var tasks = await db.SaveTasks
                .Where(t => projectIds.Contains(t.ProjectId))
                .Select(t => new SaveTasksModel
                {
                    TaskId = t.TaskId,
                    AssignedTo = t.AssignedTo,
                    CreatedAt = t.CreatedAt,
                    CreatedBy = t.CreatedBy,
                    Description = t.Description,
                    DueDate = t.DueDate,
                    Note = t.Note,
                    Priority = t.Priority,
                    Status = t.Status,
                    Title = t.Title
                })
                .ToListAsync();

            return View(tasks);
        }

        public async Task<IActionResult> Subtasks(int taskID)
        {
            // Lấy danh sách công việc phụ
            var subtasks = await db.Subtasks
                .Where(st => st.TaskId == taskID)
                .ToListAsync();

            // Chuyển đổi công việc phụ thành ViewModel
            var subtaskViewModels = subtasks.Select(st => new SubtaskViewModel
            {
                SubtaskId = st.SubtaskId,
                Title = st.Title,
                Description = st.Description,
                Status = st.Status
            }).ToList();

            // Lấy danh sách công việc phụ đã được nhận
            var assignedSubtasks = await db.AssignedSubtasks
                .Where(s => s.TaskId == taskID && s.Status == "Đang thực hiện")
                .Select(s => s.SubtaskId)
                .Where(id => id.HasValue) // Loại bỏ giá trị null
                .Select(id => id.Value) // Lấy giá trị không null
                .ToListAsync();

            // Tạo ViewModel cho view
            var viewModel = new Assigned_And_Suptask_Model
            {
                TaskId = taskID,
                Subtasks = subtaskViewModels,
                AssignedSubtaskIds = assignedSubtasks
            };

            // Truyền ViewModel vào view
            ViewBag.taskId = taskID;
            return View(viewModel);
        }





        public async Task<IActionResult> AcceptSubtask(int subtaskId, int taskID)
        {
            // Lấy ID người dùng hiện tại từ Session
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
       
            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để nhận công việc.";
                return RedirectToAction("Index", "Home");
            }

            // Tìm công việc phụ theo subtaskId
            var subtask = await db.Subtasks.FindAsync(subtaskId);

            if (subtask == null)
            {
                TempData["ErrorMessage"] = "Công việc phụ không tồn tại.";
                return RedirectToAction("Index", "Home");
            }

          
            var assignedSubtask = new AssignedSubtask
            {
                SubtaskId = subtaskId,
                TaskId = taskID,
                ProjectId = subtask.ProjectId, 
                MemberId = currentUserId,
                Status = "Đang thực hiện",
                AssignedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            db.AssignedSubtasks.Add(assignedSubtask);       
            subtask.Status = "Đang thực hiện";
            subtask.AssignedTo = currentUserId; 
            db.Subtasks.Update(subtask);
            await db.SaveChangesAsync();
          

            TempData["SuccessMessageFromAcceptSubtask"] = "Công việc phụ đã được nhận thành công!";
            return RedirectToAction("Subtasks", "ListTask", new { taskID = taskID }); 
        }


    }
}
