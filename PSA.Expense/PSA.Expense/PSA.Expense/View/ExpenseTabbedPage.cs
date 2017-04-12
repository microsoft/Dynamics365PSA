using Common.Model;
using Common.Utilities.Resources;
using Common.View;
using Common.View.CustomControl;
using PSA.Expense.ViewModel;
using System;
using Xamarin.Forms;

namespace PSA.Expense.View
{
    public class ExpenseTabbedPage : ContentView
    {
        // Controls
        public PickerEx statusSwitcher;
        public ListView listView;
        public Label noItemsLabel;

        // ViewModel
        private ExpenseCollectionViewModel viewModel;

        public ExpenseTabbedPage(ExpenseCollectionViewModel viewModel) : base()
        {
            this.viewModel = viewModel;

            // Main layout
            StackLayout mainLayout = new StackLayout();
            mainLayout.Spacing = 0;
            mainLayout.VerticalOptions = LayoutOptions.FillAndExpand;
            mainLayout.HorizontalOptions = LayoutOptions.FillAndExpand;

            // Title only on windows phone. There is a title added for the other platforms.
            Device.OnPlatform(WinPhone: () =>
            {
                Label header = new Label();
                header.SetBinding(Label.TextProperty, "Title");
                header.Style = (Style)Application.Current.Resources["Label_PageHeader"];

                mainLayout.Children.Add(header);
            });

            // Status filter
            statusSwitcher = ControlFactory.CreatePicker(
                null, viewModel.OnExpenseStatusSelected, Enum.GetNames(typeof(ExpenseCollectionViewModel.ExpenseStatus)), (int)ExpenseCollectionViewModel.ExpenseStatus.Draft);

            // List view
            this.initializeListView();

            // No items found label
            noItemsLabel = new Label
            {
                Text = AppResources.NoEntries,
                HorizontalOptions = LayoutOptions.Center,
                IsVisible = false,
                HeightRequest = 50
            };

            mainLayout.Children.Add(statusSwitcher);
            mainLayout.Children.Add(noItemsLabel);
            mainLayout.Children.Add(listView);

            this.Content = mainLayout;

            // We only update the visibility bindings once the first load completes
            this.viewModel.LoadExpensesCompleted += this.ViewModel_LoadExpensesCompleted;
        }

        private void initializeListView()
        {            
            listView = new ListView
            {
                ItemTemplate = new DataTemplate(LoadExpensePreviewTemplate),
                ItemsSource = viewModel.ExpensesCollection,
                // iOS list scrolls top item out of view when HasUnevenRows=true.
                HasUnevenRows = Device.OnPlatform(false, true, true),
                IsPullToRefreshEnabled = true,
                RowHeight = ExpensePreview.HeightExpensePreview,
                SeparatorVisibility = SeparatorVisibility.None,
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand
            };

            listView.ItemSelected += async (sender, args) =>
            {
                msdyn_expense selectedExpense = args.SelectedItem as msdyn_expense;
                if (selectedExpense != null)
                {
                    await this.NavigateToExpenseDetailsView(selectedExpense);
                    // Deselect the item
                    listView.SelectedItem = null;
                }
            };
        }

        public ViewCell LoadExpensePreviewTemplate()
        {
            ViewCell expense = new ViewCell();
            expense.View = new ExpensePreview();
            return expense;
        }

        protected async System.Threading.Tasks.Task NavigateToExpenseDetailsView(msdyn_expense expense)
        {
            ExpenseMainView expenseDetails = ExpenseViewFactory.CreateExpenseView(expense);
            await this.Navigation.PushAsync(expenseDetails);
        }

        private void ViewModel_LoadExpensesCompleted(object sender, EventArgs e)
        {
            if (this.viewModel.ExpensesCollection != null)
            {
                bool hasItems = this.viewModel.ExpensesCollection.Count > 0;
                noItemsLabel.IsVisible = !hasItems;
                listView.IsVisible = hasItems;
            }
        }
    }
}
