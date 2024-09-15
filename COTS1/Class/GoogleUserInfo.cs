using Google.Apis.Auth.OAuth2;
using Google.Apis.Gmail.v1.Data;
using Google.Apis.Gmail.v1;
using Google.Apis.PeopleService.v1;
using Google.Apis.Services;
using Google.Apis.Util.Store;
using Google.Apis.Auth.OAuth2.Flows;
using Google.Apis.Auth.OAuth2.Responses;

namespace COTS1.Class
{
    public class GoogleUserInfo
    {
        private readonly PeopleServiceService _peopleService;

        public GoogleUserInfo(string accessToken)
        {
            var credential = GoogleCredential.FromAccessToken(accessToken);
            _peopleService = new PeopleServiceService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = "COTS"
            });
        }

        public async Task<string> GetUserEmailAsync()
        {
            // Thực hiện yêu cầu để lấy thông tin người dùng
            var request = _peopleService.People.Get("people/me");
            request.PersonFields = "emailAddresses"; // Yêu cầu trường emailAddresses
            var response = await request.ExecuteAsync();

            // Lấy địa chỉ email từ phản hồi
            var emailAddresses = response.EmailAddresses;
            if (emailAddresses != null && emailAddresses.Count > 0)
            {
                return emailAddresses[0].Value;
            }

            return null; // Không có địa chỉ email
        }
        static string[] Scopes = { GmailService.Scope.GmailReadonly };
        static string ApplicationName = "COST1";

        public static async Task<int> GetUnreadEmailCountFromSenderAsync(string senderEmail, string accessToken)
        {
            // Tạo Gmail API service với accessToken
            var credential = GoogleCredential.FromAccessToken(accessToken);

            var service = new GmailService(new BaseClientService.Initializer()
            {
                HttpClientInitializer = credential,
                ApplicationName = ApplicationName,
            });

            // Tạo truy vấn tìm email từ người gửi cụ thể và chưa đọc
            var request = service.Users.Messages.List("me");
            request.LabelIds = "INBOX"; // Chỉ lọc email trong Inbox
            request.Q = $"from:{senderEmail} is:unread"; // Truy vấn email từ người gửi và chưa đọc

            ListMessagesResponse response = await request.ExecuteAsync();

            // Trả về số lượng email chưa đọc từ người gửi cụ thể
            return response.Messages != null ? response.Messages.Count : 0;
        }


        public static async Task<Dictionary<string, int>> GetUnreadEmailCountFromSendersAsync(string[] senderEmails, string accessToken)
        {
            Dictionary<string, int> unreadEmailsBySender = new Dictionary<string, int>();

            foreach (var senderEmail in senderEmails)
            {
                int unreadCount = await GetUnreadEmailCountFromSenderAsync(senderEmail, accessToken);
                unreadEmailsBySender[senderEmail] = unreadCount;
            }

            return unreadEmailsBySender;
        }



    }
}
