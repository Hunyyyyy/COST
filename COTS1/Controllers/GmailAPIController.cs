using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using COTS1.Class;
using System.Text.RegularExpressions;
using COTS1.Models.EmailModel;
using Microsoft.EntityFrameworkCore;
using COTS1.Data;




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

            var senders = new[] { "ngonhathuy501@gmail.com", "ngonhat501@gmail.com" };
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

                // Xử lý lấy email (nhận hoặc gửi)
                async Task GetEmailsByUrl(string apiUrl, List<EmailSummary> emailList, Func<EmailSummary, bool> filter = null)
                {
                    var response = await client.GetAsync(apiUrl);
                    if (response.IsSuccessStatusCode)
                    {
                        var responseString = await response.Content.ReadAsStringAsync();
                        var emailsResponse = JsonConvert.DeserializeObject<EmailListResponse>(responseString);

                        if (emailsResponse?.Messages != null && emailsResponse.Messages.Any())
                        {
                            var emails = emailsResponse.Messages.Take(MaxResults).ToList();
                            foreach (var email in emails)
                            {
                                var detail = await GetEmailDetails(email.Id);
                                if (filter == null || filter(detail))
                                {
                                    emailList.Add(detail);
                                }
                            }
                        }
                    }
                }

                // Lấy email đã nhận và kiểm tra nếu không phải là công việc
                await GetEmailsByUrl(inboxApiUrl, emailDetails.ReceivedEmails, detail => !detail.Subject.StartsWith("Công Việc:"));

                // Lấy email công việc và cập nhật ViewBag.CheckTask nếu có
                await GetEmailsByUrl(inboxApiUrl, emailDetails.TaskEmails, detail =>
                {
                    if (detail.Subject.StartsWith("Công Việc:"))
                    {
                        ViewBag.CheckTask = "Task";  // Cập nhật ViewBag nếu tìm thấy email công việc
                        return true;
                    }
                    return false;
                });

                // Lấy email đã gửi
                await GetEmailsByUrl(sentApiUrl, emailDetails.SentEmails);
            }

            // Trường hợp không có email nhận hoặc gửi
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



        public async Task<EmailSummary> GetEmailDetails(string messageId)
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
                    string formattedSentDate = null;
                    if (DateTime.TryParse(sentDateStr, out sentDate))
                    {
                        formattedSentDate = sentDate.ToString("dd/MM/yyyy HH:mm");
                    }

                    // Trả về đối tượng EmailSummary
                    return new EmailSummary
                    {
                        Id = email.Id,
                        Sender = sender,
                        Subject = subject,
                        SentDate = formattedSentDate,
                        BodyContents = bodyContents,
                        Attachments = attachments
                    };
                }
                else
                {
                    throw new Exception("Failed to retrieve email details.");
                }
            }
        }

        string DecodeBase64(string base64Data)
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



        public async Task<ActionResult> ShowEmailDetails(string messageId,string CheckTask)
        {
            var emailSummary = await GetEmailDetails(messageId);
            var emailTitle = $"{emailSummary.Subject}";
            var emailSender = $"Người gửi: {emailSummary.Sender}";
            var emailSentDate = $"Ngày gửi: {emailSummary.SentDate}";

            // Kết hợp nội dung email
            var emailBody = string.Join("\n", emailSummary.BodyContents);
            var emailContent = $"{emailSender}\n{emailSentDate}\n{emailBody}";

            // Phân tích email
            var taskViewModel = AnalyzeEmail(emailContent, emailTitle);
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
       public async Task<IActionResult> EmailNotification(string recipients, string subject, string message,string from,string type)
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

            if (type!=null){
                return RedirectToAction("ShowEmailDetails");
            }
           


            return RedirectToAction("ViewEmailNotification");
        }
        //nhận nhiệm vụ
        public TaskViewModel AnalyzeEmail(string emailContent,string title)
        {
            var details = new TaskViewModel
            {
                 Title = title ?? "Không có tiêu đề"
            };

            // Phân tích tiêu đề
           
            /*var titleMatch = Regex.Match(emailContent, @"Công Việc:\s*(.+?)\s*(?:From:|$)");

            details.Title = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : "Không có tiêu đề";*/

            // Phân tích mô tả
            var descriptionMatch = Regex.Match(emailContent, @"Mô tả:\s*([\s\S]+?)\s*Ngày hết hạn:");
            var descriptionText = descriptionMatch.Success ? descriptionMatch.Groups[1].Value.Trim() : "Không có mô tả";
            details.Description = ParseDescriptions(descriptionText);

            // Phân tích ngày hết hạn
            var dueDateMatch = Regex.Match(emailContent, @"Ngày hết hạn:\s*(\d{2}/\d{2}/\d{4})");
            details.DueDate = dueDateMatch.Success ? DateTime.ParseExact(dueDateMatch.Groups[1].Value.Trim(), "dd/MM/yyyy", null) : DateTime.Now.AddDays(7);

            // Phân tích mức độ ưu tiên
            var priorityMatch = Regex.Match(emailContent, @"Mức độ ưu tiên:\s*(.+)");
            details.Priority = priorityMatch.Success ? priorityMatch.Groups[1].Value.Trim() : "Không có";

            // Phân tích người nhận
            var recipientsMatch = Regex.Match(emailContent, @"Người nhận:\s*(.+)");
            details.Recipients = recipientsMatch.Success ? recipientsMatch.Groups[1].Value.Trim() : "Không có người nhận";

            // Phân tích trạng thái
            details.Status = "Chưa bắt đầu";

            return details;
        }
        //save task
        [HttpPost]
        public async Task<IActionResult> SaveTask(string Title,string Description,DateTime DueDate,string Priority)
        {
            if (ModelState.IsValid)
            {
                // Tạo đối tượng nhiệm vụ mới
                var task = new SaveTasks
                {
                    Title =Title,
                    Description = string.Join(", ", Description),
                    DueDate = DueDate,
                    Priority =Priority,
                    Status = "Đang thực hiện",
                    CreatedAt = DateTime.Now,
                    AssignedTo = 3/* Id của người nhận (bạn có thể cần lấy từ model hoặc người dùng hiện tại) */,
                    CreatedBy = 2/* Id của người tạo (quản lý) */
        };

                // Thêm nhiệm vụ vào cơ sở dữ liệu
                _dbContext.Tasks.Add(task);
                await _dbContext.SaveChangesAsync();
                // Tách các công việc con từ mô tả và lưu vào bảng Subtasks
                var subtasks = SplitTasks(task.Description);
                foreach (var subtaskViewModel in subtasks)
                {
                    var subtask = new Subtask
                    {
                        TaskId = task.TaskId, // Sử dụng TaskId của nhiệm vụ vừa tạo
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
            // Giả sử các subtasks được phân tách bằng dấu xuống dòng hoặc dấu chấm phẩy
            var subtasks = taskDescription
                .Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(subtask => new SubtaskViewModel
                {
                    Title = subtask.Trim(),
                    Description = subtask.Trim(),
                    Status = "Chưa nhận"
                }).ToList();

            return subtasks;
        }

        private List<string> ParseDescriptions(string descriptionText)
        {
            var descriptions = new List<string>();

            // Tách các công việc theo định dạng "Công việc N:"
            var matches = Regex.Matches(descriptionText, @"Công việc \d+:\s*([^\d]+)(?=\s*Công việc \d+:|$)");

            foreach (Match match in matches)
            {
                if (match.Success)
                {
                    descriptions.Add(match.Groups[1].Value.Trim());
                }
            }

            return descriptions;
        }

       




    }
}
