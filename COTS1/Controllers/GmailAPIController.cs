﻿using COTS1.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using COTS1.Class;
using System.Text.RegularExpressions;




namespace COTS1.Controllers
{
    public class GmailAPIController : Controller
    {
        private readonly IHttpContextAccessor _contextAccessor;
       /* private readonly GetMails _getMails;*/
        public GmailAPIController(IHttpContextAccessor contextAccessor  /*GetMails getMails*/)
        {
            _contextAccessor = contextAccessor;
           /* _getMails = getMails;*/
        }
        public IActionResult Index()
        {
            return View();
        }
       
        public async Task<ActionResult> Code(string code, string state)
        {
            if (string.IsNullOrEmpty(code))
            {
                // Xử lý lỗi nếu không có mã
                //return new HttpStatusCodeResult(400, "No authorization code provided.");
            }
           
            var clientId = "425757132188-sf8k8r5bo25f9dsg0sla9so95ljbp8ki.apps.googleusercontent.com";
            var clientSecret = "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";
            var redirectUri = "https://localhost:7242/GmailAPI/Code";

            
            var tokenResponse = await ExchangeCodeForTokenAsync(code, clientId, clientSecret, redirectUri);                   
            if (tokenResponse != null)
            {
                dynamic json = JsonConvert.DeserializeObject(tokenResponse);

                string accessToken = json.access_token;
                string refreshToken = json.refresh_token;                
                HttpContext.Session.SetString("AccessToken", accessToken);
                HttpContext.Session.SetString("RefreshToken", refreshToken);                
                ViewBag.Message = "Authorization successful! Access token obtained.";
            }
            else
            {
                ViewBag.Message = "Failed to obtain access token.";
            }

            return RedirectToAction("Index", "Home");
        }

        // Phương thức để trao đổi mã lấy mã thông báo
        //Đoạn mã này thực hiện việc trao đổi mã xác thực (authorization code)
        //lấy mã thông báo truy cập (access token) từ Google OAuth 2.0 server.
        //Đây là một phần quan trọng của quy trình OAuth 2.0, cho phép ứng dụng
        //của bạn nhận được mã thông báo truy cập sau khi người dùng cấp quyền.
        private async Task<dynamic> ExchangeCodeForTokenAsync(string code, string clientId, string clientSecret, string redirectUri)
        {
            var values = new Dictionary<string, string>
        {
                //code: Mã xác thực (authorization code) nhận được từ bước trước đó
                //khi người dùng đăng nhập và cấp quyền.
            { "code", code },
            //client_id: ID của ứng dụng đã đăng ký với Google.
            { "client_id", clientId },
            //client_secret: Khóa bí mật của ứng dụng, cũng được cung cấp khi đăng ký
            //với Google.
            { "client_secret", clientSecret },
            //redirect_uri: URL mà bạn đã chỉ định để nhận phản hồi từ Google.
            { "redirect_uri", redirectUri },
            //grant_type: Được thiết lập là "authorization_code", cho biết rằng bạn
            //đang yêu cầu mã thông báo dựa trên mã xác thực.
            { "grant_type", "authorization_code" }
        };
            //FormUrlEncodedContent(values): Tạo nội dung cho yêu cầu HTTP POST,
            //với các tham số được mã hóa theo định dạng URL-encoded.
            var content = new FormUrlEncodedContent(values);

            using (var client = new HttpClient())
            {
                var response = await client.PostAsync("https://oauth2.googleapis.com/token", content);
                var responseString = await response.Content.ReadAsStringAsync();

                // Xử lý responseString để lấy thông tin mã thông báo
                return response.IsSuccessStatusCode ? responseString : null;
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

                // Lấy email đã nhận
                var inboxResponse = await client.GetAsync(inboxApiUrl);
                if (inboxResponse.IsSuccessStatusCode)
                {
                    var responseString = await inboxResponse.Content.ReadAsStringAsync();
                    var emailsResponse = JsonConvert.DeserializeObject<EmailListResponse>(responseString);
                    var emails = emailsResponse.Messages.Take(MaxResults).ToList();
                    foreach (var email in emails)
                    {
                        var detail = await GetEmailDetails(email.Id);
                        if (detail.Subject != null && !detail.Subject.StartsWith("Công Việc:"))
                        {
                            emailDetails.ReceivedEmails.Add(detail);
                            ViewBag.CheckTask="Task";
                        }
                    }
                }
                //lay mail cong viec
                var taskResponse = await client.GetAsync(inboxApiUrl);
                if (inboxResponse.IsSuccessStatusCode)
                {
                    var responseString = await inboxResponse.Content.ReadAsStringAsync();
                    var emailsResponse = JsonConvert.DeserializeObject<EmailListResponse>(responseString);
                    var emails = emailsResponse.Messages.Take(MaxResults).ToList();
                    foreach (var email in emails)
                    {
                        var detail = await GetEmailDetails(email.Id);
                        if (detail.Subject != null && detail.Subject.StartsWith("Công Việc:"))
                        {
                            emailDetails.TaskEmails.Add(detail);
                        }
                    }
                }
                // Lấy email đã gửi
                var sentResponse = await client.GetAsync(sentApiUrl);
                if (sentResponse.IsSuccessStatusCode)
                {
                    var responseString = await sentResponse.Content.ReadAsStringAsync();
                    var emailsResponse = JsonConvert.DeserializeObject<EmailListResponse>(responseString);
                    var emails = emailsResponse.Messages.Take(MaxResults).ToList();
                    foreach (var email in emails)
                    {
                        var detail = await GetEmailDetails(email.Id);
                        emailDetails.SentEmails.Add(detail);
                    }
                }
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
                    var email = JsonConvert.DeserializeObject<EmailModels>(responseString);

                    var sender = email.Payload?.Headers?.FirstOrDefault(h => h.Name == "From")?.Value;
                    var subject = email.Payload?.Headers?.FirstOrDefault(h => h.Name == "Subject")?.Value;

                    var bodyText = email.Snippet;
                    var sentDateStr = email.Payload?.Headers?.FirstOrDefault(h => h.Name == "Date")?.Value;

                    // Format the sent date
                    DateTime sentDate;
                    string formattedSentDate = null;
                    if (DateTime.TryParse(sentDateStr, out sentDate))
                    {
                        formattedSentDate = sentDate.ToString("dd/MM/yyyy HH:mm");
                    }

                    var bodyContents = new List<string>();
                    var attachments = new List<AttachmentInfo>();

                    if (email.Payload?.Parts != null)
                    {
                        foreach (var part in email.Payload.Parts)
                        {
                            if (part.Body != null && !string.IsNullOrEmpty(part.Body.Data))
                            {
                                if (part.MimeType == "text/plain" || part.MimeType == "text/html")
                                {
                                    var decodedBody = DecodeBase64(part.Body.Data);
                                    bodyContents.Add(decodedBody);
                                }
                                else if (part.MimeType.StartsWith("image/") || part.MimeType.StartsWith("application/"))
                                {
                                    // Handle attachments
                                    var attachment = new AttachmentInfo
                                    {
                                        FileName = part.Filename,
                                        MimeType = part.MimeType,
                                        Data = DecodeBase64(part.Body.Data)
                                    };
                                    attachments.Add(attachment);
                                }
                            }

                            // Check for nested parts
                            if (part.Parts != null)
                            {
                                foreach (var nestedPart in part.Parts)
                                {
                                    if (nestedPart.Body != null && !string.IsNullOrEmpty(nestedPart.Body.Data))
                                    {
                                        var decodedBody = DecodeBase64(nestedPart.Body.Data);
                                        bodyContents.Add(decodedBody);
                                    }
                                }
                            }
                        }
                    }


                    return new EmailSummary
                    {
                        Id = email.Id,
                        Sender = sender,
                        Subject = subject,
                        Snippet = bodyText,
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

    
        public async Task<ActionResult> ShowEmailDetails(string messageId,string CheckTask)
        {
            var emailSummary = await GetEmailDetails(messageId);
            var emailTitle = "Tiêu đề:"+emailSummary.Subject;
            var emailSender = "Sender:"+emailSummary.Sender;
            var emailSentDate = emailSummary.SentDate;
            var emailSnippet = emailSummary.Snippet;
            var emailContent = emailTitle+ emailSender+ emailSentDate+ emailSnippet;
            var taskViewModel = AnalyzeEmail(emailContent);
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


        string DecodeBase64(string base64Data)
        {
            try
            {
               base64Data = base64Data.Replace(" ", "").Replace("\r", "").Replace("\n", "");
                int mod4 = base64Data.Length % 4;
                if (mod4 > 0)
                {
                    base64Data = base64Data.PadRight(base64Data.Length + 4 - mod4, '=');
                }
                var bytes = Convert.FromBase64String(base64Data);
                return Encoding.UTF8.GetString(bytes);
            }
            catch (FormatException ex)
            {
                return $"Error decoding base64 data: {ex.Message}";
            }
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
        public TaskViewModel AnalyzeEmail(string emailContent)
        {
            var details = new TaskViewModel();

            // Phân tích tiêu đề
            var titleMatch = Regex.Match(emailContent, @"Tiêu đề:\s*(.+)\s*Sender:");
            details.Title = titleMatch.Success ? titleMatch.Groups[1].Value.Trim() : "Không có tiêu đề";

            // Phân tích mô tả
            var descriptionMatch = Regex.Match(emailContent, @"Mô tả:\s*([\s\S]+?)\s*Ngày hết hạn:");
            var descriptionText = descriptionMatch.Success ? descriptionMatch.Groups[1].Value.Trim() : "Không có mô tả";
            details.Description = ParseDescriptions(descriptionText);

            // Phân tích ngày hết hạn
            var dueDateMatch = Regex.Match(emailContent, @"Ngày hết hạn:\s*(\d{2}/\d{2}/\d{4})");
            details.DueDate = dueDateMatch.Success ? DateTime.ParseExact(dueDateMatch.Groups[1].Value.Trim(), "dd/MM/yyyy", null) : DateTime.Now.AddDays(7);

            // Phân tích mức độ ưu tiên
            var priorityMatch = Regex.Match(emailContent, @"Mức độ ưu tiên:\s*(.+)");
            details.Priority = priorityMatch.Success ? priorityMatch.Groups[1].Value.Trim() : "Thấp";

            // Phân tích người nhận
            var recipientsMatch = Regex.Match(emailContent, @"Người nhận:\s*(.+)");
            details.Recipients = recipientsMatch.Success ? recipientsMatch.Groups[1].Value.Trim() : "Không có người nhận";

            // Phân tích trạng thái
            details.Status = "Chưa bắt đầu";

            return details;
        }
        private List<string> ParseDescriptions(string descriptionText)
        {
            var descriptions = new List<string>();

            // Tách các công việc theo định dạng "Công việc N:"
            var matches = Regex.Matches(descriptionText, @"Công việc \d+:\s*([^\d]+)");

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
