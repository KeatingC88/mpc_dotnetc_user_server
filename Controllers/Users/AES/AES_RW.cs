using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;

namespace mpc_dotnetc_user_server.Controllers.Users.AES
{
    public class AES_RW
    {
        private static readonly string secretKey = "z0nz0fb!gb0sz664";// The same secret key (must be 16 bytes for AES-128)
        private readonly byte[] key = Encoding.UTF8.GetBytes(secretKey);
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
            byte[] decrypted_bytes;
            byte[] string_bytes = Convert.FromBase64String(str);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;// Same mode as encryption
                aes.Padding = PaddingMode.PKCS7;// Same padding size

                using (ICryptoTransform decryptor = aes.CreateDecryptor(aes.Key, null))
                {
                    decrypted_bytes = decryptor.TransformFinalBlock(string_bytes, 0, string_bytes.Length);
                }
            }

            return Encoding.UTF8.GetString(decrypted_bytes);
        }

        private string Encrypt(string str)
        {
            byte[] encrypted_bytes;
            byte[] string_bytes = Encoding.UTF8.GetBytes(str);

            using (Aes aes = Aes.Create())
            {
                aes.Key = key;
                aes.Mode = CipherMode.ECB;  // Ensure ECB mode (no IV)
                aes.Padding = PaddingMode.PKCS7;  // Ensure the same padding

                using (ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, null))
                {
                    encrypted_bytes = encryptor.TransformFinalBlock(string_bytes, 0, string_bytes.Length);
                }
            }
            
            return Convert.ToBase64String(encrypted_bytes);
        }
    }
}
