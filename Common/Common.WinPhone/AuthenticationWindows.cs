using Common.Utilities.Authentication;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using ADALAuthenticationResult = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult;
using ADALAuthenticationStatus = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationStatus;

namespace Common.WinPhone
{
    public class AuthenticationWindows : Authentication
    {
        protected AuthenticationContext authContext;
        protected AuthenticationResult authResult;
        private TaskCompletionSource<AuthenticationResult> acquireTokenTcs;

        private async Task InitializeAuthentication()
        {
            if (authContext == null || authContext.Authority != OAuthUrl)
            {
                authContext = await AuthenticationContext.CreateAsync(OAuthUrl);
            }
        }

        protected override async Task<AuthenticationResult> AcquireToken(string serverUrl, string clientId, string redirectUrl)
        {
            await InitializeAuthentication();

            acquireTokenTcs = new TaskCompletionSource<AuthenticationResult>();
            authContext.AcquireTokenAndContinue(serverUrl, clientId, new Uri(redirectUrl), p => { });
            return await acquireTokenTcs.Task;
        }

        protected override async Task<AuthenticationResult> AcquireTokenSilent(string serverUrl, string clientId, string redirectUrl)
        {
            await InitializeAuthentication();

            var authResult = await authContext.AcquireTokenSilentAsync(serverUrl, clientId);
            return Convert(authResult);
        }

        public override async Task<AuthenticationResult> AcquireTokenByRefreshToken(string resource)
        {
            await InitializeAuthentication();

            var authResult = await authContext.AcquireTokenByRefreshTokenAsync(this.Result.RefreshToken, this.ClientId, resource);
            return Convert(authResult);
        }

        public async Task ContinueAcquireToken(IWebAuthenticationBrokerContinuationEventArgs args)
        {
            var result = await authContext.ContinueAcquireTokenAsync(args);
            acquireTokenTcs.SetResult(Convert(result));
        }

        protected override void ClearTokens()
        {
            if (authContext != null)
            {
                authContext.TokenCache.Clear();
            }
            authContext = null;
        }

        private AuthenticationResult Convert(ADALAuthenticationResult authenticationResult)
        {
            return new AuthenticationResult()
            {
                Status = Convert(authenticationResult.Status),
                AccessToken = authenticationResult.AccessToken,
                Error = authenticationResult.Error,
                ErrorDescription = authenticationResult.ErrorDescription,
                UserFamilyName = authenticationResult.UserInfo == null ? null : authenticationResult.UserInfo.FamilyName,
                UserGivenName = authenticationResult.UserInfo == null ? null : authenticationResult.UserInfo.GivenName,
                UserDisplayableId = authenticationResult.UserInfo == null ? null : authenticationResult.UserInfo.DisplayableId
            };
        }

        private AuthenticationStatus Convert(ADALAuthenticationStatus authenticationStatus)
        {
            switch (authenticationStatus)
            {
                case ADALAuthenticationStatus.Success: return AuthenticationStatus.Success;
                case ADALAuthenticationStatus.ClientError: return AuthenticationStatus.ClientError;
                case ADALAuthenticationStatus.ServiceError: return AuthenticationStatus.ServiceError;
                default: return AuthenticationStatus.Success;
            }
        }
    }
}
