using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users._Index;
using mpc_dotnetc_user_server.Models.Users.Account_Groups;
using mpc_dotnetc_user_server.Models.Users.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Account_Type;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Friends;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Report;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Deactivate;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Password_Change;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.WebSocket_Chat;
using System.Data;
using System.Text;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class Users_Repository : IUsers_Repository
    {
        private long TimeStamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        private readonly Users_Database_Context _UsersDBC;
        private readonly Constants _Constants;

        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly IPassword Password;

        public Users_Repository(
            Users_Database_Context Users_Database_Context,
            Constants constants,
            IAES aes,
            IJWT jwt,
            IPassword password
        ) {
            _UsersDBC = Users_Database_Context;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Password = password;
        }

        private async Task<string> Generate_User_Public_ID()
        {
            string user_public_id;
            Random random = new();

            do
            {
                user_public_id = new string(Enumerable
                    .Repeat("0123456789", 5)
                    .Select(s => s[random.Next(s.Length)])
                    .ToArray());

            } while (await _UsersDBC.User_IDsTbl.AnyAsync(x => x.Public_ID == user_public_id));

            return user_public_id;
        }
        private async Task<User_Secret_DTO> Generate_User_Secret_ID()
        {
            Random random = new();
            string user_secret_id;
            string user_secret_hash_id;
            string user_encrypted_secret;
            string character_set = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

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

                user_encrypted_secret = AES.Process_Encryption(user_secret_id);

            } while (await _UsersDBC.User_IDsTbl.AnyAsync(x => x.Secret_Hash_ID == user_secret_hash_id));

            return new User_Secret_DTO
            {
                Encryption = user_encrypted_secret,
                Hash = user_secret_hash_id
            };
        }
        public async Task<User_Data_DTO> Create_Account_By_Email(Complete_Email_Registration dto)
        {
            string user_public_id = await Generate_User_Public_ID();
            User_Secret_DTO user_secret = await Generate_User_Secret_ID();

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                Public_ID = user_public_id,
                Secret_ID = user_secret.Encryption,
                Secret_Hash_ID = user_secret.Hash,
                Created_by = 0,
                Created_on = TimeStamp(),
                Updated_on = TimeStamp(),
                Updated_by = 0
            };

            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, true)
                .SetProperty(col => col.Deleted_on, TimeStamp())
                .SetProperty(col => col.Updated_on, TimeStamp())
                .SetProperty(col => col.Updated_by, ID_Record.ID)
                .SetProperty(col => col.Deleted_by, ID_Record.ID)
                .SetProperty(col => col.Client_time, dto.Client_time)
                .SetProperty(col => col.Server_Port, dto.Server_Port)
                .SetProperty(col => col.Server_IP, dto.Server_IP)
                .SetProperty(col => col.Client_Port, dto.Client_Port)
                .SetProperty(col => col.Client_IP, dto.Client_IP)
                .SetProperty(col => col.Client_IP, dto.Remote_IP)
                .SetProperty(col => col.Client_Port, dto.Remote_Port)
                .SetProperty(col => col.User_agent, dto.User_agent)
                .SetProperty(col => col.Window_width, dto.Window_width)
                .SetProperty(col => col.Window_height, dto.Window_height)
                .SetProperty(col => col.Screen_width, dto.Screen_width)
                .SetProperty(col => col.Screen_height, dto.Screen_height)
                .SetProperty(col => col.RTT, dto.RTT)
                .SetProperty(col => col.Orientation, dto.Orientation)
                .SetProperty(col => col.Data_saver, dto.Data_saver)
                .SetProperty(col => col.Color_depth, dto.Color_depth)
                .SetProperty(col => col.Pixel_depth, dto.Pixel_depth)
                .SetProperty(col => col.Connection_type, dto.Connection_type)
                .SetProperty(col => col.Down_link, dto.Down_link)
                .SetProperty(col => col.Device_ram_gb, dto.Device_ram_gb)
            );

            await _UsersDBC.Completed_Email_RegistrationTbl.AddAsync(new Completed_Email_RegistrationTbl
            {
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Updated_by = ID_Record.ID,
                Deleted = true,
                Deleted_by = ID_Record.ID,
                Deleted_on = TimeStamp(),
                Language_Region = @$"{dto.Language}-{dto.Region}",
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Code = dto.Code,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                Client_time = dto.Client_time,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.Selected_NameTbl.AddAsync(new Selected_NameTbl
            {
                Name = $@"{dto.Name}",
                End_User_ID = ID_Record.ID,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 0
            });

            await _UsersDBC.Login_Email_AddressTbl.AddAsync(new Login_Email_AddressTbl
            {
                End_User_ID = ID_Record.ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_on = 0,
                Deleted_by = 0
            });

            await _UsersDBC.Login_PasswordTbl.AddAsync(new Login_PasswordTbl
            {
                End_User_ID = ID_Record.ID,
                Password = Password.Process_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}{_Constants.JWT_SECURITY_KEY}")).Result,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_on = 0,
                Deleted_by = 0
            });

            await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                End_User_ID = ID_Record.ID,
                Language_code = dto.Language,
                Region_code = dto.Region,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_on = 0,
                Deleted_by = 0
            });

            await Update_End_User_Selected_Alignment(new Selected_App_Alignment
            {
                End_User_ID = ID_Record.ID,
                Alignment = dto.Alignment
            });

            await Update_End_User_Selected_Nav_Lock(new Selected_Navbar_Lock
            {
                End_User_ID = ID_Record.ID,
                Locked = dto.Nav_lock,
            });

            await Update_End_User_Selected_Text_Alignment(new Selected_App_Text_Alignment
            {
                End_User_ID = ID_Record.ID,
                Text_alignment = dto.Text_alignment,
            });

            await Update_End_User_Selected_Theme(new Selected_Theme
            {
                End_User_ID = ID_Record.ID,
                Theme = dto.Theme
            });

            await Update_End_User_Account_Roles(new Account_Role
            {
                End_User_ID = ID_Record.ID,
                Roles = "User"
            });

            await Update_End_User_Account_Groups(new Account_Group
            {
                End_User_ID = ID_Record.ID,
                Groups = "0"
            });

            await Update_End_User_Account_Type(new Account_Types
            {
                End_User_ID = ID_Record.ID,
                Type = 1
            });

            await Update_End_User_Selected_Grid_Type(new Selected_App_Grid_Type
            {
                End_User_ID = ID_Record.ID,
                Grid = dto.Grid_type
            });

            await Update_End_User_Selected_Status(new Selected_Status
            {
                End_User_ID = ID_Record.ID,
                Status = 2,
            });
            
            string token = JWT.Create_Email_Account_Token(new JWT_DTO
            {
                End_User_ID = ID_Record.ID,
                User_groups = "0",
                User_roles = "User",
                Account_type = 1,
                Email_address = dto.Email_Address
            }).Result;

            await Update_End_User_Login_Time_Stamp(new Login_Time_Stamp
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_History
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_Port = dto.Remote_Port,
                Remote_IP = dto.Remote_IP,
                Server_Port = dto.Server_Port,
                Server_IP = dto.Server_IP,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.SaveChangesAsync();

            return new User_Data_DTO
            {
                created_on = TimeStamp(),
                login_on = TimeStamp(),
                location = dto.Location,
                login_type = "email",
                account_type = 1,
                grid_type = dto.Grid_type,
                online_status = 2,
                id = ID_Record.ID,
                name = $@"{dto.Name}#{user_public_id}",
                email_address = dto.Email_Address,
                language = dto.Language,
                region = dto.Region,
                alignment = dto.Alignment,
                nav_lock = dto.Nav_lock,
                text_alignment = dto.Text_alignment,
                theme = dto.Theme,
                roles = "User",
                groups = "0"
            };
        }
        public async Task<User_Data_DTO> Create_Account_By_Twitch(Complete_Twitch_Registeration dto)
        {
            string user_public_id = await Generate_User_Public_ID();
            User_Secret_DTO user_secret = await Generate_User_Secret_ID();

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                Public_ID = user_public_id,
                Secret_ID = user_secret.Encryption,
                Secret_Hash_ID = user_secret.Hash,
                Created_by = 0,
                Created_on = TimeStamp(),
                Updated_on = TimeStamp(),
                Updated_by = 0
            };

            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Twitch_IDsTbl.AddAsync(new Twitch_IDsTbl
            {
                End_User_ID = ID_Record.ID,
                Twitch_ID = dto.Twitch_ID,
                User_Name = dto.Twitch_User_Name,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 9
            });

            await _UsersDBC.Selected_NameTbl.AddAsync(new Selected_NameTbl
            {
                Name = $@"{dto.Twitch_Name}",
                End_User_ID = ID_Record.ID,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_on = 0,
                Deleted_by = 0
            });

            await _UsersDBC.Twitch_Email_AddressTbl.AddAsync(new Twitch_Email_AddressTbl
            {
                End_User_ID = ID_Record.ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 0
            });

            await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                End_User_ID = ID_Record.ID,
                Language_code = dto.Language,
                Region_code = dto.Region,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
                Deleted = false,
                Deleted_by = 0,
                Deleted_on = 0
            });

            await Update_End_User_Selected_Alignment(new Selected_App_Alignment
            {
                End_User_ID = ID_Record.ID,
                Alignment = dto.Alignment,
            });

            await Update_End_User_Selected_Nav_Lock(new Selected_Navbar_Lock
            {
                End_User_ID = ID_Record.ID,
                Locked = dto.Nav_lock,
            });

            await Update_End_User_Selected_Text_Alignment(new Selected_App_Text_Alignment
            {
                End_User_ID = ID_Record.ID,
                Text_alignment = dto.Text_alignment,
            });

            await Update_End_User_Selected_Theme(new Selected_Theme
            {
                End_User_ID = ID_Record.ID,
                Theme = dto.Theme
            });

            await Update_End_User_Account_Roles(new Account_Role
            {
                End_User_ID = ID_Record.ID,
                Roles = "User"
            });

            await Update_End_User_Account_Groups(new Account_Group
            {
                End_User_ID = ID_Record.ID,
                Groups = "0"
            });

            await Update_End_User_Account_Type(new Account_Types
            {
                End_User_ID = ID_Record.ID,
                Type = 1
            });

            await Update_End_User_Selected_Grid_Type(new Selected_App_Grid_Type
            {
                End_User_ID = ID_Record.ID,
                Grid = dto.Grid_type
            });

            await Update_End_User_Selected_Status(new Selected_Status
            {
                End_User_ID = ID_Record.ID,
                Status = 2,
            });

            await Update_End_User_Login_Time_Stamp(new Login_Time_Stamp
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_History
            {
                End_User_ID = ID_Record.ID,
                Login_on = TimeStamp(),
                Client_time = dto.Client_time,
                Location = dto.Location,
                Remote_Port = dto.Remote_Port,
                Remote_IP = dto.Remote_IP,
                Server_Port = dto.Server_Port,
                Server_IP = dto.Server_IP,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.SaveChangesAsync();

            return new User_Data_DTO
            {
                id = ID_Record.ID,
                name = $@"{dto.Name}#{user_public_id}",
                email_address = dto.Email_Address,
                language = dto.Language,
                region = dto.Region,
                alignment = dto.Alignment,
                nav_lock = dto.Nav_lock,
                text_alignment = dto.Text_alignment,
                theme = dto.Theme,
                roles = "User",
                groups = "0",
                account_type = 1,
                grid_type = dto.Grid_type,
                online_status = 2,
                created_on = TimeStamp(),
                login_on = TimeStamp(),
                location = dto.Location,
                login_type = "TWITCH",
            };
        }
        public async Task<string> Create_Pending_Email_Registration_Record(Pending_Email_Registration dto)
        {
            await _UsersDBC.Pending_Email_RegistrationTbl.AddAsync(new Pending_Email_RegistrationTbl
            {
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                Language_Region = $"{dto.Language}-{dto.Region}",
                Email_Address = dto.Email_Address,
                Location = dto.Location,
                Client_time = dto.Client_Time_Parsed,
                Code = dto.Code,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                email_address = dto.Email_Address,
                code = dto.Code,
                language = dto.Language,
                region = dto.Region,
                created_on = TimeStamp(),
            });
        }
        public async Task<bool> Create_Contact_Us_Record(Contact_Us dto)
        {
            await _UsersDBC.Contact_UsTbl.AddAsync(new Contact_UsTbl
            {
                End_User_ID = dto.End_User_ID,
                Subject_Line = dto.Subject_line,
                Summary = dto.Summary,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });

            await _UsersDBC.SaveChangesAsync();

            return true;
        }
        public async Task<bool> Create_End_User_Status_Record(Selected_Status dto)
        {
            await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
            {
                End_User_ID = dto.End_User_ID,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = dto.End_User_ID,
                Online = true,
                Updated_by = dto.End_User_ID
            });
            return true;
        }
        public async Task<bool> Create_Website_Bug_Record(Reported_Website_Bug dto)
        {
            await _UsersDBC.Reported_Website_BugTbl.AddAsync(new Reported_Website_BugTbl
            {
                End_User_ID = dto.End_User_ID,
                URL = dto.URL,
                Detail = dto.Detail,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<string> Create_WebSocket_Log_Record(WebSocket_Chat_Permission dto)
        {
            try
            {
                await _UsersDBC.WebSocket_Chat_PermissionTbl.AddAsync(new WebSocket_Chat_PermissionTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Participant_ID = dto.Participant_ID,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Requested = true,
                    Approved = false,
                    Blocked = false
                });

                await _UsersDBC.SaveChangesAsync();

                return await Task.FromResult(JsonSerializer.Serialize(new{
                    updated_on = TimeStamp(),
                    updated_by = dto.End_User_ID,
                    updated_for = dto.User
                }));
            } catch {
                return "Server Error: WebSocket Log Record"; 
            }
        }
        public async Task<bool> Create_Discord_Bot_Bug_Record(Reported_Discord_Bot_Bug dto)
        {
            await _UsersDBC.Reported_Discord_Bot_BugTbl.AddAsync(new Reported_Discord_Bot_BugTbl
            {
                End_User_ID = dto.End_User_ID,
                Location = dto.Location,
                Detail = dto.Detail,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Comment_Box_Record(Comment_Box dto)
        {
            await _UsersDBC.Comment_BoxTbl.AddAsync(new Comment_BoxTbl
            {
                End_User_ID = dto.End_User_ID,
                Comment = dto.Comment,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Broken_Link_Record(Reported_Broken_Link dto)
        {
            await _UsersDBC.Reported_Broken_LinkTbl.AddAsync(new Reported_Broken_LinkTbl
            {
                End_User_ID = dto.End_User_ID,
                URL = dto.URL,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<string> Create_Reported_User_Profile_Record(Reported_Profile dto)
        {
            Reported_ProfileTbl record = new Reported_ProfileTbl
            {
                End_User_ID = dto.End_User_ID,
                Reported_ID = dto.Reported_ID,
                Page_Title = _UsersDBC.Profile_PageTbl.Where(x => x.End_User_ID == dto.Reported_ID).Select(x => x.Page_Title).SingleOrDefault(),
                Page_Description = _UsersDBC.Profile_PageTbl.Where(x => x.End_User_ID == dto.Reported_ID).Select(x => x.Page_Description).SingleOrDefault(),
                About_Me = _UsersDBC.Profile_PageTbl.Where(x => x.End_User_ID == dto.Reported_ID).Select(x => x.About_Me).SingleOrDefault(),
                Banner_URL = _UsersDBC.Profile_PageTbl.Where(x => x.End_User_ID == dto.Reported_ID).Select(x => x.Banner_URL).SingleOrDefault(),
                Reported_Reason = dto.Reported_reason,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Updated_by = dto.End_User_ID
            };
            await _UsersDBC.Reported_ProfileTbl.AddAsync(record);
            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                report_record_id = record.ID,
                reported_user_id = record.Reported_ID,
                created_on = record.Created_on,
                read_reported_user = Read_User_Data_By_ID(dto.Reported_ID).ToString(),
                read_reported_profile = Read_User_Profile_By_ID(dto.Reported_ID).ToString(),
            });
        }
        public async Task<bool> Confirmation_Code_Exists_In_Pending_Email_Address_Registration(string Code)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(x => x.Code == Code));
        }
        public async Task<string> Delete_Account_By_User_id(Delete_User dto)
        {
            await _UsersDBC.User_IDsTbl.Where(x => x.ID == long.Parse(dto.Target_User)).ExecuteUpdateAsync(s => s
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted, true)
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_by, long.Parse(dto.ID))
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_on, TimeStamp())
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_on, TimeStamp())
                .SetProperty(User_IDsTbl => User_IDsTbl.Created_on, TimeStamp())
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_by, long.Parse(dto.ID))
            );
            await _UsersDBC.SaveChangesAsync();
            return JsonSerializer.Serialize(new
            {
                deleted_by = dto.ID,
                target_user = dto.Target_User,
            });
        }
        public async Task<string> Delete_From_Web_Socket_Chat_Permissions(WebSocket_Chat_Permission dto)
        {
            try
            {
                await _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(dto => dto.Requested, dto.Requested)
                    .SetProperty(dto => dto.Blocked, dto.Blocked)
                    .SetProperty(dto => dto.Approved, dto.Approved)
                    .SetProperty(dto => dto.Deleted_on, TimeStamp())
                    .SetProperty(dto => dto.Deleted_by, dto.Participant_ID)
                    .SetProperty(dto => dto.Deleted, true)
                    .SetProperty(dto => dto.Updated_on, TimeStamp())
                    .SetProperty(dto => dto.Updated_by, dto.Participant_ID)
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    participant_id = dto.Participant_ID,
                    deleted_on = TimeStamp()
                });
            }
            catch
            {
                return "Server Error: Delete Chat Permissions Failed.";
            }
        }
        public async Task<bool> Email_Exists_In_Login_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Any(x => x.Email_Address == email_address.ToUpper()));
        }
        public async Task<bool> Email_Exists_In_Twitch_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Twitch_Email_AddressTbl.Any(x => x.Email_Address == email_address.ToUpper()));
        }
        public async Task<bool> Email_Exists_In_Discord_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Discord_Email_AddressTbl.Any(x => x.Email_Address == email_address.ToUpper()));
        }
        public async Task<User_Data_DTO> Integrate_Account_By_Email(Complete_Email_Registration dto)
        {
            await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, true)
                .SetProperty(col => col.Deleted_on, TimeStamp())
                .SetProperty(col => col.Updated_on, TimeStamp())
                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                .SetProperty(col => col.Deleted_by, dto.End_User_ID)
                .SetProperty(col => col.Client_time, dto.Client_time)
                .SetProperty(col => col.Server_Port, dto.Server_Port)
                .SetProperty(col => col.Server_IP, dto.Server_IP)
                .SetProperty(col => col.Client_Port, dto.Client_Port)
                .SetProperty(col => col.Client_IP, dto.Client_IP)
                .SetProperty(col => col.Client_IP, dto.Remote_IP)
                .SetProperty(col => col.Client_Port, dto.Remote_Port)
                .SetProperty(col => col.User_agent, dto.User_agent)
                .SetProperty(col => col.Window_width, dto.Window_width)
                .SetProperty(col => col.Window_height, dto.Window_height)
                .SetProperty(col => col.Screen_width, dto.Screen_width)
                .SetProperty(col => col.Screen_height, dto.Screen_height)
                .SetProperty(col => col.RTT, dto.RTT)
                .SetProperty(col => col.Orientation, dto.Orientation)
                .SetProperty(col => col.Data_saver, dto.Data_saver)
                .SetProperty(col => col.Color_depth, dto.Color_depth)
                .SetProperty(col => col.Pixel_depth, dto.Pixel_depth)
                .SetProperty(col => col.Connection_type, dto.Connection_type)
                .SetProperty(col => col.Down_link, dto.Down_link)
                .SetProperty(col => col.Device_ram_gb, dto.Device_ram_gb)
            );

            await _UsersDBC.Completed_Email_RegistrationTbl.AddAsync(new Completed_Email_RegistrationTbl
            {
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Updated_by = 0,
                Language_Region = @$"{dto.Language}-{dto.Region}",
                Created_on = TimeStamp(),
                Created_by = dto.End_User_ID,
                Code = dto.Code,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                Client_time = dto.Client_time,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.Login_Email_AddressTbl.AddAsync(new Login_Email_AddressTbl
            {
                End_User_ID = dto.End_User_ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = dto.End_User_ID,
                Updated_by = dto.End_User_ID
            });

            await _UsersDBC.Login_PasswordTbl.AddAsync(new Login_PasswordTbl
            {
                End_User_ID = dto.End_User_ID,
                Password = Password.Process_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}{_Constants.JWT_SECURITY_KEY}")).Result,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = dto.End_User_ID,
                Updated_by = dto.End_User_ID,
            });

            await _UsersDBC.SaveChangesAsync();

            return new User_Data_DTO
            {
                created_on = TimeStamp(),
                login_on = TimeStamp(),
                location = dto.Location,
                account_type = 1,
                grid_type = dto.Grid_type,
                online_status = 2,
                id = dto.End_User_ID,
                name = $@"{dto.Name}",
                email_address = dto.Email_Address,
                language = dto.Language,
                region = dto.Region,
                alignment = dto.Alignment,
                nav_lock = dto.Nav_lock,
                text_alignment = dto.Text_alignment,
                theme = dto.Theme,
                roles = "User",
                groups = "0"
            };
        }
        public async Task Integrate_Account_By_Twitch(Complete_Twitch_Integration dto)
        {
            await _UsersDBC.Twitch_IDsTbl.AddAsync(new Twitch_IDsTbl
            {
                End_User_ID = dto.End_User_ID,
                Twitch_ID = dto.Twitch_ID,
                User_Name = dto.Twitch_User_Name,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = dto.End_User_ID,
                Updated_by = dto.End_User_ID
            });

            await _UsersDBC.Twitch_Email_AddressTbl.AddAsync(new Twitch_Email_AddressTbl
            {
                End_User_ID = dto.End_User_ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = dto.End_User_ID,
                Updated_by = dto.End_User_ID
            });
            await _UsersDBC.SaveChangesAsync();
        }
        public async Task<string> Create_Reported_Record(Reported dto)
        {
            try
            {
                switch (dto.Report_type.ToUpper())
                {
                    case "ABUSE":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Abuse = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_abuse_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_abuse_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Abuse = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_abuse_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_abuse_record_exists_in_database.Updated_on = TimeStamp();
                            reported_abuse_record_exists_in_database.Abuse = reported_abuse_record_exists_in_database.Abuse + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_abuse_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            abuse_record_created_on = TimeStamp(),
                        });

                    case "DISRUPTION":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Disruption = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_disruption_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_disruption_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Disruption = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_disruption_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_disruption_record_exists_in_database.Updated_on = TimeStamp();
                            reported_disruption_record_exists_in_database.Disruption = reported_disruption_record_exists_in_database.Disruption + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_disruption_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            disruption_record_created_on = TimeStamp(),
                        });

                    case "SELF_HARM":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Self_harm = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_self_harm_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_self_harm_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Self_harm = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_self_harm_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_self_harm_record_exists_in_database.Updated_on = TimeStamp();
                            reported_self_harm_record_exists_in_database.Self_harm = reported_self_harm_record_exists_in_database.Self_harm + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_self_harm_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            self_harm_record_created_on = TimeStamp(),
                        });

                    case "SPAM":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Spam = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_spam_harm_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_spam_harm_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Spam = 1
                            };

                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);

                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }

                        } else {

                            reported_spam_harm_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_spam_harm_record_exists_in_database.Updated_on = TimeStamp();
                            reported_spam_harm_record_exists_in_database.Self_harm = reported_spam_harm_record_exists_in_database.Self_harm + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_spam_harm_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            spam_record_created_on = TimeStamp(),
                        });

                    case "ILLEGAL":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Illegal = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_illegal_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_illegal_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Illegal = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_illegal_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_illegal_record_exists_in_database.Updated_on = TimeStamp();
                            reported_illegal_record_exists_in_database.Illegal = reported_illegal_record_exists_in_database.Illegal + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_illegal_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            illegal_record_created_on = TimeStamp(),
                        });

                    case "HARASS":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Harass = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_harass_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_harass_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Harass = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_harass_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_harass_record_exists_in_database.Updated_on = TimeStamp();
                            reported_harass_record_exists_in_database.Harass = reported_harass_record_exists_in_database.Harass + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_harass_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            harass_record_created_on = TimeStamp(),
                        });

                    case "MISINFORM":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Misinform = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_misinform_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_misinform_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Misinform = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_misinform_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_misinform_record_exists_in_database.Updated_on = TimeStamp();
                            reported_misinform_record_exists_in_database.Misinform = reported_misinform_record_exists_in_database.Misinform + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_misinform_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            misinform_record_created_on = TimeStamp(),
                        });

                    case "NUDITY":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Nudity = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_nudity_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_nudity_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Nudity = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_nudity_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_nudity_record_exists_in_database.Updated_on = TimeStamp();
                            reported_nudity_record_exists_in_database.Nudity = reported_nudity_record_exists_in_database.Nudity + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_nudity_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            nudity_record_created_on = TimeStamp(),
                        });

                    case "FAKE":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Fake = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_fake_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_fake_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Fake = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_fake_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_fake_record_exists_in_database.Updated_on = TimeStamp();
                            reported_fake_record_exists_in_database.Fake = reported_fake_record_exists_in_database.Fake + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_fake_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            fake_record_created_on = TimeStamp(),
                        });

                    case "HATE_SPEECH":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Hate = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_hate_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_hate_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Hate = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_hate_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_hate_record_exists_in_database.Updated_on = TimeStamp();
                            reported_hate_record_exists_in_database.Hate = reported_hate_record_exists_in_database.Hate + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_hate_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            hate_record_created_on = TimeStamp(),
                        });

                    case "VIOLENCE":
                        await _UsersDBC.Reported_HistoryTbl.AddAsync(new Reported_HistoryTbl
                        {
                            End_User_ID = dto.End_User_ID,
                            Participant_ID = dto.Participant_ID,
                            Updated_on = TimeStamp(),
                            Updated_by = dto.End_User_ID,
                            Violence = 1,
                            Created_on = TimeStamp(),
                            Created_by = dto.End_User_ID,
                        });

                        var reported_violence_record_exists_in_database = await _UsersDBC.ReportedTbl
                            .Where(x => x.End_User_ID == dto.Participant_ID)
                            .FirstOrDefaultAsync();

                        if (reported_violence_record_exists_in_database == null)
                        {
                            ReportedTbl ReportedTbl_Record = new ReportedTbl
                            {
                                End_User_ID = dto.Participant_ID,
                                Updated_on = TimeStamp(),
                                Created_on = TimeStamp(),
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID,
                                Violence = 1
                            };
                            await _UsersDBC.ReportedTbl.AddAsync(ReportedTbl_Record);
                            await _UsersDBC.SaveChangesAsync();

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = ReportedTbl_Record.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }
                        else
                        {
                            reported_violence_record_exists_in_database.Updated_by = dto.End_User_ID;
                            reported_violence_record_exists_in_database.Updated_on = TimeStamp();
                            reported_violence_record_exists_in_database.Violence = reported_violence_record_exists_in_database.Violence + 1;

                            if (!string.IsNullOrWhiteSpace(dto.Report_reason))
                            {
                                await _UsersDBC.Reported_ReasonTbl.AddAsync(new Reported_ReasonTbl
                                {
                                    Reported_ID = reported_violence_record_exists_in_database.ID,
                                    Reason = dto.Report_reason
                                });
                            }
                        }

                        await _UsersDBC.SaveChangesAsync();

                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            violence_record_created_on = TimeStamp(),
                        });

                    default:
                        return "Server Error: Report Record Selection Failed.";
                }
            }
            catch
            {
                return "Server Error: Report Record Creation Failed.";
            }
        }
        public async Task<bool> Email_Exists_In_Pending_Email_Registration(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(x => x.Email_Address.ToUpper() == email_address));
        }
        public Task<bool> ID_Exists_In_Users_ID(long user_id)
        {
            return Task.FromResult(_UsersDBC.User_IDsTbl.Any(x => x.ID == user_id && x.Deleted == false));
        }
        public Task<bool> ID_Exists_In_Twitch_IDs(long user_id)
        {
            return Task.FromResult(_UsersDBC.Twitch_IDsTbl.Any(x => x.Twitch_ID == user_id && x.Deleted == false));
        }
        public Task<bool> ID_Exists_In_Discord_IDs(long user_id)
        {
            return Task.FromResult(_UsersDBC.Discord_IDsTbl.Any(x => x.Discord_ID == user_id && x.Deleted == false));
        }
        public async Task<string> Insert_End_User_Login_Time_Stamp_History(Login_Time_Stamp_History dto)
        {
            try
            {
                await _UsersDBC.Login_Time_Stamp_HistoryTbl.AddAsync(new Login_Time_Stamp_HistoryTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Deleted = false,
                    Deleted_by = 0,
                    Deleted_on = 0,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID,
                    Created_by = dto.End_User_ID,
                    Login_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth
                });

                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    login_on = TimeStamp()
                }));
            } catch {
                return Task.FromResult(JsonSerializer.Serialize("Login TS History Failed.")).Result;
            }
        }
        public async Task<string> Insert_Report_Email_Registration(Report_Email_Registration dto)
        {
            try
            {
                await _UsersDBC.Report_Email_RegistrationTbl.AddAsync(new Report_Email_RegistrationTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Created_by = 0,
                    Email_Address = dto.Email_Address,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    User_agent = dto.User_agent,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Reported Email Registration Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Pending_Email_Registration_History(Report_Failed_Pending_Email_Registration_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Pending_Email_Registration_HistoryTbl.AddAsync(new Report_Failed_Pending_Email_Registration_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Created_by = 0,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    Language_Region = dto.Language_Region,
                    Email_Address = dto.Email_Address,
                    Reason = dto.Reason,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.Email_Address,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_User_Agent_History(Report_Failed_User_Agent_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_User_Agent_HistoryTbl.AddAsync(new Report_Failed_User_Agent_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    End_User_ID = dto.End_User_ID,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Reason = dto.Reason,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Login_type = dto.Login_type,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report User Agent Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Selected_History(Report_Failed_Selected_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Selected_HistoryTbl.AddAsync(new Report_Failed_Selected_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    End_User_ID = dto.End_User_ID,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Token = dto.Token
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Selected History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Logout_History(Report_Failed_Logout_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Logout_HistoryTbl.AddAsync(new Report_Failed_Logout_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    End_User_ID = dto.End_User_ID,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Token = dto.Token
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_JWT_History_Record(Report_Failed_JWT_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_JWT_HistoryTbl.AddAsync(new Report_Failed_JWT_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    JWT_id = dto.JWT_id,
                    Client_id = dto.Client_id,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    End_User_ID = dto.End_User_ID,
                    Login_type = dto.Login_type,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Token = dto.Token
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Client_ID_History_Record(Report_Failed_Client_ID_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Client_ID_HistoryTbl.AddAsync(new Report_Failed_Client_ID_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Language_Region = dto.Language_Region,
                    Reason = dto.Reason,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    End_User_ID = dto.End_User_ID,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Token = dto.Token
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Unregistered_Email_Login_History_Record(Report_Failed_Unregistered_Email_Login_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Unregistered_Email_Login_HistoryTbl.AddAsync(new Report_Failed_Unregistered_Email_Login_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Created_by = 0,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Language_Region = dto.Language_Region,
                    Email_Address = dto.Email_Address,
                    Reason = dto.Reason,
                    User_agent = dto.User_agent,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb
                });
                await _UsersDBC.SaveChangesAsync();
                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.Email_Address,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Unregistered Email Login History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Email_Login_History_Record(Report_Failed_Email_Login_History dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Email_Login_HistoryTbl.AddAsync(new Report_Failed_Email_Login_HistoryTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = 0,
                    Created_by = 0,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    Reason = dto.Reason,
                    Language_Region = dto.Language_Region,
                    Email_Address = dto.Email_Address,
                    User_agent = dto.User_agent,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb

                });

                await _UsersDBC.SaveChangesAsync();

                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            }
            catch
            {
                return "Server Error: Report Email Login History Failed.";
            }
        }
        public async Task<string> Insert_End_User_Logout_History_Record(Logout_Time_Stamp dto)
        {
            await _UsersDBC.Logout_Time_Stamp_HistoryTbl.AddAsync(new Logout_Time_Stamp_HistoryTbl
            {
                End_User_ID = dto.End_User_ID,
                Logout_on = TimeStamp(),
                Updated_by = dto.End_User_ID,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP,
                Server_Port = dto.Server_Port,
                Client_IP = dto.Client_IP,
                Client_Port = dto.Client_Port,
                Client_time = dto.Client_Time_Parsed,
                User_agent = dto.User_agent,
                Window_height = dto.Window_height,
                Window_width = dto.Window_width,
                Screen_height = dto.Screen_height,
                Screen_width = dto.Screen_width,
                RTT = dto.RTT,
                Orientation = dto.Orientation,
                Data_saver = dto.Data_saver,
                Color_depth = dto.Color_depth,
                Pixel_depth = dto.Pixel_depth,
                Connection_type = dto.Connection_type,
                Down_link = dto.Down_link,
                Device_ram_gb = dto.Device_ram_gb
            });

            await _UsersDBC.SaveChangesAsync();

            return Task.FromResult(JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                logout_on = TimeStamp()
            })).Result;
        }
        public async Task<string> Insert_Pending_Email_Registration_History_Record(Pending_Email_Registration_History dto)
        {
            try
            {
                await _UsersDBC.Pending_Email_Registration_HistoryTbl.AddAsync(new Pending_Email_Registration_HistoryTbl
                {
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Language_Region = dto.Language_Region,
                    Email_Address = dto.Email_Address,
                    Location = dto.Location,
                    Client_time = dto.Client_time,
                    Code = dto.Code,
                    User_agent = dto.User_agent,
                    Down_link = dto.Down_link,
                    Connection_type = dto.Connection_type,
                    RTT = dto.RTT,
                    Data_saver = dto.Data_saver,
                    Device_ram_gb = dto.Device_ram_gb,
                    Orientation = dto.Orientation,
                    Screen_width = dto.Screen_width,
                    Screen_height = dto.Screen_height,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth
                });
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    email_address = dto.Email_Address,
                    language = dto.Language_Region.Split("-")[0],
                    region = dto.Language_Region.Split("-")[1],
                    updated_on = TimeStamp(),
                });
            }
            catch
            {
                return "Server Error: Email Address Registration Failed";
            }
        }
        public async Task<long> Read_User_ID_By_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Where(x => x.Email_Address == email_address).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<string?> Read_User_Email_By_ID(long id)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Where(x => x.End_User_ID == id).Select(x => x.Email_Address).SingleOrDefault());
        }
        public async Task<long> Read_User_ID_By_Twitch_Account_ID(long twitch_id)
        {
            return await Task.FromResult(_UsersDBC.Twitch_IDsTbl.Where(x => x.Twitch_ID == twitch_id).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<long> Read_User_ID_By_Twitch_Account_Email(string twitch_email)
        {
            return await Task.FromResult(_UsersDBC.Twitch_Email_AddressTbl.Where(x => x.Email_Address == twitch_email.ToUpper()).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<long> Read_User_ID_By_Discord_Account_ID(long discord_id)
        {
            return await Task.FromResult(_UsersDBC.Discord_IDsTbl.Where(x => x.Discord_ID == discord_id).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<long> Read_User_ID_By_Discord_Account_Email(string discord_email)
        {
            return await Task.FromResult(_UsersDBC.Discord_Email_AddressTbl.Where(x => x.Email_Address == discord_email).Select(x => x.End_User_ID).SingleOrDefault());
        }
        public async Task<User_Data_DTO> Read_User_Data_By_ID(long end_user_id)
        {
            var user = (from end_user in _UsersDBC.User_IDsTbl

                join account_type_table in _UsersDBC.Account_TypeTbl
                    on end_user.ID equals account_type_table.End_User_ID into resulting_account_type_table
                from account_type_table in resulting_account_type_table.DefaultIfEmpty()

                join login_email_table in _UsersDBC.Login_Email_AddressTbl
                    on end_user.ID equals login_email_table.End_User_ID into resulting_email_table
                from login_email_table in resulting_email_table.DefaultIfEmpty()

                join selected_language_table in _UsersDBC.Selected_LanguageTbl
                    on end_user.ID equals selected_language_table.End_User_ID into resulting_selected_language_table
                from selected_language_table in resulting_selected_language_table.DefaultIfEmpty()

                join selected_name_table in _UsersDBC.Selected_NameTbl
                    on end_user.ID equals selected_name_table.End_User_ID into resulting_name_table
                from selected_name_table in resulting_name_table.DefaultIfEmpty()

                join selected_custom_design_table in _UsersDBC.Selected_App_Custom_DesignTbl
                    on end_user.ID equals selected_custom_design_table.End_User_ID into resulting_selected_custom_design_table
                from selected_custom_design_table in resulting_selected_custom_design_table.DefaultIfEmpty()

                join account_groups_table in _UsersDBC.Account_GroupsTbl
                    on end_user.ID equals account_groups_table.End_User_ID into resulting_account_groups_table
                from account_groups_table in resulting_account_groups_table.DefaultIfEmpty()

                join account_roles_table in _UsersDBC.Account_RolesTbl
                    on end_user.ID equals account_roles_table.End_User_ID into resulting_account_roles_table
                from account_roles_table in resulting_account_roles_table.DefaultIfEmpty()

                join selected_grid_table in _UsersDBC.Selected_App_Grid_TypeTbl
                    on end_user.ID equals selected_grid_table.End_User_ID into resulting_selected_grid_table
                from selected_grid_table in resulting_selected_grid_table.DefaultIfEmpty()

                join selected_navbar_lock_table in _UsersDBC.Selected_Navbar_LockTbl
                    on end_user.ID equals selected_navbar_lock_table.End_User_ID into resulting_selected_navbar_lock_table
                from selected_navbar_lock_table in resulting_selected_navbar_lock_table.DefaultIfEmpty()

                join status_table in _UsersDBC.Selected_StatusTbl
                    on end_user.ID equals status_table.End_User_ID into resulting_status_table
                from status_table in resulting_status_table.DefaultIfEmpty()

                join avatar_table in _UsersDBC.Selected_AvatarTbl
                    on end_user.ID equals avatar_table.End_User_ID into resulting_avatar_table
                from avatar_table in resulting_avatar_table.DefaultIfEmpty()

                join theme_table in _UsersDBC.Selected_ThemeTbl
                    on end_user.ID equals theme_table.End_User_ID into resulting_theme_table
                from theme_table in resulting_theme_table.DefaultIfEmpty()

                join alignment_table in _UsersDBC.Selected_App_AlignmentTbl
                    on end_user.ID equals alignment_table.End_User_ID into resulting_alignment_table
                from alignment_table in resulting_alignment_table.DefaultIfEmpty()

                join text_alignment_table in _UsersDBC.Selected_App_Text_AlignmentTbl
                    on end_user.ID equals text_alignment_table.End_User_ID into resulting_text_alignment_table
                from text_alignment_table in resulting_text_alignment_table.DefaultIfEmpty()

                join identity_table in _UsersDBC.IdentityTbl
                    on end_user.ID equals identity_table.End_User_ID into resulting_identity_table
                from identity_table in resulting_identity_table.DefaultIfEmpty()

                join birth_date_table in _UsersDBC.Birth_DateTbl
                    on end_user.ID equals birth_date_table.End_User_ID into resulting_birth_date_table
                from birth_date_table in resulting_birth_date_table.DefaultIfEmpty()

                join login_table in _UsersDBC.Login_Time_StampTbl
                    on end_user.ID equals login_table.End_User_ID into resulting_login_table
                from login_table in resulting_login_table.DefaultIfEmpty()

                join twitch_id_table in _UsersDBC.Twitch_IDsTbl
                    on end_user.ID equals twitch_id_table.End_User_ID into resulting_twitch_table
                from twitch_id_table in resulting_twitch_table.DefaultIfEmpty()

                join twitch_email_address_table in _UsersDBC.Twitch_Email_AddressTbl
                    on end_user.ID equals twitch_email_address_table.End_User_ID into resulting_twitch_email_address_table
                from twitch_email_address_table in resulting_twitch_email_address_table.DefaultIfEmpty()

                join logout_table in _UsersDBC.Logout_Time_StampTbl
                    on end_user.ID equals logout_table.End_User_ID into resulting_logout_table
                from logout_table in resulting_logout_table.DefaultIfEmpty()

                where end_user.ID == end_user_id

                select new
                {
                    end_user_id = end_user.ID,
                    created_on = end_user.Created_on,
                    account_type = account_type_table.Type,
                    email_address = login_email_table.Email_Address,
                    name = selected_name_table.Name,
                    public_id = end_user.Public_ID,
                    current_language = $@"{selected_language_table.Language_code}-{selected_language_table.Region_code}",
                    language = selected_language_table.Language_code,
                    region = selected_language_table.Region_code,
                    groups = account_groups_table.Groups,
                    roles = account_roles_table.Roles,
                    grid_type = selected_grid_table.Grid,
                    nav_lock = selected_navbar_lock_table.Locked,
                    online_status = status_table.Online,
                    offline_status = status_table.Offline,
                    hidden_status = status_table.Hidden,
                    away_status = status_table.Away,
                    dnd_status = status_table.DND,
                    custom_status = status_table.Custom,
                    custom_lbl = status_table.Custom_lbl,
                    avatar_url_path = avatar_table.Avatar_url_path,
                    avatar_title = avatar_table.Avatar_title,
                    light_theme = theme_table.Light,
                    night_theme = theme_table.Night,
                    custom_theme = theme_table.Custom,
                    left_alignment = alignment_table.Left,
                    center_alignment = alignment_table.Center,
                    right_alignment = alignment_table.Right,
                    left_text_alignment = text_alignment_table.Left,
                    center_text_alignment = text_alignment_table.Center,
                    right_text_alignment = text_alignment_table.Right,
                    login_on = login_table.Login_on,
                    logout_on = logout_table != null ? logout_table.Logout_on : (long?)null,
                    gender = identity_table.Gender,
                    birth_day = birth_date_table.Day,
                    birth_month = birth_date_table.Month,
                    birth_year = birth_date_table.Year,
                    first_name = identity_table.First_Name,
                    last_name = identity_table.Last_Name,
                    middle_name = identity_table.Middle_Name,
                    maiden_name = identity_table.Maiden_Name,
                    ethnicity = identity_table.Ethnicity,
                    twitch_user_id = twitch_id_table != null ? twitch_id_table.Twitch_ID : (long?)null,
                    twitch_user_name = twitch_id_table != null ? twitch_id_table.User_Name : null,
                    twitch_email_address = twitch_email_address_table != null ? twitch_email_address_table.Email_Address : null,
                    card_border_color = selected_custom_design_table.Card_Border_Color,
                    card_header_font = selected_custom_design_table.Card_Header_Font,
                    card_header_font_color = selected_custom_design_table.Card_Header_Font_Color,
                    card_header_background_color = selected_custom_design_table.Card_Header_Background_Color,
                    card_body_font = selected_custom_design_table.Card_Body_Font,
                    card_body_background_color = selected_custom_design_table.Card_Body_Background_Color,
                    card_body_font_color = selected_custom_design_table.Card_Body_Font_Color,
                    card_footer_font_color = selected_custom_design_table.Card_Footer_Font_Color,
                    card_footer_font = selected_custom_design_table.Card_Footer_Font,
                    card_footer_background_color = selected_custom_design_table.Card_Footer_Background_Color,
                    navigation_menu_background_color = selected_custom_design_table.Navigation_Menu_Background_Color,
                    navigation_menu_font_color = selected_custom_design_table.Navigation_Menu_Font_Color,
                    navigation_menu_font = selected_custom_design_table.Navigation_Menu_Font,
                    button_background_color = selected_custom_design_table.Button_Background_Color,
                    button_font = selected_custom_design_table.Button_Font,
                    button_font_color = selected_custom_design_table.Button_Font_Color
                }).FirstOrDefault();

            if (user == null)
                return await Task.FromResult(new User_Data_DTO { });

            await _UsersDBC.SaveChangesAsync();

            return await Task.FromResult(new User_Data_DTO
            {
                id = user.end_user_id,
                account_type = user.account_type,
                email_address = user.email_address,
                name = $@"{user.name}#{user.public_id}",
                current_language = user.current_language,
                language = user.language,
                region = user.region,
                groups = user.groups ?? "",
                roles = user.roles ?? "",
                grid_type = user.grid_type,
                nav_lock = user.nav_lock,
                custom_lbl = user.custom_lbl ?? "",
                created_on = user.created_on,
                avatar_url_path = user.avatar_url_path ?? "",
                avatar_title = user.avatar_title ?? "",
                twitch_user_name = user.twitch_user_name,
                twitch_id = user.twitch_user_id ?? 0,
                login_on = user.login_on,
                logout_on = user.logout_on ?? 0,

                online_status =
                    user.online_status ? (byte)2 :
                    user.offline_status ? (byte)0 :
                    user.hidden_status ? (byte)1 :
                    user.away_status ? (byte)3 :
                    user.dnd_status ? (byte)4 :
                    user.custom_status ? (byte)5 : (byte)0,

                theme =
                    user.light_theme ? (byte)0 :
                    user.night_theme ? (byte)1 :
                    user.custom_theme ? (byte)2 : (byte)0,

                alignment =
                    user.left_alignment ? (byte)0 :
                    user.center_alignment ? (byte)1 :
                    user.right_alignment ? (byte)2 : (byte)0,

                text_alignment =
                    user.left_text_alignment ? (byte)0 :
                    user.center_text_alignment ? (byte)1 :
                    user.right_text_alignment ? (byte)2 : (byte)0,
                
                gender = user.gender ?? (byte)2,
                birth_day = user.birth_day ?? 0,
                birth_month = user.birth_month ?? 0,
                birth_year = user.birth_year ?? 0,

                first_name = user.first_name ?? "",
                last_name = user.last_name ?? "",
                middle_name = user.middle_name ?? "",
                maiden_name = user.maiden_name ?? "",
                ethnicity = user.ethnicity ?? "",
                
                card_border_color = user.card_border_color,
                card_header_font = user.card_header_font,
                card_header_font_color = user.card_header_font_color,
                card_header_background_color = user.card_header_background_color,
                card_body_font = user.card_body_font,
                card_body_background_color = user.card_body_background_color,
                card_body_font_color = user.card_body_font_color,
                card_footer_font_color = user.card_footer_font_color,
                card_footer_font = user.card_footer_font,
                card_footer_background_color = user.card_footer_background_color,
                navigation_menu_background_color = user.navigation_menu_background_color,
                navigation_menu_font_color = user.navigation_menu_font_color,
                navigation_menu_font = user.navigation_menu_font,
                button_background_color = user.button_background_color,
                button_font = user.button_font,
                button_font_color = user.button_font_color
            });
        }
        public async Task<byte[]?> Read_User_Password_Hash_By_ID(long user_id)
        {
            return await Task.FromResult(_UsersDBC.Login_PasswordTbl.Where(user => user.End_User_ID == user_id).Select(user => user.Password).SingleOrDefault());
        }
        public async Task<User_Token_Data_DTO> Read_Require_Token_Data_By_ID(long end_user_id)
        {
            string? email_address = _UsersDBC.Login_Email_AddressTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? twitch_email_address = _UsersDBC.Twitch_Email_AddressTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? discord_email_address = _UsersDBC.Discord_Email_AddressTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            long? twitch_id = _UsersDBC.Twitch_IDsTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.End_User_ID).SingleOrDefault();
            long? discord_id = _UsersDBC.Discord_IDsTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.End_User_ID).SingleOrDefault();
            string? groups = _UsersDBC.Account_GroupsTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Groups).SingleOrDefault();
            string? roles = _UsersDBC.Account_RolesTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Roles).SingleOrDefault();
            byte account_type = _UsersDBC.Account_TypeTbl.Where(x => x.End_User_ID == end_user_id).Select(x => x.Type).SingleOrDefault();

            return await Task.FromResult(new User_Token_Data_DTO
            {
                id = end_user_id,
                account_type = account_type,
                email_address = email_address ?? "",
                twitch_id = twitch_id,
                twitch_email_address = twitch_email_address ?? "",
                discord_id = discord_id,
                discord_email_address = discord_email_address ?? "",
                groups = groups ?? "",
                roles = roles ?? ""
            });
        }
        public async Task<string> Read_User_Profile_By_ID(long user_id)
        {
            var userProfile = (from end_user in _UsersDBC.User_IDsTbl

            join login_email_table in _UsersDBC.Login_Email_AddressTbl
                on end_user.ID equals login_email_table.End_User_ID into resulting_email_table
            from login_email_table in resulting_email_table.DefaultIfEmpty()

            join selected_language_table in _UsersDBC.Selected_LanguageTbl
                on end_user.ID equals selected_language_table.End_User_ID into resulting_selected_language_table
            from selected_language_table in resulting_selected_language_table.DefaultIfEmpty()

            join avatar_table in _UsersDBC.Selected_AvatarTbl
                on end_user.ID equals avatar_table.End_User_ID into resulting_avatar_table
            from avatar_table in resulting_avatar_table.DefaultIfEmpty()

            join selected_name_table in _UsersDBC.Selected_NameTbl
                on end_user.ID equals selected_name_table.End_User_ID into resulting_name_table
            from selected_name_table in resulting_name_table.DefaultIfEmpty()

            join login_table in _UsersDBC.Login_Time_StampTbl
                on end_user.ID equals login_table.End_User_ID into resulting_login_table
            from login_table in resulting_login_table.DefaultIfEmpty()

            join logout_table in _UsersDBC.Logout_Time_StampTbl
                on end_user.ID equals logout_table.End_User_ID into resulting_logout_table
            from logout_table in resulting_logout_table.DefaultIfEmpty()

            join status_table in _UsersDBC.Selected_StatusTbl
                on end_user.ID equals status_table.End_User_ID into resulting_status_table
            from status_table in resulting_status_table.DefaultIfEmpty()

            where end_user.ID == user_id

            select new
            {
                email_address = login_email_table.Email_Address,
                name = selected_name_table.Name,
                login_on = login_table.Login_on,
                logout_on = logout_table != null ? logout_table.Logout_on : 0,
                language = selected_language_table != null
                    ? @$"{selected_language_table.Language_code}-{selected_language_table.Region_code}"
                    : null,
                language_code = selected_language_table.Language_code,
                region_code = selected_language_table.Region_code,
                avatar_url_path = avatar_table.Avatar_url_path,
                avatar_title = avatar_table.Avatar_title,
                created_on = end_user.Created_on,
                status_online = status_table.Online,
                status_offline = status_table.Offline,
                status_hidden = status_table.Hidden,
                status_away = status_table.Away,
                status_dnd = status_table.DND,
                status_custom = status_table.Custom,
                custom_lbl = status_table.Custom_lbl
            }).FirstOrDefault();

            if (userProfile == null)
            {
                return await Task.FromResult(JsonSerializer.Serialize(new { }));
            }

            // Set status_code based on available flags
            byte status_code = 0;
            if (userProfile.status_offline)
                status_code = 0;
            if (userProfile.status_hidden)
                status_code = 1;
            if (userProfile.status_online)
                status_code = 2;
            if (userProfile.status_away)
                status_code = 3;
            if (userProfile.status_dnd)
                status_code = 4;
            if (userProfile.status_custom)
                status_code = 5;

            return await Task.FromResult(JsonSerializer.Serialize(new
            {
                id = user_id,
                email_address = userProfile.email_address,
                name = userProfile.name,
                login_on = userProfile.login_on,
                logout_on = userProfile.logout_on,
                language = userProfile.language,
                online_status = status_code,
                custom_lbl = userProfile.custom_lbl,
                created_on = userProfile.created_on,
                avatar_url_path = userProfile.avatar_url_path,
                avatar_title = userProfile.avatar_title
            }));
        }
        public async Task<string> Read_WebSocket_Permission_Record_For_Both_End_Users(WebSocket_Chat_Permission dto)
        {
            bool requested = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Requested).SingleOrDefault();
            bool approved = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Approved).SingleOrDefault();
            bool blocked = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Blocked).SingleOrDefault();
            bool deleted = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Deleted).SingleOrDefault();

            bool requested_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Requested).SingleOrDefault();
            bool approved_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Approved).SingleOrDefault();
            bool blocked_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Blocked).SingleOrDefault();
            bool deleted_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Deleted).SingleOrDefault();


            if (requested == true || requested_swap_ids == true)
            {
                return JsonSerializer.Serialize(new
                {
                    requested = 1,
                    blocked = 0,
                    approved = 0,
                });
            }

            if (approved == true || approved_swap_ids == true)
            {
                return JsonSerializer.Serialize(new
                {
                    requested = 0,
                    blocked = 0,
                    approved = true,
                });
            }

            if (blocked == true || blocked_swap_ids == true)
            {
                return JsonSerializer.Serialize(new
                {
                    requested = 0,
                    blocked = true,
                    approved = 0,
                });
            }

            if (requested == false && approved == false && blocked == false && requested_swap_ids == false && approved_swap_ids == false && blocked_swap_ids == false && deleted == false && deleted_swap_ids == false)
            {
                await Update_Chat_Web_Socket_Permissions(dto);
                return JsonSerializer.Serialize(new
                {
                    requested = 1,
                    blocked = 0,
                    approved = 0,
                });
            }

            if (requested == false && approved == false && blocked == false && requested_swap_ids == false && approved_swap_ids == false && blocked_swap_ids == false && (deleted == true || deleted_swap_ids == true))
            {
                await Update_Chat_Web_Socket_Permissions(new WebSocket_Chat_Permission
                {
                    End_User_ID = dto.End_User_ID,
                    Participant_ID = dto.Participant_ID,
                    Requested = true,
                    Blocked = false,
                    Approved = false
                });
                return JsonSerializer.Serialize(new
                {
                    requested = 1,
                    blocked = 0,
                    approved = 0,
                });
            }

            return JsonSerializer.Serialize(new
            {
                requested = 0,
                blocked = 0,
                approved = 0,
            });
        }
        public async Task<string> Read_End_User_WebSocket_Sent_Chat_Requests(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.End_User_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == user_id && x.Requested == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Sent_Chat_Blocks(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.End_User_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == user_id && x.Blocked == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Sent_Chat_Approvals(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.End_User_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.End_User_ID == user_id && x.Approved == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Received_Chat_Requests(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Requested == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_Received_Friend_Requests(long user_id)
        {
            if (!_UsersDBC.Friends_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.Friends_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Requested == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Received_Chat_Blocks(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Blocked == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_WebSocket_Received_Chat_Approvals(long user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Approved == true)
                    .ToList()));
            }
        }
        public async Task<string> Read_End_User_Friend_Permissions_By_ID(long user_id)
        {
            var user_sent_permissions = _UsersDBC.Friends_PermissionTbl
                .Where(friend_permission => friend_permission.End_User_ID == user_id && friend_permission.Deleted == false)
                .ToDictionary(friend_permission => friend_permission.Participant_ID,
                    friend_permission => new
                    {
                        request = friend_permission.Requested,
                        block = friend_permission.Blocked,
                        approve = friend_permission.Approved,
                        record_updated_by = friend_permission.Updated_by,
                        record_updated_on = friend_permission.Updated_on,
                        record_created_on = friend_permission.Created_on,
                        record_created_by = friend_permission.Created_by,
                        time_stamp = TimeStamp()
                    }
                );

            var user_received_permissions = _UsersDBC.Friends_PermissionTbl
                .Where(friend_permission => friend_permission.Participant_ID == user_id && friend_permission.Deleted == false)
                .ToDictionary(friend_permission => friend_permission.End_User_ID,
                    friend_permission => new {
                        request = friend_permission.Requested,
                        block = friend_permission.Blocked,
                        approve = friend_permission.Approved,
                        record_updated_by = friend_permission.Updated_by,
                        record_updated_on = friend_permission.Updated_on,
                        record_created_on = friend_permission.Created_on,
                        record_created_by = friend_permission.Created_by,
                        time_stamp = TimeStamp()
                    }
                );

            return await Task.FromResult(JsonSerializer.Serialize(new
            {
                sent_permissions = user_sent_permissions,
                received_permissions = user_received_permissions,
                time_stamped = TimeStamp()
            }));
        }
        public async Task<byte> Read_End_User_Selected_Status(long user_id)
        {
            var status = await _UsersDBC.Selected_StatusTbl.Where(x => x.End_User_ID == user_id).Select(x => new {
                x.Online,
                x.Offline,
                x.Hidden,
                x.Away,
                x.DND,
                x.Custom
            }).SingleOrDefaultAsync();

            if (status == null)
                return 255;

            return status switch
            {
                var record_is when record_is.Offline == true => 0,
                var record_is when record_is.Hidden == true => 1,
                var record_is when record_is.Online == true => 2,
                var record_is when record_is.Away == true => 3,
                var record_is when record_is.DND == true => 4,
                var record_is when record_is.Custom == true => 5,
                _ => 255
            };
        }
        public async Task<string> Update_Chat_Web_Socket_Permissions(WebSocket_Chat_Permission dto)
        {
            try
            {
                var websocket_permission_record_exists_in_database = await _UsersDBC.WebSocket_Chat_PermissionTbl
                    .Where(x => x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID || x.Participant_ID == dto.End_User_ID && x.End_User_ID == x.Participant_ID)
                    .FirstOrDefaultAsync();

                if (websocket_permission_record_exists_in_database == null)
                {
                    await _UsersDBC.WebSocket_Chat_PermissionTbl.AddAsync(new WebSocket_Chat_PermissionTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Participant_ID = dto.Participant_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Requested = dto.Requested,
                        Blocked = dto.Blocked,
                        Approved = dto.Approved
                    });
                } else {
                    websocket_permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    websocket_permission_record_exists_in_database.Updated_on = TimeStamp();
                    websocket_permission_record_exists_in_database.Deleted = false;
                    websocket_permission_record_exists_in_database.Blocked = dto.Blocked;
                    websocket_permission_record_exists_in_database.Approved = dto.Approved;
                    websocket_permission_record_exists_in_database.Requested = dto.Requested;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    participant_id = dto.Participant_ID,
                    updated_on = TimeStamp(),
                    updated_by = dto.End_User_ID
                });
            } catch {
                return "Server Error: Update Chat Permissions Failed.";
            }
        }
        public async Task<string> Update_Pending_Email_Registration_Record(Pending_Email_Registration dto)
        {
            try
            {
                await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Email_Address, dto.Email_Address)
                    .SetProperty(col => col.Code, dto.Code)
                    .SetProperty(col => col.Language_Region, @$"{dto.Language}-{dto.Region}")
                    .SetProperty(col => col.Updated_on, TimeStamp())
                    .SetProperty(col => col.Updated_by, 0)
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    email_address = dto.Email_Address,
                    language = dto.Language,
                    region = dto.Region,
                    updated_on = TimeStamp(),
                });
            }
            catch
            {
                return "Server Error: Email Address Registration Failed";
            }
        }
        public async Task<string> Update_Friend_Permissions(Friends_Permission dto)
        {
            try
            {
                var permission_record_exists_in_database = await _UsersDBC.Friends_PermissionTbl.Where(x =>
                    (x.End_User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID) ||
                    (x.End_User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID)
                ).FirstOrDefaultAsync();

                if (permission_record_exists_in_database == null)
                {
                    await _UsersDBC.Friends_PermissionTbl.AddAsync(new Friends_PermissionTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Participant_ID = dto.Participant_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Requested = dto.Requested,
                        Blocked = dto.Blocked,
                        Approved = dto.Approved
                    });
                } else if (dto.Unblock) {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = false;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Deleted = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                } else if (permission_record_exists_in_database.Blocked) {
                    return await Task.FromResult(JsonSerializer.Serialize(new
                    {
                        participant_id = dto.Participant_ID,
                        requested = false,
                        blocked = true,
                        approved = false,
                        time_stamped = TimeStamp()
                    }));
                } else if (dto.Blocked) {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = false;
                    permission_record_exists_in_database.Blocked = true;
                    permission_record_exists_in_database.Deleted = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                } else if (permission_record_exists_in_database.Deleted) {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = true;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Deleted = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                } else if (dto.Deleted) {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = false;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                    permission_record_exists_in_database.Deleted = true;
                    permission_record_exists_in_database.Deleted_on = TimeStamp();
                    permission_record_exists_in_database.Deleted_by = dto.End_User_ID;
                } else if (dto.Requested) {
                    permission_record_exists_in_database.Approved = false;
                    permission_record_exists_in_database.Requested = true;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                } else if (dto.Approved) {
                    permission_record_exists_in_database.Approved = true;
                    permission_record_exists_in_database.Requested = false;
                    permission_record_exists_in_database.Blocked = false;
                    permission_record_exists_in_database.Updated_by = dto.End_User_ID;
                    permission_record_exists_in_database.Updated_on = TimeStamp();
                }

                await _UsersDBC.SaveChangesAsync();

                return await Task.FromResult(JsonSerializer.Serialize(new
                {
                    end_user_id = dto.End_User_ID,
                    participant_id = dto.Participant_ID,
                    requested = dto.Requested,
                    blocked = dto.Blocked,
                    approved = dto.Approved,
                    time_stamped = TimeStamp()
                }));
            } catch {
                return Task.FromResult(JsonSerializer.Serialize("Friend Request Permission Failed.")).Result;
            }
        }
        public async Task<string> Update_End_User_Avatar(Selected_Avatar dto)
        {   
            try
            {
                var avatar_url_path_record = await _UsersDBC.Selected_AvatarTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (avatar_url_path_record == null)
                {
                    await _UsersDBC.Selected_AvatarTbl.AddAsync(new Selected_AvatarTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Avatar_url_path = dto.Avatar_url_path,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    avatar_url_path_record.End_User_ID = dto.End_User_ID;
                    avatar_url_path_record.Avatar_url_path = dto.Avatar_url_path;
                    avatar_url_path_record.Updated_on = TimeStamp();
                    avatar_url_path_record.Created_by = dto.End_User_ID;
                    avatar_url_path_record.Created_on = TimeStamp();
                    avatar_url_path_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    avatar_url_path = dto.Avatar_url_path
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Avatar_Title(Selected_Avatar_Title dto)
        {
            try
            {
                var avatar_title_record = await _UsersDBC.Selected_Avatar_TitleTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (avatar_title_record == null)
                {
                    await _UsersDBC.Selected_Avatar_TitleTbl.AddAsync(new Selected_Avatar_TitleTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Avatar_title = dto.Avatar_title,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    avatar_title_record.End_User_ID = dto.End_User_ID;
                    avatar_title_record.Avatar_title = dto.Avatar_title;
                    avatar_title_record.Updated_on = TimeStamp();
                    avatar_title_record.Created_by = dto.End_User_ID;
                    avatar_title_record.Created_on = TimeStamp();
                    avatar_title_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    avatar_title = dto.Avatar_title
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Name(Selected_Name dto)
        {
            try {
                await _UsersDBC.Selected_NameTbl.Where(x => x.End_User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Name, dto.Name)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    .SetProperty(col => col.Updated_on, TimeStamp())
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    name = dto.Name
                });
            } catch {
                return "Server Error: Update Name Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Alignment(Selected_App_Alignment dto)
        {
            try
            {
                var alignment_record = await _UsersDBC.Selected_App_AlignmentTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                bool left = false, center = false, right = false;

                switch (dto.Alignment)
                {
                    case 0: left = true; break;
                    case 1: center = true; break;
                    case 2: right = true; break;
                    default: return "Server Error: Invalid alignment value.";
                }

                if (alignment_record == null)
                {
                    await _UsersDBC.Selected_App_AlignmentTbl.AddAsync(new Selected_App_AlignmentTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Left = left,
                        Center = center,
                        Right = right,
                        Created_on = TimeStamp(),
                        Updated_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    alignment_record.Left = left;
                    alignment_record.Center = center;
                    alignment_record.Right = right;
                    alignment_record.Updated_on = TimeStamp();
                    alignment_record.Updated_by = dto.End_User_ID;
                }

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    alignment = dto.Alignment
                });
            } catch {
                return "Server Error: Update Alignment Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Text_Alignment(Selected_App_Text_Alignment dto)
        {
            try {

                var text_alignment_record = await _UsersDBC.Selected_App_Text_AlignmentTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                bool text_float_left = false, text_center = false, text_float_right = false;

                switch (dto.Text_alignment) {
                    case 0: text_float_left = true; break;
                    case 1: text_center = true; break;
                    case 2: text_float_right = true; break;
                }

                if (text_alignment_record == null) {
                    await _UsersDBC.Selected_App_Text_AlignmentTbl.AddAsync(new Selected_App_Text_AlignmentTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Left = text_float_left,
                        Center = text_center,
                        Right = text_float_right,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    text_alignment_record.Left = text_float_left;
                    text_alignment_record.Center = text_center;
                    text_alignment_record.Right = text_float_right;
                    text_alignment_record.Updated_by = dto.End_User_ID;
                    text_alignment_record.Updated_on = TimeStamp();
                }
                
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    text_alignment = dto.Text_alignment
                });
            } catch {
                return "Server Error: Update Text Alignment Selection Failed.";
            }
        }
        public async Task<string> Update_End_User_Account_Type(Account_Types dto)
        {
            try {
                var account_type_record = await _UsersDBC.Account_TypeTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (account_type_record == null)
                {
                    await _UsersDBC.Account_TypeTbl.AddAsync(new Account_TypeTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Type = dto.Type,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    account_type_record.End_User_ID = dto.End_User_ID;
                    account_type_record.Type = dto.Type;
                    account_type_record.Updated_on = TimeStamp();
                    account_type_record.Created_on = TimeStamp();
                    account_type_record.Updated_by = dto.End_User_ID;
                }
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    account_type = dto.Type
                });
            } catch {
                return "Server Error: Update Text Alignment Failed."; 
            }
        }
        public async Task<string> Update_End_User_Selected_Grid_Type(Selected_App_Grid_Type dto)
        {
            try
            {
                var grid_type_record = await _UsersDBC.Selected_App_Grid_TypeTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (grid_type_record == null)
                {
                    await _UsersDBC.Selected_App_Grid_TypeTbl.AddAsync(new Selected_App_Grid_TypeTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Grid = dto.Grid,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    grid_type_record.End_User_ID = dto.End_User_ID;
                    grid_type_record.Grid = dto.Grid;
                    grid_type_record.Updated_on = TimeStamp();
                    grid_type_record.Created_on = TimeStamp();
                    grid_type_record.Updated_by = dto.End_User_ID;
                }
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    grid = dto.Grid
                });
            } catch {
                return "Server Error: Update Text Alignment Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Language(Selected_Language dto)
        {
            try
            {
                var language_record = await _UsersDBC.Selected_LanguageTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (language_record == null)
                {
                    await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Language_code = dto.Language,
                        Region_code = dto.Region
                    });
                }
                else
                {
                    language_record.End_User_ID = dto.End_User_ID;
                    language_record.Language_code = dto.Language;
                    language_record.Region_code = dto.Region;
                    language_record.Updated_on = TimeStamp();
                    language_record.Created_by = dto.End_User_ID;
                    language_record.Created_on = TimeStamp();
                    language_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    region = dto.Region,
                    language = dto.Language
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Nav_Lock(Selected_Navbar_Lock dto)
        {
            try
            {
                var update_existing_record_attempt = await _UsersDBC.Selected_Navbar_LockTbl.Where(x => x.End_User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    .SetProperty(col => col.Updated_on, TimeStamp())
                    .SetProperty(col => col.Locked, dto.Locked)
                );

                if (update_existing_record_attempt == 0)
                {
                    await _UsersDBC.Selected_Navbar_LockTbl.AddAsync(new Selected_Navbar_LockTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Locked = dto.Locked
                    });
                }

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    locked = dto.Locked
                });
            } catch {
                return "Server Error: Update Text Alignment Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Status(Selected_Status dto)
        {
            try
            {
                var status_record = await _UsersDBC.Selected_StatusTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                bool online = false, offline = false, dnd = false, away = false, hidden = false, custom = false;

                switch (dto.Status) {
                    case 0: offline = true; break;
                    case 1: hidden = true; break;
                    case 2: online = true; break;
                    case 3: away = true; break;
                    case 4: dnd = true; break;
                    case 5: custom = true; break;
                    default: break;
                }

                if (status_record == null)
                {
                    await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Online = online,
                        Offline = offline,
                        Hidden = hidden,
                        DND = dnd,
                        Away = away,
                        Custom = custom,
                        Custom_lbl = dto.Custom_lbl
                    });

                } else {
                    status_record.End_User_ID = dto.End_User_ID;
                    status_record.Updated_on = TimeStamp();
                    status_record.Created_on = TimeStamp();
                    status_record.Updated_by = dto.End_User_ID;
                    status_record.Created_by = dto.End_User_ID;
                    status_record.Online = online;
                    status_record.Offline = offline;
                    status_record.Hidden = hidden;
                    status_record.DND = dnd;
                    status_record.Away = away;
                    status_record.Custom = custom;
                    status_record.Custom_lbl = dto.Custom_lbl;
                }

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    status = dto.Status
                });
            } catch {
                return "Server Error: Update Status Selection Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Theme(Selected_Theme dto)
        {
            try {
                bool Light = false, Night = false, Custom = false;

                switch (dto.Theme)
                {
                    case 0: Light = true; break;
                    case 1: Night = true; break;
                    case 2: Custom = true; break;
                }

                var theme_record = await _UsersDBC.Selected_ThemeTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (theme_record == null) {
                    await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Light = Light,
                        Night = Night,
                        Custom = Custom,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    theme_record.End_User_ID = dto.End_User_ID;
                    theme_record.Light = Light;
                    theme_record.Night = Night;
                    theme_record.Custom = Custom;
                    theme_record.Updated_on = TimeStamp();
                    theme_record.Created_by = dto.End_User_ID;
                    theme_record.Created_on = TimeStamp();
                    theme_record.Updated_by = dto.End_User_ID;
                }

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = dto.Theme
                });
            } catch {
                return "Server Error: Update Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Border_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_bored_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_bored_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Border_Color = dto.Card_Border_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_bored_color_record.End_User_ID = dto.End_User_ID;
                    card_bored_color_record.Card_Border_Color = dto.Card_Border_Color;
                    card_bored_color_record.Updated_on = TimeStamp();
                    card_bored_color_record.Created_by = dto.End_User_ID;
                    card_bored_color_record.Created_on = TimeStamp();
                    card_bored_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_border_color = dto.Card_Border_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Header_Font(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Header_Font = dto.Card_Header_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Card_Header_Font = dto.Card_Header_Font;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_header_font = dto.Card_Header_Font
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Header_Background_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_header_background_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_header_background_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Header_Background_Color = dto.Card_Header_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_header_background_color_record.End_User_ID = dto.End_User_ID;
                    card_header_background_color_record.Card_Header_Background_Color = dto.Card_Header_Background_Color;
                    card_header_background_color_record.Updated_on = TimeStamp();
                    card_header_background_color_record.Created_by = dto.End_User_ID;
                    card_header_background_color_record.Created_on = TimeStamp();
                    card_header_background_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_header_background_color = dto.Card_Header_Background_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Header_Font_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_header_font_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_header_font_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Header_Font_Color = dto.Card_Header_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_header_font_color_record.End_User_ID = dto.End_User_ID;
                    card_header_font_color_record.Card_Header_Font_Color = dto.Card_Header_Font_Color;
                    card_header_font_color_record.Updated_on = TimeStamp();
                    card_header_font_color_record.Created_by = dto.End_User_ID;
                    card_header_font_color_record.Created_on = TimeStamp();
                    card_header_font_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_header_font_color = dto.Card_Header_Font_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Footer_Font(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_footer_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_footer_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Footer_Font = dto.Card_Footer_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_footer_font_record.End_User_ID = dto.End_User_ID;
                    card_footer_font_record.Card_Footer_Font = dto.Card_Footer_Font;
                    card_footer_font_record.Updated_on = TimeStamp();
                    card_footer_font_record.Created_by = dto.End_User_ID;
                    card_footer_font_record.Created_on = TimeStamp();
                    card_footer_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_footer_font = dto.Card_Footer_Font
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Footer_Background_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_footer_background_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_footer_background_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Footer_Background_Color = dto.Card_Footer_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_footer_background_color_record.End_User_ID = dto.End_User_ID;
                    card_footer_background_color_record.Card_Footer_Background_Color = dto.Card_Footer_Background_Color;
                    card_footer_background_color_record.Updated_on = TimeStamp();
                    card_footer_background_color_record.Created_by = dto.End_User_ID;
                    card_footer_background_color_record.Created_on = TimeStamp();
                    card_footer_background_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_footer_background_color = dto.Card_Footer_Background_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Delete_End_User_Selected_App_Custom_Design(Selected_App_Custom_Design dto)
        {
            try
            {
                await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.End_User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Card_Header_Font_Color, "")
                    .SetProperty(col => col.Card_Header_Font, "")
                    .SetProperty(col => col.Card_Header_Background_Color, "")
                    .SetProperty(col => col.Card_Body_Background_Color, "")
                    .SetProperty(col => col.Card_Body_Font, "")
                    .SetProperty(col => col.Card_Body_Font_Color, "")
                    .SetProperty(col => col.Card_Footer_Background_Color, "")
                    .SetProperty(col => col.Card_Footer_Font, "")
                    .SetProperty(col => col.Card_Footer_Font_Color, "")
                    .SetProperty(col => col.Navigation_Menu_Background_Color, "")
                    .SetProperty(col => col.Navigation_Menu_Font_Color, "")
                    .SetProperty(col => col.Navigation_Menu_Font, "")
                    .SetProperty(col => col.Button_Background_Color, "")
                    .SetProperty(col => col.Button_Font, "")
                    .SetProperty(col => col.Button_Font_Color, "")
                    .SetProperty(col => col.Card_Border_Color, "")
                    .SetProperty(col => col.Updated_on, TimeStamp())
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = 0,
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Footer_Font_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var card_footer_font_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (card_footer_font_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Footer_Font_Color = dto.Card_Footer_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    card_footer_font_color_record.End_User_ID = dto.End_User_ID;
                    card_footer_font_color_record.Card_Footer_Font_Color = dto.Card_Footer_Font_Color;
                    card_footer_font_color_record.Updated_on = TimeStamp();
                    card_footer_font_color_record.Created_by = dto.End_User_ID;
                    card_footer_font_color_record.Created_on = TimeStamp();
                    card_footer_font_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_footer_font_color = dto.Card_Footer_Font_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Body_Font(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Body_Font = dto.Card_Body_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Card_Body_Font = dto.Card_Body_Font;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_body_font = dto.Card_Body_Font
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Body_Background_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Body_Background_Color = dto.Card_Body_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Card_Body_Background_Color = dto.Card_Body_Background_Color;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_body_background_color = dto.Card_Body_Background_Color
                });
            }
            catch
            {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Body_Font_Color(Selected_App_Custom_Design dto)
        {
            try {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Card_Body_Font_Color = dto.Card_Body_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Card_Body_Font_Color = dto.Card_Body_Font_Color;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_body_font_color = dto.Card_Body_Font_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Navigation_Menu_Font(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Navigation_Menu_Font = dto.Navigation_Menu_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Navigation_Menu_Font = dto.Navigation_Menu_Font;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    navigation_menu_font = dto.Navigation_Menu_Font
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Navigation_Menu_Background_Color(Selected_App_Custom_Design dto)
        {
            try {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Navigation_Menu_Background_Color = dto.Navigation_Menu_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Navigation_Menu_Background_Color = dto.Navigation_Menu_Background_Color;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    navigation_menu_Background_color = dto.Navigation_Menu_Background_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Navigation_Menu_Font_Color(Selected_App_Custom_Design dto)
        {
            try {
                var button_navigation_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_navigation_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Navigation_Menu_Font_Color = dto.Navigation_Menu_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    button_navigation_font_record.End_User_ID = dto.End_User_ID;
                    button_navigation_font_record.Navigation_Menu_Font_Color = dto.Navigation_Menu_Font_Color;
                    button_navigation_font_record.Updated_on = TimeStamp();
                    button_navigation_font_record.Created_by = dto.End_User_ID;
                    button_navigation_font_record.Created_on = TimeStamp();
                    button_navigation_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();

                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    navigation_menu_font_color = dto.Navigation_Menu_Font_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Button_Font(Selected_App_Custom_Design dto)
        {
            try {
                var button_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Button_Font = dto.Button_Font,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    button_font_record.End_User_ID = dto.End_User_ID;
                    button_font_record.Button_Font = dto.Button_Font;
                    button_font_record.Updated_on = TimeStamp();
                    button_font_record.Created_by = dto.End_User_ID;
                    button_font_record.Created_on = TimeStamp();
                    button_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    button_font = dto.Button_Font
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Button_Background_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_background_color_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_background_color_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Button_Background_Color = dto.Button_Background_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    button_background_color_record.End_User_ID = dto.End_User_ID;
                    button_background_color_record.Button_Background_Color = dto.Button_Background_Color;
                    button_background_color_record.Updated_on = TimeStamp();
                    button_background_color_record.Created_by = dto.End_User_ID;
                    button_background_color_record.Created_on = TimeStamp();
                    button_background_color_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    button_background_color = dto.Button_Background_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Button_Font_Color(Selected_App_Custom_Design dto)
        {
            try
            {
                var button_font_record = await _UsersDBC.Selected_App_Custom_DesignTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (button_font_record == null)
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Button_Font_Color = dto.Button_Font_Color,
                        Updated_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    button_font_record.End_User_ID = dto.End_User_ID;
                    button_font_record.Button_Font_Color = dto.Button_Font_Color;
                    button_font_record.Updated_on = TimeStamp();
                    button_font_record.Created_by = dto.End_User_ID;
                    button_font_record.Created_on = TimeStamp();
                    button_font_record.Updated_by = dto.End_User_ID;
                }

                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new { 
                    id = dto.End_User_ID,
                    theme = "Custom",
                    button_font_color = dto.Button_Font_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Password(Password_Change dto)
        {
            try {
                if (!dto.Email_address.IsNullOrEmpty()) {
                    await _UsersDBC.Login_PasswordTbl.Where(x => x.End_User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Password, Password.Process_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes($"{dto.New_password}"), Encoding.UTF8.GetBytes($"{dto.Email_address}{_Constants.JWT_SECURITY_KEY}")).Result)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                        .SetProperty(col => col.Updated_on, TimeStamp())
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return "Update End User Password Completed.";
            } catch {
                return "Server Error: Update Password Failed.";
            }
        }
        public async Task<string> Update_End_User_Login_Time_Stamp(Login_Time_Stamp dto)
        {
            try
            {
                var login_record = await _UsersDBC.Login_Time_StampTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (login_record == null)
                {
                    await _UsersDBC.Login_Time_StampTbl.AddAsync(new Login_Time_StampTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Deleted = false,
                        Deleted_on = 0,
                        Deleted_by = 0,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Login_on = TimeStamp(),
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Remote_IP = dto.Remote_IP,
                        Remote_Port = dto.Remote_Port,
                        Server_IP = dto.Server_IP,
                        Server_Port = dto.Server_Port,
                        Client_IP = dto.Client_IP,
                        Client_Port = dto.Client_Port,
                        User_agent = dto.User_agent,
                        Down_link = dto.Down_link,
                        Connection_type = dto.Connection_type,
                        RTT = dto.RTT,
                        Data_saver = dto.Data_saver,
                        Device_ram_gb = dto.Device_ram_gb,
                        Orientation = dto.Orientation,
                        Screen_width = dto.Screen_width,
                        Screen_height = dto.Screen_height,
                        Window_height = dto.Window_height,
                        Window_width = dto.Window_width,
                        Color_depth = dto.Color_depth,
                        Pixel_depth = dto.Pixel_depth
                    });
                } else {
                    login_record.End_User_ID = dto.End_User_ID;
                    login_record.Updated_on = TimeStamp();
                    login_record.Created_on = TimeStamp();
                    login_record.Updated_by = dto.End_User_ID;
                    login_record.Created_by = dto.End_User_ID;
                    login_record.Login_on = TimeStamp();
                    login_record.Location = dto.Location;
                    login_record.Client_time = dto.Client_time;
                    login_record.Remote_IP = dto.Remote_IP;
                    login_record.Remote_Port = dto.Remote_Port;
                    login_record.Server_IP = dto.Server_IP;
                    login_record.Server_Port = dto.Server_Port;
                    login_record.Client_IP = dto.Client_IP;
                    login_record.Client_Port = dto.Client_Port;
                    login_record.User_agent = dto.User_agent;
                    login_record.Down_link = dto.Down_link;
                    login_record.Connection_type = dto.Connection_type;
                    login_record.RTT = dto.RTT;
                    login_record.Data_saver = dto.Data_saver;
                    login_record.Device_ram_gb = dto.Device_ram_gb;
                    login_record.Orientation = dto.Orientation;
                    login_record.Screen_width = dto.Screen_width;
                    login_record.Screen_height = dto.Screen_height;
                    login_record.Window_height = dto.Window_height;
                    login_record.Window_width = dto.Window_width;
                    login_record.Color_depth = dto.Color_depth;
                    login_record.Pixel_depth = dto.Pixel_depth;
                }

                return Task.FromResult(JsonSerializer.Serialize(new {
                    id = dto.End_User_ID,
                    login_on = TimeStamp()
                })).Result;
            } catch {
                return "Server Error: Update Login Failed.";
            }
        }
        public async Task<string> Update_End_User_Logout(Logout_Time_Stamp dto)
        {
            var logout_time_stamp_record = await _UsersDBC.Logout_Time_StampTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (logout_time_stamp_record == null)
            {
                await _UsersDBC.Logout_Time_StampTbl.AddAsync(new Logout_Time_StampTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Logout_on = TimeStamp(),
                    Updated_by = dto.End_User_ID,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    User_agent = dto.User_agent,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb
                });
                await _UsersDBC.SaveChangesAsync();
            } else {
                logout_time_stamp_record.End_User_ID = dto.End_User_ID;
                logout_time_stamp_record.Logout_on = TimeStamp();
                logout_time_stamp_record.Updated_by = dto.End_User_ID;
                logout_time_stamp_record.Updated_on = TimeStamp();
                logout_time_stamp_record.Created_on = TimeStamp();
                logout_time_stamp_record.Location = dto.Location;
                logout_time_stamp_record.Remote_IP = dto.Remote_IP;
                logout_time_stamp_record.Remote_Port = dto.Remote_Port;
                logout_time_stamp_record.Server_IP = dto.Server_IP;
                logout_time_stamp_record.Server_Port = dto.Server_Port;
                logout_time_stamp_record.Client_IP = dto.Client_IP;
                logout_time_stamp_record.Client_Port = dto.Client_Port;
                logout_time_stamp_record.Client_time = dto.Client_Time_Parsed;
                logout_time_stamp_record.User_agent = dto.User_agent;
                logout_time_stamp_record.Window_height = dto.Window_height;
                logout_time_stamp_record.Window_width = dto.Window_width;
                logout_time_stamp_record.Screen_height = dto.Screen_height;
                logout_time_stamp_record.Screen_width = dto.Screen_width;
                logout_time_stamp_record.RTT = dto.RTT;
                logout_time_stamp_record.Orientation = dto.Orientation;
                logout_time_stamp_record.Data_saver = dto.Data_saver;
                logout_time_stamp_record.Color_depth = dto.Color_depth;
                logout_time_stamp_record.Pixel_depth = dto.Pixel_depth;
                logout_time_stamp_record.Connection_type = dto.Connection_type;
                logout_time_stamp_record.Down_link = dto.Down_link;
                logout_time_stamp_record.Device_ram_gb = dto.Device_ram_gb;
            }

            return Task.FromResult(JsonSerializer.Serialize(new {
                End_User_ID = dto.End_User_ID,
                logout_on = TimeStamp()
            })).Result;
        }
        public async Task<string> Update_End_User_First_Name(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    First_Name = dto.First_name,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.First_Name = dto.First_name;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                first_name = dto.First_name
            });
        }
        public async Task<string> Update_End_User_Last_Name(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Last_Name = dto.Last_name,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            } else {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Last_Name = dto.Last_name;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                last_name = dto.Last_name
            });
        }
        public async Task<string> Update_End_User_Middle_Name(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Middle_Name = dto.Middle_name,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            } else {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Middle_Name = dto.Middle_name;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new
            {
                id = dto.End_User_ID,
                middle_name = dto.Middle_name
            });
        }
        public async Task<string> Update_End_User_Maiden_Name(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Maiden_Name = dto.Maiden_name,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            } else {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Maiden_Name = dto.Maiden_name;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                maiden_name = dto.Maiden_name
            });
        }
        public async Task<string> Update_End_User_Gender(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Gender = dto.Gender,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            } else {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Gender = dto.Gender;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new { 
                id = dto.End_User_ID,
                gender = dto.Gender
            });
        }
        public async Task<string> Update_End_User_Ethnicity(Identities dto)
        {
            var identity_record = await _UsersDBC.IdentityTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (identity_record == null)
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Ethnicity = dto.Ethnicity,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                identity_record.End_User_ID = dto.End_User_ID;
                identity_record.Ethnicity = dto.Ethnicity;
                identity_record.Updated_on = TimeStamp();
                identity_record.Created_on = TimeStamp();
                identity_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                ethnicity = dto.Ethnicity
            });
        }
        public async Task<string> Update_End_User_Birth_Date(Identities dto)
        {
            var birth_date_record = await _UsersDBC.Birth_DateTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

            if (birth_date_record == null)
            {
                await _UsersDBC.Birth_DateTbl.AddAsync(new Birth_DateTbl
                {
                    End_User_ID = dto.End_User_ID,
                    Month = dto.Month,
                    Day = dto.Day,
                    Year = dto.Year,
                    Updated_on = TimeStamp(),
                    Created_on = TimeStamp(),
                    Updated_by = dto.End_User_ID
                });
            } else {
                birth_date_record.End_User_ID = dto.End_User_ID;
                birth_date_record.Day = dto.Day;
                birth_date_record.Month = dto.Month;
                birth_date_record.Year = dto.Year;
                birth_date_record.Updated_on = TimeStamp();
                birth_date_record.Created_on = TimeStamp();
                birth_date_record.Updated_by = dto.End_User_ID;
            }

            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                birth_month = dto.Month,
                birth_day = dto.Day,
                birth_year = dto.Year,
            });
        }
        public async Task<string> Update_End_User_Account_Groups(Account_Group dto)
        {
            try
            {
                var account_group_record = await _UsersDBC.Account_GroupsTbl.SingleOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (account_group_record == null)
                {
                    await _UsersDBC.Account_GroupsTbl.AddAsync(new Account_GroupsTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Groups = dto.Groups,
                        Updated_on = TimeStamp(),
                        Updated_by = dto.End_User_ID,
                        Created_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Deleted = false,
                        Deleted_by = 0,
                        Deleted_on = 0
                    });
                } else {
                    account_group_record.End_User_ID = dto.End_User_ID;
                    account_group_record.Groups = dto.Groups;
                    account_group_record.Updated_on = TimeStamp();
                    account_group_record.Updated_by = dto.End_User_ID;
                    account_group_record.Created_on = TimeStamp();
                }
                return JsonSerializer.Serialize(new { 
                    end_user_groups = dto.Groups 
                });
            } catch {
                return "Server Error: Update Account Groups Failed.";
            }
        }
        public async Task<string> Update_End_User_Account_Roles(Account_Role dto)
        {
            try
            {
                var account_roles_record = await _UsersDBC.Account_RolesTbl.FirstOrDefaultAsync(x => x.End_User_ID == dto.End_User_ID);

                if (account_roles_record == null)
                {
                    await _UsersDBC.Account_RolesTbl.AddAsync(new Account_RolesTbl
                    {
                        End_User_ID = dto.End_User_ID,
                        Roles = dto.Roles,
                        Updated_on = TimeStamp(),
                        Created_on = TimeStamp(),
                        Created_by = dto.End_User_ID,
                        Updated_by = dto.End_User_ID,
                        Deleted = false,
                        Deleted_by = 0,
                        Deleted_on = 0
                    });
                }
                else
                {
                    account_roles_record.End_User_ID = dto.End_User_ID;
                    account_roles_record.Roles = dto.Roles;
                    account_roles_record.Updated_by = dto.End_User_ID;
                    account_roles_record.Updated_on = TimeStamp();
                }

                return JsonSerializer.Serialize(new
                {
                    roles = dto.Roles
                });
            }
            catch
            {
                return "Server Error: Update End User Roles Failed.";
            }
        }

        public async Task<bool> Validate_Client_With_Server_Authorization(Report_Failed_Authorization_History dto)
        {

            if (dto.Server_User_Agent == "error" || dto.Client_User_Agent != dto.Server_User_Agent)
            {
                await Insert_Report_Failed_User_Agent_History(new Report_Failed_User_Agent_History
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Server_Port,
                    Language = dto.Language,
                    Login_type = dto.Login_type,
                    Region = dto.Region,
                    Location = dto.Location,
                    Client_time = dto.Client_Time_Parsed,
                    Reason = "User-Agent Client-Server Mismatch",
                    Controller = dto.Controller,
                    Action = dto.Action,
                    Server_User_Agent = dto.Server_User_Agent,
                    Client_User_Agent = dto.Client_User_Agent,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb
                });
                return false;
            }

            if (dto.JWT_issuer_key != _Constants.JWT_ISSUER_KEY ||
                dto.JWT_client_key != _Constants.JWT_CLIENT_KEY ||
                dto.JWT_client_address != _Constants.JWT_CLAIM_WEBPAGE)
            {
                await Insert_Report_Failed_JWT_History_Record(new Report_Failed_JWT_History
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Server_Port,
                    User_agent = dto.Client_User_Agent,
                    Client_id = dto.End_User_ID,
                    JWT_id = dto.JWT_id,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Location = dto.Location,
                    Login_type = dto.Login_type,
                    Client_time = dto.Client_Time_Parsed,
                    Reason = "JWT Client-Server Mismatch",
                    Controller = dto.Controller,
                    Action = dto.Action,
                    End_User_ID = dto.JWT_id,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_client_address = dto.JWT_client_address,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb,
                    Token = dto.Token
                });
                return false;
            }

            if (dto.Client_id != dto.JWT_id)
            {
                await Insert_Report_Failed_JWT_History_Record(new Report_Failed_JWT_History
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Server_Port,
                    User_agent = dto.Client_User_Agent,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Location = dto.Location,
                    Login_type = dto.Login_type,
                    Client_time = dto.Client_Time_Parsed,
                    Reason = "JWT Client-ID Mismatch",
                    Controller = dto.Controller,
                    Action = dto.Action,
                    End_User_ID = dto.JWT_id,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_client_address = dto.JWT_client_address,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb,
                    Token = dto.Token
                });
                return false;
            }

            if (dto.JWT_id != 0 && !ID_Exists_In_Users_ID(dto.JWT_id).Result)
            {
                await Insert_Report_Failed_JWT_History_Record(new Report_Failed_JWT_History
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Server_Port,
                    User_agent = dto.Client_User_Agent,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Location = dto.Location,
                    Login_type = dto.Login_type,
                    Client_time = dto.Client_Time_Parsed,
                    Reason = "JWT ID is Deleted or DNE",
                    Controller = dto.Controller,
                    Action = dto.Action,
                    End_User_ID = dto.JWT_id,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_client_address = dto.JWT_client_address,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb,
                    Token = dto.Token
                });
                return false;
            }

            if (dto.Client_id != 0 && !ID_Exists_In_Users_ID(dto.Client_id).Result)
            {
                await Insert_Report_Failed_Client_ID_History_Record(new Report_Failed_Client_ID_History
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Server_Port,
                    User_agent = dto.Client_User_Agent,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Location = dto.Location,
                    Login_type = dto.Login_type,
                    Client_time = dto.Client_Time_Parsed,
                    Reason = "Client ID is Deleted or DNE",
                    Controller = dto.Controller,
                    Action = dto.Action,
                    End_User_ID = dto.Client_id,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_height = dto.Screen_height,
                    Screen_width = dto.Screen_width,
                    RTT = dto.RTT,
                    Orientation = dto.Orientation,
                    Data_saver = dto.Data_saver,
                    Color_depth = dto.Color_depth,
                    Pixel_depth = dto.Pixel_depth,
                    Connection_type = dto.Connection_type,
                    Down_link = dto.Down_link,
                    Device_ram_gb = dto.Device_ram_gb,
                    Token = dto.Token,
                });
                return false;
            }

            return true;
        }
    }
}