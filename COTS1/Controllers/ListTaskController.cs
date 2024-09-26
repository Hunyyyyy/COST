using COTS1.Class;
using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using System.Threading.Tasks;

namespace COTS1.Controllers
{
    public class ListTaskController : Controller
    {
        private readonly TestNhiemVuContext db;
        private readonly ILogger<ListTaskController> _logger;

        public ListTaskController(TestNhiemVuContext db, ILogger<ListTaskController> logger)
        {
            this.db = db;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
            var isManager = await db.ProjectUsers
                .AnyAsync(pu => pu.UserId == currentUserId && pu.Role == "Manager");

            // Truyền thông tin vai trò vào ViewBag
            ViewBag.IsManager = isManager;
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

        [HttpGet]
        public async Task<IActionResult> EditTask(int taskId)
        {
            // Lấy nhiệm vụ theo taskId
            var task = await db.SaveTasks
                .Where(t => t.TaskId == taskId)
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
                .FirstOrDefaultAsync();

            if (task == null)
            {
                return NotFound("Nhiệm vụ không tồn tại.");
            }

            return View(task);
        }

        [HttpPost]
        public async Task<IActionResult> EditTask(SaveTasksModel model)
        {
            if (ModelState.IsValid)
            {
                var task = await db.SaveTasks.FindAsync(model.TaskId);

                if (task != null)
                {
                    // Cập nhật dữ liệu
                    task.Title = model.Title;
                    task.Description = model.Description;
                    task.AssignedTo = model.AssignedTo;
                    task.DueDate = model.DueDate;
                    task.Priority = model.Priority;
                    task.Status = model.Status;
                    task.Note = model.Note;

                    db.SaveTasks.Update(task);
                    await db.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Cập nhật nhiệm vụ thành công.";
                    return RedirectToAction("Index"); // Hoặc bất kỳ trang nào bạn muốn
                }

                return NotFound("Nhiệm vụ không tồn tại.");
            }

            return View(model); // Nếu có lỗi, trả về cùng dữ liệu hiện tại để chỉnh sửa
        }

        [HttpPost]
        public async Task<IActionResult> DeleteTask(int taskId)
        {
            var task = await db.SaveTasks.FindAsync(taskId);

            if (task != null)
            {
                // Xóa tất cả các công việc con liên quan
                var subtasks = await db.Subtasks.Where(st => st.TaskId == taskId).ToListAsync();
                db.Subtasks.RemoveRange(subtasks);

                // Xóa nhiệm vụ
                db.SaveTasks.Remove(task);

                try
                {
                    await db.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Xóa nhiệm vụ thành công.";
                }
                catch (DbUpdateException ex)
                {
                    // Xử lý lỗi cập nhật cơ sở dữ liệu
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa nhiệm vụ.";
                }

                return RedirectToAction("Index"); // Hoặc trang phù hợp sau khi xóa
            }

            return NotFound("Nhiệm vụ không tồn tại.");
        }

        public async Task<IActionResult> Subtasks(int taskID, string? statusFilter, string? titleFilter)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");


            var query = db.Subtasks.Where(st => st.TaskId == taskID);


            if (!string.IsNullOrEmpty(statusFilter))
            {
                query = query.Where(st => st.Status == statusFilter);
            }

  
            if (!string.IsNullOrEmpty(titleFilter))
            {
                query = query.Where(st => st.Description.Contains(titleFilter)); 
            }


            var subtasks = await query.ToListAsync();


            var subtaskViewModels = subtasks.Select(st => new SubtaskViewModel
            {
                SubtaskId = st.SubtaskId,
                Title = st.Title,
                Description = st.Description,
                Status = st.Status,
                Assigneder = st.Assigner
            }).ToList();

            // Fetch assigned subtasks
            var assignedSubtasks = await db.AssignedSubtasks
                .Where(s => s.TaskId == taskID &&
                            (s.Status == "Đang thực hiện" ||
                             s.Status == "Đã nộp" ||
                             s.Status == "Từ chối phê duyệt"))
                .Select(s => s.SubtaskId)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToListAsync();

            // Create the ViewModel
            var viewModel = new Assigned_And_Suptask_Model
            {
                TaskId = taskID,
                Subtasks = subtaskViewModels,
                AssignedSubtaskIds = assignedSubtasks,
            };

            // Fetch project ID and user role as before
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

            ViewBag.taskId = taskID;
            ViewBag.userRole = userRole;
            ViewBag.TitleFilter = titleFilter; // Store the title filter in ViewBag for repopulating input
            return View(viewModel);
        }



        public async Task<IActionResult> AcceptSubtask(int subtaskId, int taskID)
        {
            // Lấy ID người dùng hiện tại từ Session
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
            var assigneder = HttpContext.Session.GetString("UserFullName");
            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để nhận công việc.";
                return RedirectToAction("Index", "Home");
            }

            // Tìm công việc phụ theo subtaskId
            var subtask = await db.Subtasks.FindAsync(subtaskId);
            var subtasks = await db.Subtasks
                 .Where(s => s.SubtaskId == subtaskId )
                    .FirstOrDefaultAsync();
            if (subtask == null)
            {
                TempData["ErrorMessage"] = "Công việc phụ không tồn tại.";
                return RedirectToAction("Index", "Home");
            }
            subtasks.Assigner = assigneder;
            db.Subtasks.Update(subtasks);
            var assignedSubtask = new AssignedSubtask
            {
                SubtaskId = subtaskId,
                TaskId = taskID,
                ProjectId = subtask.ProjectId,
                AssignedTo = currentUserId,
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

        [HttpGet]
        public async Task<IActionResult> ListRecivedTask()
        {
            // Lấy ID người dùng hiện tại từ Session
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
            var currentUserName = HttpContext.Session.GetString("UserFullName");
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
                SubmittedSubtasks = submittedSubtasks,
               // ApprovedSubtasks = approvedSubtasks,
               // RejectedSubtasks = rejectedSubtasks
            };
            ViewBag.currentUserName = currentUserName;
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAssignedSubtask(int subtaskId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để thực hiện thao tác này.";
                return RedirectToAction("Login", "Account");
            }

            // Tìm công việc con được gán với ID và người dùng hiện tại
            var assignedSubtask = await db.AssignedSubtasks
                .FirstOrDefaultAsync(ast => ast.SubtaskId == subtaskId && ast.AssignedTo == currentUserId);

            if (assignedSubtask == null)
            {
                TempData["ErrorMessage"] = "Công việc không tồn tại hoặc bạn không có quyền xóa công việc này.";
                return RedirectToAction("ListRecivedTask");
            }
            var subtasks = await db.Subtasks
              .Where(s => s.SubtaskId == subtaskId)
              .FirstOrDefaultAsync();
            subtasks.Status = "Chưa nhận";
            // Xóa công việc con được gán
            db.AssignedSubtasks.Remove(assignedSubtask);
            db.Subtasks.Update(subtasks);
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Công việc đã được xóa thành công.";
            return RedirectToAction("ListRecivedTask");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteSubmittedSubtask(int subtaskId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để thực hiện thao tác này.";
                return RedirectToAction("Login", "Login");
            }

            // Tìm công việc đã gửi với ID và người dùng hiện tại
            var submittedSubtask = await db.SubmittedSubtasks
                .FirstOrDefaultAsync(ss => ss.SubtaskId == subtaskId && ss.UserId == currentUserId);

            if (submittedSubtask == null)
            {
                TempData["ErrorMessage"] = "Công việc không tồn tại hoặc bạn không có quyền xóa công việc này.";
                return RedirectToAction("ListRecivedTask");
            }

            // Xóa công việc đã gửi
            db.SubmittedSubtasks.Remove(submittedSubtask);
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Công việc đã được xóa thành công.";
            return RedirectToAction("ListRecivedTask");
        }

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> UpdateSubtask(int subtaskId, IFormFile fileUpload, string notes)
        {
            var userId = HttpContext.Session.GetInt32("UserIDEmail");

            if (userId == null)
            {
                return Unauthorized();
            }

            // Tìm công việc con đã nộp dựa trên subtaskId và userId
            var submittedSubtask = await db.SubmittedSubtasks
                .Where(st => st.SubtaskId == subtaskId && st.UserId == userId)
                .FirstOrDefaultAsync();

            if (submittedSubtask == null)
            {
                return NotFound("Không tìm thấy công việc đã nộp.");
            }

            // Cập nhật ghi chú
            submittedSubtask.Notes = notes;

            // Nếu có file mới, lưu file và cập nhật đường dẫn
            if (fileUpload != null && fileUpload.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                var uniqueFileName = $"{Guid.NewGuid()}_{fileUpload.FileName}";
                var filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await fileUpload.CopyToAsync(fileStream);
                }

                // Cập nhật đường dẫn file mới
                submittedSubtask.FilePath = $"/uploads/{uniqueFileName}";
            }

            db.SubmittedSubtasks.Update(submittedSubtask);
            await db.SaveChangesAsync();

            return RedirectToAction("ListRecivedTask");
        }

        [HttpPost]
        public async Task<IActionResult> SubmitSubtask(int subtaskId, IFormFile fileUpload, string notes)
        {
            var userId = HttpContext.Session.GetInt32("UserIDEmail");

            if (userId == null)
            {
                return Unauthorized();
            }

            // Kiểm tra subtaskId có tồn tại hay không
            var taskId = await db.Subtasks
                .Where(st => st.SubtaskId == subtaskId)
                .Select(st => st.TaskId)
                .FirstOrDefaultAsync();

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

            // Kiểm tra và lưu tệp đính kèm nếu có
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

                // Lưu đường dẫn file để sử dụng trong cơ sở dữ liệu
                filePath = $"/uploads/{uniqueFileName}";
            }

            // Lưu công việc con đã nộp vào SubmittedSubtasks
            var submittedSubtask = new SubmittedSubtask
            {
                SubtaskId = subtaskId,
                TaskId = (int)taskId,
                ProjectId = (int)projectId,
                UserId = (int)userId,
                SubmittedAt = DateTime.Now,
                Status = "đang chờ phê duyệt",
                Notes = notes,
                FilePath = filePath // Lưu đường dẫn file (nếu có)
            };
           
            db.SubmittedSubtasks.Add(submittedSubtask);
            var receivedTask = await db.AssignedSubtasks.FirstOrDefaultAsync(t => t.SubtaskId == subtaskId);
           

            if (receivedTask != null)
            {
                // Cập nhật trạng thái của nhiệm vụ được gán
                receivedTask.Status = "Đã nộp";
               

                // Update bản ghi trong AssignedSubtasks
                db.AssignedSubtasks.Update(receivedTask);
            }
            await db.SaveChangesAsync();
            // Cập nhật tiến trình cho công việc con

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

            // Kiểm tra nếu người dùng là Manager
            var isManager = await db.ProjectUsers
                .AnyAsync(pu => pu.UserId == userId && pu.Role == "Manager");

            // Truyền thông tin vai trò vào ViewBag
            ViewBag.IsManager = isManager;

            var projectDetails = await db.Projects
                .Where(p => p.ManagerId == userId)
                .Select(p => new ProjectModel
                {
                    ProjectName = p.ProjectName,
                    ProjectId = p.ProjectId,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate,
                    ProjectProgress = (int)p.Progress
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

        [HttpGet]
        public async Task<IActionResult> SubmittedTasksByProject(int projectId)
        {
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");

            // Kiểm tra vai trò của người dùng
            var isManager = await db.ProjectUsers
                .AnyAsync(pu => pu.UserId == currentUserId && pu.ProjectId == projectId && pu.Role == "Manager");

            if (!isManager)
            {
                ViewBag.NotManager = "Bạn không phải quản lý";
                return RedirectToAction("TrackProgress");  // Hoặc redirect tới một trang lỗi
            }

            // Lấy danh sách công việc có trạng thái 'đang chờ phê duyệt' cho dự án
            var submittedTasks = await db.SubmittedSubtasks
                .Where(ss => ss.ProjectId == projectId)
                .Select(ss => new SubmittedSubtaskViewModel
                {
                    SubmissionId = ss.SubmissionId,
                    Subtask = db.Subtasks.Where(p=>p.SubtaskId==ss.SubtaskId).Select(p=>p.Description).FirstOrDefault(),
                    Task = db.Subtasks.Where(p => p.SubtaskId == ss.SubtaskId).Select(p => p.Title).FirstOrDefault(),
                    Project = db.Projects.Where(p => p.ProjectId == ss.ProjectId).Select(p => p.ProjectName).FirstOrDefault(),
                    UserId = ss.UserId,
                    UserName = db.Users.Where(u => u.UserId == ss.UserId).Select(u => u.FullName).FirstOrDefault(),
                    SubmittedAt = ss.SubmittedAt,
                    Status = ss.Status,
                    Notes = ss.Notes,
                    FilePath = ss.FilePath // Optional
                })
                .ToListAsync();

            return View(submittedTasks);
        }
        [HttpPost]
        public async Task<IActionResult> SubmitSubtaskByManager(int subtaskId)
        {
            var userId = HttpContext.Session.GetInt32("UserIDEmail");

            if (userId == null)
            {
                return Unauthorized();
            }
            var email = db.Users.Where(p => p.UserId == userId).Select(p => p.Email).FirstOrDefault();
            // Kiểm tra subtaskId có tồn tại hay không
            var taskId = await db.Subtasks
                .Where(st => st.SubtaskId == subtaskId)
                .Select(st => st.TaskId)
                .FirstOrDefaultAsync();

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

            // Tìm kiếm công việc con đã nộp trong SubmittedSubtasks
            var submittedSubtask = await db.SubmittedSubtasks
                .Where(s => s.SubtaskId == subtaskId && s.TaskId == taskId)
                .FirstOrDefaultAsync();
            var subtasks = await db.Subtasks
             .Where(s => s.SubtaskId == subtaskId && s.TaskId == taskId)
             .FirstOrDefaultAsync();

            if (submittedSubtask == null)
            {
                return BadRequest("Công việc con chưa được nộp.");
            }
            subtasks.Status = "Đã phê duyệt";
            // Cập nhật trạng thái thành "Đã phê duyệt"
            submittedSubtask.Status = "Đã phê duyệt";
            submittedSubtask.SubmittedAt = DateTime.Now;  // Cập nhật lại thời gian phê duyệt (tuỳ chọn)

            db.Subtasks.Update(subtasks);
            db.SubmittedSubtasks.Update(submittedSubtask);
            await db.SaveChangesAsync();

            // Cập nhật tiến trình cho công việc con
            await UpdateSubtaskProgress(subtaskId, (int)userId);

            // Cập nhật tiến trình của nhiệm vụ chứa công việc con
            await UpdateTaskProgress((int)taskId);

            // Kiểm tra xem tất cả công việc con đã hoàn thành chưa
            var task = await db.SaveTasks.FindAsync(taskId);
            if (task != null)
            {
                var subtask = await db.Subtasks
                    .Where(st => st.TaskId == taskId)
                    .ToListAsync();

                var allSubtasksCompleted = !subtask.Any() || subtask.All(st =>
                    db.SubtaskProgresses
                    .Where(sp => sp.SubtaskId == st.SubtaskId)
                    .Select(sp => sp.Progress)
                    .FirstOrDefault() == 100);

                if (allSubtasksCompleted)
                {
                    task.Progress = 100; // Đặt tiến trình nhiệm vụ thành 100%
                    await db.SaveChangesAsync();

                    // Cập nhật trạng thái nhắc nhở
                    await UpdateReminderStatus((int)taskId);
                }
            }

            // Cập nhật tiến trình của dự án chứa nhiệm vụ
            await UpdateProjectProgress((int)projectId);

            return RedirectToAction("SubmittedTasksByProject", new { projectId = projectId });
        }

        private async Task UpdateReminderStatus(int taskId)
        {
            var reminders = await db.Reminders
                .Where(r => r.TaskId == taskId)
                .ToListAsync();

            foreach (var reminder in reminders)
            {
                reminder.Status = "Hoàn thành"; // Cập nhật trạng thái nhắc nhở
            }

            db.Reminders.UpdateRange(reminders);
            await db.SaveChangesAsync();
        }

        
        [HttpPost]
        public async Task<IActionResult> RejectSubtaskByManager(int subtaskId)
        {
            var userId = HttpContext.Session.GetInt32("UserIDEmail");

            if (userId == null)
            {
                return Unauthorized();
            }

            var email = db.Users.Where(p => p.UserId == userId).Select(p => p.Email).FirstOrDefault();

            // Kiểm tra subtaskId có tồn tại hay không
            var taskId = await db.Subtasks
                .Where(st => st.SubtaskId == subtaskId)
                .Select(st => st.TaskId)
                .FirstOrDefaultAsync();

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

            // Tìm kiếm công việc con đã nộp trong SubmittedSubtasks
            var submittedSubtask = await db.SubmittedSubtasks
                .Where(s => s.SubtaskId == subtaskId && s.TaskId == taskId)
                .FirstOrDefaultAsync();
            var subtasks = await db.Subtasks
                .Where(s => s.SubtaskId == subtaskId && s.TaskId == taskId)
                .FirstOrDefaultAsync();
            if (submittedSubtask == null)
            {
                return BadRequest("Công việc con chưa được nộp.");
            }
            //AssignedSubtasks
            subtasks.Status = "Từ chối phê duyệt";
            // Cập nhật trạng thái thành "Từ chối phê duyệt"
            submittedSubtask.Status = "Từ chối phê duyệt";
            // submittedSubtask.RejectedAt = DateTime.Now; // Cập nhật thời gian từ chối (tuỳ chọn)

            db.SubmittedSubtasks.Update(submittedSubtask);
            db.Subtasks.Update(subtasks);
            await db.SaveChangesAsync();

            // Lấy thông tin người đã nộp công việc con (người dùng bị từ chối)
            var userEmail = await db.Users
                .Where(u => u.UserId == submittedSubtask.UserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(userEmail))
            {
                return BadRequest("Không tìm thấy email của người dùng.");
            }

            // Gửi email thông báo từ chối
            var accessToken = HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Login");
            }

            var sendNotificationMail = new SendNotificationMail(accessToken);
            await SendNotificationMail.SendEmailAsync(
                sendNotificationMail._gmailService,
                email, // Thay bằng địa chỉ email của bạn hoặc hệ thống
                userEmail,
                "Công việc của bạn đã bị từ chối",
                $"Công việc con ID: {subtaskId} trong dự án ID: {projectId} đã bị từ chối phê duyệt."
            );

            return RedirectToAction("SubmittedTasksByProject", new { projectId = projectId });
        }

        private async Task CancelReminderIfTaskCompleted(int taskId)
        {
            // Kiểm tra xem tất cả các nhiệm vụ con của nhiệm vụ này đã được phê duyệt chưa
            var allSubtasksApproved = !await db.Subtasks
                .Where(st => st.TaskId == taskId)
                .AnyAsync(st => !db.SubmittedSubtasks
                    .Any(ss => ss.SubtaskId == st.SubtaskId && ss.Status == "Đã phê duyệt"));

            if (allSubtasksApproved)
            {
                // Lấy nhắc nhở liên quan đến nhiệm vụ
                var reminders = await db.Reminders
                    .Where(r => r.TaskId == taskId)
                    .ToListAsync();

                if (reminders.Any())
                {
                    // Xóa tất cả nhắc nhở liên quan
                    db.Reminders.RemoveRange(reminders);
                    await db.SaveChangesAsync();
                }
            }
        }

    }
}