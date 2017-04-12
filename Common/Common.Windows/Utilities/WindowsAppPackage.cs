using Common.Utilities.AppPackage;

namespace Common.Windows.Utilities
{
    public class WindowsAppPackage : IAppPackage
    {
        public string GetVersionNumber()
        {
            var version = global::Windows.ApplicationModel.Package.Current.Id.Version;
            return string.Format("{0}.{1}.{2}.{3}", version.Major, version.Minor, version.Build, version.Revision);
        }
    }
}
