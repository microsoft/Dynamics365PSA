using Common.ViewModel;
using System;
using Xamarin.Forms;

namespace Common.View
{
    public partial class Settings : ContentPage
    {
        protected SettingsViewModel ViewModel;

        public Settings ()
        {
            this.InitializeComponent();

            this.ViewModel = new SettingsViewModel(Utilities.Authentication.Authentication.Current, null);
            this.ViewModel.LogInCompleted += ViewModel_OnLogInCompleted;
            this.ViewModel.LogOutCompleted += ViewModel_OnLogOutCompleted;

            BindingContext = ViewModel;

            // on return from entry, submit the form.
            this.Entry.Completed += (s, e) =>
            {
                this.LoginButton.Command.Execute(null);
            };
        }

        /// <summary>
        /// After LogIn finished correctly go back to main page.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ViewModel_OnLogInCompleted(object sender, EventArgs eventArgs)
        {
            MessagingCenter.Send<Page>(this, Message.RefreshMainPage);

            if (this.Navigation.NavigationStack.Count == 1)
            {
                // This is the only page on the stack, so transition to main page
                BaseApp app = (BaseApp)Application.Current;
                app.switchToMainPage();
            }
        }

        /// <summary>
        /// Log out event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="eventArgs"></param>
        private void ViewModel_OnLogOutCompleted(object sender, EventArgs eventArgs)
        {
            // Need to make this page the only one on the stack, so back button exits
            BaseApp app = (BaseApp)Application.Current;
            app.switchToSettingsPage();
        }
    }
}
