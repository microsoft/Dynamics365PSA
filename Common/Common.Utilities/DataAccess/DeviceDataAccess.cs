using System;
using System.Threading.Tasks;

namespace Common.Utilities.DataAccess
{
    /// <summary>
    /// Provides function to read or write files storing on device.
    /// </summary>
    public abstract class DeviceDataAccess
    {
        private static DeviceDataAccess current;
        public static DeviceDataAccess Current
        {
            get
            {
                if (current == null)
                {
                    throw new Exception("Please call Initialize() to initialize DeviceDataAccess");
                }
                return current;
            }
            private set
            {
                current = value;
            }
        }

        /// <summary>
        /// Initializes this base utility with a more concrete class for each specific platform
        /// </summary>
        /// <typeparam name="TEncryptedDataAccess"></typeparam>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static TDeviceDataAccess Initialize<TDeviceDataAccess>()
            where TDeviceDataAccess : DeviceDataAccess, new()
        {
            if (current == null)
            {
                current = new TDeviceDataAccess();
            }
            return (TDeviceDataAccess)Current;
        }

        protected DeviceDataAccess() { }

        /// <summary>
        /// Reads data from the application package folder, which is bundled inside the app package at build time
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public abstract Task<TResult> ReadFromApplicationPackage<TResult>(string filePath);

        /// <summary>
        /// Writes data to the application local folder on the device
        /// </summary>
        /// <param name="filePath">The file to write to including folder path and file name</param>
        /// <param name="content">Content to be written</param>
        /// <returns>True if the content is written successfully</returns>
        public abstract Task<bool> WriteToLocal(string filePath, object content);

        /// <summary>
        /// Reads data from the application local folder on the device
        /// </summary>
        /// <typeparam name="T">Object type of the data</typeparam>
        /// <param name="filePath">The file to read from including folder path and file name</param>
        /// <returns></returns>
        public abstract Task<TResult> ReadFromLocal<TResult>(string filePath);

        /// <summary>
        /// Deletes the file from the application local folder on the device
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public abstract Task<bool> DeleteFromLocal(string filePath);

        /// <summary>
        /// Deletes all the local files in the app application local folder
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> DeleteAllLocal();

        /// <summary>
        /// Write a Base64 encoded string to a particular path.
        /// </summary>
        /// <param name="documentBody">Base64 encoded string document body.</param>
        /// <param name="filePath">File path to be stored (with filename and extension).</param>
        /// <returns></returns>
        public abstract Task<string> WriteBytesToPath(string documentBody, string filePath);
    }
}
