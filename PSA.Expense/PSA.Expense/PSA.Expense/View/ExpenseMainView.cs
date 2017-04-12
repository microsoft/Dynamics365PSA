using Common.Model;
using Common.Utilities;
using Common.Utilities.Resources;
using Common.View;
using Common.View.CustomControl;
using Common.ViewModel.Command;
using PSA.Expense.ViewModel;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace PSA.Expense.View
{
    /// <summary>
    /// Class that contains the common functionality and view of any expense.
    /// </summary>
    public class ExpenseMainView : TabbedPageEx, IExpenseView
    {
        protected ExpenseViewModel ViewModel;
        protected ExpenseDetailsPage ExpenseDetails;

        protected IExpenseView extendedExpenseBehavior;
        public IExpenseView ExtendedExpenseBehavior
        {
            protected get
            {
                return extendedExpenseBehavior;
            }
            set
            {
                extendedExpenseBehavior = value;
                this.ViewModel.SetCategoryHandler = extendedExpenseBehavior.SetCategory;
            }
        }

        public ExpenseMainView(msdyn_expense expense):base()
        {
            this.BackgroundColor = BaseApp.PAGE_BACKGROUND_COLOR;
            this.ViewModel = new ExpenseViewModel(expense);
            this.IsBusy = false;
        }

        /// <summary>
        /// By default create an expense with basic details page.
        /// </summary>
        public async System.Threading.Tasks.Task CreateContent()
        {
            if (ExpenseDetails == null)
            {
                ExpenseDetails = new ExpenseDetailsPage();
                this.Children.Add(ExpenseDetails);
                await ExpenseDetails.Initialize(this.ViewModel);

                // Since the expense is initially unsaved and this page has multiple tabs, force save if expense is unsaved.
                this.ViewModel.HasPendingDataToSave = (this.ViewModel.Expense == null || this.ViewModel.Expense.msdyn_expenseId == null || 
                                                       this.ViewModel.Expense.msdyn_expenseId == Guid.Empty);

                if (this.ViewModel.CanEdit())
                {
                    ToolbarItem submitCommand = new ToolbarItem
                    {
                        Text = AppResources.Submit,
                        Icon = Device.OnPlatform("upload.png", "upload.png", "Assets/Icons/upload.png"),
                        Order = ToolbarItemOrder.Primary
                    };

                    submitCommand.Command = new RelayCommandAsync( async () =>
                    {
                        this.ViewModel.IsBusy = true;
                        
                        // If submit finishes correctly, go back to previous page given we don't anticipate any other work in this page.
                        if (await this.ViewModel.Submit() && this.Navigation.NavigationStack.Count > 0)
                        {
                            MessagingCenter.Send<Page>(this, Message.RefreshMainPage);
                            await this.Navigation.PopAsync();
                        }
                        this.ViewModel.IsBusy = false;
                    });

                    this.AddCommand(submitCommand);
                }

                if (this.ViewModel.IsSubmitted())
                {
                    ToolbarItem recallCommand = new ToolbarItem
                    {
                        Text = AppResources.Recall,
                        Icon = Device.OnPlatform("back.png", "back.png", "Assets/Icons/back.png"),
                        Order = ToolbarItemOrder.Primary
                    };

                    recallCommand.Command = new RelayCommandAsync(async () =>
                    {
                        this.ViewModel.IsBusy = true;
                        
                        // If recall finishes correctly, go back to previous page given we don't anticipate any other work in this page.
                        if (await this.ViewModel.Recall() && this.Navigation.NavigationStack.Count > 0)
                        {
                            MessagingCenter.Send<Page>(this, Message.RefreshMainPage);
                            await this.Navigation.PopAsync();
                        }
                        this.ViewModel.IsBusy = false;
                    });

                    this.AddCommand(recallCommand);
                }
            }
        }

        /// <summary>
        /// Add a new page in the tabbedPage
        /// </summary>
        /// <param name="section"></param>
        public void AddPage(Page page)
        {
            this.Children.Add(page);
        }

        /// <summary>
        /// Remove page from the view
        /// </summary>
        /// <param name="page">page to be removed</param>
        public void RemovePage(Page page)
        {
            this.Children.Remove(page);
        }

        /// <summary>
        /// Returns selected expense
        /// </summary>
        /// <returns></returns>
        public msdyn_expense GetExpense()
        {
            return this.ViewModel.Expense;
        }

        /// <summary>
        /// Add new toolbarITem to the toolbar
        /// </summary>
        /// <param name="command"></param>
        public void AddCommand(ToolbarItem command)
        {
            this.ToolbarItems.Add(command);
        }

        public bool IsValidExpense()
        {
            throw new System.NotImplementedException();
        }

        public async Task<bool> Save()
        {
            return await this.ViewModel.Save();
        }

        /// <summary>
        /// Check the behaviors of the new category, if there is data that needs to be deleted, 
        /// show a warning message to the user before change category.
        /// </summary>
        /// <param name="newExpenseCategory">expense category selected by the user</param>
        /// <param name="warningMessage">If there is data to delete, message with details of the data to be deleted</param>
        /// <returns>true if the change of category was successful</returns>
        public async Task<bool> SetCategory(msdyn_expensecategory newExpenseCategory, string warningMessage = null)
        {
            bool allowSetCategory = true;
            if (!string.IsNullOrEmpty(warningMessage))
            {
                // Confirm with user the change of category
                string message = string.Format(AppResources.ChangeExpenseTypeWarning, warningMessage);
                allowSetCategory = await MessageCenter.ShowDialog(message, null, null);
            }

            if (allowSetCategory)
            {
                this.ViewModel.Expense.ExpenseCategory = newExpenseCategory;
                // Change category and update new view
                this.ExtendedExpenseBehavior = ExpenseViewFactory.DecorateExpenseView(this.ExtendedExpenseBehavior);
                await this.ExtendedExpenseBehavior.CreateContent();
            }

            return allowSetCategory;
        }

        /// <summary>
        /// Check if there is any data that needs to be saved before going back
        /// </summary>
        /// <returns></returns>
        protected override bool OnBackButtonPressed()
        {
            if (ViewModel != null && ViewModel.HasPendingDataToSave)
            {
                ShowUnsavedDataWarning();
                return true;
            }
            else
            {
                return base.OnBackButtonPressed();
            }
        }

        /// <summary>
        /// Shows a warning message to the user about unsaved data, if the user decides to discard the changes
        /// the app we will go back whiout saving data, otherwise will stay in the current form.
        /// </summary>
        protected async void ShowUnsavedDataWarning()
        {
            bool continueGoingBack = await MessageCenter.ShowDialog(AppResources.UnsavedDataWarning, null, AppResources.Discard);
            if (continueGoingBack)
            {
                ViewModel.HasPendingDataToSave = false;
                if(this.Navigation.NavigationStack.Count >  0)
                {
                    await this.Navigation.PopAsync();
                }
            }     
        }
    }
}
