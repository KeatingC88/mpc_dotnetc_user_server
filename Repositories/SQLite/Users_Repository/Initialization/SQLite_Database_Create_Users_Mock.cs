using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Models.Users._Index;
using mpc_dotnetc_user_server.Models.Users.Account_Groups;
using mpc_dotnetc_user_server.Models.Users.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Account_Type;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Services.Security;
using System.Text;

namespace mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository.Initialization
{
    public class SQLite_Database_Create_Users_Mock
    {
        private static long TimeStamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        private static Users_Database_Context UsersDBC;
        private static Constants Constants;

        // Use concrete classes instead of interfaces
        private readonly IAES AES;
        private readonly IPassword Password;

        public SQLite_Database_Create_Users_Mock() { }

        // ✔ Constructor now uses actual classes directly
        public SQLite_Database_Create_Users_Mock(
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
                        Email_Address = email_addresses[i - 1].ToUpper()
                    });
                }

                UsersDBC.SaveChanges();
            }

            // Mock Passwords
            if (!UsersDBC.Login_PasswordTbl.Any())
            {
                var passwordMap = new Dictionary<int, string>();

                for (int i = 1; i <= 25; i++)
                    passwordMap[i] = "asdfASDF1234!@#$";

                for (int i = 1; i <= 25; i++)
                {
                    string userLogin = email_addresses[i - 1].ToUpper();

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

            // ==============================
            // FULL ACCOUNT STATE SEED LOGIC
            // ==============================

            if (!UsersDBC.Selected_NameTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Selected_NameTbl.Add(new Selected_NameTbl
                    {
                        End_User_ID = i,
                        Name = $"User{i}",
                        Created_on = TimeStamp(),
                        Updated_on = TimeStamp(),
                        Created_by = i,
                        Updated_by = i,
                        Deleted = false,
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Selected_LanguageTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Selected_LanguageTbl.Add(new Selected_LanguageTbl
                    {
                        End_User_ID = i,
                        Language_code = "en",
                        Region_code = "US",
                        Created_on = TimeStamp(),
                        Updated_on = TimeStamp(),
                        Created_by = i,
                        Updated_by = i,
                        Deleted = false,
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Selected_App_AlignmentTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Selected_App_AlignmentTbl.Add(new Selected_App_AlignmentTbl
                    {
                        End_User_ID = i,
                        Center = true,
                        Left = false,
                        Right = false,
                        Updated_by = 0,
                        Updated_on = TimeStamp(),
                        Deleted = false,
                        Created_by = 0,
                        Created_on = TimeStamp(),
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Selected_Navbar_LockTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Selected_Navbar_LockTbl.Add(new Selected_Navbar_LockTbl
                    {
                        End_User_ID = i,
                        Locked = false,
                        Updated_by = 0,
                        Updated_on = TimeStamp(),
                        Deleted = false,
                        Created_by = 0,
                        Created_on = TimeStamp(),
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Selected_App_Text_AlignmentTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Selected_App_Text_AlignmentTbl.Add(new Selected_App_Text_AlignmentTbl
                    {
                        End_User_ID = i,
                        Center = true,
                        Left = false,
                        Right = false,
                        Updated_by = 0,
                        Updated_on = TimeStamp(),
                        Deleted = false,
                        Created_by = 0,
                        Created_on = TimeStamp(),
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Selected_ThemeTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Selected_ThemeTbl.Add(new Selected_ThemeTbl
                    {
                        End_User_ID = i,
                        Night = true,
                        Light = false,
                        Updated_by = 0,
                        Updated_on = TimeStamp(),
                        Deleted = false,
                        Created_by = 0,
                        Created_on = TimeStamp(),
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Account_RolesTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Account_RolesTbl.Add(new Account_RolesTbl
                    {
                        End_User_ID = i,
                        Roles = "User",
                        Updated_by = 0,
                        Updated_on = TimeStamp(),
                        Deleted = false,
                        Created_by = 0,
                        Created_on = TimeStamp(),
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Account_GroupsTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Account_GroupsTbl.Add(new Account_GroupsTbl
                    {
                        End_User_ID = i,
                        Groups = "0",
                        Updated_by = 0,
                        Updated_on = TimeStamp(),
                        Deleted = false,
                        Created_by = 0,
                        Created_on = TimeStamp(),
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Account_TypeTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Account_TypeTbl.Add(new Account_TypeTbl
                    {
                        End_User_ID = i,
                        Type = 1,
                        Updated_by = 0,
                        Updated_on = TimeStamp(),
                        Deleted = false,
                        Created_by = 0,
                        Created_on = TimeStamp(),
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Selected_App_Grid_TypeTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Selected_App_Grid_TypeTbl.Add(new Selected_App_Grid_TypeTbl
                    {
                        End_User_ID = i,
                        Grid = 1,
                        Updated_by = 0,
                        Updated_on = TimeStamp(),
                        Deleted = false,
                        Created_by = 0,
                        Created_on = TimeStamp(),
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Selected_StatusTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Selected_StatusTbl.Add(new Selected_StatusTbl
                    {
                        End_User_ID = i,
                        Online = true,
                        Offline = false,
                        DND = false,
                        Custom_lbl = "",
                        Custom = false,
                        Hidden = false,
                        Away = false,
                        Updated_by = 0,
                        Updated_on = TimeStamp(),
                        Deleted = false,
                        Created_by = 0,
                        Created_on = TimeStamp(),
                        Deleted_by = 0,
                        Deleted_on = 0
                    });

                UsersDBC.SaveChanges();
            }
            if (!UsersDBC.Login_Time_StampTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Login_Time_StampTbl.Add(new Login_Time_StampTbl
                    {
                        End_User_ID = i,
                        Login_on = TimeStamp(),
                        Client_time = TimeStamp(),
                        Location = "Seed",

                        Remote_IP = "127.0.0.1",
                        Remote_Port = 0,
                        Server_IP = "127.0.0.1",
                        Server_Port = 0,
                        Client_IP = "127.0.0.1",
                        Client_Port = 0,

                        User_agent = "Seed",
                        Window_height = "0",
                        Window_width = "0",
                        Screen_height = "0",
                        Screen_width = "0",
                        RTT = "0",
                        Orientation = "0",
                        Data_saver = "false",
                        Color_depth = "0",
                        Pixel_depth = "0",
                        Connection_type = "Seed",
                        Down_link = "0",
                        Device_ram_gb = "0"
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Login_Time_Stamp_HistoryTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Login_Time_Stamp_HistoryTbl.Add(new Login_Time_Stamp_HistoryTbl
                    {
                        End_User_ID = i,
                        Login_on = TimeStamp(),
                        Client_time = TimeStamp(),
                        Location = "Seed",

                        Remote_IP = "127.0.0.1",
                        Remote_Port = 0,
                        Server_IP = "127.0.0.1",
                        Server_Port = 0,
                        Client_IP = "127.0.0.1",
                        Client_Port = 0,

                        User_agent = "Seed",
                        Window_height = "0",
                        Window_width = "0",
                        Screen_height = "0",
                        Screen_width = "0",
                        RTT = "0",
                        Orientation = "0",
                        Data_saver = "false",
                        Color_depth = "0",
                        Pixel_depth = "0",
                        Connection_type = "Seed",
                        Down_link = "0",
                        Device_ram_gb = "0"
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Logout_Time_StampTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Logout_Time_StampTbl.Add(new Logout_Time_StampTbl
                    {
                        End_User_ID = i,
                        Logout_on = TimeStamp(),
                        Client_time = TimeStamp(),
                        Location = "Seed",

                        Remote_IP = "127.0.0.1",
                        Remote_Port = 0,
                        Server_IP = "127.0.0.1",
                        Server_Port = 0,
                        Client_IP = "127.0.0.1",
                        Client_Port = 0,

                        User_agent = "Seed",
                        Window_height = "0",
                        Window_width = "0",
                        Screen_height = "0",
                        Screen_width = "0",
                        RTT = "0",
                        Orientation = "0",
                        Data_saver = "false",
                        Color_depth = "0",
                        Pixel_depth = "0",
                        Connection_type = "Seed",
                        Down_link = "0",
                        Device_ram_gb = "0"
                    });

                UsersDBC.SaveChanges();
            }

            if (!UsersDBC.Logout_Time_Stamp_HistoryTbl.Any())
            {
                for (int i = 1; i <= 25; i++)
                    UsersDBC.Logout_Time_Stamp_HistoryTbl.Add(new Logout_Time_Stamp_HistoryTbl
                    {
                        End_User_ID = i,
                        Logout_on = TimeStamp(),
                        Client_time = TimeStamp(),
                        Location = "Seed",

                        Remote_IP = "127.0.0.1",
                        Remote_Port = 0,
                        Server_IP = "127.0.0.1",
                        Server_Port = 0,
                        Client_IP = "127.0.0.1",
                        Client_Port = 0,

                        User_agent = "Seed",
                        Window_height = "0",
                        Window_width = "0",
                        Screen_height = "0",
                        Screen_width = "0",
                        RTT = "0",
                        Orientation = "0",
                        Data_saver = "false",
                        Color_depth = "0",
                        Pixel_depth = "0",
                        Connection_type = "Seed",
                        Down_link = "0",
                        Device_ram_gb = "0"
                    });

                UsersDBC.SaveChanges();
            }

        }
    }
}