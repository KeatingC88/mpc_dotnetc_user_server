using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users._Index;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Index;
using System.Text;
using mpc_dotnetc_user_server.Services.Security;
using mpc_dotnetc_user_server.Interfaces;

namespace mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository.Initialization
{
    public class SQLite_Database_Mock
    {
        private static long TimeStamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        private static Users_Database_Context UsersDBC;
        private static Constants Constants;

        // Use concrete classes instead of interfaces
        private readonly IAES AES;
        private readonly IPassword Password;

        public SQLite_Database_Mock() { }

        // ✔ Constructor now uses actual classes directly
        public SQLite_Database_Mock(
            Users_Database_Context usersDbContext,
            Constants constants,
            IAES aes,
            IPassword password
        )
        {
            UsersDBC = usersDbContext;
            Constants = constants;
            AES = aes;
            Password = password;
        }

        private static async Task<string> Generate_Mock_User_Public_ID()
        {
            string user_public_id;
            Random random = new();

            do
            {
                user_public_id = new string(Enumerable
                    .Repeat("0123456789", 5)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            } while (await UsersDBC.User_IDsTbl.AnyAsync(x => x.Public_ID == user_public_id));

            return user_public_id;
        }

        private static async Task<User_Secret_DTO> Generate_User_Mock_Secret_ID()
        {
            Random random = new();
            string user_secret_id;
            string user_secret_hash_id;
            string user_encrypted_secret;
            string character_set = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

            AES aes = new AES(); // local instance, same as before

            do
            {
                user_secret_id = $@"
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}- 
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}- 
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}- 
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}- 
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}- 
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}- 
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}
                ";

                user_secret_hash_id = SHA256_Generator.ComputeHash(user_secret_id);
                user_encrypted_secret = aes.Process_Encryption(user_secret_id);

            } while (await UsersDBC.User_IDsTbl.AnyAsync(x => x.Secret_Hash_ID == user_secret_hash_id));

            return new User_Secret_DTO
            {
                Encryption = user_encrypted_secret,
                Hash = user_secret_hash_id
            };
        }

        public async void Initialize()
        {
            UsersDBC.Database.EnsureCreated();

            string[] email_addresses =
            [
                "alpha@lol.com","beta@lol.com","charlie@lol.com","delta@lol.com","echo@lol.com",
                "foxtrot@lol.com","golf@lol.com","hotel@lol.com","india@lol.com","juliet@lol.com",
                "kilo@lol.com","lima@lol.com","november@lol.com","oscar@lol.com","papa@lol.com",
                "quebec@lol.com","romeo@lol.com","sierra@lol.com","tango@lol.com","uniform@lol.com",
                "victor@lol.com","whiskey@lol.com","xray@lol.com","yankee@lol.com","zulu@lol.com"
            ];

            // Mock Users
            if (!UsersDBC.User_IDsTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                {
                    User_Secret_DTO mock = await Generate_User_Mock_Secret_ID();

                    UsersDBC.User_IDsTbl.Add(new User_IDsTbl
                    {
                        ID = i,
                        Public_ID = Generate_Mock_User_Public_ID().Result,
                        Secret_ID = mock.Encryption,
                        Secret_Hash_ID = mock.Hash,
                        Updated_by = i,
                        Updated_on = TimeStamp(),
                        Created_by = i,
                        Deleted = false,
                        Deleted_by = 0,
                        Deleted_on = 0
                    });
                }

                UsersDBC.SaveChanges();
            }

            // Mock Logins
            if (!UsersDBC.Login_Email_AddressTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                {
                    UsersDBC.Login_Email_AddressTbl.Add(new Login_Email_AddressTbl
                    {
                        End_User_ID = i,
                        ID = i,
                        Email_Address = email_addresses[i - 1]
                    });
                }

                UsersDBC.SaveChanges();
            }

            // Mock Passwords
            if (!UsersDBC.Login_PasswordTbl.Any())
            {
                var passwordMap = new Dictionary<int, string>();

                for (int i = 1; i <= 25; i++)
                    passwordMap[i] = "password";

                for (int i = 1; i <= 25; i++)
                {
                    string userLogin = email_addresses[i - 1];

                    byte[] encryptedPassword = Password.Create_Password_Salted_Hash_Bytes(
                        Encoding.UTF8.GetBytes(passwordMap[i]),
                        Encoding.UTF8.GetBytes($"{userLogin}{Constants.JWT_SECURITY_KEY}")
                    );

                    UsersDBC.Login_PasswordTbl.Add(new Login_PasswordTbl
                    {
                        ID = i,
                        End_User_ID = i,
                        Password = encryptedPassword
                    });
                }

                UsersDBC.SaveChanges();
            }
        }
    }
}
