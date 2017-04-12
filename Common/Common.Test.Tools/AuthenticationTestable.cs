using Common.Utilities.Authentication;
using System;
using System.Threading.Tasks;

namespace Common.Test.Tools
{
    /// <summary>
    /// A testable implementation of the abstact <c>Authentication</c> class.
    /// </summary>
    public class AuthenticationTestable : Authentication
    {
        public static string AuthenticationClientId = "AuthClientTestable";
        public static string AuthenticationRedirectUri = "AuthRedirectUriTestable";

        public override Task<AuthenticationResult> AcquireTokenByRefreshToken(string resource)
        {
            throw new NotImplementedException();
        }

        protected override Task<AuthenticationResult> AcquireToken(string serverUrl, string clientId, string redirectUrl)
        {
            throw new NotImplementedException();
        }

        protected override Task<AuthenticationResult> AcquireTokenSilent(string serverUrl, string clientId, string redirectUrl)
        {
            throw new NotImplementedException();
        }

        protected override void ClearTokens()
        {

        }

        public override Task<bool> Authenticate()
        {
            IsLoggedOut = false;
            return Task.FromResult(true);
        }

        public string ActualLogInServiceUrl { get; private set; }

        public override Task<bool> LogIn(string serviceUrl)
        {
            ActualLogInServiceUrl = serviceUrl;

            IsLoggedOut = false;
            return Task.FromResult(true);
        }

        public AuthenticationTestable()
            : base()
        {

        }

        public AuthenticationTestable(string url)
            : base()
        {
            this.ServiceUrl = url;
        }
    }
}
