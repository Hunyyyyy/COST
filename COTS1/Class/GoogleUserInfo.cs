using Google.Apis.Auth.OAuth2;
using Google.Apis.PeopleService.v1;
using Google.Apis.Services;

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
    }
}
