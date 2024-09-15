using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using static COTS1.Models.ProgressModel;

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
                .Where(t => projectIds.Contains((int)t.ProjectId))
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
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
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
            var projectId = await db.SaveTasks
           .Where(p => p.TaskId == taskID)
           .Select(p => p.ProjectId)
              .FirstOrDefaultAsync();

            if (projectId == null)
            {
                TempData["ErrorMessage"] = "Dự án không tồn tại.";
                return RedirectToAction("Index", "ListTask");
            }
            var userRole = await db.ProjectUsers
                .Where(r => r.ProjectId == projectId && r.UserId == currentUserId)
                    .Select(r => r.Role)
                        .FirstOrDefaultAsync();

            /*if (userRole == null)
            {
                TempData["ErrorMessage"] = "Bạn không tham gia dự án này.";
                return RedirectToAction("Index", "ListTask");
            }*/

            // Kiểm tra nếu vai trò của người dùng là "View", không cho phép nhận công việc
            /* if (userRole == "View")
             {
                 TempData["ErrorMessage"] = "Bạn không có quyền nhận công việc phụ.";
                 return RedirectToAction("Index", "ListTask");
             }*/
            // Truyền ViewModel vào view
            ViewBag.taskId = taskID;
            ViewBag.userRole = userRole;
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
                AssignedTo= currentUserId,
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

        public async Task<IActionResult> ListRecivedTask()
        {
            // Lấy ID người dùng hiện tại từ Session
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");

            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem danh sách công việc đã nhận.";
                return RedirectToAction("Login", "Account");
            }

            // Lấy danh sách công việc đã nhận
            var receivedTasks = await db.AssignedSubtasks
                .Where(ast => ast.AssignedTo == currentUserId)
                .Join(db.Subtasks, // Thực hiện join với bảng Subtasks
                      ast => ast.SubtaskId, // Khóa join từ bảng AssignedSubtasks
                      st => st.SubtaskId,   // Khóa join từ bảng Subtasks
                      (ast, st) => new AssignedSubtasksModel // Tạo đối tượng mới để chứa dữ liệu
                      {
                          SubtaskId = st.SubtaskId,
                          Title = st.Title,
                          Description = st.Description,
                          Status = ast.Status,
                          AssignedAt = ast.AssignedAt,
                          UpdatedAt = ast.UpdatedAt,
                          ProjectId = ast.ProjectId,
                          TaskId = ast.TaskId,
                          IsSubmitted = db.SubmittedSubtasks.Any(ss => ss.SubtaskId == st.SubtaskId && ss.UserId == currentUserId) // Kiểm tra xem công việc con đã nộp chưa
                      })
                .ToListAsync();

            // Lấy danh sách công việc đã gửi
            var submittedSubtasks = await db.SubmittedSubtasks
                .Where(p => p.UserId == currentUserId)
                .Select(p => new SubmittedSubtaskViewModel
                {
                    SubtaskId = p.SubtaskId,
                    Status = p.Status,
                    SubmittedAt = p.SubmittedAt
                })
                .ToListAsync();

            // Tạo model để truyền dữ liệu vào view
            var model = new RecivedTask_And_SubmittedSubtask_Model
            {
                ReceivedTasks = receivedTasks,
                SubmittedSubtasks = submittedSubtasks
            };

            return View(model);
        }


        public async Task<IActionResult> SubmitSubtask(int subtaskId)
        {
            var userId = HttpContext.Session.GetInt32("UserIDEmail");

            if (userId == null)
            {
                return Unauthorized();
            }

            var taskId = await db.Subtasks
                .Where(st => st.SubtaskId == subtaskId)
                .Select(st => st.TaskId)
                .FirstOrDefaultAsync();

            // Kiểm tra taskId có thể null
            if (taskId == 0)
            {
                return BadRequest("Nhiệm vụ không tồn tại.");
            }

            // Lấy ProjectId từ TaskId
            var projectId = await db.SaveTasks
                .Where(t => t.TaskId == taskId)
                .Select(t => t.ProjectId)
                .FirstOrDefaultAsync();

            if (projectId == 0)
            {
                return BadRequest("Dự án không tồn tại.");
            }

            // Lưu công việc con đã nộp
            var submittedSubtask = new SubmittedSubtask
            {
                SubtaskId = subtaskId,
                TaskId =(int) taskId,
                ProjectId = (int)projectId,
                UserId = (int)userId,
                SubmittedAt = DateTime.Now,
                Status = "Đã nộp"
            };

            db.SubmittedSubtasks.Add(submittedSubtask);
            await db.SaveChangesAsync();

            // Cập nhật tiến trình cho công việc con
            await UpdateSubtaskProgress(subtaskId, (int)userId);

            // Cập nhật tiến trình của nhiệm vụ chứa công việc con
            await UpdateTaskProgress((int)taskId);

            // Cập nhật tiến trình của dự án chứa nhiệm vụ
            await UpdateProjectProgress((int)projectId);

            return RedirectToAction("ListRecivedTask");
        }


        public async Task UpdateSubtaskProgress(int subtaskId, int userId)
        {
            var progress = await db.SubtaskProgresses
                .FirstOrDefaultAsync(sp => sp.SubtaskId == subtaskId && sp.AssignedTo == userId);

            if (progress != null)
            {
                progress.Progress = 100; // Công việc con hoàn thành
                progress.LastUpdatedAt = DateTime.Now;
            }
            else
            {
                db.SubtaskProgresses.Add(new SubtaskProgress
                {
                    SubtaskId = subtaskId,
                    AssignedTo = userId,
                    Progress = 100, // Công việc con hoàn thành
                    LastUpdatedAt = DateTime.Now
                });
            }

            await db.SaveChangesAsync();
        }

        public async Task UpdateTaskProgress(int taskId)
        {
            var subtasks = await db.Subtasks
                .Where(st => st.TaskId == taskId)
                .ToListAsync();

            var completedSubtasks = subtasks.Count(st => db.SubtaskProgresses
                .Any(sp => sp.SubtaskId == st.SubtaskId && sp.Progress == 100));

            var totalSubtasks = subtasks.Count;
            var progressPercentage = totalSubtasks > 0 ? (completedSubtasks / (double)totalSubtasks) * 100 : 0;

            var task = await db.SaveTasks.FindAsync(taskId);
            if (task != null)
            {
                task.Progress = (int)progressPercentage; // Chuyển đổi từ double sang int
                await db.SaveChangesAsync();
            }
        }

        public async Task UpdateProjectProgress(int projectId)
        {
            var tasks = await db.SaveTasks
                .Where(t => t.ProjectId == projectId)
                .ToListAsync();

            var completedTasks = tasks.Count(t => t.Progress == 100);
            var totalTasks = tasks.Count;
            var progressPercentage = totalTasks > 0 ? (completedTasks / (double)totalTasks) * 100 : 0;

            var project = await db.Projects.FindAsync(projectId);
            if (project != null)
            {
                project.Progress = (int)progressPercentage; // Chuyển đổi từ double sang int
                await db.SaveChangesAsync();
            }
        }



        public async Task<IActionResult> TrackProgress()
        {
            var userId = HttpContext.Session.GetInt32("UserIDEmail");

            if (userId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem tiến trình.";
                return RedirectToAction("Login", "Login");
            }
            var projectDetails = await db.Projects
          .Where(p => p.ManagerId == userId)
          .Select(p => new ProjectModel
          {
              ProjectName = p.ProjectName,
              ProjectId = p.ProjectId,
              StartDate= p.StartDate,
              EndDate= p.EndDate,
              ProjectProgress=(int)p.Progress
          }).ToListAsync();

            return View(projectDetails);
        }


        public async Task<IActionResult> ProjectDetails(int projectId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");

            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem tiến trình.";
                return RedirectToAction("Login", "Login");
            }

            // Kiểm tra xem người dùng có tham gia vào dự án không
            var isUserInProject = await db.ProjectUsers
                .AnyAsync(pu => pu.ProjectId == projectId && pu.UserId == currentUserId);

            if (!isUserInProject)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền truy cập dự án này.";
                return RedirectToAction("Index", "Home");
            }
            var project = await db.Projects
                .Where(p => p.ProjectId == projectId)
                .Select(p => new ProjectDetailsViewModel
                {
                    ProjectId = p.ProjectId,
                    ProjectName = p.ProjectName,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    Tasks = db.SaveTasks
                        .Where(t => t.ProjectId == projectId)
                        .Select(t => new TaskViewModel
                        {
                            TaskId = t.TaskId,
                            Title = t.Title,
                            Progress = t.Progress.HasValue ? (int)t.Progress.Value : 0, // Chuyển đổi decimal sang int
                            DueDate = t.DueDate,
                            Subtasks = db.Subtasks
                                .Where(st => st.TaskId == t.TaskId)
                                .Select(st => new SubtaskViewModel
                                {
                                    SubtaskId = st.SubtaskId,
                                    Title = st.Title,
                                    AssignedTo = st.AssignedTo.HasValue ? st.AssignedTo.Value : 0, // Kiểm tra giá trị nullable
                                    Progress = db.SubtaskProgresses
                                        .Where(sp => sp.SubtaskId == st.SubtaskId)
                                        .Select(sp => (int?)sp.Progress) // Chuyển đổi decimal sang int nullable
                                        .FirstOrDefault() ?? 0 // Sử dụng ?? cho kiểu int nullable
                                }).ToList()
                        }).ToList()
                }).FirstOrDefaultAsync();

            if (project == null)
            {
                return NotFound();
            }

            return View(project);
        }









    }
}
