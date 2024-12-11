using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Security.Cryptography;
using System.Text;

namespace mpc_dotnetc_user_server.Controllers.Users.AES
{
    public class AES_Decryptor
    {
        private readonly string SecretKey = "your-secret-key-1234567891011123"; // Use a secure key in production -- add to .env file

        public string Process(string encrypted_text)
        {
            return Decrypt(encrypted_text);
        }

        private string Decrypt(string encrypted_text)
        {
            try
            {
                // Decode the Base64 payload
                string jsonPayload = Encoding.UTF8.GetString(Convert.FromBase64String(encrypted_text));
                Console.WriteLine($"Decoded JSON Payload: {jsonPayload}");

                // Deserialize the JSON payload
                dynamic payload = Newtonsoft.Json.JsonConvert.DeserializeObject(jsonPayload);
                byte[] iv = Convert.FromBase64String(payload.iv.ToString());
                byte[] ciphertext = Convert.FromBase64String(payload.ciphertext.ToString());

                // Log the extracted IV and ciphertext
                Console.WriteLine($"Extracted IV: {BitConverter.ToString(iv)}");
                Console.WriteLine($"CipherBytes Length: {ciphertext.Length}");

                // Derive AES key
                byte[] keyBytes = SHA256.HashData(Encoding.UTF8.GetBytes("your-secret-key-1234567891011123")).Take(16).ToArray();
                Console.WriteLine($"Derived AES Key: {BitConverter.ToString(keyBytes)}");

                // Decrypt AES
                using (Aes aes = Aes.Create())
                {
                    aes.Key = keyBytes;
                    aes.IV = iv;
                    aes.Padding = PaddingMode.PKCS7;
                    aes.Mode = CipherMode.CBC;

                    using (MemoryStream memoryStream = new MemoryStream(ciphertext))
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, aes.CreateDecryptor(), CryptoStreamMode.Read))
                        {
                            using (StreamReader reader = new StreamReader(cryptoStream))
                            {
                                string result = reader.ReadToEnd();
                                Console.WriteLine($"Decrypted Text: {result}");
                                return result;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Decryption failed: {ex.Message}");
                throw;
            }
        }



    }

}
