
using System.Security.Cryptography;
using System.Text;

namespace mpc_dotnetc_user_server.Controllers
{
    public class AES
    {
        private static readonly byte[] key = SHA256.HashData(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_KEY")));
        private static readonly byte[] iv = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_IV")).Take(16).ToArray();

        public static string Process_Decryption(string encryption_code)
        {
            return Decrypt(encryption_code);
        }

        public static string Process_Encryption(string cipherBytes)
        {
            return Encrypt(cipherBytes);
        }

        private static string Decrypt(string str)
        {
            byte[] string_bytes = Convert.FromBase64String(str);
            byte[] decrypted_bytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, aes.IV))
                {
                    decrypted_bytes = decryptor.TransformFinalBlock(string_bytes, 0, string_bytes.Length);
                }
            }

            return Encoding.UTF8.GetString(decrypted_bytes);
        }

        private static string Encrypt(string str)
        {
            byte[] string_bytes = Encoding.UTF8.GetBytes(str);
            byte[] encrypted_bytes;

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.IV = iv;
                aes.Mode = CipherMode.CBC;
                aes.Padding = PaddingMode.PKCS7;

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV))
                {
                    encrypted_bytes = encryptor.TransformFinalBlock(string_bytes, 0, string_bytes.Length);
                }
            }

            return Convert.ToBase64String(encrypted_bytes);
        }
    }
}
