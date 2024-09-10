using COTS1.Class;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Cms;
using System.Net.Mail;
using System.Net;
using System.Web;
using COTS1.Data;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Crypto.Generators;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using Microsoft.AspNetCore.Http;

namespace COTS1.Controllers
{
    public class LoginController : Controller
	{
		private readonly IHttpContextAccessor _contextAccessor;
		private readonly TestNhiemVuContext db;

		
	
		/* private readonly GetMails _getMails;*/
		public LoginController(IHttpContextAccessor contextAccessor, TestNhiemVuContext context)
		{
			_contextAccessor = contextAccessor;
			db = context;
			/* _getMails = getMails;*/
		}
		public IActionResult Login()
        {
            return View();
        }
        public async Task<IActionResult> LoginFunctionAsync(string gmail,string password)
        {
            if (string.IsNullOrEmpty(gmail))
            {
                return BadRequest("Gmail address is required.");
            }
            var user = await db.Users.FirstOrDefaultAsync(u => u.Email == gmail);
            //get User from databse
        
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
               var DataUser=db.Users.Where(u=>u.Email== gmail).Select(u=> new
			   {
                   u.UserId,
                   u.FullName,
                   u.Email,      
                   u.Role
               }).FirstOrDefault();

                HttpContext.Session.SetString("UserEmail", user.Email);
                HttpContext.Session.SetInt32("UserIDEmail", user.UserId);
                HttpContext.Session.SetString("UserRole", user.Role);
                HttpContext.Session.SetString("UserFullName", user.FullName);
				
                // Xác thực thành công
                // Lưu thông tin người dùng vào session hoặc thực hiện các bước xác thực khác
                var clientId = "425757132188-sf8k8r5bo25f9dsg0sla9so95ljbp8ki.apps.googleusercontent.com";
                var redirectUri = "https://localhost:7242/GmailAPI/Code";
                //var scope = "https://mail.google.com";
                var scope = "https://mail.google.com/ https://www.googleapis.com/auth/userinfo.email https://www.googleapis.com/auth/userinfo.profile";

                var responseType = "code";
                var accessType = "offline";
                var includeGrantedScopes = "true";
                var state = Guid.NewGuid().ToString();
                var authUrl = "https://accounts.google.com/o/oauth2/v2/auth?" +
                    "scope=" + HttpUtility.UrlEncode(scope) +
                    "&access_type=" + HttpUtility.UrlEncode(accessType) +
                    "&include_granted_scopes=" + HttpUtility.UrlEncode(includeGrantedScopes) +
                    "&response_type=" + HttpUtility.UrlEncode(responseType) +
                    "&client_id=" + HttpUtility.UrlEncode(clientId) +
                    "&redirect_uri=" + HttpUtility.UrlEncode(redirectUri) +
                    "&state=" + HttpUtility.UrlEncode(state) +
                    "&login_hint=" + HttpUtility.UrlEncode(gmail); // Thêm email vào URL

                return Redirect(authUrl);
               
               
            }
            else
            {
                ViewBag.message = "Sai Email hoặc mật khẩu";
                return View("Login");
            }
            
        }
		public IActionResult Register()
		{
			return View();
		}
		public async Task<IActionResult> RegisterSaveData(string userName, string gmail, string password, string passwordConfirm)
		{
			if (password != passwordConfirm)
			{
				TempData["Error"] = "Mật khẩu xác nhận không khớp.";
				return RedirectToAction("Register");
			}

			// Băm mật khẩu
			string salt;
			string hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);

            // Tạo đối tượng người dùng
            var user = new User
			{
				FullName = userName,
				Email = gmail,
				PasswordHash = hashedPassword,
				PasswordSalt = "null",
				Role = "Member", // Ví dụ vai trò, bạn có thể thay đổi
				CreatedAt = DateTime.Now
			};

			// Lưu vào cơ sở dữ liệu
			using (var context = new TestNhiemVuContext())
			{
				context.Users.Add(user);
				await context.SaveChangesAsync();
                return await SendVerificationEmail(gmail);
            }

			// Gửi mã xác thực qua email
		
		}


		public async Task<IActionResult> SendVerificationEmail(string gmail)
		{
			// Tạo tiêu đề và mã xác nhận
			string subject = "Mã xác nhận của bạn từ COTS";
			Random random = new Random();
			int verificationCode = random.Next(1000, 9999);
			HttpContext.Session.SetInt32("verifycode", verificationCode);

			// Nội dung email (HTML hoặc văn bản thường)
			string body = $"<h2 style='text-align: center;'>Mã xác nhận của bạn là: {verificationCode}</h2>";

			// Gửi email qua SMTP với mật khẩu ứng dụng
			var smtpClient = new SmtpClient("smtp.gmail.com")
			{
				Port = 587,
				Credentials = new NetworkCredential("quanlynhiemvuemployee@gmail.com", "tqoz nqyw unrh xsjz"),
				EnableSsl = true,
			};

			var mailMessage = new MailMessage
			{
				From = new MailAddress("quanlynhiemvuemployee@gmail.com"),
				Subject = subject,
				Body = body,
				IsBodyHtml = true,
			};

			mailMessage.To.Add(gmail);

			try
			{
				await smtpClient.SendMailAsync(mailMessage);
				
				return RedirectToAction("VerifyCode");
			}
			catch (Exception ex)
			{
				TempData["Error"] = $"Lỗi khi gửi mã: {ex.Message}";
				return RedirectToAction("Register");
			}
		}

		public IActionResult VerifyCode()
		{
			return View();
		}

        public IActionResult CheckVerifyCode(int code1, int code2, int code3, int code4)
		{
			string enteredCode = $"{code1}{code2}{code3}{code4}";

			// Lấy mã xác nhận đã lưu (từ Session hoặc CSDL)
			int? storedCode = HttpContext.Session.GetInt32("verifycode");

			if (storedCode != null && storedCode.ToString() == enteredCode)
			{
				// Xác thực thành công
			
				// Xóa mã khỏi Session sau khi xác thực
				HttpContext.Session.Remove("VerificationCode");
				ViewBag.message = "Bạn đã đăng kí thành công";
				return View("Login");
			}
			else
			{
				// Xác thực thất bại
				ViewBag.message = "Mã xác nhận không đúng!";
				return View("VerifyCode");
			}
		}
	}
}
