using COTS1.Class;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using Org.BouncyCastle.Cms;

namespace COTS1.Controllers
{
    public class TasksController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public TasksController(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public async Task<IActionResult> CreateTasks()
        {
            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Mail", "Home"); // Redirect to login/authentication page
            }
            var googleUserInfo = new GoogleUserInfo(accessToken);
            var email = await googleUserInfo.GetUserEmailAsync();
            ViewBag.UserEmail = email;
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> SendEmailTasks(string recipients, string Title, string Description, DateTime DueDate, string Priority, string from, string type)
        {
            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
                return RedirectToAction("Mail", "Home"); // Redirect to login/authentication page
            }

            var sendNotificationMail = new SendNotificationMail(accessToken);

            // Tách các địa chỉ email bằng dấu phẩy
            var emailAddresses = recipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
            var message = "Mô tả:\n" + Description +"\n"+ "Ngày hết hạn:" + DueDate + "\n" + "Mức độ ưu tiên:" + Priority+"\n";
            var NameTask = "Công Việc:" + Title;
            foreach (var email in emailAddresses)
            {
                await SendNotificationMail.SendEmailAsync(sendNotificationMail._gmailService, from.Trim(), email.Trim(), NameTask, message);
            }
            TempData["Message"] = "Gửi Email thành công!";
            return RedirectToAction("CreateTasks");
        }
        public IActionResult ReceiveTask(string Title, string Description, string DueDate, string Priority, string status)
        {
            var taskViewModel = new TaskViewModel
            {
                Title = Title,
                Description = Description.Split(',').ToList(), // Convert the comma-separated string back to a list
                DueDate = DateTime.ParseExact(DueDate, "dd/MM/yyyy", null),
                Priority = Priority,
                Status = status
            };

            return View();
        }


    }
}
