using Android.App;
using Android.Content;
using Common.Utilities.Authentication;
using Common.Utilities.Resources;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Threading.Tasks;
using ADALAuthenticationResult = Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult;

namespace Common.Android.Utilities
{
    public class AndroidAuthentication : Authentication
    {
        protected AuthenticationContext authContext;

        private Context context;
        public Context Context
        {
            get
            {
                return context;
            }
            internal set
            {
                context = value;
                authContext = null;
            }
        }

        public AndroidAuthentication(Context context)
        {
            this.context = context;
        }

        private void InitializeAuthentication()
        {
            if (authContext == null || authContext.Authority != OAuthUrl)
            {
                authContext = new AuthenticationContext(OAuthUrl);
            }
        }

        protected override async Task<AuthenticationResult> AcquireToken(string serverUrl, string clientId, string redirectUrl)
        {
            InitializeAuthentication();

            try
            {
                var authResult = await authContext.AcquireTokenAsync(serverUrl, clientId, new Uri(redirectUrl), new PlatformParameters(context as Activity));
                return Convert(authResult);
            }
            catch (Microsoft.IdentityModel.Clients.ActiveDirectory.AdalException ex)
            {
                return new AuthenticationResult()
                {
                    Status = AuthenticationStatus.ClientError,
                    Error = AppResources.errorTitle,
                    ErrorDescription = ex.ErrorCode + " - " + ex.Message
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult()
                {
                    Status = AuthenticationStatus.ClientError,
                    Error = AppResources.errorTitle,
                    ErrorDescription = ex.Message
                };
            }
        }

        protected override async Task<AuthenticationResult> AcquireTokenSilent(string serverUrl, string clientId, string redirectUrl)
        {
            InitializeAuthentication();

            try
            {
                var authResult = await authContext.AcquireTokenSilentAsync(serverUrl, clientId);
                return Convert(authResult);
            }
            catch (Microsoft.IdentityModel.Clients.ActiveDirectory.AdalException ex)
            {
                return new AuthenticationResult()
                {
                    Status = AuthenticationStatus.ClientError,
                    Error = AppResources.errorTitle,
                    ErrorDescription = ex.ErrorCode
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult()
                {
                    Status = AuthenticationStatus.ClientError,
                    Error = AppResources.errorTitle,
                    ErrorDescription = ex.Message
                };
            }
        }

        public override async Task<AuthenticationResult> AcquireTokenByRefreshToken(string resource)
        {
            InitializeAuthentication();

            try
            {
                var authResult = await authContext.AcquireTokenSilentAsync(resource, this.ClientId);
                return Convert(authResult);
            }
            catch (Microsoft.IdentityModel.Clients.ActiveDirectory.AdalException ex)
            {
                return new AuthenticationResult()
                {
                    Status = AuthenticationStatus.ClientError,
                    Error = AppResources.errorTitle,
                    ErrorDescription = ex.ErrorCode
                };
            }
            catch (Exception ex)
            {
                return new AuthenticationResult()
                {
                    Status = AuthenticationStatus.ClientError,
                    Error = AppResources.errorTitle,
                    ErrorDescription = ex.Message
                };
            }
        }

        public void ContinueAcquireToken(int requestCode, Result resultCode, Intent data)
        {
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
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
                Status = AuthenticationStatus.Success,
                AccessToken = authenticationResult.AccessToken,
                Error = null,
                ErrorDescription = null,
                UserFamilyName = authenticationResult.UserInfo == null ? null : authenticationResult.UserInfo.FamilyName,
                UserGivenName = authenticationResult.UserInfo == null ? null : authenticationResult.UserInfo.GivenName,
                UserDisplayableId = authenticationResult.UserInfo == null ? null : authenticationResult.UserInfo.DisplayableId
            };
        }
    }
}