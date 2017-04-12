using Common.Utilities.DataAccess;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Windows.Storage;

namespace Common.Windows.Utilities
{
    public class WindowsDeviceDataAccess : DeviceDataAccess
    {
        public override async Task<TResult> ReadFromApplicationPackage<TResult>(string filePath)
        {
            StorageFolder root = global::Windows.ApplicationModel.Package.Current.InstalledLocation;
            return await Read<TResult>(filePath, root);
        }

        public override async Task<bool> WriteToLocal(string filePath, object content)
        {
            StorageFolder root = ApplicationData.Current.LocalFolder;
            return await Write(filePath, root, content);
        }

        public override async Task<TResult> ReadFromLocal<TResult>(string filePath)
        {
            StorageFolder root = ApplicationData.Current.LocalFolder;
            return await Read<TResult>(filePath, root);
        }

        public override async Task<bool> DeleteFromLocal(string filePath)
        {
            StorageFolder root = ApplicationData.Current.LocalFolder;

            var file = await root.GetFileAsync(filePath);
            if (file != null)
            {
                await file.DeleteAsync();
            }

            return true;
        }

        public override async Task<bool> DeleteAllLocal()
        {
            StorageFolder root = ApplicationData.Current.LocalFolder;

            var files = await root.GetFilesAsync();
            if (files != null)
            {
                foreach (var file in files)
                {
                    await file.DeleteAsync();
                }
            }

            return true;
        }

        private async Task<TResult> Read<TResult>(string filePath, StorageFolder root)
        {
            try
            {
                var folderPathAndFileName = ExtractFolderPathAndFileName(filePath);
                StorageFolder folder = await GetFolderAsync(folderPathAndFileName.FolderPath, root);

                if (folder == null)
                {
                    return default(TResult);
                }

                StorageFile file = await folder.GetFileAsync(folderPathAndFileName.FileName);

                using (Stream stream = await file.OpenStreamForReadAsync())
                {
                    return await DataAccessUtil.ReadFromStream<TResult>(stream);
                }
            }
            catch (FileNotFoundException)
            {
                return default(TResult);
            }
        }

        private async Task<bool> Write(string filePath, StorageFolder root, object content)
        {
            var folderPathAndFileName = ExtractFolderPathAndFileName(filePath);
            StorageFolder folder = await GetFolderAsync(folderPathAndFileName.FolderPath, root);

            if (folder == null)
            {
                folder = await root.CreateFolderAsync(folderPathAndFileName.FolderPath);
            }

            StorageFile file = await folder.CreateFileAsync(folderPathAndFileName.FileName, CreationCollisionOption.ReplaceExisting);

            using (Stream stream = await file.OpenStreamForWriteAsync())
            {
                return await DataAccessUtil.WriteToStream(stream, content);
            }
        }

        private async Task<StorageFolder> GetFolderAsync(string filePath, StorageFolder root)
        {
            try
            {
                StorageFolder folder = root;

                List<string> partTokens = filePath.Replace("\\", "/").Split('/').ToList();
                partTokens.RemoveAt(partTokens.Count - 1);

                foreach (string folderName in partTokens)
                    folder = await folder.GetFolderAsync(folderName);

                return folder;
            }
            catch
            {
                return null;
            }
        }

        private FolderPathAndFileName ExtractFolderPathAndFileName(string filePath)
        {
            List<string> pathTokens = filePath.Replace("\\", "/").Split('/').ToList();
            var fileName = pathTokens[pathTokens.Count - 1];
            var folderPath = filePath.Replace(fileName, string.Empty);

            return new FolderPathAndFileName(folderPath, fileName);
        }

        public override Task<string> WriteBytesToPath(string documentBody, string filePath)
        {
            throw new NotImplementedException();
        }

        private struct FolderPathAndFileName
        {
            public string FolderPath { get; set; }
            public string FileName { get; set; }

            public FolderPathAndFileName(string folderPath, string fileName) : this()
            {
                FolderPath = folderPath;
                FileName = fileName;
            }
        }
    }
}
