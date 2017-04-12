using Common.Model.Map;
using Common.Utilities;
using System;
using System.Threading.Tasks;
using Windows.System;

namespace Common.Windows
{
    public class WindowsAppLauncher : AppLauncher
    {
        public override Task OpenFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public override async Task OpenMapAsync(string address, Coordinate coordinate)
        {
            string uri = "bingmaps:";

            if (address != null && coordinate != null)
            {
                uri += string.Format("?collection=point.{0}_{1}_{2}", coordinate.Latitude, coordinate.Longitude, Uri.EscapeDataString(address));
            }
            else if (address != null && coordinate == null)
            {
                uri += "?q=" + Uri.EscapeDataString(address);
            }

            await Launcher.LaunchUriAsync(new Uri(uri));
        }
    }
}
