using Android.Content;
using Android.Content.PM;
using Common.Utilities.AppPackage;

namespace Common.Android.Utilities
{
    public class AndroidAppPackage : IAppPackage
    {
        public Context Context { get; internal set; }

        public AndroidAppPackage(Context context)
        {
            this.Context = context;
        }

        public string GetVersionNumber()
        {
            PackageInfo pInfo = Context.PackageManager.GetPackageInfo(Context.PackageName, 0);
            return pInfo.VersionName;
        }
    }
}