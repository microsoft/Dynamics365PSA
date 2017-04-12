using Common.Utilities.DataAccess;
using System;
using System.IO;
using System.Threading.Tasks;
using Windows.Storage;

namespace Common.WinPhone
{
    public class WinPhoneDeviceDataAccess : DeviceDataAccess
    {
        // current application folder
        static private StorageFolder localFolder = ApplicationData.Current.LocalFolder;

        public override Task<TResult> ReadFromApplicationPackage<TResult>(string filePath)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Cache data locally
        /// </summary>
        /// <param name="content">object to be cached</param>
        /// <param name="filename">file name for cache</param>
        /// <returns></returns>
        public override async Task<bool> WriteToLocal(string filename, object content)
        {
            bool success = false;
            if (!string.IsNullOrEmpty(filename))
            {
                StorageFile File = await localFolder.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
                try
                {
                    string json = DataAccessUtil.SerializeObject(content);
                    using (var writer = new StreamWriter(File.Path, false))
                    {
                        await writer.WriteAsync(json);
                        success = true;
                    }
                }
                catch
                {
                    success = false;
                }
            }
            return success;
        }

        /// <summary>
        /// Read from local folder the specified file and deserialze the object before return
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="filename"></param>
        /// <returns></returns>
        public override async Task<TResult> ReadFromLocal<TResult>(string filename)
        {
            StorageFile file;
            TResult result = default(TResult);
            if (!string.IsNullOrEmpty(filename))
            {
                try
                {
                    file = await localFolder.GetFileAsync(filename);
                    string data = await FileIO.ReadTextAsync(file);
                    result = DataAccessUtil.DeserializeObject<TResult>(data);
                }
                catch
                {
                    return result;
                }
            }
            return result;
            
        }

        public override async Task<bool> DeleteFromLocal(string filePath)
        {
            var file = await localFolder.GetFileAsync(filePath);
            await file.DeleteAsync();
            return true;
        }
        
        public override async Task<bool> DeleteAllLocal()
        {
            var files = await localFolder.GetFilesAsync();
            foreach (var file in files)
            {
                await file.DeleteAsync();
            }

            return true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="documentBody"></param>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public override async Task<string> WriteBytesToPath(string documentBody, string filePath)
        {
            if (!String.IsNullOrEmpty(filePath) && documentBody != null)
            {
                StorageFolder localFolder = ApplicationData.Current.LocalFolder;
                StorageFile file = await localFolder.CreateFileAsync(filePath, CreationCollisionOption.ReplaceExisting);
                await FileIO.WriteBytesAsync(file, Convert.FromBase64String(documentBody));
                return file.Path;
            }

            return String.Empty;
        }

        private string AppendPathToFile(string filename)
        {
            return Path.Combine(ApplicationData.Current.LocalFolder.Path, filename);
        }
    }
}
