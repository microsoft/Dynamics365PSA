using Common.Model.Map;
using System;
using System.Threading.Tasks;

namespace Common.Utilities
{
    /// <summary>
    /// Opens another app based on the content type
    /// </summary>
    public abstract class AppLauncher
    {
        private static AppLauncher current;
        public static AppLauncher Current
        {
            get
            {
                if (current == null)
                {
                    throw new Exception("Please call Initialize() to initialize AppLauncher");
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
        /// <typeparam name="TAppLauncher"></typeparam>
        /// <returns></returns>
        public static AppLauncher Initialize<TAppLauncher>()
            where TAppLauncher : AppLauncher, new()
        {
            if (current == null)
            {
                current = new TAppLauncher();
            }
            return Current;
        }

        protected AppLauncher() { }

        public abstract Task OpenMapAsync(string address, Coordinate coordinate);

        public abstract Task OpenFileAsync(string filePath);
    }
}