using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Services;
using MimeKit;

namespace COTS1.Class
{
    public class SendNotificationMail
    {
        internal readonly GmailService _gmailService;

        public SendNotificationMail(string accessToken)
        {
            _gmailService = GetGmailService(accessToken);
        }

        private static GmailService GetGmailService(string accessToken)
        {
            var credential = GoogleCredential.FromAccessToken(accessToken);

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = "COTS",
            });

            return service;
        }

        public static async Task SendEmailAsync(GmailService service, string fromEmailAddress, string toEmailAddress, string subject, string bodyText)
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress("", fromEmailAddress));
            emailMessage.To.Add(new MailboxAddress("", toEmailAddress));
            emailMessage.Subject = subject;
            emailMessage.Body = new TextPart("plain") { Text = bodyText };

            using (var stream = new MemoryStream())
            {
                await emailMessage.WriteToAsync(stream);
                var rawMessage = Base64UrlEncode(stream.ToArray());

                var message = new Message
                {
                    Raw = rawMessage
                };

                await service.Users.Messages.Send(message, "me").ExecuteAsync();
            }
        }

        private static string Base64UrlEncode(byte[] input)
        {
            var output = Convert.ToBase64String(input)
                .Replace('+', '-')
                .Replace('/', '_')
                .Replace("=", "");
            return output;
        }
    }
}