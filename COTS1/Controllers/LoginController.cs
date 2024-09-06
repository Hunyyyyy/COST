using Microsoft.AspNetCore.Mvc;

namespace COTS1.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
        public IActionResult Regigter()
        {
            
            //nhap gamil-->check mail
            //checkmail==true -->tao tai khoan thanh cong
            //false-->mail ko hop le
            //tạo pass
            //pass
            //return RedirectToAction("Index","Home");
            return View();
        }
        public IActionResult Forgotpass()
        {
            return View();
        }
    }
}
