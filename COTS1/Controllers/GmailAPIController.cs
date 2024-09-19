using COTS1.Class;
using COTS1.Data;
using COTS1.Models;
using COTS1.Models.EmailModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;

namespace COTS1.Controllers
{
    public class GmailAPIController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly TestNhiemVuContext _dbContext;
        /* private readonly GetMails _getMails;*/

        public GmailAPIController(IHttpContextAccessor contextAccessor, TestNhiemVuContext dbContext  /*GetMails getMails*/)
        {
            _contextAccessor = contextAccessor;
            _dbContext = dbContext;
            /* _getMails = getMails;*/
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Code(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                // Xử lý lỗi nếu không có mã
                return BadRequest("No authorization code provided.");
            }

            var clientId = "425757132188-sf8k8r5bo25f9dsg0sla9so95ljbp8ki.apps.googleusercontent.com";
            var clientSecret = "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";
            var redirectUri = "https://localhost:7242/GmailAPI/Code"; // Thay đổi theo URL redirect của bạn

            try
            {
                var tokenResponse = await ExchangeCodeForTokenAsync(code, clientId, clientSecret, redirectUri);

                if (tokenResponse != null)
                {
                    dynamic json = JsonConvert.DeserializeObject(tokenResponse);

                    string accessToken = json.access_token;
                    string refreshToken = json.refresh_token;

                    // Lưu token vào Session
                    HttpContext.Session.SetString("AccessToken", accessToken);
                    HttpContext.Session.SetString("RefreshToken", refreshToken);

                    // Thông báo thành công
                    ViewBag.Message = "Authorization successful! Access token obtained.";
                }
                else
                {
                    // Thông báo lỗi
                    ViewBag.Message = "Failed to obtain access token.";
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi có sự cố trong quá trình lấy token
                ViewBag.Message = $"An error occurred: {ex.Message}";
            }

            return RedirectToAction("Index", "Home");
        }

        private async Task<string> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            var tokenRequestUrl = "https://oauth2.googleapis.com/token";

            using (var client = new HttpClient())
            {
                var tokenRequestBody = new FormUrlEncodedContent(new[]
                {
            new KeyValuePair<string, string>("code", code),
            new KeyValuePair<string, string>("client_id", clientId),
            new KeyValuePair<string, string>("client_secret", clientSecret),
            new KeyValuePair<string, string>("redirect_uri", redirectUri),
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
        });

                var tokenResponse = await client.PostAsync(tokenRequestUrl, tokenRequestBody);
                var responseString = await tokenResponse.Content.ReadAsStringAsync();

                if (tokenResponse.IsSuccessStatusCode)
                {
                    return responseString;
                }
                else
                {
                    // Xử lý lỗi nếu có
                    throw new HttpRequestException($"Failed to exchange code for token: {responseString}");
                }
            }
        }

        //làm mới token
        public async Task<string> RefreshAccessTokenAsync(string refreshToken, string clientId, string clientSecret)
        {
            var values = new Dictionary<string, string>
     {
         { "client_id", clientId },
         { "client_secret", clientSecret },
         { "refresh_token", refreshToken },
         { "grant_type", "refresh_token" }
     };

            var content = new FormUrlEncodedContent(values);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://oauth2.googleapis.com/token", content);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    dynamic json = JsonConvert.DeserializeObject(responseString);
                    return json.access_token;
                }
                else
                {
                    return null;
                }
            }
        }

        public async Task<ActionResult> GetEmails()
        {
            var currentUserId = HttpContext.Session.GetInt32("UserIDEmail");
            const int MaxResults = 10;
            var accessToken = HttpContext.Session.GetString("AccessToken");
            var refreshToken = HttpContext.Session.GetString("RefreshToken");
            var clientId = "425757132188-sf8k8r5bo25f9dsg0sla9so95ljbp8ki.apps.googleusercontent.com";
            var clientSecret = "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";

            if (string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                accessToken = await RefreshAccessTokenAsync(refreshToken, clientId, clientSecret);
                if (accessToken != null)
                {
                    HttpContext.Session.SetString("AccessToken", accessToken);
                }
                else
                {
                    return BadRequest("Access token is missing.");
                }
            }

            if (currentUserId == null)
            {
                TempData["ErrorMessage"] = "Bạn cần đăng nhập để xem danh sách dự án.";
                return RedirectToAction("Login", "Login");
            }

            // Lấy danh sách các ProjectId mà người dùng hiện tại đang tham gia
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
                      (pu, u) => u.Email)
                .ToListAsync();

            var senders = emails.Distinct().ToArray(); // Loại bỏ các email trùng lặp

            var inboxQuery = $"label:inbox ({string.Join(" OR ", senders.Select(s => $"from:{s}"))})";
            var sentQuery = $"label:sent ({string.Join(" OR ", senders.Select(s => $"to:{s}"))})";

            var inboxApiUrl = $"https://www.googleapis.com/gmail/v1/users/me/messages?q={Uri.EscapeDataString(inboxQuery)}";
            var sentApiUrl = $"https://www.googleapis.com/gmail/v1/users/me/messages?q={Uri.EscapeDataString(sentQuery)}";

            var emailDetails = new MailViewModel
            {
                ReceivedEmails = new List<EmailSummary>(),
                SentEmails = new List<EmailSummary>(),
                TaskEmails = new List<EmailSummary>()
            };

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

                // Lấy email đã nhận và kiểm tra nếu không phải là công việc
                await GetEmailsByUrlAsync(client, inboxApiUrl, emailDetails.ReceivedEmails, MaxResults);
                await GetEmailsByUrlAsync(client, inboxApiUrl, emailDetails.TaskEmails, MaxResults, detail => detail.Subject.StartsWith("Công Việc:"));

                // Lấy email đã gửi
                await GetEmailsByUrlAsync(client, sentApiUrl, emailDetails.SentEmails, MaxResults);
            }

            if (!emailDetails.ReceivedEmails.Any())
            {
                ViewBag.NoReceivedEmails = "No received emails found.";
            }

            if (!emailDetails.SentEmails.Any())
            {
                ViewBag.NoSentEmails = "No sent emails found.";
            }

            return View(emailDetails);
        }

        private async Task GetEmailsByUrlAsync(HttpClient client, string apiUrl, List<EmailSummary> emailList, int maxResults, Func<EmailSummary, bool> filter = null)
        {
            var response = await client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var emailsResponse = JsonConvert.DeserializeObject<EmailListResponse>(responseString);

                if (emailsResponse?.Messages != null && emailsResponse.Messages.Any())
                {
                    var emails = emailsResponse.Messages.Take(maxResults).ToList();
                    var emailDetailsTasks = emails.Select(email => GetEmailDetailsAsync(email.Id, filter)).ToArray();
                    var emailDetails = await Task.WhenAll(emailDetailsTasks);

                    emailList.AddRange(emailDetails.Where(detail => detail != null));
                }
            }
        }

        private async Task<EmailSummary> GetEmailDetailsAsync(string messageId, Func<EmailSummary, bool> filter = null)
        {
            var accessToken = HttpContext.Session.GetString("AccessToken");
            string apiUrl = $"https://www.googleapis.com/gmail/v1/users/me/messages/{messageId}";

            using (var client = new HttpClient())
            {
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                var response = await client.GetAsync(apiUrl);
                var responseString = await response.Content.ReadAsStringAsync();

                if (response.IsSuccessStatusCode)
                {
                    var email = JsonConvert.DeserializeObject<EmailMessageJson>(responseString);

                    // Lấy thông tin tiêu đề, người gửi và ngày gửi
                    var sender = email.Payload?.Headers?.FirstOrDefault(h => h.Name == "From")?.Value;
                    var subject = email.Payload?.Headers?.FirstOrDefault(h => h.Name == "Subject")?.Value;
                    var sentDateStr = email.Payload?.Headers?.FirstOrDefault(h => h.Name == "Date")?.Value;

                    var bodyContents = new List<string>();
                    var attachments = new List<AttachmentInfo>();

                    // Xử lý nội dung email trong các phần Body và Parts
                    if (email.Payload?.Body?.Data != null)
                    {
                        // Giải mã phần body nếu không có Parts
                        var decodedBody = DecodeBase64Url(email.Payload.Body.Data);
                        bodyContents.Add(decodedBody);
                    }

                    if (email.Payload?.Parts != null)
                    {
                        // Duyệt qua các phần Parts của email
                        foreach (var part in email.Payload.Parts)
                        {
                            if (part.MimeType == "text/plain" || part.MimeType == "text/html")
                            {
                                // Giải mã và thêm phần nội dung
                                if (part.Body?.Data != null)
                                {
                                    var decodedPart = DecodeBase64Url(part.Body.Data);
                                    bodyContents.Add(decodedPart);
                                }
                            }

                            // Nếu có tệp đính kèm
                            if (!string.IsNullOrEmpty(part.Filename) && part.Body?.Data != null)
                            {
                                attachments.Add(new AttachmentInfo
                                {
                                    FileName = part.Filename,
                                    MimeType = part.MimeType,
                                    Data = part.Body.Data
                                });
                            }
                        }
                    }

                    // Định dạng ngày gửi
                    DateTime sentDate;
                    if (!DateTime.TryParse(sentDateStr, out sentDate))
                    {
                        // Nếu không chuyển đổi được, gán giá trị mặc định hoặc xử lý lỗi
                        sentDate = DateTime.Now; // Hoặc sử dụng giá trị mặc định khác
                    }

                    var emailSummary = new EmailSummary
                    {
                        Id = email.Id,
                        Sender = sender,
                        Subject = subject,
                        SentDate = sentDate,
                        BodyContents = bodyContents,
                        Attachments = attachments
                    };

                    // Đánh dấu email đã đọc
                    await GoogleUserInfo.MarkEmailAsReadAsync(messageId, accessToken);

                    // Kiểm tra điều kiện lọc nếu có
                    if (filter == null || filter(emailSummary))
                    {
                        return emailSummary;
                    }
                }
                else
                {
                    throw new Exception("Failed to retrieve email details.");
                }
            }

            return null;
        }

        private string DecodeBase64(string base64Data)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64Data);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public string DecodeBase64Url(string base64Url)
        {
            // Base64 URL-safe thường thay thế '+' bằng '-', '/' bằng '_', và không có padding '='
            string base64 = base64Url.Replace('-', '+').Replace('_', '/');

            // Thêm padding nếu cần thiết
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }

            var data = Convert.FromBase64String(base64);
            return Encoding.UTF8.GetString(data);
        }

        public async Task<ActionResult> ShowEmailDetails(string messageId, string CheckTask)
        {
            var emailSummary = await GetEmailDetailsAsync(messageId);
            var emailTitle = $"{emailSummary.Subject}";
            var emailSender = $"{emailSummary.Sender}";

            // Kết hợp nội dung email
            var emailBody = string.Join("\n", emailSummary.BodyContents);
            var emailContent = $"{emailBody}";

            // Phân tích email
            var taskViewModel = AnalyzeEmail(emailContent, emailTitle, emailSender, emailSummary.SentDate);
            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");
            var googleUserInfo = new GoogleUserInfo(accessToken);
            var email = await googleUserInfo.GetUserEmailAsync();
            var combinedModel = new ShowEmailDetail_And_Task
            {
                Email = emailSummary,
                Task = taskViewModel
            };
            ViewBag.UserEmail = email;
            ViewBag.CheckTask = CheckTask;
            return View(combinedModel);
        }

        /* private Email ParseEmailJson(string jsonString)
         {
             return JsonConvert.DeserializeObject<Email>(jsonString);
         }*/

        public async Task<IActionResult> ViewEmailNotification()
        {
            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");
            var googleUserInfo = new GoogleUserInfo(accessToken);
            var email = await googleUserInfo.GetUserEmailAsync();

            ViewBag.UserEmail = email;
            return View();
        }

        public async Task<IActionResult> EmailNotification(string recipients, string subject, string message, string from, string type)
        {
            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");

            /* if (string.IsNullOrEmpty(accessToken))
             {
                 return RedirectToAction("Authenticate", "Account"); // Redirect to login/authentication page
             }*/

            var sendNotificationMail = new SendNotificationMail(accessToken);

            // Tách các địa chỉ email bằng dấu phẩy
            var emailAddresses = recipients.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var email in emailAddresses)
            {
                await SendNotificationMail.SendEmailAsync(sendNotificationMail._gmailService, from.Trim(), email.Trim(), subject, message);
            }
            TempData["Message"] = "Gửi Email thành công!";

            if (type != null)
            {
                return RedirectToAction("ShowEmailDetails");
            }

            return RedirectToAction("ViewEmailNotification");
        }

        //nhận nhiệm vụ
        public TaskViewModel AnalyzeEmail(string emailContent, string title, string sender, DateTime sentDay)
        {
            var details = new TaskViewModel
            {
                Title = title ?? "Không có tiêu đề",
                Sender = sender.ToString(),
                SentDay = sentDay
            };

            // Xử lý mô tả
            var cleanedDescriptionText = Regex.Replace(emailContent, @"Ngày hết hạn:.*?Mức độ ưu tiên:.*", "").Trim();
            details.Description = ParseDescriptions(cleanedDescriptionText);

            // Xử lý ghi chú

            var notesMatch = Regex.Match(emailContent, @"Ghi chú:\s*(.+)");
            details.Notes = notesMatch.Success ? notesMatch.Groups[1].Value.Trim() : "Không có ghi chú";

            // Xử lý ngày hết hạn và mức độ ưu tiên
            var dueDateMatch = Regex.Match(emailContent, @"Ngày hết hạn:\s*(\d{2}/\d{2}/\d{4})");
            details.DueDate = dueDateMatch.Success ? DateTime.ParseExact(dueDateMatch.Groups[1].Value.Trim(), "dd/MM/yyyy", null) : DateTime.Now.AddDays(7);

            var priorityMatch = Regex.Match(emailContent, @"Mức độ ưu tiên:\s*(.+)");
            details.Priority = priorityMatch.Success ? priorityMatch.Groups[1].Value.Trim() : "Không có";

            // Xử lý người nhận
            var recipientsMatch = Regex.Match(emailContent, @"Người nhận:\s*(.+)");
            details.Recipients = recipientsMatch.Success ? recipientsMatch.Groups[1].Value.Trim() : "Không có người nhận";

            // Xử lý trạng thái mặc định
            details.Status = "Chưa bắt đầu";

            return details;
        }

        private List<string> ParseDescriptions(string descriptionText)
        {
            var descriptions = new List<string>();

            // Loại bỏ các phần không mong muốn
            var cleanedDescriptionText = Regex.Replace(descriptionText, @"Ngày hết hạn:.*|Mức độ ưu tiên:.*|Ghi chú:.*", "").Trim();

            // Tách các công việc chính và phụ
            var mainTasks = Regex.Split(cleanedDescriptionText, @"(?=\s+Công việc \d+:)").Where(task => !string.IsNullOrWhiteSpace(task)).ToList();

            foreach (var task in mainTasks)
            {
                var trimmedTask = task.Trim();
                var lines = trimmedTask.Split('\n').Select(line => line.Trim()).Where(line => !string.IsNullOrEmpty(line)).ToList();

                // Thêm công việc chính vào danh sách
                var mainTaskTitle = lines.FirstOrDefault();
                if (mainTaskTitle != null)
                {
                    descriptions.Add(mainTaskTitle);
                }

                // Thêm công việc phụ vào danh sách với dấu cộng
                for (int i = 1; i < lines.Count; i++)
                {
                    descriptions.Add(lines[i]);
                }
            }

            // Thêm ghi chú

            return descriptions;
        }

        //save task

        [HttpPost]
        public async Task<IActionResult> SaveTask(string Title, string Description, DateTime DueDate, string Priority, string Note, string Sender)
        {
            // Lấy thông tin người dùng
            var AssignedTo = HttpContext.Session.GetString("UserEmail");
            var manager = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == Sender);
            var Assigned = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == AssignedTo);

            if (ModelState.IsValid)
            {
                // Tạo đối tượng nhiệm vụ mới
                var task = new SaveTask
                {
                    Title = Title,
                    Note = Note,
                    Description = Description,
                    DueDate = DueDate,
                    Priority = Priority,
                    Status = "Đang thực hiện",
                    CreatedAt = DateTime.Now,
                    AssignedTo = Assigned?.UserId, // Id của người nhận nhiệm vụ
                    CreatedBy = manager?.UserId // Id của người tạo nhiệm vụ
                };

                // Thêm nhiệm vụ vào cơ sở dữ liệu
                _dbContext.SaveTasks.Add(task);
                await _dbContext.SaveChangesAsync(); // Lưu nhiệm vụ và lấy TaskId

                // Lấy TaskId của nhiệm vụ vừa tạo
                var taskId = task.TaskId;

                // Tách các công việc con từ mô tả và lưu vào bảng Subtasks
                var subtasks = SplitTasks(Description);
                foreach (var subtaskViewModel in subtasks)
                {
                    var subtask = new Subtask
                    {
                        TaskId = taskId, // Sử dụng TaskId của nhiệm vụ vừa tạo
                        Title = subtaskViewModel.Title,
                        Description = subtaskViewModel.Description,
                        Status = subtaskViewModel.Status
                    };

                    _dbContext.Subtasks.Add(subtask); // Lưu từng subtask vào CSDL
                }

                await _dbContext.SaveChangesAsync(); // Lưu các subtasks

                // Thông báo thành công
                TempData["SuccessMessage"] = "Nhiệm vụ đã được lưu thành công!";
                return RedirectToAction("Index", "ListTask"); // Chuyển hướng tới danh sách nhiệm vụ hoặc một trang khác
            }

            // Nếu có lỗi, trả về form
            return View("ShowEmailDetails");
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