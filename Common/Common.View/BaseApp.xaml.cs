using Common.Utilities;
using Common.Utilities.Authentication;
using Common.Utilities.Resources;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Common.View
{
    public partial class BaseApp : Application
    {
        #region Colors

        public static Color PAGE_BACKGROUND_COLOR;
        public static Color PAGE_HEADER_COLOR;
        public static Color TEXT_COLOR;
        public static Color TEXT_COLOR_DISABLED;
        public static Color TEXT_COLOR_LABEL;
        public static Color TEXT_COLOR_HEADER;
        public static Color TEXT_COLOR_MAIN_ACTION;
        public static Color TEXT_COLOR_TITLE;
        public static Color ICON_COLOR;

        /// <summary>
        /// Used for controls like entry, picker, date picker
        /// </summary>
        public static Color CONTROL_BORDER_COLOR;
        public static Color CONTROL_BORDER_COLOR_DISABLED;

        #endregion

        #region Pages

        /// <summary>
        /// By default will be AppMainPage
        /// </summary>
        /// <returns></returns>
        public virtual Page GetMainPage()
        {
            return new AppTabbedPage();
        }

        /// <summary>
        /// Get a new instance of the settings page.
        /// </summary>
        /// <returns></returns>
        public Page GetSettingsPage()
        {
            return new Settings();
        }

        /// <summary>
        /// Create a new NavigationPage used as the main page for the application.
        /// </summary>
        /// <param name="root">Optional. Page to set as the root of the navigation.</param>
        /// <returns>Styled instance of NavigationPage.</returns>
        public NavigationPage GetNavigationPage(Page root = null)
        {
            // Creating a navigation page with null seems to cause problems.
            NavigationPage result = root == null ? new NavigationPage() : new NavigationPage(root);

            result.BarBackgroundColor = PAGE_HEADER_COLOR;
            result.BarTextColor = TEXT_COLOR_HEADER;

            return result;
        }

        /// <summary>
        /// Set the Settings page as the new application root, and navigate to it.
        /// </summary>
        public void switchToSettingsPage()
        {
            this.switchToPage(this.GetSettingsPage());
        }

        /// <summary>
        /// Set the app main page as the new application root, and navigate to it.
        /// </summary>
        public void switchToMainPage()
        {
            this.switchToPage(this.GetMainPage());
        }

        private void switchToPage(Page switchToPage)
        {
            // Change the app navigation 
            this.MainPage = this.GetNavigationPage(switchToPage);
        }

        #endregion

        protected AppUtilities AppUtilities;

        public BaseApp(AppUtilities appUtilities)
        {
            this.SetColors();

            // Standard XAML initialization
            this.InitializeComponent();

            this.AppUtilities = appUtilities;

            // iOS requires "Application windows are expected to have a root view controller at the end of application launch"
            // So for iOS only, set a view
            Device.OnPlatform(iOS: () =>
            {
                // Create nav page and set the first page.
                this.MainPage = this.GetNavigationPage(new ContentPage());
            });
        }

        /// <summary>
        /// The app client ID.
        /// </summary>
        protected virtual string AuthenticationClientId
        {
            get
            {                
                return Guid.Empty.ToString();
            }
        }

        /// <summary>
        /// The app Redirect URI
        /// </summary>
        protected virtual string AuthenticationRedirectUri
        {
            get
            {
                return "https://*.crm.dynamics.com";
            }
        }

        /// <summary>
        /// Initializes the platform specific app components and the delegates for the messages
        /// </summary>
        /// <returns></returns>
        public virtual async Task Initialize()
        {
            MessageCenter.ShowErrorDelegate = ShowMessageAction;
            MessageCenter.ShowMessageDelegate = ShowMessageAction;
            MessageCenter.ShowDialogDelegate = ShowDialogAction;

            //Initialize ClientId and redirect URL of the app
            EnvironmentVariables.Initialize(this.AuthenticationClientId, this.AuthenticationRedirectUri);

            // This task must be awaited before adding pages 
            // so that the Authentication is completely initialized.
            await this.AppUtilities.Initialize();

            NavigationPage np = this.GetNavigationPage();

            if (Authentication.Current.IsAuthenticationInfoMissing())
            {
                // Security                
                Task t = np.PushAsync(this.GetSettingsPage());
                t.Wait();
            }
            else
            {
                Task t = np.PushAsync(this.GetMainPage());
                t.Wait();
            }

            MainPage = np;
        }

        public async Task ShowMessageAction(string message, string title = null, string buttonOk = null)
        {
            buttonOk = buttonOk ?? AppResources.OK;
            await this.MainPage.DisplayAlert(title, message, buttonOk);
        }

        public Task<bool> ShowDialogAction(string message, string title = null, string buttonOk = null, string buttonCancel = null)
        {
            buttonOk = buttonOk ?? AppResources.OK;
            buttonCancel = buttonCancel ?? AppResources.Cancel;
            return this.MainPage.DisplayAlert(title, message, buttonOk, buttonCancel);
        }

        /// <summary>
        /// Override this method of a different kind of color scheme if needed
        /// </summary>
        public virtual void SetColors()
        {
            TEXT_COLOR = Color.Black;
            TEXT_COLOR_DISABLED = Color.FromHex("FF404040");
            TEXT_COLOR_TITLE = Color.Black;

            // Need to contrast
            PAGE_BACKGROUND_COLOR = Color.White;
            PAGE_HEADER_COLOR = Color.FromHex("FF048ADC");
            TEXT_COLOR_HEADER = PAGE_BACKGROUND_COLOR;
            TEXT_COLOR_LABEL = Color.Black;
            ICON_COLOR = Color.FromHex("FFC6C6C6");

            //Buttons:
            TEXT_COLOR_MAIN_ACTION = Color.FromHex("FF253D5D");

            //Borders:
            CONTROL_BORDER_COLOR = Color.Black;
            CONTROL_BORDER_COLOR_DISABLED = Color.FromHex("FFA3A3A3");
        }

        protected override async void OnStart()
        {
            // OK to have async void here. The method is very much like an event handler.

            // Handle when your app starts
            await this.Initialize();
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }
    }
}
