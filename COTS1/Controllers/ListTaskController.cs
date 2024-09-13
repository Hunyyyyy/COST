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
        public async Task<IActionResult> AddMember(string gmail)
        {
            // Tìm thành viên theo email
            var addMember = await db.Users
                .Where(users => users.Email == gmail)
                .Select(user => new
                {
                    user.UserId,
                    user.FullName,
                    user.Email
                }).FirstOrDefaultAsync();

            if (addMember == null)
            {
               
                TempData["ErrorMessage"] = "Không tìm thấy thành viên với email này.";
                return RedirectToAction("ErrorPage"); // Redirect về trang lỗi hoặc thông báo không tìm thấy
            }
            /*var memberToAdd = new GroupMember
            {
                UserId = addMember.UserId,
                FullName = addMember.FullName,
                Email = addMember.Email,
                // Thêm các thuộc tính khác nếu cần
            };

            // Thêm thành viên vào cơ sở dữ liệu
            db.GroupMembers.Add(memberToAdd);
            await db.SaveChangesAsync();*/

            // Chuyển dữ liệu đến hành động "Subtasks"
            return RedirectToAction("Subtasks", new { memberId = addMember.UserId });
        }


    }
}
