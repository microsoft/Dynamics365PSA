using Common.Utilities.DataAccess;
using Foundation;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Common.iOS.Utilities
{
    public class IOSDeviceDataAccess : DeviceDataAccess
    {
        public override async Task<TResult> ReadFromApplicationPackage<TResult>(string filePath)
        {
            var documentPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.ApplicationDirectory, NSSearchPathDomain.User)[0].Path;
            var fullFilePath = Path.Combine(documentPath, filePath);
            await Task.Delay(1);
            var content = File.ReadAllText(fullFilePath);
            return DataAccessUtil.DeserializeObject<TResult>(content);
        }

        public override async Task<bool> WriteToLocal(string filePath, object content)
        {
            var documentPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
            var fullFilePath = Path.Combine(documentPath, filePath);
            var contentString = DataAccessUtil.SerializeObject(content);
            NSFileManager fileManager = NSFileManager.DefaultManager;
            await Task.Delay(1);
            try
            {
                File.WriteAllText(fullFilePath, contentString);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<TResult> ReadFromLocal<TResult>(string filePath)
        {
            var documentPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
            var fullFilePath = Path.Combine(documentPath, filePath);
            await Task.Delay(1);
            try
            {
                var content = File.ReadAllText(fullFilePath);
                return DataAccessUtil.DeserializeObject<TResult>(content);
            }
            catch (Exception)
            {
                return default(TResult);
            }
        }

        public override async Task<bool> DeleteFromLocal(string filePath)
        {
            NSFileManager fileManager = NSFileManager.DefaultManager;
            var documentPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
            var fulFilePath = Path.Combine(documentPath, filePath);
            NSError error;
            await Task.Delay(1);
            try
            {
                fileManager.Remove(fulFilePath, out error);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public override async Task<bool> DeleteAllLocal()
        {
            NSFileManager fileManager = NSFileManager.DefaultManager;
            var documentPath = NSFileManager.DefaultManager.GetUrls(NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User)[0].Path;
            NSError error;
            string[] files = fileManager.GetDirectoryContent(documentPath, out error);
            await Task.Delay(1);            
            try
            {
                foreach (var file in files)
                {
                    fileManager.Remove(Path.Combine(documentPath, file), out error);
                }
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Save the file from the Base64 endoded raw data provided.
        /// </summary>
        /// <param name="documentBody">Base64 encoded string.</param>
        /// <param name="filePath">File path to be stored (with filename and extension).</param>
        /// <returns>String that has the file path of the saved file.</returns>
        public override async Task<string> WriteBytesToPath(string documentBody, string filePath)
        {
            byte[] documentByteArr = Convert.FromBase64String(documentBody);

            if (documentByteArr.Length > 0)
            {
                if (!filePath.Contains("/"))
                {
                    filePath = this.AppendPathToFile(filePath);
                }
                // Write all the contents to a local file.
                File.WriteAllBytes(filePath, documentByteArr);
            }

            return filePath;
        }

        /// <summary>
        /// Returns the full path of the filename provided.
        /// </summary>
        /// <param name="filename">Filename of the file for which to get the path.</param>
        /// <returns>The full path for the file.</returns>
        private string AppendPathToFile(string filename)
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), filename);
        }
    }
}