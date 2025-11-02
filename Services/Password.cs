using mpc_dotnetc_user_server.Interfaces;
using System.Security.Cryptography;

namespace mpc_dotnetc_user_server.Services
{
    public class Password : IPassword
    {
        public Password()
        {
            
        }

        public byte[] Create_Password_Salted_Hash_Bytes(byte[] original_password, byte[] salt)
        {
            HashAlgorithm algorithm = SHA256.Create();

            byte[] salted_bytes = new byte[original_password.Length + salt.Length];

            for (int i = 0; i < original_password.Length; i++)
            {
                salted_bytes[i] = original_password[i];
            }

            for (int i = 0; i < salt.Length; i++)
            {
                salted_bytes[original_password.Length + i] = salt[i];
            }

            return algorithm.ComputeHash(salted_bytes);
        }

        public bool Compare_Password_Byte_Arrays(byte[] array_1, byte[] array_2)
        {
            if (array_1.Length != array_2.Length)
            {
                return false;
            }

            for (int i = 0; i < array_1.Length; i++)
            {
                if (array_1[i] != array_2[i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}