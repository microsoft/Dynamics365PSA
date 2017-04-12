using Common.iOS;
using Common.iOS.Utilities;
using Common.View;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(Settings), typeof(PSA.Time.iOS.SettingsViewRenderer))]
namespace PSA.Time.iOS
{
    public class SettingsViewRenderer : PageRenderer
    {
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();

            // Set the Platform parameter
            (Common.Utilities.Authentication.Authentication.Current as IOSAuthentication).PlatformParameters = new PlatformParameters(this);
        }
    }
}
