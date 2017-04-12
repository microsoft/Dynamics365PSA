using Common.Utilities;
using Common.Utilities.Authentication;
using System.Threading.Tasks;

namespace Common.Test.Tools
{
    /// <summary>
    /// Testable implementation of the AppUtilities class.
    /// </summary>
    public class AppUtilitiesTestable : AppUtilities
    {
        protected override void InitializeAppLauncher()
        {
        }

        protected override async Task InitializeAuthentication()
        {
            // Implement the pattern followed by WinPhone/Android/iOS
            await Authentication.Initialize(new AuthenticationTestable());
        }

        protected override void InitializeCamera()
        {
        }

        protected override void InitializeDeviceDataAccess()
        {
        }

        protected override void InitializeTimer()
        {
        }
    }
}
