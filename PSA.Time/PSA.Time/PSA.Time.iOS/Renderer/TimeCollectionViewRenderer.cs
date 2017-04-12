using Common.iOS;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using PSA.Time.iOS;
using PSA.Time.View;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(TimeCollectionView), typeof(TimeCollectionViewRenderer))]
namespace PSA.Time.iOS
{
    class TimeCollectionViewRenderer : PageRenderer
    {
        TimeCollectionView page;
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as TimeCollectionView;
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);
            // Xamarin.Forms wraps page's view controller
            ViewController.ParentViewController.NavigationItem.SetHidesBackButton(true, false);
        }
    }
}