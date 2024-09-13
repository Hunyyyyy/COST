﻿using COTS1.Class;
using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Cms;

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
            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Mail", "Home"); // Redirect to login/authentication page
            }
            var googleUserInfo = new GoogleUserInfo(accessToken);
            var email = await googleUserInfo.GetUserEmailAsync();
            var project = await _dbContext.Projects.Where(p => p.ProjectId == projectId)
                .Select(p => new Project
                {
                    ProjectName = p.ProjectName,
                    ProjectId = p.ProjectId,
                }).ToListAsync();


            if (project != null)
            {
                ViewBag.UserEmail = email;
                ViewBag.ProjectId = projectId;
                return View(project);
            }
            else
            {
                return View();
            }
        }
       
        [HttpPost]
        public async Task<IActionResult> SaveTask(string Title, string Description, DateTime DueDate, string Priority, string? Note, string from, int ProjectId)
        {
            var manager = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == from);

            // Nếu Note là null, gán giá trị mặc định
            string CheckNote = string.IsNullOrEmpty(Note) ? "Không có ghi chú" : Note;

            if (ModelState.IsValid)
            {
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
                    CreatedAt = DateTime.Now
                };

                _dbContext.SaveTasks.Add(saveTask);
                await _dbContext.SaveChangesAsync();

                TempData["SuccessMessage"] = "Nhiệm vụ đã được lưu thành công!";
                return RedirectToAction("CreateTaskProject", "ProjectManager", new { projectId = ProjectId });
            }

            return View("CreateTasks", "Tasks");
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
