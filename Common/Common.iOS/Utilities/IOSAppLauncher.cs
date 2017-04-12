using Common.Model.Map;
using Common.Utilities;
using System;
using System.Threading.Tasks;

namespace Common.iOS.Utilities
{
    class IOSAppLauncher : AppLauncher
    {
        public override async Task OpenFileAsync(string filePath)
        {
            throw new NotImplementedException();
        }

        public override Task OpenMapAsync(string address, Coordinate coordinate)
        {
            throw new NotImplementedException();
        }
    }
}
