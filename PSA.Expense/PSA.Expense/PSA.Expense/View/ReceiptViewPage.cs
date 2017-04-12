using Xamarin.Forms;

namespace PSA.Expense.View
{
    /// <summary>
    /// This page is used in iOS to view expense receipts since there is no way
    /// to open and view the images in the Photos app.
    /// </summary>
    public class ReceiptViewPage : ContentPage
    {
        public ReceiptViewPage(string filepath, string filename) : base()
        {
            this.Title = filename;
            
            this.Content = new StackLayout
            {
                Padding = new Thickness(15, 15, 15, 15),
                Children =
                {
                    new Image
                    {
                        Source = ImageSource.FromFile(filepath),
                        VerticalOptions = LayoutOptions.Center,
                        HorizontalOptions = LayoutOptions.Center,                                                
                    }
                }
            };
        }
    }
}
