using Common.Model;
using Common.Utilities.Metadata;
using Common.View;
using Common.View.CustomControl;
using Common.View.ValueConverter;
using PSA.Expense.View.ValueConverter;
using Xamarin.Forms;

namespace PSA.Expense.View
{
    public class ExpensePreview : Grid
    {
        public static int HeightExpensePreview = Device.OnPlatform<int>(62, 62, 75);
        protected Label amountLabel;
        protected Label previewLabel;
        protected Label transactionDateLabel;
        protected Label statusLabel;
        protected BoxView separator;
        protected Label expenseTypeIcon;
        protected bool selected;

        public ExpensePreview()
        {
            this.BackgroundColor = BaseApp.PAGE_BACKGROUND_COLOR;
            RowSpacing = 2;
            RowDefinitions = new RowDefinitionCollection
            {
                new RowDefinition{ Height = 6 }, // 0.Margin
                new RowDefinition{ Height = GridLength.Auto }, // 1.Icon, preview, amount
                new RowDefinition{ Height = GridLength.Auto }, // 2.Icon, date, status
                new RowDefinition{ Height = 6 }, // 3.Margin
                new RowDefinition{ Height = 1 } // 4.Separator
            };
            ColumnSpacing = 1;
            ColumnDefinitions = new ColumnDefinitionCollection
            {
                new ColumnDefinition{Width = 25}, // 0.margin or indicator
                new ColumnDefinition{Width = new GridLength(1, GridUnitType.Auto)}, // 1.icon
                new ColumnDefinition{Width = 10}, // 2.margin
                new ColumnDefinition{Width = new GridLength(1, GridUnitType.Auto)}, // 3.preview and date
                new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star)}, // 4.amount and status
                new ColumnDefinition{Width = 15} // 5.margin
            };

            // Create views with bindings for displaying each property.
            amountLabel = ControlFactory.CreateLabel("FormattedTransactionAmount", null, ControlFactory.Label_BigAmount);
            previewLabel = ControlFactory.CreateLabel("msdyn_ExpenseCategory.Name");
            transactionDateLabel = ControlFactory.CreateLabel("msdyn_TransactionDate", "{0:D}", ControlFactory.Label_Subtitle);
            statusLabel = ControlFactory.CreateLabel("msdyn_ExpenseStatus", "{0}", ControlFactory.Label_Indicator, new OptionSetConverter<msdyn_expense_msdyn_expensestatus>());

            separator = new BoxView();
            separator.HeightRequest = 1;
            separator.Color = Color.FromHex("#FFE1E3E4");

            LabelIcon attachment = ControlFactory.CreateIcon(LabelHandler.ATTACH_SYMBOL, ControlFactory.Small_Label_Icon);
            attachment.Opacity = 0.8;
            attachment.SetBinding(Image.IsVisibleProperty, "HasReceipts");

            expenseTypeIcon = ControlFactory.CreateIcon(null, ControlFactory.Label_Icon);
            expenseTypeIcon.SetBinding(Label.TextProperty, new Binding("ExpenseCategory.msdyn_ExpenseType", 
                BindingMode.OneWay, new ExpenseTypeToIconConverter()));
            expenseTypeIcon.SetBinding(Label.TextColorProperty, new Binding("ExpenseCategory.msdyn_ExpenseType",
                BindingMode.OneWay, new ExpenseTypeToColorConverter()));

            int row = 1;
            this.Children.Add(attachment, 0, 1, row, row + 2 );
            this.Children.Add(expenseTypeIcon, 1, 2, row, row + 2);
            this.Children.Add(previewLabel, 3, row);
            this.Children.Add(amountLabel, 4, row);
            row++;
            this.Children.Add(transactionDateLabel, 3, row);
            this.Children.Add(statusLabel, 4, row);

            row = row + 2;
            this.Children.Add(separator, 0, 6, row, row + 1);
        }
    }
}
