using Common.Utilities;
using Common.Utilities.AppPackage;
using Common.Utilities.Authentication;
using Common.Utilities.Resources;
using Common.ViewModel.Command;
using System;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Common.ViewModel
{
    [DataContract]
    public class SettingsViewModel : BaseViewModel
    {
        private const string HTTP = "http://";
        private const string HTTPS = "https://";
        private const string DYNAMICS_COM = ".dynamics.com";
        private const string CRMLIVETIE_COM = ".crmlivetie.com";
        private const string DYNAMICSINT_COM = ".dynamics-int.com";
        private const string DEFAULT_SERVICE_URL_POSTFIX = ".crm.dynamics.com";

        #region Events

        public event EventHandler LogInCompleted;
        public event EventHandler LogOutCompleted;

        #endregion

        #region Properties
        
        private string serviceUrl;
        private string suggestedServiceUrl;

        [DataMember]
        public string ServiceUrl
        {
            get
            {
                return serviceUrl;
            }
            set
            {
                serviceUrl = value;
                NotifyPropertyChanged();

                GenerateSuggestedServiceUrl();
            }
        }

        [DataMember]
        public string SuggestedServiceUrl
        {
            get
            {
                return suggestedServiceUrl;
            }
            set
            {
                suggestedServiceUrl = value;
                NotifyPropertyChanged();
            }
        }

        private string appVersionNumber;
        [DataMember]
        public string AppVersionNumber
        {
            get
            {
                return appVersionNumber;
            }
            set
            {
                appVersionNumber = value;
                NotifyPropertyChanged();
            }
        }
        
        #region Commands

        protected RelayCommandAsync logCommand;
        /// <summary>
        ///  Command called when user clicks to LogIn or LogOut
        /// </summary>
        public RelayCommandAsync LogCommand
        {
            get
            {
                return logCommand ?? (logCommand = new RelayCommandAsync(async () =>
                    {
                        if (authentication.IsLoggedOut)
                        {
                            await LogIn();
                        }
                        else
                        {
                            await LogOut();
                        }
                    }));
            }
        }

        private RelayCommand applySuggestedServiceUrlCommand;
        public RelayCommand ApplySuggestedServiceUrlCommand
        {
            get
            {
                return applySuggestedServiceUrlCommand ?? (applySuggestedServiceUrlCommand = new RelayCommand(() => ApplySuggestedServiceUrl()));
            }
            set
            {
                applySuggestedServiceUrlCommand = value;
            }
        }

        #endregion

        private Authentication authentication;
        private IAppPackage appPackage;

        public bool IsLoggedOut
        {
            get
            {
                if (authentication != null)
                {
                    return authentication.IsLoggedOut;
                }
                return true;
            }
            set
            {
                NotifyPropertyChanged();
            }
        }

        #region Labels

        public new string Title
        {
            get
            {
                return AppResources.AccountSettings;
            }
        }

        public string LogLabel
        {
            get
            {
                if (this.IsLoggedOut)
                {
                    return AppResources.LogIn;
                }
                else
                {
                    return AppResources.LogOut;
                }
            }
            protected set
            {
                NotifyPropertyChanged();
            }
        }

        public string Description
        {
            get
            {
                return AppResources.settingsDescription;
            }
        }

        public string ServiceUrlLabel
        {
            get
            {
                return AppResources.endPoint;
            }
        }

        #endregion

        #endregion

        public SettingsViewModel(Authentication authentication, IAppPackage appPackage) : base()
        {
            this.appPackage = appPackage;
            this.AppVersionNumber = appPackage == null ? string.Empty : "v" + appPackage.GetVersionNumber();

            this.authentication = authentication;
            if (authentication != null && !string.IsNullOrEmpty(authentication.ServiceUrl))
            {
                this.ServiceUrl = authentication.ServiceUrl.Replace(HTTPS, string.Empty).Replace(HTTP, string.Empty);
            }
            else
            {
                this.ServiceUrl = string.Empty;
            }

            this.IsBusy = false;
        }

        public async Task LogIn()
        {
            IsBusy = true;
            ServiceUrl = FixServiceUrl(ServiceUrl);

            var success = await authentication.LogIn(HTTPS + ServiceUrl);
            IsBusy = false;

            if (success)
            {
                LogLabel = null;
                if (LogInCompleted != null)
                    LogInCompleted(this, EventArgs.Empty);
            }
        }

        public async Task LogOut()
        {
            IsBusy = true;

            var success = await authentication.LogOut();
            IsBusy = false;

            if (success)
            {
                await MessageCenter.ShowMessage(AppResources.LogOutOk);
                this.IsLoggedOut = true;
                LogLabel = null;

                if (LogOutCompleted != null)
                    LogOutCompleted(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Update the url to have the correct protocol and CRM server.
        /// </summary>
        /// <param name="url">string to update with the correct protocol and CRM server.</param>
        /// <returns>url updated with the correct protocol and CRM server.</returns>
        private string FixServiceUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return string.Empty;
            }

            string result = url;

            // Remove https and http
            if (result.StartsWith(HTTPS))
            {
                result = result.Substring(HTTPS.Length, result.Length - HTTPS.Length).Trim();
            }

            if (result.StartsWith(HTTP))
            {
                result = result.Substring(HTTP.Length, result.Length - HTTP.Length).Trim();
            }

            // Is the correct CRM domain name present
            if (!result.EndsWith(SettingsViewModel.CRMLIVETIE_COM)
                && !result.EndsWith(SettingsViewModel.DYNAMICS_COM)
                && !result.EndsWith(SettingsViewModel.DYNAMICSINT_COM))
            {
                result += SettingsViewModel.DYNAMICS_COM;
            }

            return result.Trim();
        }

        private void GenerateSuggestedServiceUrl()
        {
            var serviceUrl = ServiceUrl == null ? null : ServiceUrl.Trim();

            if (string.IsNullOrWhiteSpace(serviceUrl))
                SuggestedServiceUrl = null;
            else
            {
                // No need to suggest any more
                if (serviceUrl.EndsWith(DYNAMICS_COM) || serviceUrl.EndsWith(CRMLIVETIE_COM) || serviceUrl.EndsWith(DYNAMICSINT_COM))
                {
                    SuggestedServiceUrl = null;
                }
                else
                {
                    bool isDone = false;

                    isDone = TryGenerateSuggestedServiceUrlWithPostfix(serviceUrl, DEFAULT_SERVICE_URL_POSTFIX)
                        || TryGenerateSuggestedServiceUrlWithPostfix(serviceUrl, DYNAMICS_COM)
                        || TryGenerateSuggestedServiceUrlWithPostfix(serviceUrl, CRMLIVETIE_COM)
                        || TryGenerateSuggestedServiceUrlWithPostfix(serviceUrl, DYNAMICSINT_COM);

                    if (!isDone)
                        SuggestedServiceUrl = serviceUrl + DEFAULT_SERVICE_URL_POSTFIX;
                }
            }
        }

        /// <summary>
        /// Check to see if ServiceUrl is already end with a part of postfix
        /// If yes, generate SuggestedServiceUrl by appending the rest of postfix to ServiceUrl
        /// If no, return false
        /// </summary>
        /// <param name="serviceUrl"></param>
        /// <param name="postfix"></param>
        /// <returns></returns>
        private bool TryGenerateSuggestedServiceUrlWithPostfix(string serviceUrl, string postfix)
        {
            for (int i = postfix.Length - 1; i > 0; i--)
            {
                var subPostfix = postfix.Substring(0, i);
                if (serviceUrl.EndsWith(subPostfix))
                {
                    SuggestedServiceUrl = serviceUrl.Remove(serviceUrl.Length - subPostfix.Length) + postfix;
                    return true;
                }
            }

            return false;
        }

        private void ApplySuggestedServiceUrl()
        {
            ServiceUrl = SuggestedServiceUrl;
        }

        public void TriggerOnPropertyChangedForAllProperties()
        {
            NotifyPropertyChanged("ServiceUrl");
            NotifyPropertyChanged("SuggestedServiceUrl");
            NotifyPropertyChanged("AppVersionNumber");
        }
    }
}
