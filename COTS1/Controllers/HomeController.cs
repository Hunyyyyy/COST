using COTS1.Class;
using COTS1.Data;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Web;

namespace COTS1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TestNhiemVuContext _dbContext;
        public HomeController(ILogger<HomeController> logger,IHttpContextAccessor contextAccessor, TestNhiemVuContext dbContext)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
        }

        public async Task<IActionResult> Index()
        {

            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
            if (currentUserId == null)
            {
                return RedirectToAction("Login", "Login"); // Điều hướng đến trang đăng nhập nếu không có UserId
            }

            var userProjects = await _dbContext.ProjectUsers
                .Where(pu => pu.UserId == currentUserId)
                .Select(pu => pu.ProjectId)
                .ToListAsync();

            // Lấy danh sách email của tất cả người dùng trong các dự án đó
            var emails = await _dbContext.ProjectUsers
                .Where(pu => userProjects.Contains(pu.ProjectId))
                .Join(_dbContext.Users,
                      pu => pu.UserId,
                      u => u.UserId,
                      (pu, u) => new { u.Email })
                .Select(x => x.Email)
                .ToListAsync();


            var reminders = await _dbContext.Reminders
                    .Where(r => r.UserId == currentUserId)
                    .Include(r => r.Project)
                    .ToListAsync();

            var reminderViewModels = reminders.Select(r => new ReminderViewModel
            {
                ReminderId = r.ReminderId,
                ProjectId = r.ProjectId,
                UserId = r.UserId,
                ReminderContent = r.ReminderContent,
                ReminderDate = r.ReminderDate,
                IsAcknowledged = r.IsAcknowledged,
                CreatedAt = r.CreatedAt,
                Project = r.Project,
                User = r.User,
                DaysRemaining = (r.ReminderDate - DateTime.Now).Days,
                ProjectName = r.Project.ProjectName,
                Status = (r.ReminderDate < DateTime.Now) ? "Đã quá hạn" : $"Còn {(r.ReminderDate - DateTime.Now).Days} ngày",
                EndDate = r.Project.EndDate,
                TaskReminders = new List<TaskReminderViewModel>() // You can populate this if needed
            }).ToList();

            ViewData["Reminders"] = reminderViewModels;

            return View(reminderViewModels);
          
        }
        public async Task<int> GetSumUnreadEmails(int currentUserId, string accessToken)
        {
            // Lấy danh sách các dự án mà người dùng thuộc về
            var userProjects = await _dbContext.ProjectUsers
                .Where(pu => pu.UserId == currentUserId)
                .Select(pu => pu.ProjectId)
                .ToListAsync();

            // Lấy danh sách email của tất cả người dùng trong các dự án đó
            var emails = await _dbContext.ProjectUsers
                .Where(pu => userProjects.Contains(pu.ProjectId))
                .Join(_dbContext.Users,
                      pu => pu.UserId,
                      u => u.UserId,
                      (pu, u) => new { u.Email })
                .Select(x => x.Email)
                .ToListAsync();

            // Chuyển danh sách emails thành mảng senders
            var senders = emails.ToArray();
            var unreadEmails = await GoogleUserInfo.GetUnreadEmailCountFromSendersAsync(senders, accessToken);
            int sumEmail = unreadEmails.Values.Sum(); // Tổng số email chưa đọc

            return sumEmail;
        }


        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");

            if (currentUserId != null && !string.IsNullOrEmpty(accessToken))
            {
                ViewBag.SumEmail = await GetSumUnreadEmails((int)currentUserId, accessToken);
            }
            else
            {
                ViewBag.SumEmail = 0; // Nếu không có UserId hoặc accessToken thì không có email chưa đọc
            }

            // Tiếp tục thực hiện action
            await next();
        }

        public IActionResult Mail()
        {
            // Ki?m tra xem token có t?n t?i trong session không
          AccessToken a = new AccessToken(_contextAccessor);
    string tokenOrRedirectUrl = a.CheckToken();

    // Nếu chưa có token, chuyển hướng đến trang xác thực
    if (tokenOrRedirectUrl.StartsWith("https://"))
    {
        return Redirect(tokenOrRedirectUrl);
    }
            return RedirectToAction("GetEmails", "GmailAPI");
        }
        public IActionResult SendNotification()
        {
            // Ki?m tra xem token có t?n t?i trong session không
            AccessToken a = new AccessToken(_contextAccessor);
            string tokenOrRedirectUrl = a.CheckToken();

            // Nếu chưa có token, chuyển hướng đến trang xác thực
            if (tokenOrRedirectUrl.StartsWith("https://"))
            {
                return Redirect(tokenOrRedirectUrl);
            }
            return RedirectToAction("ViewEmailNotification", "GmailAPI");
        }
      

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
