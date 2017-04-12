using Common.Utilities.Authentication;
using Common.Utilities.Resources;
using Foundation;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Security;
using System;
using System.Threading.Tasks;

namespace Common.iOS.Utilities
{
    public class IOSAuthentication : Authentication
    {
        protected AuthenticationContext authContext;
        
        public IPlatformParameters PlatformParameters { get; set; }

        public IOSAuthentication(IPlatformParameters parameters)
        {
            this.PlatformParameters = parameters;
        }

        private void InitializeAuthentication()
        {
            if (authContext == null || authContext.Authority != OAuthUrl)
            {
                authContext = new AuthenticationContext(OAuthUrl, true);
            }
        }

        protected override async Task<AuthenticationResult> AcquireToken(string serverUrl, string clientId, string redirectUrl)
        {
            InitializeAuthentication();
            try
            {
                var result = await authContext.AcquireTokenAsync(serverUrl, clientId, new Uri(redirectUrl), this.PlatformParameters);
                return Convert(result);
            }
            catch (AdalException ex)
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
                var result = await authContext.AcquireTokenSilentAsync(resource, this.ClientId);
                return Convert(result);
            }
            catch (AdalException ex)
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

        protected override async Task<Authentication.AuthenticationResult> AcquireTokenSilent(string serverUrl, string clientId, string redirectUrl)
        {
            InitializeAuthentication();
            try
            {
                var result = await authContext.AcquireTokenSilentAsync(serverUrl, clientId);
                return Convert(result);
            }
            catch (AdalException ex)
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

        protected override void ClearTokens()
        {
            if (authContext != null)
            {
                authContext.TokenCache.Clear();
            }
            authContext = null;
        }

        private AuthenticationResult Convert(Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationResult authenticationResult)
        {
            return new AuthenticationResult()
            {
                Status = AuthenticationStatus.Success, //only get Success or Exception from authentication
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