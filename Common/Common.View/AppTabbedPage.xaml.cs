using Common.Utilities.Authentication;
using Common.Utilities.Resources;
using Common.View.CustomControl;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Common.View
{
    public partial class AppTabbedPage :  TabbedPageEx 
    {
        public AppTabbedPage()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        protected override async void OnAppearing()
        {
            // OK for async void since OnAppearing is a top level event.
            base.OnAppearing();
            await this.Refresh();
        }

        /// <summary>
        /// Register ServiceUrlMissingHandler and call CreateContent.
        /// </summary>
        protected virtual void Initialize()
        {
            Authentication.Current.RegisterServiceUrlMissingHandler(() => 
            {
                BaseApp app = (BaseApp)Application.Current;
                app.switchToSettingsPage();
                return Task.FromResult(true);
            });

            this.SetBindingContext();

            this.CreateContent();
        }

        /// <summary>
        /// Either boot the user to Settings/Login page, or call Refresh.
        /// </summary>
        /// <returns></returns>
        protected virtual async Task Refresh()
        {
            // Check if it has all the data or need to show the settings option to the user.
            // We need OAuth and service urls, so check "Authentication.Current.IsAuthenticationInfoMissing()"
            // instead of "Authentication.Current.IsLoggedOut"
            if (Authentication.Current.IsAuthenticationInfoMissing())
            {
                await Authentication.Current.TriggerServiceUrlMissingHandler();
            }
            else
            {
                await this.LoadData();
            }
        }

        /// <summary>
        /// Add Settings button to the toolbar.
        /// </summary>
        protected virtual void SetToolbarItems()
        {
            // Add Settings
            ToolbarItem settings = new ToolbarItem()
            {
                Text = AppResources.Settings
            };
            
            if (Device.OS == TargetPlatform.iOS)
            {
                settings.Order = ToolbarItemOrder.Primary;
                settings.Icon = "settings.png";
            }
            else
            {
                settings.Order = ToolbarItemOrder.Secondary;
            }
            
            settings.Clicked += async (sender, args) =>
            {
                BaseApp app = (BaseApp)Application.Current;
                await this.Navigation.PushAsync(app.GetSettingsPage());
            };

            this.ToolbarItems.Add(settings);
        }

        /// <summary>
        /// Set the binding context.
        /// </summary>
        protected virtual void SetBindingContext()
        {
        }

        /// <summary>
        /// Add controls or other content to the page.
        /// </summary>
        protected virtual void CreateContent()
        {
            this.ToolbarItems.Clear();
            this.SetToolbarItems();
        }

        /// <summary>
        /// Load data.
        /// </summary>
        /// <returns>Task to support async calling.</returns>
        protected virtual async Task LoadData()
        {
            await Task.FromResult(true);
        }
    }
}
