using PSA.Time.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using UIKit;

[assembly: ExportRenderer(typeof(ListView), typeof(CustomListViewRenderer))]
namespace PSA.Time.iOS
{
    /// <summary>
    /// Custom listview renderer for iOS to remove the default separators when there is no content
    /// </summary>
    public class CustomListViewRenderer : ListViewRenderer
    {
        public CustomListViewRenderer()
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<ListView> e)
        {
            base.OnElementChanged(e);

            if (Control == null)
                return;

            var tableView = Control as UITableView;
            tableView.SectionIndexColor = TimeApp.PAGE_HEADER_COLOR.ToUIColor();
            tableView.SectionIndexBackgroundColor = TimeApp.PAGE_BACKGROUND_COLOR.ToUIColor();
            tableView.TableFooterView = new UIView();
        }
    }
}