using Common.Model;
using Common.Utilities;
using Common.Utilities.DataAccess.ServiceAccess;
using Common.Utilities.Extensions;
using Common.Utilities.Metadata;
using Common.Utilities.Resources;
using Common.View;
using Common.View.CustomControl;
using Common.ViewModel;
using Common.ViewModel.Command;
using PSA.Expense.ViewModel;
using System;
using Xamarin.Forms;

namespace PSA.Expense.View
{
    public class ExpenseDetailsPage : BaseContentPage
    {
        protected ExpenseViewModel ViewModel;
        protected PickerEx projectPicker;
        protected PickerEx categoryPicker;
        protected PickerEx currencyPicker;
        protected DatePickerEx transactionDatePicker;
        protected EditorEx descriptionEditor;
        protected EntryEx amountEntry;
        protected EntryEx salesTaxEntry;
        protected ToolbarItem deleteExpenseCommand;
        protected ToolbarItem saveExpenseCommand;

        public ExpenseDetailsPage()
            : base()
        {
            this.Title = AppResources.Details;

            // This is needed to prevent the toolbar from overlapping on the controls in the page.
            OnPlatform<Thickness> op = (OnPlatform<Thickness>)Application.Current.Resources["PagePadding"];
            this.Padding = Device.OnPlatform(op.iOS, op.Android, op.WinPhone);
        }

        /// <summary>
        /// Initialize expense view model
        /// </summary>
        /// <param name="viewModel"></param>
        protected override void InitViewModel(BaseViewModel viewModel)
        {
            this.ViewModel = viewModel as ExpenseViewModel;
            if (this.ViewModel == null)
            {
                this.ViewModel = new ExpenseViewModel(new msdyn_expense());
            }
        }

        /// <summary>
        /// Load reference data and set defaults 
        /// </summary>
        /// <returns></returns>
        protected override async System.Threading.Tasks.Task LoadData()
        {
            await ViewModel.LoadReferenceData();
            ViewModel.SetDefaultsValues();
        }

        protected override void OnBindingContextChanged()
        {
            categoryPicker.SetBinding(PickerEx.SelectedItemProperty, new Binding(path: "Expense.msdyn_ExpenseCategory", converter: new Common.View.ValueConverter.EntityPreviewConverter<msdyn_expensecategory>(), converterParameter: this.ViewModel, mode: BindingMode.TwoWay));            
            categoryPicker.ItemsSource = ViewModel.GetReferenceKeys<msdyn_expensecategory>(msdyn_expensecategory.EntityLogicalName);

            projectPicker.SetBinding(PickerEx.SelectedItemProperty, new Binding(path: "Expense.msdyn_Project", converter: new Common.View.ValueConverter.EntityPreviewConverter<msdyn_project>(), converterParameter: this.ViewModel, mode: BindingMode.TwoWay));
            projectPicker.ItemsSource = ViewModel.GetReferenceKeys<msdyn_project>(msdyn_project.EntityLogicalName);

            currencyPicker.SetBinding(PickerEx.SelectedItemProperty, new Binding(path: "Expense.TransactionCurrencyId", converter: new Common.View.ValueConverter.EntityPreviewConverter<TransactionCurrency>(), converterParameter: this.ViewModel, mode: BindingMode.TwoWay));
            currencyPicker.ItemsSource = ViewModel.GetReferenceKeys<TransactionCurrency>(TransactionCurrency.EntityLogicalName);

            this.IsEnabled = ViewModel.CanEdit();
            projectPicker.IsEnabled = this.IsEnabled;
            categoryPicker.IsEnabled = this.IsEnabled;
            currencyPicker.IsEnabled = this.IsEnabled;
            transactionDatePicker.IsEnabled = this.IsEnabled;
            descriptionEditor.IsEnabled = this.IsEnabled;
            amountEntry.IsEnabled = this.IsEnabled;
            salesTaxEntry.IsEnabled = this.IsEnabled;

            base.OnBindingContextChanged();
        }

        protected override void SetBindingContext()
        {
            projectPicker = ControlFactory.CreatePicker(null, ViewModel.OnProjectSelected);
            categoryPicker = ControlFactory.CreatePicker(null, ViewModel.OnCategorySelected);
            currencyPicker = ControlFactory.CreatePicker(null, ViewModel.OnCurrencySelected);
            transactionDatePicker = ControlFactory.CreateDatePicker("Expense.msdyn_TransactionDate_utc", DateTime.Today, "D");
            descriptionEditor = ControlFactory.CreateEditor("Expense.msdyn_name");
            descriptionEditor.TextChanged += this.createEditorTextChangedHandler(msdyn_timeentry.DescriptionLength);

            amountEntry = ControlFactory.CreateEntry("Expense.TransactionAmount", BindingMode.TwoWay, Keyboard.Numeric, "{0:N2}", String.Format("{0:N2}", 0));
            amountEntry.Focused += (s, e) =>
            {
                if (amountEntry.Text.Equals("0.00") || amountEntry.Text.Equals("0"))
                {
                    amountEntry.Text = String.Empty;
                }
            };

            salesTaxEntry = ControlFactory.CreateEntry("Expense.SalesTaxAmount", BindingMode.TwoWay, Keyboard.Numeric, "{0:N2}", String.Format("{0:N2}", 0));
            salesTaxEntry.Focused += (s, e) =>
            {
                if (salesTaxEntry.Text.Equals("0.00") || salesTaxEntry.Text.Equals("0"))
                {
                    salesTaxEntry.Text = String.Empty;
                }
            };
            this.BindingContext = ViewModel;
        }

        protected override void CreateContent(StackLayout details)
        {
            LabelIcon filterIcon = ControlFactory.CreateIcon(LabelHandler.FILTER_SYMBOL, ControlFactory.Small_Label_Icon);
            SetProjectFilterName(filterIcon);

            object[] expenseDetailsRows = {
                new Xamarin.Forms.View[]{
                    ControlFactory.CreateStaticLabel(AppResources.Project), 
                    filterIcon,
                },
                projectPicker,
                null,
                ControlFactory.CreateStaticLabel(AppResources.Category),
                categoryPicker,
                null,
                ControlFactory.CreateStaticLabel(AppResources.Date),
                transactionDatePicker,
                null,
                new Xamarin.Forms.View[]{
                    ControlFactory.CreateStaticLabel(AppResources.Amount),
                    ControlFactory.CreateStaticLabel(AppResources.Currency)
                },
                new Xamarin.Forms.View[]{
                    amountEntry,
                    currencyPicker
                },
                null,
                ControlFactory.CreateStaticLabel(AppResources.SalesTax),
                salesTaxEntry,
                null,
                ControlFactory.CreateStaticLabel(AppResources.Description),
                descriptionEditor
            };

            TapGestureRecognizer filterTapGestureRecognizer = new TapGestureRecognizer() { NumberOfTapsRequired = 1 };
            filterTapGestureRecognizer.Tapped += FilterProject;
            filterIcon.GestureRecognizers.Add(filterTapGestureRecognizer);

            // Add Expense preview at top of the page
            Grid expensePreview = new ExpensePreview();
            expensePreview.SetBinding<ExpenseViewModel>(Grid.BindingContextProperty, vm => vm.Expense, BindingMode.OneWay);
            expensePreview.RowDefinitions[0].Height = 0; // Remove default margin at top
            expensePreview.RowDefinitions.Add(new RowDefinition { Height = 10 }); //Margin bottom
            details.Children.Add(expensePreview);

            // Add expense details
            Grid expenseDetails = new Grid();
            expenseDetails.ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition{Width = 10}, // 0.Margin 
                new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star)}, // 1. 1st column
                new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star)}, // 2. 2nd column
                new ColumnDefinition{Width = 10} // 3.margin
            };
            expenseDetails.ColumnSpacing = 2;
            expenseDetails.RowSpacing = 0;
            ControlFactory.AutoFill(expenseDetails, expenseDetailsRows, 1, 2);
            details.Children.Add(new ScrollView() { Content = expenseDetails });
        }

        private EventHandler<TextChangedEventArgs> createEditorTextChangedHandler(int allowedTextSize)
        {
            return (object sender, TextChangedEventArgs e) =>
            {
                EditorEx editor = sender as EditorEx;

                if (e.OldTextValue == null && this.ViewModel.HasPendingDataToSave)
                {
                    this.ViewModel.HasPendingDataToSave = false;
                }

                if (e.NewTextValue.Length > allowedTextSize)
                {
                    editor.Text = e.NewTextValue.Substring(0, allowedTextSize);

                    // Let user know you've truncated the text.
                    MessageCenter.ShowMessage(string.Format(AppResources.TextSizeLimitedTo, allowedTextSize)).DoNotAwait();
                }

                editor.InvalidateLayout();
            };
        }

        /// <summary>
        /// Change current filter and open the look up
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected async void FilterProject(object sender, EventArgs e)
        {
            LabelIcon filterIcon = sender as LabelIcon;
            if (filterIcon != null)
            {
                FilterQuery filter = await this.SetProjectFilterName(filterIcon, true);
                if (filter != null)
                {
                    ControlFactory.SetItems(projectPicker, filter.Result);
                }
            }
        }

        /// <summary>
        /// Sets the label of the project filter
        /// </summary>
        /// <param name="filterIcon">Label with the filter symbol</param>
        /// <param name="switchFilter">True if before setting the name it should switch</param>
        /// <returns>Current Filter</returns>
        protected async System.Threading.Tasks.Task<FilterQuery> SetProjectFilterName(LabelIcon filterIcon, bool switchFilter = false)
        {
            if (filterIcon != null)
            {
                FilterQuery filter = await this.ViewModel.GetFilterProject(switchFilter);
                if (filter != null)
                {
                    filterIcon.Text = String.Format("{0} {1}", LabelHandler.FILTER_SYMBOL, filter.Name);
                    return filter;
                }
            }
            return null;
        }

        protected override void SetToolbarItems()
        {
            base.SetToolbarItems();
            if (ViewModel.CanEdit())
            {
                // Delete command.
                deleteExpenseCommand = new ToolbarItem
                {
                    Text = AppResources.Delete,
                    Icon = Device.OnPlatform("delete.png", "delete.png", "Assets/Icons/delete.png"),
                    Order = ToolbarItemOrder.Primary,
                    IsDestructive = true
                };

                deleteExpenseCommand.Command = new RelayCommandAsync(async () =>
                {
                    ViewModel.IsBusy = true;
                    if (await this.ViewModel.Delete() && this.Navigation.NavigationStack.Count > 0)
                    {
                        MessagingCenter.Send<Page>(this, Message.RefreshMainPage);
                        await this.Navigation.PopAsync();
                    }
                    ViewModel.IsBusy = false;
                });

                // Save command.
                saveExpenseCommand = new ToolbarItem
                {
                    Text = AppResources.Save,
                    Icon = Device.OnPlatform("save.png", "save.png", "Assets/Icons/save.png"),
                    Order = ToolbarItemOrder.Primary
                };

                saveExpenseCommand.Command = new RelayCommandAsync(async () =>
                {
                    ViewModel.IsBusy = true;
                    if (await this.ViewModel.ForcedSave() && this.Navigation.NavigationStack.Count > 0)
                    {
                        MessagingCenter.Send<Page>(this, Message.RefreshMainPage);
                        await this.Navigation.PopAsync();
                    }
                    ViewModel.IsBusy = false;
                });

                this.ToolbarItems.Add(saveExpenseCommand);
                this.ToolbarItems.Add(deleteExpenseCommand);
            }
        }
    }
}
