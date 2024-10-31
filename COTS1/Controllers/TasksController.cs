using COTS1.Class;
using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Cms;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;


namespace COTS1.Controllers
{
    public class TasksController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TestNhiemVuContext _dbContext;

        public TasksController(IHttpContextAccessor contextAccessor, TestNhiemVuContext dbContext)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
            
        }

        public async Task<IActionResult> CreateTasks(int projectId)
        {
         
            var email = HttpContext.Session.GetString("UserEmail");
			var project = await _dbContext.Projects
                .Where(p => p.ProjectId == projectId)
                .Select(p => new Project
                {
                    ProjectName = p.ProjectName,
                    ProjectId = p.ProjectId,
                }).FirstOrDefaultAsync();

            if (project != null)
            {
                ViewBag.UserEmail = email;
                ViewBag.ProjectId = projectId;
                ViewBag.ProjectName = project.ProjectName;
                return View(project);
            }
            else
            {
                // Trả về một đối tượng Project mặc định
                return RedirectToAction("CreateTaskProject", "ProjectManager"); // Trả về một Project rỗng
            }
        }

        [HttpPost]
        public async Task<IActionResult> SaveTask(string Title, string Description, DateTime DueDate, string Priority, string? Note, string from, int ProjectId, IFormFile? fileUpload) // Cho phép fileUpload là null
        {
            var manager = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == from);

            // Nếu Note là null, gán giá trị mặc định
            string CheckNote = string.IsNullOrEmpty(Note) ? "Không có ghi chú" : Note;

            // Kiểm tra và lưu tệp đính kèm nếu có (fileUpload không bắt buộc)
            string filePath = null;
            if (fileUpload != null && fileUpload.Length > 0)
            {
                // Đường dẫn thư mục để lưu file
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Tạo đường dẫn cho file
                var uniqueFileName = $"{Guid.NewGuid()}_{fileUpload.FileName}";
                filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Lưu file vào thư mục
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(fileStream);
                }

                // Đường dẫn file để sử dụng nếu cần, không bắt buộc lưu vào cơ sở dữ liệu
                filePath = $"/uploads/{uniqueFileName}";
            }

            if (ModelState.IsValid)
            {
                // Thêm nhiệm vụ chính vào bảng SentTasksLists
                var task = new SentTasksList
                {
                    ProjectId = ProjectId,
                    Title = Title,
                    Note = CheckNote,
                    Description = Description,
                    DueDate = DueDate,
                    Priority = Priority,
                    Status = "Đang chờ",
                    CreatedAt = DateTime.Now,
                    CreatedBy = manager?.UserId
                };

                _dbContext.SentTasksLists.Add(task);
                await _dbContext.SaveChangesAsync();

                // Thêm nhiệm vụ chính vào bảng SaveTasks
                var saveTask = new SaveTask
                {
                    ProjectId = ProjectId,
                    Title = Title,
                    Description = Description,
                    DueDate = DueDate,
                    Priority = Priority,
                    Status = "Đang chờ",
                    AssignedTo = null,
                    CreatedBy = manager?.UserId,
                    CreatedAt = DateTime.Now,
                    //FilePath = filePath // Có thể null nếu không có file
                };

                _dbContext.SaveTasks.Add(saveTask);
                await _dbContext.SaveChangesAsync();

                var taskId = saveTask.TaskId;
                var getday = _dbContext.SaveTasks
                    .Where(b => b.ProjectId == ProjectId && b.TaskId == taskId)
                    .Select(c => new
                    {
                        DueDate = c.DueDate,
                        CreatedAt = c.CreatedAt
                    })
                    .FirstOrDefault();

                if (getday != null)
                {
                    DateTime dueDate = getday.DueDate;
                    DateTime create = (DateTime)getday.CreatedAt;

                    // Tính toán số ngày còn lại giữa DueDate và CreatedAt
                    TimeSpan timeSpanRemaining = dueDate - create;
                    int reminderDate = (int)timeSpanRemaining.Days;

                    var saveTaskReminder = new SaveTasksReminder
                    {
                        Priority = Priority,
                        Title = Title,
                        Description = Description,
                        DueDate = DueDate,
                        TempReminderDate = reminderDate, // Lưu trữ số ngày đã tính toán
                        ProjectId = ProjectId,
                        AssignedTo = null,
                        CreatedBy = manager?.UserId,
                        CreatedAt = DateTime.Now
                    };

                    _dbContext.SaveTasksReminders.Add(saveTaskReminder);
                    await _dbContext.SaveChangesAsync();
                }

                // Tách các công việc phụ từ mô tả nhiệm vụ chính
                var subtasks = SplitTasks(saveTask.Description);

                // Lưu các công việc phụ vào cơ sở dữ liệu
                foreach (var subtask in subtasks)
                {
                    _dbContext.Subtasks.Add(new Subtask
                    {
                        TaskId = taskId,
                        ProjectId = ProjectId,
                        Title = subtask.Title,
                        Description = subtask.Description,
                        Status = subtask.Status,
                        AssignedTo = null, // Cập nhật nếu cần thiết
                        CreatedAt = DateTime.Now
                    });
                }

                await _dbContext.SaveChangesAsync();

                TempData["CreateTasksuccess"] = "Nhiệm vụ đã được lưu thành công!";
                return RedirectToAction("CreateTaskProject", "ProjectManager", new { projectId = ProjectId });
            }

            return RedirectToAction("CreateTasks", "Tasks", new { projectId = ProjectId });
        }


        public List<SubtaskViewModel> SplitTasks(string taskDescription)
        {
            var subtasks = new List<SubtaskViewModel>();

            // Tách các công việc chính và công việc phụ
            var taskSections = taskDescription.Split(new[] { "Công việc ", "Ghi chú:" }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var section in taskSections)
            {
                // Tách phần tiêu đề và các công việc phụ
                var parts = section.Split(new[] { '+' }, StringSplitOptions.RemoveEmptyEntries);
                var mainTaskTitle = parts.First().Trim(); // Tiêu đề công việc chính

                for (int i = 1; i < parts.Length; i++)
                {
                    var subtask = parts[i].Trim();
                    if (!string.IsNullOrEmpty(subtask))
                    {
                        subtasks.Add(new SubtaskViewModel
                        {
                            Title = mainTaskTitle,
                            Description = subtask,
                            Status = "Chưa nhận" // Hoặc có thể thay đổi trạng thái mặc định nếu cần
                        });
                    }
                }
            }

            return subtasks;
        }
    }
}