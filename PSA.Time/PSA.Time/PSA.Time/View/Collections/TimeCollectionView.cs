using Common.Model;
using Common.Utilities;
using Common.Utilities.Resources;
using Common.View;
using PSA.Time.ViewModel;
using System;
using Xamarin.Forms;
using Tasks = System.Threading.Tasks;

namespace PSA.Time.View
{
    /// <summary>
    /// Main page for listing time entries.
    /// </summary>
    public class TimeCollectionView : AppBasePage
    {
        // Controls
        private ToolbarItem multiSelectModeButton;
        private ToolbarItem createNewTimeButton;
        private ToolbarItem submitButton;
        private ToolbarItem recallButton;
        protected TimeTabbedPage listPage;

        // View model to provide data and control UI
        private TimeCollectionViewModel viewModel;
        protected TimeCollectionViewModel ViewModel
        {
            get
            {
                if (this.viewModel == null)
                {
                    this.viewModel = new TimeCollectionViewModel();
                }

                return this.viewModel;
            }
        }

        public TimeCollectionView() : base()
        {
            this.Title = this.ViewModel.Title;

            OnPlatform<Thickness> op = (OnPlatform<Thickness>)Application.Current.Resources["PagePadding"];
            this.Padding = Device.OnPlatform(op.iOS, op.Android, op.WinPhone);
            
            // Call MessagingCenter.Send<Page>(Page, Message.RefreshMainPage) from any Page to refresh this page
            MessagingCenter.Subscribe<Page>(this, Message.RefreshMainPage, async (sender) =>
            {
                await this.Refresh();
            });
        }

        protected override void SetBindingContext()
        {
            if (this.BindingContext == null)
            {
                this.BindingContext = this.ViewModel;
            }
        }

        protected override void CreateContent()
        {
            base.CreateContent();

            listPage = new TimeTabbedPage(this.ViewModel);
            listPage.listView.RefreshCommand = new Command(async () => await this.Refresh());
            listPage.listView.SetBinding(ListView.ItemsSourceProperty, "Days");
            listPage.listView.SetBinding(ListView.IsRefreshingProperty, "IsBusy");

            this.Content = listPage;
        }

        protected override async Tasks.Task LoadData()
        {
            this.ViewModel.IsBusy = true;

            await this.ViewModel.LoadTimes();

            this.ViewModel.IsBusy = false;
        }

        protected override async Tasks.Task Refresh()
        {
            this.ViewModel.IsBusy = true;

            // Start loading data
            await base.Refresh();

            this.ViewModel.IsBusy = false;
        }

        #region Action bar buttons and handlers

        /// <summary>
        /// Adds the default buttons: New, Multiselect, Settings
        /// </summary>
        protected override void SetToolbarItems()
        {
            this.addCreateNewTimeButton();
            this.addMultiSelectModeButton();

            // Call base after creating buttons so that settings button is added after those.
            base.SetToolbarItems();
        }

        /// <summary>
        /// Adds buttons for multiselect mode: Submit, Recall
        /// </summary>
        private void SetToolbarItemsMultiSelect()
        {
            this.addSubmitTimeEntryButton();
            this.addRecallTimeEntryButton();

            // Don't call base.SetToolbarItems because 
            // we don't want a settings button in multiselect mode.
        }

        private void addRecallTimeEntryButton()
        {
            if (null == recallButton)
            {
                recallButton = ToolbarHelper.createRecallButton();
                recallButton.Clicked += RecallButton_Clicked;
            }

            this.ToolbarItems.Add(recallButton);
        }

        private void addSubmitTimeEntryButton()
        {
            if (null == submitButton)
            {
                submitButton = ToolbarHelper.createSubmitButton();
                submitButton.Clicked += SubmitButton_Clicked; ;
            }

            this.ToolbarItems.Add(submitButton);
        }

        /// <summary>
        /// Add a button to enter multiselect mode for the time entry list.
        /// </summary>
        private void addMultiSelectModeButton()
        {
            if (null == multiSelectModeButton)
            {
                multiSelectModeButton = ToolbarHelper.createMultiselectButton();
                multiSelectModeButton.Clicked += MultiSelectMode_Clicked;
            }

            this.ToolbarItems.Add(multiSelectModeButton);
        }

        /// <summary>
        /// Adds a button to create new time entries.
        /// </summary>
        private void addCreateNewTimeButton()
        {
            if (null == createNewTimeButton)
            {
                createNewTimeButton = ToolbarHelper.createAddTimeEntryButton();

                createNewTimeButton.Clicked += async (sender, args) =>
                {
                    msdyn_timeentry timeEntry = new msdyn_timeentry();

                    // Create a time entry using the current month and navigate to details page.
                    if (this.viewModel != null)
                    {
                        this.viewModel.DefaultDateFromCurrentFilter(timeEntry);
                    }

                    await NavigateToTimeDetailsView(timeEntry);
                };
            }

            this.ToolbarItems.Add(createNewTimeButton);
        }

        private async void RecallButton_Clicked(object sender, EventArgs e)
        {
            // async void OK for top level event handler.
            Tasks.Task<bool> recallTask = this.viewModel.RecallSelectedTimeEntries();
            await this.multiselectButtonHelper(recallTask, AppResources.RecallError);
        }

        private async void SubmitButton_Clicked(object sender, EventArgs e)
        {
            // async void OK for top level event handler.
            Tasks.Task<bool> submitTask = this.viewModel.SubmitSelectedTimeEntries();
            await this.multiselectButtonHelper(submitTask, AppResources.SubmitError);
        }

        private void MultiSelectMode_Clicked(object sender, System.EventArgs e)
        {
            // Enter multiselect mode
            this.ViewModel.MultiselectModeEnabled = true;
            this.listPage.setMultiSelectMode(true);
          
            this.ToolbarItems.Clear();
            this.SetToolbarItemsMultiSelect();
        }

        private async Tasks.Task multiselectButtonHelper(Tasks.Task<bool> multiselectActionTask, string errorMessage)
        {
            this.ViewModel.IsBusy = true;

            bool success = await multiselectActionTask;
            if (success)
            {
                Tasks.Task refreshTask = this.Refresh();
                this.endMultiSelectMode();
                await refreshTask;
            }
            else
            {
                await MessageCenter.ShowErrorMessage(errorMessage);
            }

            this.ViewModel.IsBusy = false;
        }

        #endregion

        protected async Tasks.Task NavigateToTimeDetailsView(msdyn_timeentry Time)
        {
            await TimeDetailsPage.NavigateToTimeDetailsView(Time, this.Navigation);
        }

        protected override bool OnBackButtonPressed()
        {
            // Back button cancels multiselect mode
            if (this.ViewModel.MultiselectModeEnabled)
            {
                this.endMultiSelectMode();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }

        private void endMultiSelectMode()
        {
            // Exit multiselect mode
            this.ViewModel.MultiselectModeEnabled = false;
            this.listPage.setMultiSelectMode(false);

            // Change buttons to submit and recall
            this.ToolbarItems.Clear();
            this.SetToolbarItems();            
        }
    }
}
