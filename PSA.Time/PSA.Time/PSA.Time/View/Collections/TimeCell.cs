using Common.Model;
using Common.Utilities;
using Common.Utilities.Resources;
using Common.View;
using Common.ViewModel;
using PSA.Time.ViewModel;
using Xamarin.Forms;

namespace PSA.Time.View
{
    public class TimeCell : ViewCell
    {
        protected MenuItem moreAction;
        protected MenuItem deleteAction;

        private Page hostPage;
        public Page HostPage
        {
            get
            {
                return hostPage;
            }
            set
            {
                hostPage = value;
                this.initWithContextPage();
            }
        }

        /// <summary>
        /// Create a new instance of the TimeCell class.
        /// </summary>
        public TimeCell() : base()
        {
        }

        /// <summary>
        /// Create a new instance of the TimeCell class and initialize the HostPage property.
        /// </summary>
        /// <param name="context">TimeTabbedPage used to initialize the HostPage property.</param>
        public TimeCell(Page context) : base()
        {
            this.HostPage = context;
        }

        private void initWithContextPage()
        {
            this.initMoreAction();
            this.addContextAction(moreAction);

            this.initDeleteAction();
            this.addContextAction(deleteAction);
        }

        private void addContextAction(MenuItem menuItem)
        {
            this.ContextActions.Add(menuItem);
        }

        private void initDeleteAction()
        {
            deleteAction = new MenuItem
            {
                Text = AppResources.Delete,
                IsDestructive = true
            };

            deleteAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));

            deleteAction.Clicked += async (sender, e) =>
            {
                MenuItem mi = (MenuItem)sender;

                msdyn_timeentry time = (msdyn_timeentry)mi.BindingContext;
                TimeViewModel model = new TimeViewModel(time);
                BaseViewModel viewModel = (BaseViewModel)this.HostPage.BindingContext;

                viewModel.IsBusy = true;
                await model.Delete();

                MessagingCenter.Send<Page>(this.HostPage, Message.RefreshMainPage);
                viewModel.IsBusy = false;             
            };            
        }

        private void initMoreAction()
        {
            moreAction = new MenuItem();
            moreAction.SetBinding(MenuItem.CommandParameterProperty, new Binding("."));

            moreAction.Clicked += async (sender, e) =>
            {
                BaseViewModel viewModel = (BaseViewModel)this.HostPage.BindingContext;
                viewModel.IsBusy = true;

                MenuItem mi = (MenuItem)sender;
                msdyn_timeentry time = (msdyn_timeentry)mi.BindingContext;
                TimeViewModel model = new TimeViewModel(time);                

                msdyn_timeentry_msdyn_entrystatus? status = time.EntryStatus;

                if (status != null)
                {
                    if (status == msdyn_timeentry_msdyn_entrystatus.Submitted)
                    {
                        if (!await model.Recall())
                        {
                            await MessageCenter.ShowErrorMessage(AppResources.RecallError);
                        }
                    }
                    else
                    {
                        if (!await model.Submit())
                        {
                            await MessageCenter.ShowErrorMessage(AppResources.SubmitError);
                        }
                    }
                }

                viewModel.IsBusy = false;
                MessagingCenter.Send<Page>(this.HostPage, Message.RefreshMainPage);
            };
        }

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();
            msdyn_timeentry time = (msdyn_timeentry)this.BindingContext;

            if (time == null)
            {                
                return;
            }

            msdyn_timeentry_msdyn_entrystatus? status = time.EntryStatus;

            if (status != null)
            {
                if (status == msdyn_timeentry_msdyn_entrystatus.Submitted)
                {
                    moreAction.Text = AppResources.Recall;
                }
                else
                {
                    moreAction.Text = AppResources.Submit;
                }
            }
            
            if (status == msdyn_timeentry_msdyn_entrystatus.Draft)
            {
                if (!this.ContextActions.Contains(deleteAction))
                {
                    this.ContextActions.Add(deleteAction);
                }
            }
            else
            {
                if (this.ContextActions.Contains(deleteAction))
                {
                    this.ContextActions.Remove(deleteAction);
                }
            }
        }
    }
}
