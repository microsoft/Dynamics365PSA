using Common.Utilities.DataAccess;
using Common.Utilities.DataAccess.ServiceAccess;
using Common.Utilities.Office;
using Common.Utilities.Resources;
using System;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Common.Utilities.Authentication
{
    /// <summary>
    /// Provides function to authenticate against an CRM instance
    /// </summary>
    public abstract class Authentication
    {
        public const string ServiceUrlKey = "AuthenticationServiceUrl";
        public const string OAuthUrlKey = "AuthenticationOAuthUrl";

        public delegate Task ServiceUrlMissingHandler();

        public string ServiceUrl
        {
            get
            {
                return OrganizationServiceProxy.Current.ServiceUrl;
            }
            protected set
            {
                if (OrganizationServiceProxy.Current.ServiceUrl != value)
                {
                    OAuthUrl = null;
                }

                OrganizationServiceProxy.Current.ServiceUrl = value;
            }
        }

        protected string OAuthUrl { get; set; }

        public AuthenticationResult Result { get; private set; }

        private ServiceUrlMissingHandler askForServiceUrl;
        protected ServiceUrlMissingHandler AskForServiceUrl
        {
            get
            {
                if (askForServiceUrl == null)
                    throw new ArgumentNullException("Please call Authentication.RegisterServiceUrlMissingHandler() to handle service url missing");
                return askForServiceUrl;
            }
            set
            {
                askForServiceUrl = value;
            }
        }

        private static Authentication current;
        public static Authentication Current
        {
            get
            {
                if (current == null)
                {
                    throw new Exception("Please call Initialize() to initialize Authentication");
                }
                return current;
            }
            private set
            {
                current = value;
            }
        }

        public bool IsLoggedOut { get; protected set; }

        protected string ClientId { get; set; }
        protected string RedirectUri { get; set; }

        /// <summary>
        /// Initializes this base utitlity with a more concrete class for each specific platform
        /// </summary>
        /// <typeparam name="TAuthentication"></typeparam>
        /// <returns></returns>
        public static async Task<Authentication> Initialize<TAuthentication>(TAuthentication authentication)  
            where TAuthentication : Authentication
        {
            if (current == null)
            {
                current = authentication;
                await current.Initialize();
            }
            return Current;
        }

        protected Authentication()
        {
            this.ClientId = EnvironmentVariables.Instance.AuthenticationClientId;
            this.RedirectUri = EnvironmentVariables.Instance.AuthenticationRedirectUri;
        }

        /// <summary>
        /// Retrieves saved ServiceUrl and OAuthUrl if any
        /// </summary>
        /// <returns></returns>
        protected async Task Initialize()
        {
            await LoadCredentials();
            IsLoggedOut = true;
        }

        public void RegisterServiceUrlMissingHandler(ServiceUrlMissingHandler serviceUrlMissingHandler)
        {
            this.askForServiceUrl = serviceUrlMissingHandler;
        }

        public async Task TriggerServiceUrlMissingHandler()
        {
            await AskForServiceUrl();
        }

        /// <summary>
        /// Logs in to CRM. Triggers asking for service url if one is not provided in Authentication
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LogIn()
        {
            if (string.IsNullOrEmpty(this.ServiceUrl))
            {
                ClearTokens();
                await AskForServiceUrl();
                return false;
            }
            else
                return await LogIn(this.ServiceUrl);
        }

        /// <summary>
        /// Logs in to CRM using the provided service url
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> LogIn(string serviceUrl)
        {
            // If OAuthUrl is null and is not signed out, we need to clear sign out before sign in to ensure consistency
            if (String.IsNullOrEmpty(OAuthUrl) && !IsLoggedOut)
            {
                await LogOut();
            }

            if (!CrmUrlVerification.ValidateCrmUrl(serviceUrl))
            {
                await MessageCenter.ShowErrorMessage(AppResources.errorOAuthUrlRetrieveFailed);
                return false;
            }

            this.ServiceUrl = serviceUrl;
            if (string.IsNullOrEmpty(this.OAuthUrl))
            {
                this.OAuthUrl = await DiscoverAuthority();

                if (string.IsNullOrEmpty(this.OAuthUrl))
                {
                    await MessageCenter.ShowErrorMessage(AppResources.errorOAuthUrlRetrieveFailed);
                    return false;
                }
            }
            
            var isAuthenticated = await this.Authenticate();

            // Only save credentials when able to authenticate, else call log out to clear all credentials
            if (isAuthenticated)
                await SaveCredentials();
            else
                await LogOut();

            return isAuthenticated;
        }

        /// <summary>
        /// Clears all credentials and logs out
        /// </summary>
        /// <returns></returns>
        public async Task<bool> LogOut()
        {
            ClearTokens();
            await ClearAllFilesAndCredentials();

            Office365.Current.LogOut();
            Delve.Current.LogOut();

            IsLoggedOut = true;
            return IsLoggedOut;
        }

        /// <summary>
        /// Acquires token interactively when there is no saved token
        ///</summary>
        /// <returns></returns>
        protected abstract Task<AuthenticationResult> AcquireToken(string serverUrl, string clientId, string redirectUrl);

        /// <summary>
        /// Acquires token silently without prompting to the user when there is saved token
        /// </summary>
        /// <returns></returns>
        protected abstract Task<AuthenticationResult> AcquireTokenSilent(string serverUrl, string clientId, string redirectUrl);

        /// <summary>
        /// Acquires token using refresh token. This can use used to authenticate against another service without prompting the user again
        /// </summary>
        /// <param name="resource"></param>
        /// <returns></returns>
        public abstract Task<AuthenticationResult> AcquireTokenByRefreshToken(string resource);

        /// <summary>
        /// Clears all the stored tokens
        /// </summary>
        protected abstract void ClearTokens();

        /// <summary>
        /// Authenticates with the service url. Opens a webpage if needed for user to sign in
        /// </summary>
        /// <returns></returns>
        public virtual async Task<bool> Authenticate()
        {
            Exception error = null;

            try
            {
                // Try to get a token without triggering any user prompt
                // ADAL will check whether the requested token is in the cache or can be obtained without user interaction (e.g. via a refresh token)
                Result = await AcquireTokenSilent(ServiceUrl, ClientId, ServiceUrl);

                if (Result == null || Result.Status != AuthenticationStatus.Success)
                {
                    // Acquiring a token without user interaction was not possible, trigger an authentication experience
                    Result = await AcquireToken(ServiceUrl, ClientId, ServiceUrl);
                }
            }
            catch (Exception ex)
            {
                error = ex;
            }

            if (error == null && Result != null && Result.Status == AuthenticationStatus.Success)
            {
                OrganizationServiceProxy.Current.AccessToken = Result.AccessToken;
                IsLoggedOut = false;
                return true;
            }
            else
            {
                if (Result != null)
                {
                    if (Result.Error == "authentication_canceled" || Result.ErrorDescription.StartsWith("authentication_canceled")
                        || Result.Error == "access_denied" || Result.ErrorDescription.StartsWith("access_denied") || Result.ErrorDescription.StartsWith("invalid_grant"))
                    {
                        await MessageCenter.ShowError(AppResources.errorAuthenticationCancelled, AppResources.errorTitle);
                    }
                    else
                    {
                        await MessageCenter.ShowError(Result.ErrorDescription, Result.Error);
                    }
                }
                else if (error is TaskCanceledException)
                {
                    await MessageCenter.ShowError(AppResources.errorAuthenticationCancelled, AppResources.errorTitle);
                }
                else
                {
                    await MessageCenter.ShowError(error.Message, null);
                }
                return false;
            }
        }

        /// <summary>
        /// Checks if service url and OAuth url are ready, which are requirements for authenticating
        /// </summary>
        /// <returns></returns>
        public bool IsAuthenticationInfoMissing()
        {
            return (string.IsNullOrEmpty(ServiceUrl) || string.IsNullOrEmpty(OAuthUrl));
        }

        /// <summary>
        /// Retrieves OAuth endpoint url. See http://msdn.microsoft.com/en-us/library/dn531009.aspx#bkmk_oauth_discovery for details
        /// </summary>
        /// <returns></returns>
        private async Task<string> DiscoverAuthority()
        {
            string oAuthUrl = null;
            if (string.IsNullOrEmpty(this.ServiceUrl))
            {
                return null;
            }

            try
            {
                HttpResponseMessage httpResponse = await OrganizationServiceProxy.Current.DiscoverAuthority(this.ServiceUrl);

                HttpHeaderValueCollection<AuthenticationHeaderValue> wwwAuthenticateHeaderValueCollection = httpResponse.Headers.WwwAuthenticate;
                if (wwwAuthenticateHeaderValueCollection != null)
                {
                    // Sample
                    // Production:     Bearer authorization_uri=https://login.windows.net/00000000-0000-0000-0000-000000000000/oauth2/authorize
                    // Pre-production: Bearer authorization_uri=https://login.windows-ppe.net/00000000-0000-0000-0000-000000000000/oauth2/authorize, resource_id = https://xxxxxxxxxxxx.crm.crmlivetie.com/
                    string wwwAuthenticateHeader = wwwAuthenticateHeaderValueCollection.ToString().Replace("Bearer", "").Trim();
                    string authorizationUriToken = wwwAuthenticateHeader.Split(',').FirstOrDefault(p => p.Trim().StartsWith("authorization_uri")).Trim();

                    // For phone, we don't need oauth2/authorize part
                    oAuthUrl = authorizationUriToken.Substring("authorization_uri=".Length, authorizationUriToken.Length - "oauth2/authorize".Length - "authorization_uri=".Length);
                }
                else
                {
                    oAuthUrl = null;
                }
            }
            catch
            {
#if DEBUG
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
#endif
                oAuthUrl = null;
            }

            return oAuthUrl;
        }

        private async Task SaveCredentials()
        {
            // Use non-encrypted data access since we are only storing the service URL and OAuth URL. 
            await DeviceDataAccess.Current.WriteToLocal(ServiceUrlKey + ".dat", ServiceUrl);
            await DeviceDataAccess.Current.WriteToLocal(OAuthUrlKey + ".dat", OAuthUrl);
        }

        private async Task LoadCredentials()
        {
            // Use non-encrypted data access since we are only storing the service URL and OAuth URL. 
            ServiceUrl = await DeviceDataAccess.Current.ReadFromLocal<string>(ServiceUrlKey + ".dat");
            OAuthUrl = await DeviceDataAccess.Current.ReadFromLocal<string>(OAuthUrlKey + ".dat");
        }

        private async Task ClearAllFilesAndCredentials()
        {
            // Since we are not using encrypted data access, clear local variable and data access.
            OAuthUrl = null;
            await DeviceDataAccess.Current.DeleteAllLocal();
        }

        [DataContract]
        public class AuthenticationResult
        {
            [DataMember]
            public AuthenticationStatus Status { get; set; }
            [DataMember]
            public string AccessToken { get; set; }
            [DataMember]
            public string RefreshToken { get; set; }
            [DataMember]
            public string Error { get; set; }
            [DataMember]
            public string ErrorDescription { get; set; }
            [DataMember]
            public string UserFamilyName { get; set; }
            [DataMember]
            public string UserGivenName { get; set; }
            [DataMember]
            public string UserDisplayableId { get; set; }
        }

        public enum AuthenticationStatus
        {
            Success,
            ServiceError,
            ClientError,
        }
    }
}
