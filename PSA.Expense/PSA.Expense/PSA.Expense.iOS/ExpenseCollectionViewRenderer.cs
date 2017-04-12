using PSA.Expense.iOS;
using PSA.Expense.View;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ExpenseCollectionView), typeof(ExpenseCollectionViewRenderer))]
namespace PSA.Expense.iOS
{
    class ExpenseCollectionViewRenderer : PageRenderer
    {
        ExpenseCollectionView page;
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as ExpenseCollectionView;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            // Xamarin.Forms wraps page's view controller
            ViewController.ParentViewController.NavigationItem.SetHidesBackButton(true, false);
        }
    }
}