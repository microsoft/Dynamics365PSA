using Common.Utilities.DataAccess;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace Common.Test.Tools
{
    /// <summary>
    /// A testable implementation of the abstact <c>DeviceDataAccess</c> class.
    /// </summary>
    public class DeviceDataAccessTestable : DeviceDataAccess
    {
        // Use a dictionary to store items, to mock actual device
        // Since the methods are async, we need to provide thread safety, so use ConcurrentDictionary
        private ConcurrentDictionary<string, object> data;

        /// <summary>
        /// Implements a delay in miliseconds for async DeviceDataAccess operations.
        /// </summary>
        public int Delay { get; set; }

        public DeviceDataAccessTestable()
        {
            this.data = new ConcurrentDictionary<string, object>();
            this.Delay = 0;
        }

        public override async Task<bool> DeleteAllLocal()
        {
            await Task.Delay(this.Delay);
            data.Clear();
            return true;
        }

        public override async Task<bool> DeleteFromLocal(string filePath)
        {
            await Task.Delay(this.Delay);

            object removed;
            
            return data.TryRemove(filePath, out removed);
        }

        public override Task<TResult> ReadFromApplicationPackage<TResult>(string filePath)
        {
            throw new NotImplementedException();
        }

        public override async Task<TResult> ReadFromLocal<TResult>(string filePath)
        {
            await Task.Delay(this.Delay);
            return(TResult)data[filePath];
        }

        public override async Task<bool> WriteToLocal(string filePath, object content)
        {
            await Task.Delay(this.Delay);
            return data.TryAdd(filePath, content);
        }

        public override Task<string> WriteBytesToPath(string documentBody, string filePath)
        {
            throw new NotImplementedException();
        }
    }
}
