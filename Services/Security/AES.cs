using System.Security.Cryptography;
using System.Text;
using mpc_dotnetc_user_server.Interfaces;

namespace mpc_dotnetc_user_server.Services.Security
{
    public class AES : IAES
    {
        public readonly byte[] key;
        public readonly byte[] iv;

        public AES()
        {
            key = SHA256.HashData(Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_KEY") ?? string.Empty));
            iv = Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("ENCRYPTION_IV") ?? string.Empty).Take(16).ToArray();
        }

        public AES(byte[] _key, byte[] _iv)
        {
            key = _key;
            iv = _iv;
        }

        public string Process_Decryption(string encryption_code)
        {
            return Decrypt(encryption_code);
        }

        public string Process_Encryption(string cipherBytes)
        {
            return Encrypt(cipherBytes);
        }

        private string Decrypt(string str)
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

        private string Encrypt(string str)
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