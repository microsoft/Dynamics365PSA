using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Common.Utilities.DataAccess;
using Windows.Security.Credentials;

namespace Common.Archives.Utilities
{
    public class WindowsEncryptedDataAccess : EncryptedDataAccess
    {
        public string AppPackageName { get; private set; }

        private bool isInitialized = false;
        private const string encryptedDataAccessSignalFileName = "EncryptedSignal.cer";
        private const string encryptedDataAccessSignalFileContent = "Encrypted";

        public WindowsEncryptedDataAccess()
        {
            AppPackageName = global::Windows.ApplicationModel.Package.Current.Id.Name;
        }

        // You can only store up to ten credentials per app in the Credential Locker. If you try to store more than ten credentials, you will encounter an Exception.
        // https://msdn.microsoft.com/en-us/library/windows/apps/hh701231.aspx
        public override async Task<bool> Save(string key, string value)
        {
            await Initialize();

            if (key == null) throw new ArgumentNullException("EncryptedDataAccess.Save: key cannot be null");

            try
            {
                var passwordVault = new PasswordVault();
                passwordVault.Add(new PasswordCredential(AppPackageName, key, value));
                return await Task.FromResult(true);
            }
            catch
            {
#if DEBUG
                if (Debugger.IsAttached)
                {
                    Debugger.Break();
                }
#endif
                throw;
            }
        }

        public override async Task<string> Load(string key)
        {
            await Initialize();

            if (key == null) throw new ArgumentNullException("EncryptedDataAccess.Load: key cannot be null");

            try
            {
                var passwordVault = new PasswordVault();
                var passwordCredential = passwordVault.Retrieve(AppPackageName, key);

                if (passwordCredential != null)
                    return await Task.FromResult(passwordCredential.Password);
                else
                    return await Task.FromResult<string>(null);
            }
            // If key is not found, it will throw exception, therefore ignore that exception
            catch { }

            return await Task.FromResult<string>(null);
        }

        public override async Task<bool> Delete(string key)
        {
            await Initialize();

            if (key == null) throw new ArgumentNullException("EncryptedDataAccess.Delete: key cannot be null");

            try
            {
                var passwordVault = new PasswordVault();
                var passwordCredential = passwordVault.Retrieve(AppPackageName, key);
                passwordVault.Remove(passwordCredential);
                return await Task.FromResult(true);
            }
            // If key is not found, it will throw exception, therefore ignore that exception
            catch { }

            return await Task.FromResult(true);
        }

        public override async Task<bool> DeleteAll()
        {
            await Initialize();

            var passwordVault = new PasswordVault();
            var passwordCredentials = passwordVault.RetrieveAll();

            if (passwordCredentials != null)
            {
                foreach (var passwordCredential in passwordCredentials)
                {
                    passwordVault.Remove(passwordCredential);
                }
            }

            return await Task.FromResult(true);
        }

        private async Task Initialize()
        {
            if (!isInitialized)
            {
                isInitialized = true;

                var passwordVault = new PasswordVault();
                var passwordCredentials = passwordVault.RetrieveAll();

                // When the app is uninstalled, the PasswordVault is not cleaned up
                // Therefore when the app is reinstalled again, it is still able to access the PasswordVault of the previous version
                // Solution: use a signal file as a signal to indicate if the app is just installed
                // If the file is not there, the app has just been installed, therefore clean all the credentials of the previous version, then write the file
                // Else, proceed without cleaning anything

                // Read signal file
                var signalFile = await DeviceDataAccess.Current.ReadFromLocal<string>(encryptedDataAccessSignalFileName);
                if (signalFile != encryptedDataAccessSignalFileContent)
                {
                    // File not exists, clean up the PasswordVault and create the file
                    if (passwordCredentials != null)
                    {
                        foreach (var passwordCredential in passwordCredentials)
                        {
                            passwordVault.Remove(passwordCredential);
                        }
                    }

                    await DeviceDataAccess.Current.WriteToLocal(encryptedDataAccessSignalFileName, encryptedDataAccessSignalFileContent);
                }
                //else
                //{
                //    File exists, do nothing
                //}

                // Clean up credentials with same key but different resource, as that may be created by the older version of the
                // app, which needs to be cleaned up to prevent reaching the credential limit.
                if (passwordCredentials != null)
                {
                    foreach (var passwordCredential in passwordCredentials)
                    {
                        if (passwordCredential.Resource != AppPackageName)
                            passwordVault.Remove(passwordCredential);
                    }
                }
            }
        }
    }
}
