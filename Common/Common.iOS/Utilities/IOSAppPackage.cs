using Common.Utilities.AppPackage;
using Foundation;

namespace Common.iOS.Utilities
{
    public class IOSAppPackage : IAppPackage
    {
        public string GetVersionNumber()
        {
            var infoPlist = NSBundle.MainBundle.InfoDictionary;
            return infoPlist["CFBundleShortVersionString"] + "." + infoPlist["CFBundleVersion"];
        }
    }
}