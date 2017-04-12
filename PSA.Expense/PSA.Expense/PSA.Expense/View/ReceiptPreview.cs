using Common.Model;
using Common.Utilities.Metadata;
using Common.Utilities.Resources;
using PSA.Expense.ViewModel;
using System;
using Xamarin.Forms;

namespace Common.View
{
    public class ReceiptPreview : ViewCell
    {
        protected Label attachment;
        protected ReceiptViewModel ViewModel;
        public static int HeightReceiptPreview = 70;

        public ReceiptPreview(ReceiptViewModel viewModel)
        {
            this.ViewModel = viewModel;
            Grid receiptTemplate = new Grid
            {
                RowSpacing = 2,
                RowDefinitions = {
                    new RowDefinition{ Height = 5 }, // 0.Margin
                    new RowDefinition{ Height = GridLength.Auto }, // 1.Preview, Name
                    new RowDefinition{ Height = GridLength.Auto }, // 2.Preview, Date
                    new RowDefinition{ Height = 5 }, // 3.Margin
                    new RowDefinition{ Height = 1 } // 4.Separator
                },
                ColumnSpacing = 0,
                ColumnDefinitions = 
                {
                    new ColumnDefinition{Width = 8}, // 0.Margin
                    new ColumnDefinition{Width = new GridLength(1, GridUnitType.Auto)}, // 1.Preview 
                    new ColumnDefinition{Width = 8}, // 2.Margin
                    new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star)}, // 3.Name and date
                },
                HorizontalOptions = LayoutOptions.Fill
            };
            
            attachment = ControlFactory.CreateIcon(null);
            receiptTemplate.Children.Add(attachment, 1, 2, 1, 3);

            Label title = new Label();
            title.SetBinding<Annotation>(Label.TextProperty, vm => vm.FileName, BindingMode.OneWay, null);
            receiptTemplate.Children.Add(title, 3, 1);

            Label date = ControlFactory.CreateLabel("CreatedOn", "{0:f}", ControlFactory.Label_Subtitle);
            date.Opacity = 0.8;
            receiptTemplate.Children.Add(date, 3, 2);

            BoxView separator = new BoxView();
            separator.Opacity = 0.8;
            separator.Color = Color.Gray;
            receiptTemplate.Children.Add(separator, 0, 4, 4, 5);

            this.View = receiptTemplate;
            CreateContextActions();
        }

        private void CreateContextActions()
        {
            if (ViewModel != null && ViewModel.SelectedExpense.CanEdit())
            {
                MenuItem delete = new MenuItem
                {
                    Text = AppResources.Delete,
                    Icon = Device.OnPlatform("delete.png", "delete.png", "Assets/Icons/delete.png"),
                    IsDestructive = true
                };
                delete.Clicked += delete_Clicked;
                delete.SetBinding<Annotation>(MenuItem.CommandParameterProperty, vm => vm.Id);
                this.ContextActions.Add(delete);
            }
        }

        protected async void delete_Clicked(object sender, EventArgs e)
        {
            MenuItem deleteMenuItem = sender as MenuItem;
            if (deleteMenuItem != null)
            {
                Guid? receiptToDelete = deleteMenuItem.CommandParameter as Guid?;
                await this.ViewModel.Delete(receiptToDelete ?? Guid.Empty);
            }
        }

        protected override void OnBindingContextChanged()
        {
            Annotation note = this.BindingContext as Annotation;
            if (note != null)
            {
                if (note.MimeType != null && note.MimeType.Contains("image"))
                {
                    // Set icon for image
                    attachment.Text = LabelHandler.IMAGE_SYMBOL;
                }
                else
                {
                    // Set icon for file
                    attachment.Text = LabelHandler.DOCUMENT_SYMBOL;
                }
            }
            base.OnBindingContextChanged();
        }
    }
}
