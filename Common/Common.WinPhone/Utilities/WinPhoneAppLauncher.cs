using Common.Model.Map;
using Common.Utilities;
using System;
using System.Threading.Tasks;
using Windows.Storage;

namespace Common.WinPhone
{
    public class WinPhoneAppLauncher : AppLauncher
    {
        public override async Task OpenFileAsync(string filePath)
        {
            try
            {
                StorageFile fileToOpen = await StorageFile.GetFileFromPathAsync(filePath);
                await Windows.System.Launcher.LaunchFileAsync(fileToOpen);
            }
            catch
            {
                // File does not exist so do nothing.
            }
        }

        public override Task OpenMapAsync(string address, Coordinate coordinate)
        {
            throw new NotImplementedException();
        }
    }
}
