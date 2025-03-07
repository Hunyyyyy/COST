﻿using COTS1.Class;
using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.Text.RegularExpressions;
using X.PagedList;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace COTS1.Controllers
{
    public class ProjectManager : Controller
    {
        private readonly TestNhiemVuContext db;
        private readonly IHttpContextAccessor _contextAccessor;

        public ProjectManager(TestNhiemVuContext db, IHttpContextAccessor contextAccessor)
        {
            this.db = db;
            _contextAccessor = contextAccessor;
        }

		/*  public IActionResult Index()
          {
              return View();
          }*/
		[HttpGet]
        public async Task<IActionResult> Index(int? page, string searchTerm)
        {

            var managerID = HttpContext.Session.GetInt32("UserIDEmail");

            if (managerID == null)
            {
                return Unauthorized();
            }

            // Check if the user is a manager
            var isManager = await db.ProjectUsers
                .AnyAsync(pu => pu.UserId == managerID && pu.Role == "Manager");

            ViewBag.IsManager = isManager;

            int pageSize = 12;  // Number of items per page
            int pageNumber = page ?? 1;  // Current page, default to page 1

            ViewBag.CurrentFilter = searchTerm;  // Store the search term

            // Get the list of projects for the manager
            var query = db.Projects
                .Where(p => p.ManagerId == managerID.Value)
                .Select(n => new ProjectModel
                {
                    ProjectName = n.ProjectName,
                    ProjectId = n.ProjectId,
                    CreatedAt = n.CreatedAt,
                    Status = n.Status
                });
            int count = 0;
            // Filter the list if there is a search term
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(p => p.ProjectName.Contains(searchTerm));
                if (!query.Any()) // Kiểm tra số lượng phần tử trong query
                {
                    TempData["NotFound"] = $"Không tìm thấy kết quả cho từ khóa: {searchTerm}";
                }
            }

            // Fetch the paginated list directly from the query
            var pagedList = query.ToPagedList(pageNumber, pageSize);  // Pagination applied directly to the query

            return View(pagedList);
        }



        [HttpPost]
        public JsonResult CheckNameProject(string name)
        {
            if (name.IsNullOrEmpty())
            {
                return Json(new { isValid = false, message = "Tên dự án không được để trống." });
            }
            else if (name.Length < 3)
            {
                return Json(new { isValid = false, message = "Tên dự án phải có ít nhất 3 ký tự." });
            }
            else if (char.IsPunctuation(name[0]) || char.IsSymbol(name[0]))
            {
                return Json(new { isValid = false, message = "Tên dự án không được bắt đầu bằng ký tự đặc biệt." });
            }

            // Nếu tất cả các kiểm tra đều hợp lệ
            return Json(new { isValid = true });
        }
        [HttpPost]
        public async Task<IActionResult> CreateProject(string NameProject)
        {
           
            var managerID = HttpContext.Session.GetInt32("UserIDEmail");

            if (managerID == null)
            {
                return Unauthorized();
            }
            if (string.IsNullOrWhiteSpace(NameProject) ||
                    NameProject.Length <= 3 ||
                    NameProject.Length >= 50)
            {
                //ModelState.AddModelError("NameProject", "Tên dự án không hợp lệ. Tên không được để trống, phải bắt đầu bằng chữ cái, có độ dài từ 4 đến 49 ký tự.");
                //return BadRequest(new { success = false, message = "Kiểm tra thất bại: Tên dự án không hợp lệ." }); ;
                TempData["ErrorMessage"] = "Tạo không thành công!";
                return RedirectToAction("Index");
            }
            if (NameProject != null && ModelState.IsValid)
            {
                // Lưu dự án mới
                var saveNameProject = new Project
                {
                    ProjectName = NameProject,
                    ManagerId = managerID.Value,
                    StartDate = DateTime.Now,
                    EndDate = null
                };
                db.Projects.Add(saveNameProject);
                await db.SaveChangesAsync();

                var projectId = saveNameProject.ProjectId;

                // Lưu người tạo dự án vào nhóm và set vai trò "Manager"
                var projectUser = await db.ProjectUsers
                    .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == managerID.Value);

                if (projectUser == null)
                {
                    projectUser = new ProjectUser
                    {
                        ProjectId = projectId,
                        UserId = managerID.Value,
                        Role = "Manager"
                    };
                    db.ProjectUsers.Add(projectUser);
                }
                else
                {
                    projectUser.Role = "Manager";
                    db.ProjectUsers.Update(projectUser);
                }

                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Dự án đã được tạo và bạn đã được gán vai trò 'Manager'.";
            }

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> DeleteProject(int projectId)
        {
            var project = await db.Projects.FindAsync(projectId);

            if (project != null)
            {
                try
                {
                    // Xóa tất cả các thành viên liên quan đến dự án
                    var projectMembers = db.ProjectUsers.Where(pm => pm.ProjectId == projectId);
                    var AssignedSubtasks = db.AssignedSubtasks.Where(pm => pm.ProjectId == projectId);
                    var projectRemind  = db.Reminders.Where(pm => pm.ProjectId == projectId);
                    var projectSaveTaskReminder = db.SaveTasksReminders.Where(pm => pm.ProjectId == projectId);
                    var projectSentTask = db.SentTasksLists.Where(pm => pm.ProjectId == projectId);
                    var SaveTasks = db.SaveTasks.Where(pm => pm.ProjectId == projectId);
                    if (projectMembers != null && projectSentTask != null&& projectRemind!=null&& projectSaveTaskReminder!=null)
                    {
                        db.ProjectUsers.RemoveRange(projectMembers);
                        db.Reminders.RemoveRange(projectRemind);
                        db.SaveTasksReminders.RemoveRange(projectSaveTaskReminder);
                        db.SentTasksLists.RemoveRange(projectSentTask);
                        db.AssignedSubtasks.RemoveRange(AssignedSubtasks);
                        db.SaveTasks.RemoveRange(SaveTasks);
                    }



                    // Xóa dự án
                    db.Projects.Remove(project);
                    await db.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Dự án đã được xóa thành công.";
                    return RedirectToAction("Index"); // Hoặc trang phù hợp sau khi xóa
                }
                catch (DbUpdateException ex)
                {
                    // Xử lý lỗi cập nhật cơ sở dữ liệu
                    TempData["ErrorMessage"] = "Đã xảy ra lỗi khi xóa dự án. " + ex.Message;
                }
            }

            return NotFound("Dự án không tồn tại.");
        }

        [HttpPost]
        public async Task<IActionResult> RenameProject(int projectId, string newProjectName)
        {
            var managerID = HttpContext.Session.GetInt32("UserIDEmail");

            // Kiểm tra xem người dùng hiện tại có phải là Manager của dự án này không
            var project = await db.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId && p.ManagerId == managerID);

            if (project == null)
            {
                TempData["ErrorMessage"] = "Bạn không có quyền đổi tên dự án này hoặc dự án không tồn tại.";
                return RedirectToAction("Index");
            }

            // Kiểm tra tên mới có hợp lệ không
            if (string.IsNullOrEmpty(newProjectName))
            {
                TempData["ErrorMessage"] = "Tên dự án mới không được để trống.";
                return RedirectToAction("Index");
            }
            if (newProjectName.Length <= 2)
            {
                TempData["ErrorMessage"] = "Tên dự án mới phải có độ dài lớn hơn 2 ký tự.";
                return RedirectToAction("Index");
            }
            if (newProjectName.Length >=50)
            {
                TempData["ErrorMessage"] = "Tên dự án mới không được có độ dài lớn hơn 50 ký tự.";
                return RedirectToAction("Index");
            }
            if (Regex.IsMatch(newProjectName, @"^[^a-zA-Z0-9]"))
            {
                TempData["ErrorMessage"] = "Tên dự án mới không được bắt đầu bằng ký tự đặc biệt.";
                return RedirectToAction("Index");
            }

            project.ProjectName = newProjectName;
            await db.SaveChangesAsync();

            TempData["SuccessMessage"] = "Tên dự án đã được đổi thành công!";
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> CreateTaskProject(int projectId)
        {
            HttpContext.Session.SetInt32("projectId", projectId);
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
            // Kiểm tra vai trò của người dùng
            var isManager = await db.ProjectUsers
                .AnyAsync(pu => pu.UserId == currentUserId && pu.Role == "Manager");

            // Truyền thông tin vai trò vào ViewBag
            ViewBag.IsManager = isManager;
            var tasks = await db.SentTasksLists
                .Where(t => t.ProjectId == projectId) // Lọc theo trưởng nhóm đã nhận nhiệm vụ
                .Select(t => new SentTaskListModel
                {
                    Title = t.Title,
                    TaskId = t.TaskId,
                    DueDate = t.DueDate,
                    Priority = t.Priority,
                    Status = t.Status,
                    Note = t.Note,
                    CreatedAt = t.CreatedAt // Nếu bạn có Include trên AssignedTo
                })
                .ToListAsync();
            ViewBag.ProjectId = projectId;

            return View(tasks);
        }

        public async Task<IActionResult> SettingProject(int? projectId)
        {
            // Lấy ProjectId từ session nếu không có giá trị nào được truyền vào
            if (projectId == null)
            {
                projectId = HttpContext.Session.GetInt32("ProjectId");
            }

            if (projectId == null)
            {
                TempData["FailMessage"] = "Dự án không tồn tại.";
                return RedirectToAction("Index");
            }

            var project = await db.Projects
             .Where(p => p.ProjectId == projectId)
             .Select(p => new ProjectModel
             {
                 ProjectId = p.ProjectId,
                 ProjectName = p.ProjectName,
                 Description = p.Description,
                 StartDate = p.StartDate,
                 EndDate = p.EndDate,
                 Status = p.Status,
                 Users = db.ProjectUsers
                     .Where(pu => pu.ProjectId == projectId)
                     .Select(pu => new ProjectUserModel
                     {
                         UserId = pu.UserId,
                         ProjectId = pu.ProjectId,
                         Email = db.Users.Where(u => u.UserId == pu.UserId).Select(u => u.Email).FirstOrDefault(),
                         Role = pu.Role
                     }).ToList()
             }).FirstOrDefaultAsync();

            if (project == null)
            {
                TempData["FailMessage"] = "Dự án không tồn tại.";
                return RedirectToAction("Index");
            }
            var projectContent = new ProjectAndUserGroupModel
            {
                Project = project,
                ProjectUserModel = project.Users // Set this to project.Users as you have already populated it
            };
            ViewBag.ProjectId = projectId;
            return View(projectContent);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSettingProject(int projectId, string NameProject, string Description, DateTime StartDay, DateTime EndDay, string Status)
        {
            var nameUser = HttpContext.Session.GetString("UserFullName");
            var managerID = HttpContext.Session.GetInt32("UserIDEmail");

            var project = await db.Projects.FirstOrDefaultAsync(p => p.ProjectId == projectId);

            if (project != null)
            {

                project.ProjectName = NameProject;
                project.Description = Description;
                project.StartDate = StartDay;
                project.EndDate = EndDay;
                project.Status = Status;

                db.Projects.Update(project);
                await db.SaveChangesAsync();

                // Tạo nhắc nhở với ReminderDate là EndDay và DaysRemaining tính từ EndDay - StartDay
                var reminder = new Reminder
                {
                    ProjectId = projectId,
                    ReminderContent = $"Nhắc nhở: Dự án {NameProject} sắp hết hạn vào {EndDay.ToShortDateString()}.",
                    ReminderDate = EndDay, // Đặt ngày nhắc nhở là EndDay
                    CreatedAt = DateTime.Now,
                    UserId = managerID.Value,
                    ProjectName = NameProject,
                    FullName = nameUser,
                    IsAcknowledged = false
                };
                db.Reminders.Add(reminder);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật dự án thành công và nhắc nhở đã được tạo!";
            }
            else
            {
                TempData["ErrorMessage"] = "Dự án không tồn tại!";
            }

            return RedirectToAction("SettingProject", new { projectId = projectId });
        }

        [HttpGet]
        public async Task<IActionResult> SaveRole(string Role, int projectId, int userId)
        {
            if (string.IsNullOrWhiteSpace(Role))
            {
                TempData["FailMessage"] = "Vui lòng chọn vai trò hợp lệ.";
                return RedirectToAction("SettingProject", new { projectId = projectId });
            }

            // Tìm kiếm người dùng đã tồn tại trong bảng ProjectUsers
            var projectUser = await db.ProjectUsers
                .FirstOrDefaultAsync(pu => pu.ProjectId == projectId && pu.UserId == userId);

            if (projectUser != null)
            {
                // Cập nhật trường Role nếu bản ghi tồn tại
                projectUser.Role = Role;
                db.ProjectUsers.Update(projectUser);
                await db.SaveChangesAsync();

                TempData["SuccessMessage"] = "Đã cập nhật vai trò thành công.";
            }
            else
            {
                TempData["FailMessage"] = "Người dùng không tồn tại trong dự án.";
            }

            ViewBag.ProjectId = projectId;
            return RedirectToAction("SettingProject", new { projectId = projectId });
        }

        [HttpGet]
        public async Task<IActionResult> SearchUsers(string EmailUser, int projectId)
        {
            if (string.IsNullOrWhiteSpace(EmailUser))
            {
                TempData["FailFindUser"] = "Vui lòng nhập email để tìm kiếm.";
                return RedirectToAction("SettingProject", new { projectId = projectId });
            }

            var users = await db.Users
                .Where(u => u.Email == EmailUser)
                .Select(u => new UserModel
                {
                    Email = u.Email,
                    UserId = u.UserId,
                })
                .FirstOrDefaultAsync();

            if (users == null)
            {
                TempData["FailFindUser"] = "Không tìm thấy người dùng này.";
                return RedirectToAction("SettingProject", new { projectId = projectId });
            }
            else
            {
                var projectUser = new ProjectUser
                {
                    ProjectId = projectId,
                    UserId = users.UserId,
                    Role = "View"
                };

                db.ProjectUsers.Add(projectUser);
                await db.SaveChangesAsync();
                TempData["SuccessFindUser"] = "Đã thêm người dùng.";
                ViewBag.ProjectId = projectId;
                return RedirectToAction("SettingProject", new { projectId = projectId });
            }
        }

        public async Task<IActionResult> SendEmailTasksProject(int taskId)
        {
            var from = HttpContext.Session.GetString("UserEmail");
            var projectId = HttpContext.Session.GetInt32("projectId");

            // Lấy danh sách người dùng trong project
            var group = await db.ProjectUsers
                .Where(p => p.ProjectId == projectId)
                .Select(u => u.UserId)
                .ToListAsync();

            // Lấy danh sách email người dùng thuộc project
            var to = await db.Users
                .Where(u => group.Contains(u.UserId))
                .Select(u => u.Email)
                .ToListAsync();

            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");

            if (to == null || !to.Any()) // Nếu không có người dùng
            {
                TempData[$"MessageSendMailFail_{taskId}"] = "Chưa có nhóm!";
                return RedirectToAction("CreateTaskProject", new { projectId });
            }

            // Lấy tên project
            var nameProject = await db.Projects
                .Where(p => p.ProjectId == projectId)
                .Select(p => p.ProjectName)
                .FirstOrDefaultAsync();

            // Lấy tên nhiệm vụ
            var nameTask = await db.SentTasksLists
                .Where(p => p.ProjectId == projectId)
                .Select(p => p.Title)
                .FirstOrDefaultAsync();

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Login", "Login");
            }

            // Gửi email cho từng người dùng
            var sendNotificationMail = new SendNotificationMail(accessToken);
            foreach (var email in to)
            {
                await SendNotificationMail.SendEmailAsync(
                    sendNotificationMail._gmailService,
                    from.Trim(),
                    email.Trim(),
                    "Quản lý dự án " + nameProject + " vừa tạo một nhiệm vụ mới!",
                    nameTask
                );
            }

            TempData[$"MessageSendMail_{taskId}"] = "Gửi Email thành công!";
            return RedirectToAction("CreateTaskProject", new { projectId });
        }
    }
}