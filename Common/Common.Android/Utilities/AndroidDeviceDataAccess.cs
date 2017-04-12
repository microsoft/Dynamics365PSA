using Android.Content;
using Common.Utilities.DataAccess;
using System;
using System.IO;
using System.Threading.Tasks;
using Environment = Android.OS.Environment;

namespace Common.Android.Utilities
{
    public class AndroidDeviceDataAccess : DeviceDataAccess
    {
        private Context context;
        public Context Context
        {
            get
            {
                return context;
            }
            internal set
            {
                context = value;
            }
        }

        public override async Task<TResult> ReadFromApplicationPackage<TResult>(string filePath)
        {
            if (filePath.StartsWith("Assets"))
                filePath = filePath.Replace("Assets/", "");

            try
            {
                using (var stream = context.Assets.Open(filePath))
                {
                    return await DataAccessUtil.ReadFromStream<TResult>(stream);
                }
            }
            catch (Java.IO.FileNotFoundException)
            {
                return default(TResult);
            }
        }

        public override async Task<bool> WriteToLocal(string filePath, object content)
        {
            using (var stream = context.OpenFileOutput(filePath, FileCreationMode.Private))
            {
                return await DataAccessUtil.WriteToStream(stream, content);
            }
        }

        public override async Task<TResult> ReadFromLocal<TResult>(string filePath)
        {
            try
            {
                using (var stream = context.OpenFileInput(filePath))
                {
                    return await DataAccessUtil.ReadFromStream<TResult>(stream);
                }
            }
            catch (Java.IO.FileNotFoundException)
            {
                return default(TResult);
            }
        }

        public override async Task<bool> DeleteFromLocal(string filePath)
        {
            return await Task.FromResult(context.DeleteFile(filePath));
        }

        public override async Task<bool> DeleteAllLocal()
        {
            var files = context.FileList();
            foreach (var file in files)
                await DeleteFromLocal(file);
            return true;
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
            return Path.Combine(Environment.ExternalStorageDirectory.Path, filename);
        }
    }
}