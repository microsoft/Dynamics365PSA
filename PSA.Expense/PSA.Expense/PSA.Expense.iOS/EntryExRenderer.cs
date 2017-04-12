using Common.View.CustomControl;
using PSA.Expense.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(EntryEx), typeof(EntryExRenderer))]
namespace PSA.Expense.iOS
{
    public class EntryExRenderer: EntryRenderer
    {
        protected override void OnElementChanged(ElementChangedEventArgs<Entry> e)
        {
            base.OnElementChanged(e);
            if ((Control != null) && (Control.KeyboardType == UIKit.UIKeyboardType.DecimalPad))
            {
                Control.TextAlignment = UIKit.UITextAlignment.Right;
            }
        }
    }
}