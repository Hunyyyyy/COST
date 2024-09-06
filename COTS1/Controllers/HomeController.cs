using COTS1.Class;
using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using System.Web;

namespace COTS1.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpContextAccessor _contextAccessor;

        public HomeController(ILogger<HomeController> logger,IHttpContextAccessor contextAccessor)
        {
            _logger = logger;
            _contextAccessor = contextAccessor;
        }

        public IActionResult Index()
        {
            return View();
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
