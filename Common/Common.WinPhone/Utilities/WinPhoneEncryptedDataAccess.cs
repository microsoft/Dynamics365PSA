using Common.Utilities.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Common.Archives.Utilities
{
    public class WinPhoneEncryptedDataAccess : EncryptedDataAccess
    {
        public override async Task<bool> Save(string key, string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            var encrypted = ProtectedData.Protect(bytes, null);
            var encryptedString = Convert.ToBase64String(encrypted);
            return await DeviceDataAccess.Current.WriteToLocal(ConvertKeyToFileName(key), encryptedString);
        }

        public override async Task<string> Load(string key)
        {
            var encryptedString = await DeviceDataAccess.Current.ReadFromLocal<string>(ConvertKeyToFileName(key));

            if (encryptedString == null)
                return null;

            var bytes = Convert.FromBase64String(encryptedString);
            var decrypted = ProtectedData.Unprotect(bytes, null);
            var decryptedString = Encoding.UTF8.GetString(decrypted, 0, decrypted.Length);
            return decryptedString;
        }

        public override async Task<bool> Delete(string key)
        {
            return await DeviceDataAccess.Current.DeleteFromLocal(ConvertKeyToFileName(key));
        }

        public override async Task<bool> DeleteAll()
        {
            return await DeviceDataAccess.Current.DeleteAllLocal();
        }

        private string ConvertKeyToFileName(string key)
        {
            return key + ".cer";
        }
    }
}
