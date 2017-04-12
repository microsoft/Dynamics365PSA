using Android.Content;
using Common.Android.Utilities;
using Common.Utilities;
using Common.Utilities.Authentication;
using Common.Utilities.Camera;
using Common.Utilities.DataAccess;
using Common.Utilities.Resources;
using Common.Utilities.Telemetry;
using Common.View.Localize;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Common.Android
{
    public class AndroidAppUtilities : AppUtilities
    {
        private Context context;
        private static AndroidAppUtilities current;

        public AndroidAppUtilities(Context context, ITelemetry telemetry = null)
        {
            this.context = context;
            current = this;

            AppResources.Culture = DependencyService.Get<ILocalize>().GetCurrentCultureInfo();
            DependencyService.Get<ILocalize>().SetLocale();

            MessageCenter.ShowErrorDelegate = AndroidDialog.Current.ShowMessageAction;
            MessageCenter.ShowMessageDelegate = AndroidDialog.Current.ShowMessageAction;
            MessageCenter.ShowDialogDelegate = AndroidDialog.Current.ShowDialogAction;
            AndroidDialog.Current.Context = context;

            InitializeAppPackage(new AndroidAppPackage(context));
            InitializeTelemetry(telemetry);
        }

        // In Android, everything (file IO, show dialog, etc.) needs a Context object, which must be the current opening Activity
        // This method should be called every time a new Activity is opened, so that all utilities get access to that
        public static void SetContext(Context context)
        {
            current.context = context;
            (current.AppPackage as AndroidAppPackage).Context = context;

            (AppLauncher.Current as AndroidAppLauncher).Context = context;
            (Authentication.Current as AndroidAuthentication).Context = context;
            (CameraUtil.Current as CameraAndroid).Context = context;
            (DeviceDataAccess.Current as AndroidDeviceDataAccess).Context = context;
            AndroidDialog.Current.Context = context;
        }

        protected override void InitializeAppLauncher()
        {
            AppLauncher.Initialize<AndroidAppLauncher>();
            (AppLauncher.Current as AndroidAppLauncher).Context = context;
        }

        protected override async Task InitializeAuthentication()
        {
            await Authentication.Initialize<AndroidAuthentication>(new AndroidAuthentication(context));
        }

        protected override void InitializeCamera()
        {
            CameraUtil.Initialize<CameraAndroid>();
            (CameraUtil.Current as CameraAndroid).Context = context;
        }

        protected override void InitializeDeviceDataAccess()
        {
            DeviceDataAccess.Initialize<AndroidDeviceDataAccess>();
            (DeviceDataAccess.Current as AndroidDeviceDataAccess).Context = context;
        }

        protected override void InitializeTimer()
        {
            Timer.Initialize<AndroidTimer>();
        }
    }
}