using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Common.View;
using PSA.Expense.iOS;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;
using Common.iOS;

[assembly: ExportRenderer(typeof(Settings), typeof(SettingsRenderer))]
namespace PSA.Expense.iOS
{
    class SettingsRenderer : PageRenderer
    {
        Settings page;
    
        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            page = e.NewElement as Settings;
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            IOSAppUtilities util = new IOSAppUtilities(new PlatformParameters(this));
            await util.Initialize();
        }
    }
}