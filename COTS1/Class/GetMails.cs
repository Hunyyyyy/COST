namespace COTS1.Class
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using COTS1.Models;
    using Google.Apis.Auth.OAuth2;
    using Microsoft.AspNetCore.Http;
    using Newtonsoft.Json;

    public class GetMails
    {
        private readonly HttpClient _client;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public GetMails(HttpClient client, IHttpContextAccessor httpContextAccessor)
        {
            _client = client;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<List<EmailSummary>> FetchEmailsAsync(string apiUrl)
        {
            const int MaxResults = 10;
            var accessToken = _httpContextAccessor.HttpContext.Session.GetString("AccessToken");
            var refreshToken = _httpContextAccessor.HttpContext.Session.GetString("RefreshToken");
            var clientId = "425757132188-sf8k8r5bo25f9dsg0sla9so95ljbp8ki.apps.googleusercontent.com";
            var clientSecret = "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";
            var emailSummaries = new List<EmailSummary>();
            _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

            if (string.IsNullOrEmpty(accessToken) && !string.IsNullOrEmpty(refreshToken))
            {
                accessToken = await RefreshAccessTokenAsync(refreshToken, clientId, clientSecret);
                if (accessToken != null)
                {
                    _httpContextAccessor.HttpContext.Session.SetString("AccessToken", accessToken);
                }
                else
                {
                   // return BadRequest("Access token is missing.");
                }
            }
            var response = await _client.GetAsync(apiUrl);
            if (response.IsSuccessStatusCode)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                var emailsResponse = JsonConvert.DeserializeObject<EmailListResponse>(responseString);
                var emails = emailsResponse.Messages.Take(MaxResults).ToList();

                foreach (var email in emails)
                {
                    var detail = await GetEmailDetails(email.Id);
                    emailSummaries.Add(detail);
                }
            }
            else
            {
                throw new Exception("Failed to retrieve emails.");
            }

            return emailSummaries;
        }
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
        public async Task<MailViewModel> GetMailViewModelAsync()
        {
            var senders = new[] { "ngonhathuy501@gmail.com", "ngonhat501@gmail.com" };
            var inboxQuery = $"label:inbox ({string.Join(" OR ", senders.Select(s => $"from:{s}"))})";
            var sentQuery = $"label:sent ({string.Join(" OR ", senders.Select(s => $"to:{s}"))})";

            var inboxApiUrl = $"https://www.googleapis.com/gmail/v1/users/me/messages?q={Uri.EscapeDataString(inboxQuery)}";
            var sentApiUrl = $"https://www.googleapis.com/gmail/v1/users/me/messages?q={Uri.EscapeDataString(sentQuery)}";

            var emailDetails = new MailViewModel
            {
                ReceivedEmails = await FetchEmailsAsync(inboxApiUrl),
                SentEmails = await FetchEmailsAsync(sentApiUrl)
            };

            return emailDetails;
        }

        private async Task<EmailSummary> GetEmailDetails(string messageId)
        {
            var apiUrl = $"https://www.googleapis.com/gmail/v1/users/me/messages/{messageId}";

            var response = await _client.GetAsync(apiUrl);
            var responseString = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var email = JsonConvert.DeserializeObject<EmailModels>(responseString);

                var sender = email.Payload?.Headers?.FirstOrDefault(h => h.Name == "From")?.Value;
                var subject = email.Payload?.Headers?.FirstOrDefault(h => h.Name == "Subject")?.Value;
                var snippet = email.Snippet;
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

                // Extract email content and attachments
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
                    Snippet = snippet,
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

        private string DecodeBase64(string base64EncodedData)
        {
            try
            {
                var base64 = base64EncodedData.Replace('-', '+').Replace('_', '/');
                switch (base64.Length % 4)
                {
                    case 2: base64 += "=="; break;
                    case 3: base64 += "="; break;
                }
                var bytes = Convert.FromBase64String(base64);
                return Encoding.UTF8.GetString(bytes);
            }
            catch
            {
                return "Error decoding base64 data.";
            }
        }
    }

    }
