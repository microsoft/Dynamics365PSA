using Common.Utilities;
using Common.Utilities.Authentication;
using Common.Utilities.DataAccess;
using Common.Utilities.Telemetry;
using Common.Windows.Utilities;
using System.Threading.Tasks;

namespace Common.Windows
{
    public class WindowsAppUtilities : AppUtilities
    {
        public WindowsAppUtilities(ITelemetry telemetry = null)
        {
            WindowsRuntimeResourceManager.InjectIntoResxGeneratedApplicationResourcesClass(typeof(Common.Utilities.Resources.AppResources));

            MessageCenter.ShowErrorDelegate = WindowsDialog.Current.ShowMessageAction;
            MessageCenter.ShowMessageDelegate = WindowsDialog.Current.ShowMessageAction;
            MessageCenter.ShowDialogDelegate = WindowsDialog.Current.ShowDialogAction;

            InitializeAppPackage(new WindowsAppPackage());
            InitializeTelemetry(telemetry);
        }

        protected override void InitializeAppLauncher()
        {
            AppLauncher.Initialize<WindowsAppLauncher>();
        }

        protected override async Task InitializeAuthentication()
        {
            await Authentication.Initialize<WindowsAuthentication>(new WindowsAuthentication());
        }

        protected override void InitializeCamera()
        {
            
        }

        protected override void InitializeDeviceDataAccess()
        {
            DeviceDataAccess.Initialize<WindowsDeviceDataAccess>();
        }

        protected override void InitializeTimer()
        {
            Timer.Initialize<WindowsTimer>();
        }
    }
}
