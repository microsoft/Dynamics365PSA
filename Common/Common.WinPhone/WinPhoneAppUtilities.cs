using Common.Utilities;
using Common.Utilities.Authentication;
using Common.Utilities.Camera;
using Common.Utilities.DataAccess;
using System.Threading.Tasks;

namespace Common.WinPhone
{
    /// <summary>
    /// Register concrete  implementations of Windows phone specific components like : Camera, File access and Authentication
    /// </summary>
    public class WinPhoneAppUtilities : AppUtilities
    {
        protected override async Task InitializeAuthentication()
        {
            await Authentication.Initialize<AuthenticationWindows>(new AuthenticationWindows());
        }

        protected override void InitializeAppLauncher()
        {
            AppLauncher.Initialize<WinPhoneAppLauncher>();
        }

        protected override void InitializeCamera()
        {
            CameraUtil.Initialize<CameraWindows>();
        }

        protected override void InitializeDeviceDataAccess()
        {
            DeviceDataAccess.Initialize<WinPhoneDeviceDataAccess>();
        }

        protected override void InitializeTimer()
        {
            
        }
    }
}
