using System;
using System.Threading.Tasks;

namespace Common.Archives.Utilities
{
    /// <summary>
    /// Provides function to save or load encrypted data. This should be used in case we need to store
    /// user credentials like password, authentication tokens, etc.
    /// </summary>
    public abstract class EncryptedDataAccess
    {
        private static EncryptedDataAccess current;
        public static EncryptedDataAccess Current
        {
            get
            {
                if (current == null)
                {
                    throw new Exception("Please call Initialize() to initialize EncryptedDataAccess");
                }
                return current;
            }
            private set
            {
                current = value;
            }
        }

        /// <summary>
        /// Initializes this base utitlity with a more concrete class for each specific platform
        /// </summary>
        /// <typeparam name="TEncryptedDataAccess"></typeparam>
        /// <param name="resource"></param>
        /// <returns></returns>
        public static EncryptedDataAccess Initialize<TEncryptedDataAccess>()
            where TEncryptedDataAccess : EncryptedDataAccess, new()
        {
            if (current == null)
            {
                current = new TEncryptedDataAccess();
            }
            return Current;
        }

        protected EncryptedDataAccess() { }

        /// <summary>
        /// Saves a value to the encrypted store, with the provided key.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        public abstract Task<bool> Save(string key, string value);

        /// <summary>
        /// Loads a value from the encrypted store, with the provided key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract Task<string> Load(string key);

        /// <summary>
        /// Deletes the value with the provided key from the encrypted store.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public abstract Task<bool> Delete(string key);

        /// <summary>
        /// Deletes all encrypted data.
        /// </summary>
        /// <returns></returns>
        public abstract Task<bool> DeleteAll();
    }
}
