using System.Web;

namespace COTS1.Class
{
    public class AccessToken
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public AccessToken(IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
        }

        public string CheckToken()
        {
            var accessToken = _contextAccessor.HttpContext.Session.GetString("AccessToken");

            if (string.IsNullOrEmpty(accessToken))
            {
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
                    "&state=" + HttpUtility.UrlEncode(state);
                return authUrl;
            }
            return accessToken;
        }
    }
}