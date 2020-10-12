using System;
using System.Configuration;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;

namespace listener_server.common
{
    class configUtility
    {
        private static readonly byte[] salt = Encoding.Unicode.GetBytes("uTpRaMN$FN_mnK0--i7Q&34*43wHgae8K");

        // Encrypt individual property
        public static void EncryptConfigItem(string key, string value, string cfgPath = null)
        {
            UpdateKey(key, EncryptString(value), cfgPath);
        }

        // Encrypt Archer credentials
        public static void EncryptConfig(string cfgPath = null)
        {
            //Values you want encrypted
            //EncryptConfigItem("archer_password", AppConfigHelper.ArcherPassword, cfgPath);
            UpdateKey("are_values_encrypted", "true", cfgPath);
        }

        // Encrypt the provided string
        private static string EncryptString(string dataToEncrypt)
        {
            byte[] encryptedData = ProtectedData.Protect(Encoding.Unicode.GetBytes(dataToEncrypt), salt, DataProtectionScope.LocalMachine);
            return string.Concat("SE4S4L7", Convert.ToBase64String(encryptedData));
        }

        // Decrypt the provided encrypted string
        public static string DecryptString(string encryptedData)
        {
            try
            {
                byte[] decryptedData = ProtectedData.Unprotect(Convert.FromBase64String(encryptedData.Substring(7)), salt, DataProtectionScope.LocalMachine);
                return Encoding.Unicode.GetString(decryptedData);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        // Update a key in the app.config file for the running application
        private static void UpdateKey(string keyName, string newValue, string cfgPath = null)
        {
            var configPath = cfgPath ?? Assembly.GetExecutingAssembly().CodeBase;

            Configuration config = ConfigurationManager.OpenExeConfiguration(configPath.Replace("file:///", string.Empty));
            config.AppSettings.Settings[keyName].Value = newValue;

            config.Save();
        }
    }
}