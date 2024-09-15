﻿using COTS1.Class;
using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;

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
        public async Task<IActionResult> Index(string NameProject)
        {
            var managerID = HttpContext.Session.GetInt32("UserIDEmail");

            if (NameProject != null && managerID!=null && ModelState.IsValid) {
                var saveNameProject = new Project
                {
                    ProjectName = NameProject,
                    ManagerId = managerID,
                   StartDate=DateTime.Now,
                   EndDate=null,
                };
                db.Projects.Add(saveNameProject);
                await db.SaveChangesAsync();
            }
            if (managerID != null)
            {
                var listProject = await db.Projects.Where(p => p.ManagerId == managerID)
               .Select(n => new ProjectModel
               {
                   ProjectName = n.ProjectName,
                   ProjectId = n.ProjectId,
                   CreatedAt=n.CreatedAt,
                   Status=n.Status
               }).ToListAsync();
                return View(listProject);
            }
          return View();
            
        }
        public async Task<IActionResult> CreateTaskProject(int projectId)
        {

            HttpContext.Session.SetInt32("projectId", projectId);
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
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
                         Email =  db.Users.Where(u => u.UserId == pu.UserId).Select(u => u.Email).FirstOrDefault(),
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
            // Lấy dự án hiện tại từ cơ sở dữ liệu dựa trên ProjectId
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

                TempData["SuccessMessage"] = "Cập nhật dự án thành công!";
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
                   Role="View"
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
