using Common.Utilities.AppPackage;
using Common.Utilities.Telemetry;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Common.Utilities
{
    public abstract class AppUtilities
    {
        public static async Task PrintError(string message, string title = null, string buttonOk = null)
        {
#if DEBUG
            if (Debugger.IsAttached)
            {
                // An unhandled exception has occurred; break into the debugger
                Debugger.Break();
            }
            await Task.FromResult(true);
#else
            await Task.FromResult(true);
#endif
        }

        /// <summary>
        /// Initializes the utilities used in the app:
        /// - DeviceDataAccess to access the file system on the device
        /// - EncryptedDataAccess to access the encrypted credentials
        /// - Camera to access the device camera and capture pictures
        /// - Timer to add support for timer in PCL project
        /// - Authentication to initialize the data web proxy with the service url used to authenticate
        /// - AppLauncher to interact with other apps
        /// </summary>
        /// <returns>Asynchronous task. You MUST let the task complete before attempting to use
        /// any of the services provided from AppUtilities.</returns>
        public virtual async Task Initialize()
        {
            if (MessageCenter.ShowMessageDelegate == null)
                MessageCenter.ShowMessageDelegate = PrintError;

            InitializeDeviceDataAccess();            
            InitializeAppLauncher();
            InitializeCamera();
            InitializeTimer();
            await InitializeAuthentication();
        }

        protected abstract void InitializeAppLauncher();

        protected abstract Task InitializeAuthentication();

        protected abstract void InitializeCamera();

        protected abstract void InitializeDeviceDataAccess();        

        protected abstract void InitializeTimer();

        protected IAppPackage appPackage;
        public IAppPackage AppPackage
        {
            get
            {
                if (appPackage == null)
                    HandleUtilityNotInitialized("AppPackage");
                return appPackage;
            }

            private set { appPackage = value; }
        }
        
        protected ITelemetry telemetry;
        public ITelemetry Telemetry
        {
            get
            {
                if (telemetry == null)
                    HandleUtilityNotInitialized("Telemetry");
                return telemetry;
            }

            private set { telemetry = value; }
        }

        protected void InitializeAppPackage(IAppPackage appPackage)
        {
            this.AppPackage = appPackage;
        }

        protected void InitializeTelemetry(ITelemetry telemetry)
        {
            this.Telemetry = telemetry;
        }

        private void HandleUtilityNotInitialized(string utilityName)
        {
            // Dev level error only
            throw new InvalidOperationException(string.Format("{0} is not initialized. Please pass in a {0} when creating AppUtilities", utilityName));
        }
    }
}
