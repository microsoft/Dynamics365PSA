using Common.iOS.Utilities;
using Common.Utilities;
using Common.Utilities.Authentication;
using Common.Utilities.Camera;
using Common.Utilities.DataAccess;
using Common.Utilities.Resources;
using Common.Utilities.Telemetry;
using Common.View.Localize;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Common.iOS
{
    public class IOSAppUtilities : AppUtilities
    {
        IPlatformParameters parameters;

        public IOSAppUtilities() {}

        public IOSAppUtilities(IPlatformParameters parameters, ITelemetry telemetry = null)
        {
            this.parameters = parameters;
            
            AppResources.Culture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            DependencyService.Get<ILocalize>().SetLocale();

            InitializeAppPackage(new IOSAppPackage());
            InitializeTelemetry(telemetry);
        }

        protected override void InitializeAppLauncher()
        {
            AppLauncher.Initialize<IOSAppLauncher>();
        }

        protected override async Task InitializeAuthentication()
        {
            await Authentication.Initialize<IOSAuthentication>(new IOSAuthentication(this.parameters));
        }

        protected override void InitializeCamera()
        {
            CameraUtil.Initialize<CameraiOS>();
        }

        protected override void InitializeDeviceDataAccess()
        {
            DeviceDataAccess.Initialize<IOSDeviceDataAccess>();
        }

        protected override void InitializeTimer()
        {
            Timer.Initialize<IOSTimer>();
        }
    }
}