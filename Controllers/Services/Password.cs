using mpc_dotnetc_user_server.Controllers.Interfaces;
using System.Security.Cryptography;

namespace mpc_dotnetc_user_server.Controllers.Services
{
    public class Password : IPassword
    {
        public Password()
        {
            
        }

        public async Task<byte[]> Process_Password_Salted_Hash_Bytes(byte[] original_password, byte[] salt) 
        {
            await Task.Run(() => {
                return Create_Password_Salted_Hash_Bytes(original_password, salt).Result;
            });
            return [];
        }

        public async Task<bool> Process_Comparison_Between_Password_Salted_Hash_Bytes(byte[] array_containing_bytes_1, byte[] array_containing_bytes_2)
        {
            await Task.Run(() => {
                return Compare_Password_Byte_Arrays(array_containing_bytes_1, array_containing_bytes_2).Result;
            });
            return false;
        }

        private async Task<byte[]> Create_Password_Salted_Hash_Bytes(byte[] original_password, byte[] salt)
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

            return await Task.FromResult(algorithm.ComputeHash(salted_bytes));
        }

        private async Task<bool> Compare_Password_Byte_Arrays(byte[] array_1, byte[] array_2)
        {
            await Task.Run(() => {

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
            });
            return false;
        }
    }
}