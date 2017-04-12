using Common.Model;
using Common.Utilities.Resources;
using Common.View;
using PSA.Expense.ViewModel;
using Xamarin.Forms;

namespace PSA.Expense.View
{
    /// <summary>
    /// Dynamically shows each group of expenses as ExpenseListView inside a tab, 
    /// so it will have as many tabs as objects in groupedExpenses array
    /// </summary>
    public class ExpenseCollectionView : AppBasePage
    {                
        protected ExpenseCollectionViewModel viewModel;
                        
        public ExpenseTabbedPage listPage;

        protected ExpenseCollectionViewModel ViewModel
        {
            get
            {
                if (this.viewModel == null)
                {
                    this.viewModel = new ExpenseCollectionViewModel();
                }

                return this.viewModel;
            }
        }

        public ExpenseCollectionView() : base()
        {            
            this.Title = this.ViewModel.Title;

            // This is needed to prevent the toolbar from overlapping on the controls in the page.
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

        /// <summary>
        /// This method is called once for every tabbed page. The current tab page is always last on the growing itemSource
        /// list so if there are no expenses in the last element in the array, we insert a "no entries" label instead.
        /// </summary>
        protected override void CreateContent()
        {
            base.CreateContent();

            listPage = new ExpenseTabbedPage(this.ViewModel);
            listPage.listView.RefreshCommand = new Command(async () => await this.Refresh());            
            listPage.listView.SetBinding(ListView.IsRefreshingProperty, "IsBusy");

            this.Content = listPage;
        }

        protected override async System.Threading.Tasks.Task LoadData()
        {
            this.ViewModel.IsBusy = true;
            this.Title = ViewModel.Title != null ? ViewModel.Title.ToUpper() : string.Empty;

            await ViewModel.LoadExpenses();
                        
            this.ViewModel.IsBusy = false;
        }

        protected override async System.Threading.Tasks.Task Refresh()
        {
            this.ViewModel.IsBusy = true;

            // Start loading data
            await base.Refresh();

            this.ViewModel.IsBusy = false;
        }

        protected override void SetToolbarItems()
        {
            ToolbarItem createNewExpense = new ToolbarItem
            {
                Text = AppResources.Create,
                Icon = Device.OnPlatform("add.png", "add.png", "Assets/Icons/add.png"),
                Order = ToolbarItemOrder.Primary
            };

            createNewExpense.Clicked += async (sender, args) =>
            {
                ExpenseMainView expenseDetails = ExpenseViewFactory.CreateExpenseView(new msdyn_expense());
                await this.Navigation.PushAsync(expenseDetails);
            };

            this.ToolbarItems.Add(createNewExpense);
            base.SetToolbarItems();
        }
    }
}
