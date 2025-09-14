using System.Dynamic;
using System.Text.Json;
using System.Text;
using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Selection;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using mpc_dotnetc_user_server.Models.Users.Selected.Alignment;
using mpc_dotnetc_user_server.Models.Users.Selected.Avatar;
using mpc_dotnetc_user_server.Models.Users.Selected.Language;
using mpc_dotnetc_user_server.Models.Users.Selected.Name;
using mpc_dotnetc_user_server.Models.Users.Selected.Navbar_Lock;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Models.Users.Selected.Password_Change;
using Microsoft.IdentityModel.Tokens;
using mpc_dotnetc_user_server.Models.Users.WebSocket_Chat;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Account_Type;
using mpc_dotnetc_user_server.Models.Users.Account_Roles;
using mpc_dotnetc_user_server.Models.Users.Account_Groups;
using mpc_dotnetc_user_server.Models.Users.Selected.Deactivate;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;

namespace mpc_dotnetc_user_server.Models.Users.Index
{
    public class Users_Repository : IUsers_Repository
    {
        private readonly ulong TimeStamp = Convert.ToUInt64(DateTimeOffset.Now.ToUnixTimeSeconds());

        private readonly Users_Database_Context _UsersDBC;
        private readonly Random random = new();
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
        )
        {
            _UsersDBC = Users_Database_Context;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Password = password;
        }

        public async Task<User_Data_DTO> Create_Account_By_Email(Complete_Email_RegistrationDTO dto)
        {

            string character_set = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string user_public_id = @$"{new string(Enumerable.Repeat("0123456789", 5).Select(s => s[random.Next(s.Length)]).ToArray())}";
            ulong clocked = TimeStamp;

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.User_IDsTbl.Count() + 1),
                Public_id = user_public_id,
                Secret_id = AES.Process_Encryption($@"
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}
                "),
                Created_by = 0,
                Created_on = clocked,
                Updated_on = clocked,
                Updated_by = 0
            };
            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, 1)
                .SetProperty(col => col.Deleted_on, clocked)
                .SetProperty(col => col.Updated_on, clocked)
                .SetProperty(col => col.Updated_by, ID_Record.ID)
                .SetProperty(col => col.Deleted_by, ID_Record.ID)
                .SetProperty(col => col.Client_time, dto.Client_time)
                .SetProperty(col => col.Server_Port, dto.Server_Port)
                .SetProperty(col => col.Server_IP, dto.Server_IP_Address)
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
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Completed_Email_RegistrationTbl.AddAsync(new Completed_Email_RegistrationTbl
            {
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = clocked,
                Updated_by = (ulong)0,
                Language_Region = @$"{dto.Language}-{dto.Region}",
                Created_on = clocked,
                Created_by = ID_Record.ID,
                Code = dto.Code,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP_Address,
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
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Selected_NameTbl.AddAsync(new Selected_NameTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Selected_NameTbl.Count() + 1),
                Name = $@"{dto.Name}",
                User_ID = ID_Record.ID,
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_Email_AddressTbl.AddAsync(new Login_Email_AddressTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_Email_AddressTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_PasswordTbl.AddAsync(new Password_ChangeTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_PasswordTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Password = Password.Process_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}{_Constants.JWT_SECURITY_KEY}")).Result,
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                User_ID = ID_Record.ID,
                Language_code = dto.Language,
                Region_code = dto.Region,
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Alignment(new Selected_App_AlignmentDTO
            {
                End_User_ID = ID_Record.ID,
                Alignment = dto.Alignment.ToString(),
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Nav_Lock(new Selected_Navbar_LockDTO
            {
                End_User_ID = ID_Record.ID,
                Locked = dto.Nav_lock.ToString(),
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_TextAlignment(new Selected_App_Text_AlignmentDTO
            {
                End_User_ID = ID_Record.ID,
                Text_alignment = dto.Text_alignment.ToString(),
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Theme(new Selected_ThemeDTO
            {
                End_User_ID = ID_Record.ID,
                Theme = dto.Theme.ToString()
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Account_Roles(new Account_RolesDTO
            {
                End_User_ID = ID_Record.ID,
                Roles = "User"
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Account_Groups(new Account_GroupsDTO
            {
                End_User_ID = ID_Record.ID,
                Groups = "0"
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Account_Type(new Account_TypeDTO
            {
                End_User_ID = ID_Record.ID,
                Type = 1
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Grid_Type(new Selected_App_Grid_TypeDTO
            {
                End_User_ID = ID_Record.ID,
                Grid = dto.Grid_type.ToString()
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Status(new Selected_StatusDTO
            {
                End_User_ID = ID_Record.ID,
                Online_status = 2.ToString(),
            });
            await _UsersDBC.SaveChangesAsync();

            string token = JWT.Create_Email_Account_Token(new JWT_DTO
            {
                End_User_ID = ID_Record.ID,
                User_groups = "0",
                User_roles = "User",
                Account_type = 1,
                Email_address = dto.Email_Address
            }).Result;

            await Update_End_User_Login_Time_Stamp(new Login_Time_StampDTO
            {
                End_User_ID = ID_Record.ID,
                Login_on = clocked,
                Client_Time_Parsed = dto.Client_time,
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP_Address = dto.Server_IP_Address,
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

            await Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_HistoryDTO
            {
                End_User_ID = ID_Record.ID,
                Login_on = clocked,
                Client_Time_Parsed = dto.Client_time,
                Location = dto.Location,
                Remote_Port = dto.Remote_Port,
                Remote_IP = dto.Remote_IP,
                Server_Port = dto.Server_Port,
                Server_IP_Address = dto.Server_IP_Address,
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
                created_on = clocked,
                login_on = clocked,
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
        public async Task<User_Data_DTO> Integrate_Account_By_Email(Complete_Email_RegistrationDTO dto)
        {
            ulong clocked = TimeStamp;

            await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                .SetProperty(col => col.Deleted, 1)
                .SetProperty(col => col.Deleted_on, clocked)
                .SetProperty(col => col.Updated_on, clocked)
                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                .SetProperty(col => col.Deleted_by, dto.End_User_ID)
                .SetProperty(col => col.Client_time, dto.Client_time)
                .SetProperty(col => col.Server_Port, dto.Server_Port)
                .SetProperty(col => col.Server_IP, dto.Server_IP_Address)
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
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Completed_Email_RegistrationTbl.AddAsync(new Completed_Email_RegistrationTbl
            {
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = clocked,
                Updated_by = (ulong)0,
                Language_Region = @$"{dto.Language}-{dto.Region}",
                Created_on = clocked,
                Created_by = dto.End_User_ID,
                Code = dto.Code,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP_Address,
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
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_Email_AddressTbl.AddAsync(new Login_Email_AddressTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_Email_AddressTbl.Count() + 1),
                User_ID = dto.End_User_ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = dto.End_User_ID,
                Updated_by = dto.End_User_ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Login_PasswordTbl.AddAsync(new Password_ChangeTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Login_PasswordTbl.Count() + 1),
                User_ID = dto.End_User_ID,
                Password = Password.Process_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}{_Constants.JWT_SECURITY_KEY}")).Result,
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = dto.End_User_ID,
                Updated_by = dto.End_User_ID,
            });
            await _UsersDBC.SaveChangesAsync();

            string time = clocked.ToString();

            return new User_Data_DTO
            {
                created_on = clocked,
                login_on = clocked,
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
        public async Task<User_Data_DTO> Create_Account_By_Twitch(Complete_Twitch_RegisterationDTO dto)
        {
            string character_set = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            string user_public_id = @$"{new string(Enumerable.Repeat("0123456789", 5).Select(s => s[random.Next(s.Length)]).ToArray())}";
            ulong clocked = TimeStamp;

            User_IDsTbl ID_Record = new User_IDsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.User_IDsTbl.Count() + 1),
                Public_id = user_public_id,
                Secret_id = AES.Process_Encryption($@"
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}-
                    {new string(Enumerable.Repeat(character_set, 64).Select(s => s[random.Next(s.Length)]).ToArray())}
                "),
                Created_by = 0,
                Created_on = clocked,
                Updated_on = clocked,
                Updated_by = 0
            };
            await _UsersDBC.User_IDsTbl.AddAsync(ID_Record);
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Twitch_IDsTbl.AddAsync(new Twitch_IDsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Twitch_IDsTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Twitch_ID = dto.Twitch_ID,
                User_Name = dto.Twitch_User_Name,
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Selected_NameTbl.AddAsync(new Selected_NameTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Selected_NameTbl.Count() + 1),
                Name = $@"{dto.Twitch_Name}",
                User_ID = ID_Record.ID,
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Twitch_Email_AddressTbl.AddAsync(new Twitch_Email_AddressTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Twitch_Email_AddressTbl.Count() + 1),
                User_ID = ID_Record.ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
            {
                User_ID = ID_Record.ID,
                Language_code = dto.Language,
                Region_code = dto.Region,
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = ID_Record.ID,
                Updated_by = ID_Record.ID,
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Alignment(new Selected_App_AlignmentDTO
            {
                End_User_ID = ID_Record.ID,
                Alignment = dto.Alignment.ToString(),
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Nav_Lock(new Selected_Navbar_LockDTO
            {
                End_User_ID = ID_Record.ID,
                Locked = dto.Nav_lock.ToString(),
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_TextAlignment(new Selected_App_Text_AlignmentDTO
            {
                End_User_ID = ID_Record.ID,
                Text_alignment = dto.Text_alignment.ToString(),
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Theme(new Selected_ThemeDTO
            {
                End_User_ID = ID_Record.ID,
                Theme = dto.Theme.ToString()
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Account_Roles(new Account_RolesDTO
            {
                End_User_ID = ID_Record.ID,
                Roles = "User"
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Account_Groups(new Account_GroupsDTO
            {
                End_User_ID = ID_Record.ID,
                Groups = "0"
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Account_Type(new Account_TypeDTO
            {
                End_User_ID = ID_Record.ID,
                Type = 1
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Grid_Type(new Selected_App_Grid_TypeDTO
            {
                End_User_ID = ID_Record.ID,
                Grid = dto.Grid_type.ToString()
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Selected_Status(new Selected_StatusDTO
            {
                End_User_ID = ID_Record.ID,
                Online_status = 2.ToString(),
            });
            await _UsersDBC.SaveChangesAsync();

            await Update_End_User_Login_Time_Stamp(new Login_Time_StampDTO
            {
                End_User_ID = ID_Record.ID,
                Login_on = clocked,
                Client_Time_Parsed = dto.Client_time,
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP_Address = dto.Server_IP_Address,
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
            await _UsersDBC.SaveChangesAsync();

            await Insert_End_User_Login_Time_Stamp_History(new Login_Time_Stamp_HistoryDTO
            {
                End_User_ID = ID_Record.ID,
                Login_on = clocked,
                Client_Time_Parsed = dto.Client_time,
                Location = dto.Location,
                Remote_Port = dto.Remote_Port,
                Remote_IP = dto.Remote_IP,
                Server_Port = dto.Server_Port,
                Server_IP_Address = dto.Server_IP_Address,
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
                created_on = clocked,
                login_on = clocked,
                location = dto.Location,
                login_type = "twitch",
            };
        }
        public async Task Integrate_Account_By_Twitch(Complete_Twitch_IntegrationDTO dto)
        {
            ulong clocked = TimeStamp;

            await _UsersDBC.Twitch_IDsTbl.AddAsync(new Twitch_IDsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Twitch_IDsTbl.Count() + 1),
                User_ID = dto.End_User_ID,
                Twitch_ID = dto.Twitch_ID,
                User_Name = dto.Twitch_User_Name,
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = dto.End_User_ID,
                Updated_by = dto.End_User_ID
            });
            await _UsersDBC.SaveChangesAsync();

            await _UsersDBC.Twitch_Email_AddressTbl.AddAsync(new Twitch_Email_AddressTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Twitch_Email_AddressTbl.Count() + 1),
                User_ID = dto.End_User_ID,
                Email_Address = dto.Email_Address.ToUpper(),
                Updated_on = clocked,
                Created_on = clocked,
                Created_by = dto.End_User_ID,
                Updated_by = dto.End_User_ID
            });
            await _UsersDBC.SaveChangesAsync();
        }

        public async Task<string> Create_Pending_Email_Registration_Record(Pending_Email_RegistrationDTO dto)
        {
            ulong clocked = TimeStamp;
            await _UsersDBC.Pending_Email_RegistrationTbl.AddAsync(new Pending_Email_RegistrationTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Pending_Email_RegistrationTbl.Count() + 1),
                Updated_on = clocked,
                Created_on = clocked,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP_Address,
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
                created_on = TimeStamp.ToString(),
            });
        }
        public async Task<bool> Email_Exists_In_Login_Email_AddressTbl(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Any(x => x.Email_Address == email_address.ToUpper()));
        }
        public async Task<bool> Email_Exists_In_Twitch_Email_AddressTbl(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Twitch_Email_AddressTbl.Any(x => x.Email_Address == email_address.ToUpper()));
        }
        public async Task<bool> Email_Exists_In_Discord_Email_AddressTbl(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Discord_Email_AddressTbl.Any(x => x.Email_Address == email_address.ToUpper()));
        }
        public async Task<bool> Create_Contact_Us_Record(Contact_UsDTO dto)
        {
            await _UsersDBC.Contact_UsTbl.AddAsync(new Contact_UsTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Contact_UsTbl.Count() + 1),
                USER_ID = dto.End_User_ID,
                Subject_Line = dto.Subject_line,
                Summary = dto.Summary,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_End_User_Status_Record(Selected_StatusDTO dto)
        {
            await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Selected_StatusTbl.Count() + 1),
                User_ID = dto.End_User_ID,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Created_by = dto.End_User_ID,
                Online = 1,
                Updated_by = dto.End_User_ID
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Website_Bug_Record(Reported_Website_BugDTO dto)
        {
            await _UsersDBC.Reported_Website_BugTbl.AddAsync(new Reported_Website_BugTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_Website_BugTbl.Count() + 1),
                USER_ID = dto.End_User_ID,
                URL = dto.URL,
                Detail = dto.Detail,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<string> Create_WebSocket_Log_Record(WebSocket_Chat_PermissionDTO dto)
        {
            try {
                await _UsersDBC.WebSocket_Chat_PermissionTbl.AddAsync(new WebSocket_Chat_PermissionTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.WebSocket_Chat_PermissionTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    Participant_ID = dto.Participant_ID,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = 0,
                    Requested = 1,
                    Approved = 0,
                    Blocked = 0
                });
                await _UsersDBC.SaveChangesAsync();

                return await Task.FromResult(JsonSerializer.Serialize(new{
                    updated_on = TimeStamp,
                    updated_by = dto.End_User_ID,
                    updated_for = dto.User
                }));
            } catch {
                return "Server Error: WebSocket Log Record"; 
            }
        }
        public async Task<bool> Create_Discord_Bot_Bug_Record(Reported_Discord_Bot_BugDTO dto)
        {
            await _UsersDBC.Reported_Discord_Bot_BugTbl.AddAsync(new Reported_Discord_Bot_BugTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_Discord_Bot_BugTbl.Count() + 1),
                USER_ID = dto.End_User_ID,
                Location = dto.Location,
                Detail = dto.Detail,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Comment_Box_Record(Comment_BoxDTO dto)
        {
            await _UsersDBC.Comment_BoxTbl.AddAsync(new Comment_BoxTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Comment_BoxTbl.Count() + 1),
                USER_ID = dto.End_User_ID,
                Comment = dto.Comment,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<bool> Create_Broken_Link_Record(Reported_Broken_LinkDTO dto)
        {
            await _UsersDBC.Reported_Broken_LinkTbl.AddAsync(new Reported_Broken_LinkTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_Broken_LinkTbl.Count() + 1),
                USER_ID = dto.End_User_ID,
                URL = dto.URL,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = 0
            });
            await _UsersDBC.SaveChangesAsync();
            return true;
        }
        public async Task<string> Create_Reported_User_Profile_Record(Reported_ProfileDTO dto)
        {
            Reported_ProfileTbl record = new Reported_ProfileTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.Reported_ProfileTbl.Count() + 1),
                USER_ID = dto.End_User_ID,
                Reported_ID = dto.Reported_ID,
                Page_Title = _UsersDBC.Profile_PageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.Page_Title).SingleOrDefault(),
                Page_Description = _UsersDBC.Profile_PageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.Page_Description).SingleOrDefault(),
                About_Me = _UsersDBC.Profile_PageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.About_Me).SingleOrDefault(),
                Banner_URL = _UsersDBC.Profile_PageTbl.Where(x => x.User_ID == dto.Reported_ID).Select(x => x.Banner_URL).SingleOrDefault(),
                Reported_Reason = dto.Reported_Reason,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = ulong.Parse(dto.ID)
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
        public async Task<string> Delete_Account_By_User_id(Delete_UserDTO dto)
        {
            await _UsersDBC.User_IDsTbl.Where(x => x.ID == ulong.Parse(dto.Target_User)).ExecuteUpdateAsync(s => s
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted, 1)
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_by, ulong.Parse(dto.ID))
                .SetProperty(User_IDsTbl => User_IDsTbl.Deleted_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Created_on, TimeStamp)
                .SetProperty(User_IDsTbl => User_IDsTbl.Updated_by, ulong.Parse(dto.ID))
            );
            await _UsersDBC.SaveChangesAsync();
            return JsonSerializer.Serialize(new
            {
                deleted_by = dto.ID,
                target_user = dto.Target_User,
            });
        }
        public async Task<User_Data_DTO> Read_User_Data_By_ID(ulong end_user_id)
        {
            bool nav_lock = _UsersDBC.Selected_Navbar_LockTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Locked).SingleOrDefault();
            byte account_type = _UsersDBC.Account_TypeTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Type).SingleOrDefault();
            byte grid_type = _UsersDBC.Selected_App_Grid_TypeTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Grid).SingleOrDefault();
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == end_user_id).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Custom).SingleOrDefault();
            byte status_type = 0;
            byte light = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Light).SingleOrDefault();
            byte night = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Night).SingleOrDefault();
            byte custom_theme = _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Custom).SingleOrDefault();
            byte theme_type = 0;
            byte left_aligned = _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Left).SingleOrDefault();
            byte center_aligned = _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Center).SingleOrDefault();
            byte right_aligned = _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Right).SingleOrDefault();
            byte alignment_type = 0;
            byte left_text_aligned = _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Left).SingleOrDefault();
            byte center_text_aligned = _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Center).SingleOrDefault();
            byte right_text_aligned = _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Right).SingleOrDefault();
            byte text_alignment_type = 0;
            //Third Party IDs
            ulong? twitch_user_id = _UsersDBC.Twitch_IDsTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Twitch_ID).SingleOrDefault();
            string? twitch_user_name = _UsersDBC.Twitch_IDsTbl.Where(x => x.User_ID == end_user_id).Select(x => x.User_Name).SingleOrDefault();
            ulong? discord_user_id = _UsersDBC.Discord_IDsTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Discord_ID).SingleOrDefault();
            ulong login_timestamp = _UsersDBC.Login_Time_StampTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Login_on).SingleOrDefault();
            ulong logout_timestamp = _UsersDBC.Logout_Time_StampTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Logout_on).SingleOrDefault();
            ulong created_on = _UsersDBC.User_IDsTbl.Where(x => x.ID == end_user_id).Select(x => x.Created_on).SingleOrDefault();
            byte? gender = _UsersDBC.IdentityTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Gender).SingleOrDefault();
            byte? birth_day = _UsersDBC.Birth_DateTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Day).SingleOrDefault();
            byte? birth_month = _UsersDBC.Birth_DateTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Month).SingleOrDefault();
            ulong? birth_year = _UsersDBC.Birth_DateTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Year).SingleOrDefault();
            string? customLbl = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Custom_lbl).SingleOrDefault();
            //Email Addresses
            string? email_address = _UsersDBC.Login_Email_AddressTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? twitch_email_address = _UsersDBC.Twitch_Email_AddressTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? discord_email_address = _UsersDBC.Discord_Email_AddressTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();

            string? region_code = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Region_code).SingleOrDefault();
            string? language_code = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Language_code).SingleOrDefault();
            string? avatar_url_path = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? avatar_title = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Avatar_title).SingleOrDefault();
            string? name = _UsersDBC.Selected_NameTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Name).SingleOrDefault();
            string? end_user_public_id = _UsersDBC.User_IDsTbl.Where(x => x.ID == end_user_id).Select(x => x.Public_id).SingleOrDefault();
            string? first_name = _UsersDBC.IdentityTbl.Where(x => x.User_ID == end_user_id).Select(x => x.First_Name).SingleOrDefault();
            string? last_name = _UsersDBC.IdentityTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Last_Name).SingleOrDefault();
            string? middle_name = _UsersDBC.IdentityTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Middle_Name).SingleOrDefault();
            string? maiden_name = _UsersDBC.IdentityTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Maiden_Name).SingleOrDefault();
            string? ethnicity = _UsersDBC.IdentityTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Ethnicity).SingleOrDefault();
            string? groups = _UsersDBC.Account_GroupsTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Groups).SingleOrDefault();
            string? roles = _UsersDBC.Account_RolesTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Roles).SingleOrDefault();
            string? card_border_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Border_Color).SingleOrDefault();
            string? card_header_font = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Header_Font).SingleOrDefault();
            string? card_header_font_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Header_Font_Color).SingleOrDefault();
            string? card_header_background_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Header_Background_Color).SingleOrDefault();
            string? card_body_font = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Body_Font).SingleOrDefault();
            string? card_body_background_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Body_Background_Color).SingleOrDefault();
            string? card_body_font_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Body_Font_Color).SingleOrDefault();
            string? card_footer_font_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Footer_Font_Color).SingleOrDefault();
            string? card_footer_font = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Footer_Font).SingleOrDefault();
            string? card_footer_background_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Card_Footer_Background_Color).SingleOrDefault();
            string? navigation_menu_background_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Navigation_Menu_Background_Color).SingleOrDefault();
            string? navigation_menu_font_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Navigation_Menu_Font_Color).SingleOrDefault();
            string? navigation_menu_font = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Navigation_Menu_Font).SingleOrDefault();
            string? button_background_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Button_Background_Color).SingleOrDefault();
            string? button_font = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Button_Font).SingleOrDefault();
            string? button_font_color = _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Button_Font_Color).SingleOrDefault();

            if (status_offline == 1)
                status_type = 0;
            if (status_hidden == 1)
                status_type = 1;
            if (status_online == 1)
                status_type = 2;
            if (status_away == 1)
                status_type = 3;
            if (status_dnd == 1)
                status_type = 4;
            if (status_custom == 1)
                status_type = 5;
            if (light == 1)
                theme_type = 0;
            if (night == 1)
                theme_type = 1;
            if (custom_theme == 1)
                theme_type = 2;
            if (left_aligned == 1)
                alignment_type = 0;
            if (center_aligned == 1)
                alignment_type = 1;
            if (right_aligned == 1)
                alignment_type = 2;
            if (left_text_aligned == 1)
                text_alignment_type = 0;
            if (center_text_aligned == 1)
                text_alignment_type = 1;
            if (right_text_aligned == 1)
                text_alignment_type = 2;

            return await Task.FromResult(new User_Data_DTO
            {
                id = end_user_id,
                account_type = account_type,
                email_address = email_address,
                twitch_id = twitch_user_id,
                twitch_user_name = twitch_user_name,
                discord_id = discord_user_id,
                twitch_email_address = twitch_email_address,
                discord_email_address = discord_email_address,
                name = $@"{name}#{end_user_public_id}",
                login_on = login_timestamp,
                logout_on = logout_timestamp,
                current_language = $@"{language_code}-{region_code}",
                language = language_code,
                region = region_code,
                online_status = status_type,
                custom_lbl = customLbl,
                created_on = created_on,
                avatar_url_path = avatar_url_path,
                avatar_title = avatar_title,
                theme = theme_type,
                alignment = alignment_type,
                text_alignment = text_alignment_type,
                gender = gender,
                birth_day = birth_day,
                birth_month = birth_month,
                birth_year = birth_year,
                first_name = first_name,
                last_name = last_name,
                middle_name = middle_name,
                maiden_name = maiden_name,
                ethnicity = ethnicity,
                groups = groups,
                roles = roles,
                grid_type = grid_type,
                nav_lock = nav_lock,
                card_border_color = card_border_color,
                card_header_font = card_header_font,
                card_header_font_color = card_header_font_color,
                card_header_background_color = card_header_background_color,
                card_body_font = card_body_font,
                card_body_background_color = card_body_background_color,
                card_body_font_color = card_body_font_color,
                card_footer_font_color = card_footer_font_color,
                card_footer_font = card_footer_font,
                card_footer_background_color = card_footer_background_color,
                navigation_menu_background_color = navigation_menu_background_color,
                navigation_menu_font_color = navigation_menu_font_color,
                navigation_menu_font = navigation_menu_font,
                button_background_color = button_background_color,
                button_font = button_font,
                button_font_color = button_font_color
            });
        }

        public async Task<User_Token_Data_DTO> Read_Require_Token_Data_By_ID(ulong end_user_id)
        {
            string? email_address = _UsersDBC.Login_Email_AddressTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? twitch_email_address = _UsersDBC.Twitch_Email_AddressTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? discord_email_address = _UsersDBC.Discord_Email_AddressTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Email_Address).SingleOrDefault();
            ulong? twitch_id = _UsersDBC.Twitch_IDsTbl.Where(x => x.User_ID == end_user_id).Select(x => x.User_ID).SingleOrDefault();
            ulong? discord_id = _UsersDBC.Discord_IDsTbl.Where(x => x.User_ID == end_user_id).Select(x => x.User_ID).SingleOrDefault();
            string? groups = _UsersDBC.Account_GroupsTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Groups).SingleOrDefault();
            string? roles = _UsersDBC.Account_RolesTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Roles).SingleOrDefault();
            byte account_type = _UsersDBC.Account_TypeTbl.Where(x => x.User_ID == end_user_id).Select(x => x.Type).SingleOrDefault();

            return await Task.FromResult(new User_Token_Data_DTO
            {
                id = end_user_id,
                account_type = account_type,
                email_address = email_address ?? "",
                twitch_id = twitch_id,
                twitch_email_address = twitch_email_address ?? "",
                discord_id = discord_id,
                discord_email_address= discord_email_address ?? "",
                groups = groups ?? "",
                roles = roles ?? ""
            });
        }

        public async Task<string> Read_User_Profile_By_ID(ulong user_id)
        {
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == user_id).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == user_id).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == user_id).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == user_id).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == user_id).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == user_id).Select(x => x.Custom).SingleOrDefault();
            string? custom_label = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == user_id).Select(x => x.Custom_lbl).SingleOrDefault();
            byte status_code = 0;

            ulong LoginTS = _UsersDBC.Login_Time_StampTbl.Where(x => x.User_ID == user_id).Select(x => x.Login_on).SingleOrDefault();
            ulong LogoutTS = _UsersDBC.Logout_Time_StampTbl.Where(x => x.User_ID == user_id).Select(x => x.Logout_on).SingleOrDefault();
            ulong created_on = _UsersDBC.User_IDsTbl.Where(x => x.ID == user_id).Select(x => x.Created_on).SingleOrDefault();

            string? email_address = _UsersDBC.Login_Email_AddressTbl.Where(x => x.User_ID == user_id).Select(x => x.Email_Address).SingleOrDefault();
            string? region_code = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == user_id).Select(x => x.Region_code).SingleOrDefault();
            string? language_code = _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == user_id).Select(x => x.Language_code).SingleOrDefault();
            string? avatar_url_path = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == user_id).Select(x => x.Avatar_url_path).SingleOrDefault();
            string? avatar_title = _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == user_id).Select(x => x.Avatar_title).SingleOrDefault();
            string? name = _UsersDBC.Selected_NameTbl.Where(x => x.User_ID == user_id).Select(x => x.Name).SingleOrDefault();

            if (status_offline == 1)
                status_code = 0;
            if (status_hidden == 1)
                status_code = 1;
            if (status_online == 1)
                status_code = 2;
            if (status_away == 1)
                status_code = 3;
            if (status_dnd == 1)
                status_code = 4;
            if (status_custom == 1)
                status_code = 5;

            return await Task.FromResult(JsonSerializer.Serialize(new {
                id = user_id,
                email_address = email_address,
                name = name,
                login_on = LoginTS,
                logout_on = LogoutTS,
                language = @$"{language_code}-{region_code}",
                online_status = status_code,
                custom_lbl = custom_label,
                created_on = created_on,
                avatar_url_path = avatar_url_path,
                avatar_title = avatar_title,
            }));
        }
        public async Task<string> Read_WebSocket_Permission_Record_For_Both_End_Users(WebSocket_Chat_PermissionDTO dto)
        {
            byte requested = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Requested).SingleOrDefault();
            byte approved = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Approved).SingleOrDefault();
            byte blocked = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Blocked).SingleOrDefault();
            bool deleted = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.End_User_ID && x.Participant_ID == dto.Participant_ID).Select(x => x.Deleted).SingleOrDefault();

            byte requested_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Requested).SingleOrDefault();
            byte approved_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Approved).SingleOrDefault();
            byte blocked_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Blocked).SingleOrDefault();
            bool deleted_swap_ids = _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.Participant_ID && x.Participant_ID == dto.End_User_ID).Select(x => x.Deleted).SingleOrDefault();


            if (requested == 1 || requested_swap_ids == 1)
            {
                return JsonSerializer.Serialize(new
                {
                    requested = 1,
                    blocked = 0,
                    approved = 0,
                });
            }

            if (approved == 1 || approved_swap_ids == 1)
            {
                return JsonSerializer.Serialize(new
                {
                    requested = 0,
                    blocked = 0,
                    approved = 1,
                });
            }

            if (blocked == 1 || blocked_swap_ids == 1)
            {
                return JsonSerializer.Serialize(new
                {
                    requested = 0,
                    blocked = 1,
                    approved = 0,
                });
            }

            if (requested == 0 && approved == 0 && blocked == 0 && requested_swap_ids == 0 && approved_swap_ids == 0 && blocked_swap_ids == 0 && deleted == false && deleted_swap_ids == false)
            {
                await Create_WebSocket_Permission_Record(dto);
                return JsonSerializer.Serialize(new
                {
                    requested = 1,
                    blocked = 0,
                    approved = 0,
                });
            }

            if (requested == 0 && approved == 0 && blocked == 0 && requested_swap_ids == 0 && approved_swap_ids == 0 && blocked_swap_ids == 0 && (deleted == true || deleted_swap_ids == true))
            {
                await Update_Chat_Web_Socket_Permissions_Tbl(new WebSocket_Chat_PermissionTbl
                {
                    User_ID = dto.End_User_ID,
                    Participant_ID = dto.Participant_ID,
                    Updated_on = TimeStamp,
                    Updated_by = dto.End_User_ID,
                    Requested = 1,
                    Blocked = 0,
                    Approved = 0
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
        public async Task<string> Read_All_End_User_WebSocket_Sent_Chat_Requests(ulong user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.User_ID == user_id)) {
                return "";
            } else {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == user_id && x.Requested == 1)
                    .ToList()));
            }
        }
        public async Task<string> Read_All_End_User_WebSocket_Sent_Chat_Blocks(ulong user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.User_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == user_id && x.Blocked == 1)
                    .ToList()));
            }
        }
        public async Task<string> Read_All_End_User_WebSocket_Sent_Chat_Approvals(ulong user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.User_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == user_id && x.Approved == 1)
                    .ToList()));
            }
        }
        public async Task<string> Read_All_End_User_WebSocket_Received_Chat_Requests(ulong user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Requested == 1)
                    .ToList()));
            }
        }
        public async Task<string> Read_All_End_User_WebSocket_Received_Chat_Blocks(ulong user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Blocked == 1)
                    .ToList()));
            }
        }
        public async Task<string> Read_All_End_User_WebSocket_Received_Chat_Approvals(ulong user_id)
        {
            if (!_UsersDBC.WebSocket_Chat_PermissionTbl.Any(x => x.Participant_ID == user_id))
            {
                return "";
            }
            else
            {
                return await Task.FromResult(JsonSerializer.Serialize(
                    _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.Participant_ID == user_id && x.Approved == 1)
                    .ToList()));
            }
        }
        public async Task<byte> Read_End_User_Selected_Status(Selected_StatusDTO dto)
        {
            byte status_online = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).Select(x => x.Online).SingleOrDefault();
            byte status_offline = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).Select(x => x.Offline).SingleOrDefault();
            byte status_hidden = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).Select(x => x.Hidden).SingleOrDefault();
            byte status_away = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).Select(x => x.Away).SingleOrDefault();
            byte status_dnd = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).Select(x => x.DND).SingleOrDefault();
            byte status_custom = _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).Select(x => x.Custom).SingleOrDefault();
            byte Status = 0;
            if (status_offline == 1)
                Status = 0;
            if (status_hidden == 1)
                Status = 1;
            if (status_online == 1)
                Status = 2;
            if (status_away == 1)
                Status = 3;
            if (status_dnd == 1)
                Status = 4;
            if (status_custom == 1)
                Status = 5;
            return await Task.FromResult(Status);
        }
        public async Task<string> Create_Reported_WebSocket_Records(Reported_WebSocketDTO dto)
        {
            try
            {
                switch (dto.Report_type.ToUpper())
                {
                    case "THREAT":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Threat = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Threat = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong threat_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Threat).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Threat, (threat_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            threat_record_created_on = TimeStamp,
                        });
                    case "SPAM":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Spam = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Spam = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong spam_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Spam).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Spam, (spam_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            spam_record_created_on = TimeStamp,
                        });
                    case "BLOCK":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Block = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Block = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong block_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Block).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Block, (block_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            block_record_created_on = TimeStamp,
                        });
                    case "ABUSE":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Abuse = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Abuse = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong abuse_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Abuse).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Abuse, (abuse_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            abuse_record_created_on = TimeStamp,
                        });
                    case "MISINFORM":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Misinform = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Misinform = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong misinform_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Misinform).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Misinform, (misinform_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            misinform_record_created_on = TimeStamp,
                        });
                    case "HARASS":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Harass = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Harass = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong harass_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Harass).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Harass, (harass_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            harass_record_created_on = TimeStamp,
                        });
                    case "FAKE":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Fake = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Fake = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong fake_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Fake).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Fake, (fake_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            fake_account_record_created_on = TimeStamp,
                        });
                    case "HATE":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Hate = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Hate = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong hate_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Nudity).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Hate, (hate_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            hate_record_created_on = TimeStamp,
                        });
                    case "NUDITY":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Nudity = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Nudity = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong nudity_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Nudity).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Nudity, (nudity_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            nudity_record_created_on = TimeStamp,
                        });
                    case "VIOLENCE":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Violence = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Violence = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong violence_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Violence).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Violence, (violence_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            violence_record_created_on = TimeStamp,
                        });
                    case "ILLEGAL":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Illegal = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Illegal = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong illegal_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Illegal).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Illegal, (illegal_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            illegal_record_created_on = TimeStamp,
                        });
                    case "SELF_HARM":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Self_harm = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Self_harm = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong self_harm_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Self_harm).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Self_harm, (self_harm_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            self_harm_record_created_on = TimeStamp,
                        });
                    case "DISRUPTION":
                        await _UsersDBC.Reported_WebSocket_HistoryTbl.AddAsync(new Reported_WebSocket_HistoryTbl
                        {
                            User_ID = dto.Participant_ID,
                            Participant_ID = dto.End_User_ID,
                            Updated_on = TimeStamp,
                            Updated_by = dto.End_User_ID,
                            Disruption = 1,
                            Created_on = TimeStamp,
                            Created_by = dto.End_User_ID,
                        });

                        if (!_UsersDBC.Reported_WebSocketTbl.Any(x => x.User_ID == dto.Participant_ID))
                        {
                            await _UsersDBC.Reported_WebSocketTbl.AddAsync(new Reported_WebSocketTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Reported_WebSocketTbl.Count() + 1),
                                User_ID = dto.Participant_ID,
                                Disruption = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID,
                                Created_by = dto.End_User_ID
                            });
                        }
                        else
                        {
                            ulong disruption_count = _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).Select(x => x.Disruption).SingleOrDefault();
                            await _UsersDBC.Reported_WebSocketTbl.Where(x => x.User_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Disruption, (disruption_count + 1))
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }

                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            reported = dto.Participant_ID,
                            disruption_record_created_on = TimeStamp,
                        });
                    default:
                        return "Server Error: Report Record Selection Failed.";
                }
            } catch {
                return "Server Error: Report Record Creation Failed."; 
            }
        }
        public async Task<string> Update_Chat_Web_Socket_Permissions_Tbl(WebSocket_Chat_PermissionTbl dto)
        {
            try
            {
                if (_UsersDBC.WebSocket_Chat_PermissionTbl.Any((x) => x.User_ID == dto.User_ID && x.Participant_ID == dto.Participant_ID))
                {
                    await _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.User_ID && x.Participant_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                            .SetProperty(dto => dto.Requested, dto.Requested)
                            .SetProperty(dto => dto.Blocked, dto.Blocked)
                            .SetProperty(dto => dto.Approved, dto.Approved)
                            .SetProperty(dto => dto.Updated_on, TimeStamp)
                            .SetProperty(dto => dto.Deleted, false)
                            .SetProperty(dto => dto.Updated_by, dto.Participant_ID)
                        );
                    await _UsersDBC.SaveChangesAsync();
                    return JsonSerializer.Serialize(new
                    {
                        id = dto.User_ID,
                        participant_id = dto.Participant_ID,
                        updated_on = TimeStamp,
                        updated_by = dto.Participant_ID
                    });
                }
                else if (_UsersDBC.WebSocket_Chat_PermissionTbl.Any((x) => x.User_ID == dto.Participant_ID && x.Participant_ID == dto.User_ID))
                {
                    await _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.Participant_ID && x.Participant_ID == dto.User_ID).ExecuteUpdateAsync(s => s
                            .SetProperty(dto => dto.Requested, dto.Requested)
                            .SetProperty(dto => dto.Blocked, dto.Blocked)
                            .SetProperty(dto => dto.Approved, dto.Approved)
                            .SetProperty(dto => dto.Updated_on, TimeStamp)
                            .SetProperty(dto => dto.Deleted, false)
                            .SetProperty(dto => dto.Updated_by, dto.User_ID)
                        );
                    await _UsersDBC.SaveChangesAsync();
                    return JsonSerializer.Serialize(new
                    {
                        id = dto.User_ID,
                        participant_id = dto.Participant_ID,
                        updated_on = TimeStamp,
                        updated_by = dto.User_ID
                    });
                }
                return "Server Error: Update Chat Permission Selection Failed.";
            }
            catch {
                return "Server Error: Update Chat Permissions Failed."; 
            }
        }
        public async Task<string> Delete_Chat_Web_Socket_Permissions_Tbl(WebSocket_Chat_PermissionTbl dto)
        {
            try {
                ulong clock = TimeStamp;
                await _UsersDBC.WebSocket_Chat_PermissionTbl.Where(x => x.User_ID == dto.User_ID && x.Participant_ID == dto.Participant_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(dto => dto.Requested, dto.Requested)
                    .SetProperty(dto => dto.Blocked, dto.Blocked)
                    .SetProperty(dto => dto.Approved, dto.Approved)
                    .SetProperty(dto => dto.Deleted_on, clock)
                    .SetProperty(dto => dto.Deleted_by, dto.Participant_ID)
                    .SetProperty(dto => dto.Deleted, true)
                    .SetProperty(dto => dto.Updated_on, clock)
                    .SetProperty(dto => dto.Updated_by, dto.Participant_ID)
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new { 
                    id = dto.User_ID,
                    participant_id = dto.Participant_ID,
                    deleted_on = clock
                });
            }
            catch
            {
                return "Server Error: Delete Chat Permissions Failed.";
            }
        }
        public async Task<string> Insert_Pending_Email_Registration_History_Record(Pending_Email_Registration_HistoryDTO dto)
        {
            try
            {
                await _UsersDBC.Pending_Email_Registration_HistoryTbl.AddAsync(new Pending_Email_Registration_HistoryTbl
                {
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Language_Region = $"{dto.Language}-{dto.Region}",
                    Email_Address = dto.Email_Address,
                    Location = dto.Location,
                    Client_time = dto.Client_Time_Parsed,
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
                    language = dto.Language,
                    region = dto.Region,
                    updated_on = TimeStamp.ToString(),
                });
            } catch {
                return "Server Error: Email Address Registration Failed"; 
            }
        }
        public async Task<string> Update_Pending_Email_Registration_Record(Pending_Email_RegistrationDTO dto)
        {
            try {
                await _UsersDBC.Pending_Email_RegistrationTbl.Where(x => x.Email_Address == dto.Email_Address).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Email_Address, dto.Email_Address)
                    .SetProperty(col => col.Code, dto.Code)
                    .SetProperty(col => col.Language_Region, @$"{dto.Language}-{dto.Region}")
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, (ulong)0)
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    email_address = dto.Email_Address,
                    language = dto.Language,
                    region = dto.Region,
                    updated_on = TimeStamp.ToString(),
                });
            } catch {
                return "Server Error: Email Address Registration Failed";
            }
        }
        public async Task<string> Update_End_User_Avatar(Selected_AvatarDTO dto)
        {
            try {
                if (!_UsersDBC.Selected_AvatarTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_AvatarTbl.AddAsync(new Selected_AvatarTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_AvatarTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Avatar_url_path = dto.Avatar_url_path,
                        Updated_by = dto.End_User_ID
                    });
                } else { 
                    await _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Avatar_url_path, dto.Avatar_url_path));
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    avatar_url_path = dto.Avatar_url_path
                });
            } catch {
                return "Server Error: Update Avatar Failed."; 
            }
        }
        public async Task<string> Update_End_User_Avatar_Title(Selected_Avatar_TitleDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_AvatarTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_AvatarTbl.AddAsync(new Selected_AvatarTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_AvatarTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Avatar_title = dto.Avatar_title,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                { 
                    await _UsersDBC.Selected_AvatarTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Avatar_title, dto.Avatar_title));
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    avatar_title = dto.Avatar_title
                });
            } catch {
                return "Server Error: Update Avatar Failed."; 
            }

        }
        public async Task<string> Update_End_User_Name(Selected_NameDTO dto)
        {
            try {
                await _UsersDBC.Selected_NameTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Name, dto.Name)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    .SetProperty(col => col.Updated_on, TimeStamp)
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
        public async Task<string> Update_End_User_Selected_Alignment(Selected_App_AlignmentDTO dto)
        {
            try {
                switch (byte.Parse(dto.Alignment))
                {
                    case 0:
                        try {
                            if (!_UsersDBC.Selected_App_AlignmentTbl.Any(x => x.User_ID == dto.End_User_ID))
                            {
                                await _UsersDBC.Selected_App_AlignmentTbl.AddAsync(new Selected_App_AlignmentTbl
                                {
                                    ID = Convert.ToUInt64(_UsersDBC.Selected_App_AlignmentTbl.Count() + 1),
                                    User_ID = dto.End_User_ID,
                                    Updated_on = TimeStamp,
                                    Created_on = TimeStamp,
                                    Left = 1,
                                    Updated_by = dto.End_User_ID
                                });
                            }
                            else
                            {
                                await _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                    .SetProperty(col => col.Left, 1)
                                    .SetProperty(col => col.Center, 0)
                                    .SetProperty(col => col.Right, 0)
                                    .SetProperty(col => col.Updated_on, TimeStamp)
                                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                );
                            }
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                alignment = 0
                            });
                        } catch {
                            return "Server Error: Update User Alignment 0 Failed.";
                        }
                    case 2:
                        try {
                            if (!_UsersDBC.Selected_App_AlignmentTbl.Any(x => x.User_ID == dto.End_User_ID))
                            {
                                await _UsersDBC.Selected_App_AlignmentTbl.AddAsync(new Selected_App_AlignmentTbl
                                {
                                    ID = Convert.ToUInt64(_UsersDBC.Selected_App_AlignmentTbl.Count() + 1),
                                    User_ID = dto.End_User_ID,
                                    Right = 1,
                                    Updated_on = TimeStamp,
                                    Created_on = TimeStamp,
                                    Updated_by = dto.End_User_ID
                                });
                            }
                            else
                            {
                                await _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                    .SetProperty(col => col.Left, 0)
                                    .SetProperty(col => col.Center, 0)
                                    .SetProperty(col => col.Right, 1)
                                    .SetProperty(col => col.Updated_on, TimeStamp)
                                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                );
                            }
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                alignment = 2
                            });
                        } catch {
                            return "Server Error: Update User Alignment 2 Failed.";
                        }
                    case 1:
                        try
                        {
                            if (!_UsersDBC.Selected_App_AlignmentTbl.Any(x => x.User_ID == dto.End_User_ID))
                            {
                                await _UsersDBC.Selected_App_AlignmentTbl.AddAsync(new Selected_App_AlignmentTbl
                                {
                                    ID = Convert.ToUInt64(_UsersDBC.Selected_App_AlignmentTbl.Count() + 1),
                                    User_ID = dto.End_User_ID,
                                    Center = 1,
                                    Updated_on = TimeStamp,
                                    Created_on = TimeStamp,
                                    Updated_by = dto.End_User_ID
                                });
                            }
                            else
                            {
                                await _UsersDBC.Selected_App_AlignmentTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                    .SetProperty(col => col.Left, 0)
                                    .SetProperty(col => col.Center, 1)
                                    .SetProperty(col => col.Right, 0)
                                    .SetProperty(col => col.Updated_on, TimeStamp)
                                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                );
                            }
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                alignment = 1
                            });
                        }
                        catch
                        {
                            return "Server Error: Update User Alignment 1 Failed.";
                        }
                    default:
                        return "Server Error: Update User Alignment Selection Failed.";

                }
            } catch {
                return "Server Error: Update Alignment Failed.";
                
            }
        }
        public async Task<string> Update_End_User_Selected_TextAlignment(Selected_App_Text_AlignmentDTO dto)
        {
            try {
                switch (byte.Parse(dto.Text_alignment))
                {
                    case 0:
                        if (!_UsersDBC.Selected_App_Text_AlignmentTbl.Any(x => x.User_ID == dto.End_User_ID))
                        {
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.AddAsync(new Selected_App_Text_AlignmentTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_App_Text_AlignmentTbl.Count() + 1),
                                User_ID = dto.End_User_ID,
                                Left = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID
                            });
                        }
                        else
                        { 
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Left, 1)
                                .SetProperty(col => col.Center, 0)
                                .SetProperty(col => col.Right, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            text_alignment = dto.Text_alignment
                        });
                    case 2:
                        if (!_UsersDBC.Selected_App_Text_AlignmentTbl.Any(x => x.User_ID == dto.End_User_ID))
                        {
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.AddAsync(new Selected_App_Text_AlignmentTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_App_Text_AlignmentTbl.Count() + 1),
                                User_ID = dto.End_User_ID,
                                Right = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID
                            });
                        }
                        else
                        { 
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Left, 0)
                                .SetProperty(col => col.Center, 0)
                                .SetProperty(col => col.Right, 1)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            text_alignment = dto.Text_alignment
                        });
                    case 1:
                        if (!_UsersDBC.Selected_App_Text_AlignmentTbl.Any(x => x.User_ID == dto.End_User_ID))
                        {
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.AddAsync(new Selected_App_Text_AlignmentTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_App_Text_AlignmentTbl.Count() + 1),
                                User_ID = dto.End_User_ID,
                                Center = 1,
                                Updated_on = TimeStamp,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID
                            });
                        }
                        else
                        { 
                            await _UsersDBC.Selected_App_Text_AlignmentTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Left, 0)
                                .SetProperty(col => col.Center, 1)
                                .SetProperty(col => col.Right, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            text_alignment = dto.Text_alignment
                        });
                    default:
                        return "Server Error: Invalid Update Text Alignment.";
                }
            } catch {
                return "Server Error: Update Text Alignment Selection Failed.";
            }
        }
        public async Task<string> Update_End_User_Account_Type(Account_TypeDTO dto)
        {
            try {
                if (!_UsersDBC.Account_TypeTbl.Any(x => x.User_ID == dto.End_User_ID)) {
                    await _UsersDBC.Account_TypeTbl.AddAsync(new Account_TypeTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Account_TypeTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Type = dto.Type,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    await _UsersDBC.Account_TypeTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Type, dto.Type)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    account_type = dto.Type
                });
            } catch {
                return "Server Error: Update Text Alignment Failed."; 
            }
        }
        public async Task<string> Update_End_User_Selected_Grid_Type(Selected_App_Grid_TypeDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Grid_TypeTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Grid_TypeTbl.AddAsync(new Selected_App_Grid_TypeTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Grid_TypeTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Grid = byte.Parse(dto.Grid),
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Grid_TypeTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Grid, byte.Parse(dto.Grid))
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    grid = dto.Grid
                });
            } catch {
                return "Server Error: Update Text Alignment Failed.";            
            }
        }
        public async Task<string> Update_End_User_Selected_Language(Selected_LanguageDTO dto)
        {
            try {
                if (!_UsersDBC.Selected_LanguageTbl.Any((x => x.User_ID == dto.End_User_ID)))
                {
                    await _UsersDBC.Selected_LanguageTbl.AddAsync(new Selected_LanguageTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_LanguageTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID
                    });
                } else {
                    await _UsersDBC.Selected_LanguageTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Language_code, dto.Language)
                        .SetProperty(col => col.Region_code, dto.Region)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    region = dto.Region,
                    language = dto.Language
                });
            } catch {
                return "Server Error: Update Text Alignment Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Nav_Lock(Selected_Navbar_LockDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_Navbar_LockTbl.Any((x => x.User_ID == dto.End_User_ID)))
                {
                    await _UsersDBC.Selected_Navbar_LockTbl.AddAsync(new Selected_Navbar_LockTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_Navbar_LockTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Locked = bool.Parse(dto.Locked)
                    });
                }
                else
                {
                    await _UsersDBC.Selected_Navbar_LockTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Locked, bool.Parse(dto.Locked))
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    locked = dto.Locked
                });
            } catch {
                return "Server Error: Update Text Alignment Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Status(Selected_StatusDTO dto)
        {
            try {
                switch (byte.Parse(dto.Online_status))
                {
                    case 0:
                        try
                        {
                            if (!_UsersDBC.Selected_StatusTbl.Any((x => x.User_ID == dto.End_User_ID)))
                            {
                                await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
                                {
                                    ID = Convert.ToUInt64(_UsersDBC.Selected_StatusTbl.Count() + 1),
                                    User_ID = dto.End_User_ID,
                                    Updated_on = TimeStamp,
                                    Created_on = TimeStamp,
                                    Updated_by = dto.End_User_ID,
                                    Created_by = dto.End_User_ID,
                                    Offline = 1
                                });
                            } else {
                                await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                    .SetProperty(col => col.Offline, 1)
                                    .SetProperty(col => col.Hidden, 0)
                                    .SetProperty(col => col.Online, 0)
                                    .SetProperty(col => col.Away, 0)
                                    .SetProperty(col => col.DND, 0)
                                    .SetProperty(col => col.Custom, 0)
                                    .SetProperty(col => col.Custom_lbl, "")
                                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                    .SetProperty(col => col.Updated_on, TimeStamp)
                                );
                            }
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = "Offline"
                            });
                        } catch {
                            return "Server Error: Update User Display Status 0 Failed.";
                            
                        }
                    case 1:
                        try
                        {
                            if (!_UsersDBC.Selected_StatusTbl.Any((x => x.User_ID == dto.End_User_ID)))
                            {
                                await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
                                {
                                    ID = Convert.ToUInt64(_UsersDBC.Selected_StatusTbl.Count() + 1),
                                    User_ID = dto.End_User_ID,
                                    Updated_on = TimeStamp,
                                    Created_on = TimeStamp,
                                    Updated_by = dto.End_User_ID,
                                    Created_by = dto.End_User_ID,
                                    Hidden = 1
                                });
                            }
                            else
                            {
                                await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                    .SetProperty(col => col.Offline, 0)
                                    .SetProperty(col => col.Hidden, 1)
                                    .SetProperty(col => col.Online, 0)
                                    .SetProperty(col => col.Away, 0)
                                    .SetProperty(col => col.DND, 0)
                                    .SetProperty(col => col.Custom, 0)
                                    .SetProperty(col => col.Custom_lbl, "")
                                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                    .SetProperty(col => col.Updated_on, TimeStamp)
                                );
                            }
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = "Hidden"
                            });
                        } catch {
                            return "Server Error: Update User Display Status 1 Failed.";
                            
                        }
                    case 2:
                        try
                        {
                            if (!_UsersDBC.Selected_StatusTbl.Any((x => x.User_ID == dto.End_User_ID)))
                            {
                                await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
                                {
                                    ID = Convert.ToUInt64(_UsersDBC.Selected_StatusTbl.Count() + 1),
                                    User_ID = dto.End_User_ID,
                                    Updated_on = TimeStamp,
                                    Created_on = TimeStamp,
                                    Updated_by = dto.End_User_ID,
                                    Created_by = dto.End_User_ID,
                                    Online = 1
                                });
                            }
                            else
                            {
                                await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                    .SetProperty(col => col.Offline, 0)
                                    .SetProperty(col => col.Hidden, 0)
                                    .SetProperty(col => col.Online, 1)
                                    .SetProperty(col => col.Away, 0)
                                    .SetProperty(col => col.DND, 0)
                                    .SetProperty(col => col.Custom, 0)
                                    .SetProperty(col => col.Custom_lbl, "")
                                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                    .SetProperty(col => col.Updated_on, TimeStamp)
                                );
                            }
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = "Online"
                            });
                        }
                        catch
                        {
                            return "Server Error: Update User Display Status 2 Failed.";
                            
                        }
                    case 3:
                        try
                        {
                            if (!_UsersDBC.Selected_StatusTbl.Any((x => x.User_ID == dto.End_User_ID)))
                            {
                                await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
                                {
                                    ID = Convert.ToUInt64(_UsersDBC.Selected_StatusTbl.Count() + 1),
                                    User_ID = dto.End_User_ID,
                                    Updated_on = TimeStamp,
                                    Created_on = TimeStamp,
                                    Updated_by = dto.End_User_ID,
                                    Created_by = dto.End_User_ID,
                                    Away = 1
                                });
                            }
                            else
                            {
                                await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                    .SetProperty(col => col.Offline, 0)
                                    .SetProperty(col => col.Hidden, 0)
                                    .SetProperty(col => col.Online, 0)
                                    .SetProperty(col => col.Away, 1)
                                    .SetProperty(col => col.DND, 0)
                                    .SetProperty(col => col.Custom, 0)
                                    .SetProperty(col => col.Custom_lbl, "")
                                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                    .SetProperty(col => col.Updated_on, TimeStamp)
                                );
                            }
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = "Away"
                            });
                        }
                        catch
                        {
                            return "Server Error: Update User Display Status 3 Failed.";
                            
                        }
                    case 4:
                        try
                        {
                            if (!_UsersDBC.Selected_StatusTbl.Any((x => x.User_ID == dto.End_User_ID)))
                            {
                                await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
                                {
                                    ID = Convert.ToUInt64(_UsersDBC.Selected_StatusTbl.Count() + 1),
                                    User_ID = dto.End_User_ID,
                                    Updated_on = TimeStamp,
                                    Created_on = TimeStamp,
                                    Updated_by = dto.End_User_ID,
                                    Created_by = dto.End_User_ID,
                                    DND = 1
                                });
                            }
                            else
                            {
                                await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                    .SetProperty(col => col.Offline, 0)
                                    .SetProperty(col => col.Hidden, 0)
                                    .SetProperty(col => col.Online, 0)
                                    .SetProperty(col => col.Away, 0)
                                    .SetProperty(col => col.DND, 1)
                                    .SetProperty(col => col.Custom, 0)
                                    .SetProperty(col => col.Custom_lbl, "")
                                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                    .SetProperty(col => col.Updated_on, TimeStamp)
                                );
                            }
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = "Do Not Disturb"
                            });
                        }
                        catch
                        {
                            return "Server Error: Update User Display Status 4 Failed.";
                            
                        }
                    case 5:
                        try
                        {
                            if (!_UsersDBC.Selected_StatusTbl.Any((x => x.User_ID == dto.End_User_ID)))
                            {
                                await _UsersDBC.Selected_StatusTbl.AddAsync(new Selected_StatusTbl
                                {
                                    ID = Convert.ToUInt64(_UsersDBC.Selected_StatusTbl.Count() + 1),
                                    User_ID = dto.End_User_ID,
                                    Updated_on = TimeStamp,
                                    Created_on = TimeStamp,
                                    Updated_by = dto.End_User_ID,
                                    Created_by = dto.End_User_ID,
                                    Custom = 1
                                });
                            }
                            else
                            {
                                await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                    .SetProperty(col => col.Offline, 0)
                                    .SetProperty(col => col.Hidden, 1)
                                    .SetProperty(col => col.Online, 0)
                                    .SetProperty(col => col.Away, 0)
                                    .SetProperty(col => col.DND, 0)
                                    .SetProperty(col => col.Custom, 0)
                                    .SetProperty(col => col.Custom_lbl, dto.Custom_lbl)
                                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                    .SetProperty(col => col.Updated_on, TimeStamp)
                                );
                            }
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = dto.Custom_lbl
                            });
                        }
                        catch
                        {
                            return "Server Error: Update User Display Status 5 Failed.";
                            
                        }
                    case 10:
                        try {
                            await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Offline, 1)
                                .SetProperty(col => col.Hidden, 1)
                                .SetProperty(col => col.Online, 0)
                                .SetProperty(col => col.Away, 0)
                                .SetProperty(col => col.DND, 0)
                                .SetProperty(col => col.Custom, 0)
                                .SetProperty(col => col.Custom_lbl, "")
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                            );
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = "Hidden"
                            });
                        } catch {
                            return "Server Error: Update User Display Status 10 Failed.";
                        }
                        
                    case 20:
                        try {
                            await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Offline, 1)
                                .SetProperty(col => col.Hidden, 0)
                                .SetProperty(col => col.Online, 1)
                                .SetProperty(col => col.Away, 0)
                                .SetProperty(col => col.DND, 0)
                                .SetProperty(col => col.Custom, 0)
                                .SetProperty(col => col.Custom_lbl, "")
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                            );
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = "Online"
                            });
                        } catch {
                            return "Server Error: Update User Display Status 20 Failed.";
                        }
                    case 30:
                        try
                        {
                            await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                            .SetProperty(col => col.Offline, 1)
                            .SetProperty(col => col.Hidden, 0)
                            .SetProperty(col => col.Online, 0)
                            .SetProperty(col => col.Away, 1)
                            .SetProperty(col => col.DND, 0)
                            .SetProperty(col => col.Custom, 0)
                            .SetProperty(col => col.Custom_lbl, "")
                            .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            .SetProperty(col => col.Updated_on, TimeStamp)
                        );
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = "Away"
                            });
                        }
                        catch
                        {
                            return "Server Error: Update User Display Status 30 Failed.";
                        }
                    case 40:
                        try
                        {
                            await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Offline, 1)
                                .SetProperty(col => col.Hidden, 0)
                                .SetProperty(col => col.Online, 0)
                                .SetProperty(col => col.Away, 0)
                                .SetProperty(col => col.DND, 1)
                                .SetProperty(col => col.Custom, 0)
                                .SetProperty(col => col.Custom_lbl, "")
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                            );
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = "Do Not Disturb"
                            });
                        } catch {
                            return "Server Error: Update User Display Status 40 Failed.";
                        }
                    case 50:
                        try
                        {
                            await _UsersDBC.Selected_StatusTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Offline, 1)
                                .SetProperty(col => col.Hidden, 0)
                                .SetProperty(col => col.Online, 0)
                                .SetProperty(col => col.Away, 0)
                                .SetProperty(col => col.DND, 0)
                                .SetProperty(col => col.Custom, 1)
                                .SetProperty(col => col.Custom_lbl, dto.Custom_lbl)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                            );
                            await _UsersDBC.SaveChangesAsync();
                            return JsonSerializer.Serialize(new
                            {
                                id = dto.End_User_ID,
                                online_status = dto.Custom_lbl
                            });
                        } catch {
                            return "Server Error: Update User Display Status 50 Failed.";
                        }
                    default:
                        return "Server Error: Update Status Unknown.";
                }
            } catch {
                return "Server Error: Update Status Selection Failed.";
            }
        }
        public async Task<string> Update_End_User_Selected_Theme(Selected_ThemeDTO dto)
        {
            try {
                int theme = int.Parse(dto.Theme);
                switch (theme)
                {
                    case 0:
                        if (!_UsersDBC.Selected_ThemeTbl.Any(x => x.User_ID == dto.End_User_ID))
                        {
                            await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_ThemeTbl.Count() + 1),
                                User_ID = dto.End_User_ID,
                                Light = 1,
                                Updated_on = TimeStamp,
                                Created_by = dto.End_User_ID,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID
                            });
                        } else { 
                            await _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Light, 1)
                                .SetProperty(col => col.Night, 0)
                                .SetProperty(col => col.Custom, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            theme = 0
                        });
                    case 1:
                        if (!_UsersDBC.Selected_ThemeTbl.Any(x => x.User_ID == dto.End_User_ID))
                        {
                            await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_ThemeTbl.Count() + 1),
                                User_ID = dto.End_User_ID,
                                Night = 1,
                                Updated_on = TimeStamp,
                                Created_by = dto.End_User_ID,
                                Created_on = TimeStamp,
                                Updated_by = dto.End_User_ID
                            });
                        }
                        else
                        { 
                            await _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Light, 0)
                                .SetProperty(col => col.Night, 1)
                                .SetProperty(col => col.Custom, 0)
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            theme = 1
                        });
                    case 2:
                        if (!_UsersDBC.Selected_ThemeTbl.Any(x => x.User_ID == dto.End_User_ID))
                        {
                            await _UsersDBC.Selected_ThemeTbl.AddAsync(new Selected_ThemeTbl
                            {
                                ID = Convert.ToUInt64(_UsersDBC.Selected_ThemeTbl.Count() + 1),
                                User_ID = dto.End_User_ID,
                                Night = 1,
                                Created_by = dto.End_User_ID,
                                Created_on = TimeStamp,
                                Updated_on = TimeStamp,
                                Updated_by = dto.End_User_ID
                            });
                        } else { 
                            await _UsersDBC.Selected_ThemeTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                                .SetProperty(col => col.Updated_on, TimeStamp)
                                .SetProperty(col => col.Updated_by, dto.End_User_ID)
                                .SetProperty(col => col.Light, 0)
                                .SetProperty(col => col.Night, 0)
                                .SetProperty(col => col.Custom, 1)
                            );
                        }
                        await _UsersDBC.SaveChangesAsync();
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            theme = "Custom"
                        });
                    default:
                        return JsonSerializer.Serialize(new
                        {
                            id = dto.End_User_ID,
                            error = "Invalid Theme Request"
                        });

                }
            } catch {
                return "Server Error: Update Theme Failed."; 
            }
        }
        public async Task<string> Update_End_User_Card_Border_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl  
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Border_Color = dto.Card_Border_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                { 
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Border_Color, dto.Card_Border_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_border_color = dto.Card_Border_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Header_Font(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Header_Font = dto.Card_Header_Font,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Header_Font, dto.Card_Header_Font)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_header_font = dto.Card_Header_Font
                });
            } catch {
                return "Server Error: Update Custom Theme Failed."; 
            }
        }
        public async Task<string> Update_End_User_Card_Header_Background_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Header_Background_Color = dto.Card_Header_Background_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Header_Background_Color, dto.Card_Header_Background_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_header_background_color = dto.Card_Header_Background_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed."; 
            }
        }
        public async Task<string> Update_End_User_Card_Header_Font_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Header_Font_Color = dto.Card_Header_Font_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Header_Font_Color, dto.Card_Header_Font_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_header_font_color = dto.Card_Header_Font_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Footer_Font(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Footer_Font = dto.Card_Footer_Font,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Footer_Font, dto.Card_Footer_Font)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_footer_font = dto.Card_Footer_Font
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Footer_Background_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Footer_Background_Color = dto.Card_Footer_Background_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Footer_Background_Color, dto.Card_Footer_Background_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_footer_background_color = dto.Card_Footer_Background_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Delete_End_User_Selected_App_Custom_Design(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
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
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                );
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = 0,
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Footer_Font_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Footer_Font_Color = dto.Card_Footer_Font_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Footer_Font_Color, dto.Card_Footer_Font_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_footer_font_color = dto.Card_Footer_Font_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Body_Font(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Body_Font = dto.Card_Body_Font,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Body_Font, dto.Card_Body_Font)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_body_font = dto.Card_Body_Font
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Body_Background_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Body_Background_Color = dto.Card_Body_Background_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Body_Background_Color, dto.Card_Body_Background_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new
                {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    card_body_background_color = dto.Card_Body_Background_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Card_Body_Font_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Card_Body_Font_Color = dto.Card_Body_Font_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Card_Body_Font_Color, dto.Card_Body_Font_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
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
        public async Task<string> Update_End_User_Navigation_Menu_Font(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Navigation_Menu_Font = dto.Navigation_Menu_Font,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Navigation_Menu_Font, dto.Navigation_Menu_Font)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new { 
                    id = dto.End_User_ID,
                    theme = "Custom",
                    navigation_menu_font = dto.Navigation_Menu_Font
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Navigation_Menu_Background_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Navigation_Menu_Background_Color = dto.Navigation_Menu_Background_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Navigation_Menu_Background_Color, dto.Navigation_Menu_Background_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new { 
                    id = dto.End_User_ID,
                    theme = "Custom",
                    navigation_menu_Background_color = dto.Navigation_Menu_Background_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Navigation_Menu_Font_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Navigation_Menu_Font_Color = dto.Navigation_Menu_Font_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Navigation_Menu_Font_Color, dto.Navigation_Menu_Font_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    navigation_menu_font_color = dto.Navigation_Menu_Font_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Button_Font(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Button_Font = dto.Button_Font,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                }
                else
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Button_Font, dto.Button_Font)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new {
                    id = dto.End_User_ID,
                    theme = "Custom",
                    button_font = dto.Button_Font
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Button_Background_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Button_Background_Color = dto.Button_Background_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Button_Background_Color, dto.Button_Background_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new { 
                    id = dto.End_User_ID,
                    theme = "Custom",
                    button_background_color = dto.Button_Background_Color
                });
            } catch {
                return "Server Error: Update Custom Theme Failed.";
            }
        }
        public async Task<string> Update_End_User_Button_Font_Color(Selected_App_Custom_DesignDTO dto)
        {
            try
            {
                if (!_UsersDBC.Selected_App_Custom_DesignTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.AddAsync(new Selected_App_Custom_DesignTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Selected_App_Custom_DesignTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Button_Font_Color = dto.Button_Font_Color,
                        Updated_on = TimeStamp,
                        Created_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    await _UsersDBC.Selected_App_Custom_DesignTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Button_Font_Color, dto.Button_Font_Color)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
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
        public async Task<string> Update_End_User_Password(Password_ChangeDTO dto)
        {
            try {
                if (!dto.Email_address.IsNullOrEmpty()) {
                    await _UsersDBC.Login_PasswordTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Password, Password.Process_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes($"{dto.New_password}"), Encoding.UTF8.GetBytes($"{dto.Email_address}{_Constants.JWT_SECURITY_KEY}")).Result)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return "Update End User Password Completed.";
            } catch {
                return "Server Error: Update Password Failed.";
            }
        }
        public async Task<string> Update_End_User_Login_Time_Stamp(Login_Time_StampDTO dto)
        {
            try
            {
                ulong clocked = TimeStamp;
                if (!_UsersDBC.Login_Time_StampTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Login_Time_StampTbl.AddAsync(new Login_Time_StampTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Login_Time_StampTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Updated_on = clocked,
                        Created_on = clocked,
                        Updated_by = dto.End_User_ID,
                        Created_by = dto.End_User_ID,
                        Login_on = clocked,
                        Location = dto.Location,
                        Client_time = dto.Client_Time_Parsed,
                        Remote_IP = dto.Remote_IP,
                        Remote_Port = dto.Remote_Port,
                        Server_IP = dto.Server_IP_Address,
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
                    await _UsersDBC.Login_Time_StampTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    .SetProperty(col => col.Login_on, clocked)
                    .SetProperty(col => col.Location, dto.Location)
                    .SetProperty(col => col.Client_time, dto.Client_Time_Parsed)
                    .SetProperty(col => col.Client_IP, dto.Remote_IP)
                    .SetProperty(col => col.Client_Port, dto.Remote_Port)
                    .SetProperty(col => col.Server_IP, dto.Server_IP_Address)
                    .SetProperty(col => col.Server_Port, dto.Server_Port)
                    .SetProperty(col => col.Client_IP, dto.Client_IP)
                    .SetProperty(col => col.Client_Port, dto.Client_Port)
                    .SetProperty(col => col.Updated_on, clocked));
                }

                await _UsersDBC.SaveChangesAsync();
                return Task.FromResult(JsonSerializer.Serialize(new {
                    id = dto.End_User_ID,
                    login_on = clocked
                })).Result;
            } catch {
                return "Server Error: Update Login Failed.";
            }
        }
        public async Task<string> Insert_End_User_Login_Time_Stamp_History(Login_Time_Stamp_HistoryDTO dto)
        {
            try
            {
                ulong clocked = TimeStamp;

                await _UsersDBC.Login_Time_Stamp_HistoryTbl.AddAsync(new Login_Time_Stamp_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Login_Time_Stamp_HistoryTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    Updated_on = clocked,
                    Created_on = clocked,
                    Updated_by = dto.End_User_ID,
                    Created_by = dto.End_User_ID,
                    Login_on = clocked,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
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
                    login_on = clocked
                }));
            } catch {
                return Task.FromResult(JsonSerializer.Serialize("Login TS History Failed.")).Result;
            }
        }
        public async Task<string> Insert_Report_Email_RegistrationTbl(Report_Email_RegistrationDTO dto)
        {
            try
            {
                await _UsersDBC.Report_Email_RegistrationTbl.AddAsync(new Report_Email_RegistrationTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Report_Email_RegistrationTbl.Count() + 1),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = 0,
                    Created_by = 0,
                    Email_Address = dto.Email_Address,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Server_IP_Address,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
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
            } catch {
                return "Server Error: Reported Email Registration Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Pending_Email_Registration_HistoryTbl(Report_Failed_Pending_Email_Registration_HistoryDTO dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Pending_Email_Registration_HistoryTbl.AddAsync(new Report_Failed_Pending_Email_Registration_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Report_Failed_Pending_Email_Registration_HistoryTbl.Count() + 1),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = 0,
                    Created_by = 0,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
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
            } catch {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_User_Agent_HistoryTbl(Report_Failed_User_Agent_HistoryDTO dto) {
            try
            {
                await _UsersDBC.Report_Failed_User_Agent_HistoryTbl.AddAsync(new Report_Failed_User_Agent_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Report_Failed_User_Agent_HistoryTbl.Count() + 1),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    User_ID = dto.End_User_ID,
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
            } catch {
                return "Server Error: Report User Agent Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Selected_HistoryTbl(Report_Failed_Selected_HistoryDTO dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Selected_HistoryTbl.AddAsync(new Report_Failed_Selected_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Report_Failed_Selected_HistoryTbl.Count() + 1),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Reason = dto.Reason,
                    User_ID = dto.End_User_ID,
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
        public async Task<string> Insert_Report_Failed_Logout_HistoryTbl(Report_Failed_Logout_HistoryDTO dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Logout_HistoryTbl.AddAsync(new Report_Failed_Logout_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Report_Failed_Logout_HistoryTbl.Count() + 1),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Reason = dto.Reason,
                    User_ID = dto.End_User_ID,
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
            } catch {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_JWT_HistoryTbl(Report_Failed_JWT_HistoryDTO dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_JWT_HistoryTbl.AddAsync(new Report_Failed_JWT_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Report_Failed_JWT_HistoryTbl.Count() + 1),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    JWT_id = dto.JWT_id,
                    Client_id = dto.Client_id,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Reason = dto.Reason,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    User_ID = dto.End_User_ID,
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
            } catch {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Client_ID_HistoryTbl(Report_Failed_Client_ID_HistoryDTO dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Client_ID_HistoryTbl.AddAsync(new Report_Failed_Client_ID_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Report_Failed_Client_ID_HistoryTbl.Count() + 1),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_time,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
                    Reason = dto.Reason,
                    Action = dto.Action,
                    Controller = dto.Controller,
                    User_ID = dto.End_User_ID,
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
            } catch {
                return "Server Error: Report Pending Email Registration History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Unregistered_Email_Login_HistoryTbl(Report_Failed_Unregistered_Email_Login_HistoryDTO dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Unregistered_Email_Login_HistoryTbl.AddAsync(new Report_Failed_Unregistered_Email_Login_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Report_Failed_Unregistered_Email_Login_HistoryTbl.Count() + 1),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = 0,
                    Created_by = 0,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
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
            } catch {
                return "Server Error: Report Unregistered Email Login History Failed.";
            }
        }
        public async Task<string> Insert_Report_Failed_Email_Login_HistoryTbl(Report_Failed_Email_Login_HistoryDTO dto)
        {
            try
            {
                await _UsersDBC.Report_Failed_Email_Login_HistoryTbl.AddAsync(new Report_Failed_Email_Login_HistoryTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Report_Failed_Email_Login_HistoryTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = 0,
                    Created_by = 0,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Client_Port,
                    Client_time = dto.Client_Time_Parsed,
                    Reason = dto.Reason,
                    Language_Region = $@"{dto.Language}-{dto.Region}",
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
                return await Task.FromResult(JsonSerializer.Serialize(new { 
                    id = dto.End_User_ID,
                    error = dto.Reason
                }));
            } catch {
                return "Server Error: Report Email Login History Failed.";
            }
        }
        public async Task<string> Insert_End_User_Logout_HistoryTbl(Logout_Time_StampDTO dto)
        {
            await _UsersDBC.Logout_Time_Stamp_HistoryTbl.AddAsync(new Logout_Time_Stamp_HistoryTbl
            {
                User_ID = dto.End_User_ID,
                Logout_on = TimeStamp,
                Updated_by = dto.End_User_ID,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Location = dto.Location,
                Remote_IP = dto.Remote_IP,
                Remote_Port = dto.Remote_Port,
                Server_IP = dto.Server_IP_Address,
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

            return Task.FromResult(JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                logout_on = TimeStamp
            })).Result;
        }
        public async Task<string> Update_End_User_Logout(Logout_Time_StampDTO dto)
        {
            if (_UsersDBC.Logout_Time_StampTbl.Any(x => x.User_ID == dto.End_User_ID))
            {
                await _UsersDBC.Logout_Time_StampTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Logout_on, TimeStamp)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    .SetProperty(col => col.Logout_on, TimeStamp)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    .SetProperty(col => col.Client_time, dto.Client_Time_Parsed)
                    .SetProperty(col => col.Server_Port, dto.Server_Port)
                    .SetProperty(col => col.Server_IP, dto.Server_IP_Address)
                    .SetProperty(col => col.Client_Port, dto.Client_Port)
                    .SetProperty(col => col.Client_IP, dto.Client_IP)
                    .SetProperty(col => col.Client_IP, dto.Remote_IP)
                    .SetProperty(col => col.Client_Port, dto.Remote_Port)
                    .SetProperty(col => col.Location, dto.Location)
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
                await _UsersDBC.SaveChangesAsync();
            }
            else
            {
                await _UsersDBC.Logout_Time_StampTbl.AddAsync(new Logout_Time_StampTbl
                {
                    User_ID = dto.End_User_ID,
                    Logout_on = TimeStamp,
                    Updated_by = dto.End_User_ID,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Location = dto.Location,
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP = dto.Server_IP_Address,
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
            }

            return Task.FromResult(JsonSerializer.Serialize(new {
                End_User_ID = dto.End_User_ID,
                logout_on = TimeStamp
            })).Result;
        }
        public Task<bool> ID_Exists_In_Users_IDTbl(ulong user_id) {
            return Task.FromResult(_UsersDBC.User_IDsTbl.Any(x => x.ID == user_id && x.Deleted == 0));
        }
        public Task<bool> ID_Exists_In_Twitch_IDsTbl(ulong user_id)
        {
            return Task.FromResult(_UsersDBC.Twitch_IDsTbl.Any(x => x.Twitch_ID == user_id && x.Deleted == 0));
        }
        public Task<bool> ID_Exists_In_Discord_IDsTbl(ulong user_id)
        {
            return Task.FromResult(_UsersDBC.Discord_IDsTbl.Any(x => x.Discord_ID == user_id && x.Deleted == 0));
        }
        public async Task<bool> Email_Exists_In_Pending_Email_RegistrationTbl(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(x => x.Email_Address.ToUpper() == email_address));
        }
        public async Task<bool> Confirmation_Code_Exists_In_Pending_Email_Address_RegistrationTbl(string Code)
        {
            return await Task.FromResult(_UsersDBC.Pending_Email_RegistrationTbl.Any(x => x.Code == Code));
        }
        public async Task<ulong> Read_User_ID_By_Email_Address(string email_address)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Where(x => x.Email_Address == email_address).Select(x => x.User_ID).SingleOrDefault());
        }
        public async Task<string?> Read_User_Email_By_ID(ulong id)
        {
            return await Task.FromResult(_UsersDBC.Login_Email_AddressTbl.Where(x => x.User_ID == id).Select(x => x.Email_Address).SingleOrDefault());
        }
        public async Task<ulong> Read_User_ID_By_Twitch_Account_ID(ulong twitch_id)
        {
            return await Task.FromResult(_UsersDBC.Twitch_IDsTbl.Where(x => x.Twitch_ID == twitch_id).Select(x => x.User_ID).SingleOrDefault());
        }
        public async Task<ulong> Read_User_ID_By_Twitch_Account_Email(string twitch_email)
        {
            return await Task.FromResult(_UsersDBC.Twitch_Email_AddressTbl.Where(x => x.Email_Address == twitch_email.ToUpper()).Select(x => x.User_ID).SingleOrDefault());
        }
        public async Task<ulong> Read_User_ID_By_Discord_Account_ID(ulong discord_id)
        {
            return await Task.FromResult(_UsersDBC.Discord_IDsTbl.Where(x => x.Discord_ID == discord_id).Select(x => x.User_ID).SingleOrDefault());
        }
        public async Task<ulong> Read_User_ID_By_Discord_Account_Email(string discord_email)
        {
            return await Task.FromResult(_UsersDBC.Discord_Email_AddressTbl.Where(x => x.Email_Address == discord_email).Select(x => x.User_ID).SingleOrDefault());
        }
        public async Task Create_WebSocket_Permission_Record(WebSocket_Chat_PermissionDTO dto)
        {
            await _UsersDBC.WebSocket_Chat_PermissionTbl.AddAsync(new WebSocket_Chat_PermissionTbl
            {
                ID = Convert.ToUInt64(_UsersDBC.WebSocket_Chat_PermissionTbl.Count() + 1),
                User_ID = dto.End_User_ID,
                Participant_ID = dto.Participant_ID,
                Updated_on = TimeStamp,
                Created_on = TimeStamp,
                Updated_by = dto.End_User_ID,
                Requested = 1,
                Blocked = 0,
                Approved = 0
            });
            await _UsersDBC.SaveChangesAsync();
        }
        public async Task<byte[]?> Read_User_Password_Hash_By_ID(ulong user_id)
        {
            return await Task.FromResult(_UsersDBC.Login_PasswordTbl.Where(user => user.User_ID == user_id).Select(user => user.Password).SingleOrDefault());
        }
        public async Task<string> Update_End_User_First_Name(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.End_User_ID))
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    First_Name = dto.First_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.First_Name, dto.First_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                first_name = dto.First_name
            });
        }
        public async Task<string> Update_End_User_Last_Name(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.End_User_ID))
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    Last_Name = dto.Last_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Last_Name, dto.Last_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                last_name = dto.Last_name
            });
        }
        public async Task<string> Update_End_User_Middle_Name(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.End_User_ID))
            { 
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    Middle_Name = dto.Middle_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.End_User_ID
                });
            }
            else
            { 
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                     .SetProperty(col => col.Middle_Name, dto.Middle_name)
                     .SetProperty(col => col.Updated_on, TimeStamp)
                     .SetProperty(col => col.Updated_by, dto.End_User_ID)
                 );
            }
            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                middle_name = dto.Middle_name
            });
        }
        public async Task<string> Update_End_User_Maiden_Name(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.End_User_ID))
            { 
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    Maiden_Name = dto.Maiden_name,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.End_User_ID
                });
            }
            else
            { 
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Maiden_Name, dto.Maiden_name)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                maiden_name = dto.Maiden_name
            });
        }
        public async Task<string> Update_End_User_Gender(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.End_User_ID))
            {
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    Gender = byte.Parse(dto.Gender),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.End_User_ID
                });
            }
            else
            {
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Gender, byte.Parse(dto.Gender))
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new { 
                id = dto.End_User_ID,
                gender = dto.Gender
            });
        }
        public async Task<string> Update_End_User_Ethnicity(IdentityDTO dto)
        {
            if (!_UsersDBC.IdentityTbl.Any(x => x.User_ID == dto.End_User_ID))
            { 
                await _UsersDBC.IdentityTbl.AddAsync(new IdentityTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.IdentityTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    Ethnicity = dto.Ethnicity,
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.End_User_ID
                });
            }
            else
            { 
                await _UsersDBC.IdentityTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Ethnicity, dto.Ethnicity)
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                ethnicity = dto.Ethnicity
            });
        }
        public async Task<string> Update_End_User_Birth_Date(IdentityDTO dto)
        {
            if (!_UsersDBC.Birth_DateTbl.Any(x => x.User_ID == dto.End_User_ID))
            { 
                await _UsersDBC.Birth_DateTbl.AddAsync(new Birth_DateTbl
                {
                    ID = Convert.ToUInt64(_UsersDBC.Birth_DateTbl.Count() + 1),
                    User_ID = dto.End_User_ID,
                    Month = byte.Parse(dto.Month),
                    Day = byte.Parse(dto.Day),
                    Year = ulong.Parse(dto.Year),
                    Updated_on = TimeStamp,
                    Created_on = TimeStamp,
                    Updated_by = dto.End_User_ID
                });
            }
            else
            { 
                await _UsersDBC.Birth_DateTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                    .SetProperty(col => col.Month, byte.Parse(dto.Month))
                    .SetProperty(col => col.Day, byte.Parse(dto.Day))
                    .SetProperty(col => col.Year, ulong.Parse(dto.Year))
                    .SetProperty(col => col.Updated_on, TimeStamp)
                    .SetProperty(col => col.Updated_by, dto.End_User_ID)
                );
            }
            await _UsersDBC.SaveChangesAsync();

            return JsonSerializer.Serialize(new {
                id = dto.End_User_ID,
                birth_month = dto.Month,
                birth_day = dto.Day,
                birth_year = dto.Year,
            });
        }
        public async Task<string> Update_End_User_Account_Groups(Account_GroupsDTO dto)
        {
            try
            {
                if (!_UsersDBC.Account_GroupsTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Account_GroupsTbl.AddAsync(new Account_GroupsTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Account_GroupsTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Groups = dto.Groups,
                        Updated_on = TimeStamp,
                        Updated_by = dto.End_User_ID,
                        Created_on = TimeStamp,
                    });
                }
                else
                {
                    await _UsersDBC.Account_GroupsTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Groups, dto.Groups)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize(new { end_user_groups = dto.Groups });
            }
            catch
            {
                return "Server Error: Update Account Groups Failed.";
            }
        }

        public async Task<string> Update_End_User_Account_Roles(Account_RolesDTO dto)
        {
            try
            {
                if (!_UsersDBC.Account_RolesTbl.Any(x => x.User_ID == dto.End_User_ID))
                {
                    await _UsersDBC.Account_RolesTbl.AddAsync(new Account_RolesTbl
                    {
                        ID = Convert.ToUInt64(_UsersDBC.Account_RolesTbl.Count() + 1),
                        User_ID = dto.End_User_ID,
                        Roles = dto.Roles,
                        Updated_on = TimeStamp,
                        Created_on = TimeStamp,
                        Updated_by = dto.End_User_ID
                    });
                } else {
                    await _UsersDBC.Account_RolesTbl.Where(x => x.User_ID == dto.End_User_ID).ExecuteUpdateAsync(s => s
                        .SetProperty(col => col.Roles, dto.Roles)
                        .SetProperty(col => col.Updated_on, TimeStamp)
                        .SetProperty(col => col.Updated_by, dto.End_User_ID)
                    );
                }
                await _UsersDBC.SaveChangesAsync();
                return JsonSerializer.Serialize( new { roles = dto.Roles });
            } catch {
                return "Server Error: Update End User Roles Failed.";
            }
        }
        public async Task<bool> Validate_Client_With_Server_Authorization(Report_Failed_Authorization_HistoryDTO dto)
        {

            if (dto.Server_User_Agent == "error" || dto.Client_User_Agent != dto.Server_User_Agent)
            {
                await Insert_Report_Failed_User_Agent_HistoryTbl(new Report_Failed_User_Agent_HistoryDTO
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP_Address = dto.Server_IP_Address,
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
                await Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP_Address = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Server_Port,
                    User_agent = dto.Client_User_Agent,
                    Client_id = dto.End_User_ID,
                    JWT_id = dto.JWT_id,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Login_type = dto.Login_type,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
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
                await Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP_Address = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Server_Port,
                    User_agent = dto.Client_User_Agent,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Login_type = dto.Login_type,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
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

            if (dto.JWT_id != 0 && !ID_Exists_In_Users_IDTbl(dto.JWT_id).Result)
            {
                await Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP_Address = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Server_Port,
                    User_agent = dto.Client_User_Agent,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Login_type = dto.Login_type,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
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

            if (dto.Client_id != 0 && !ID_Exists_In_Users_IDTbl(dto.Client_id).Result)
            {
                await Insert_Report_Failed_Client_ID_HistoryTbl(new Report_Failed_Client_ID_HistoryDTO
                {
                    Remote_IP = dto.Remote_IP,
                    Remote_Port = dto.Remote_Port,
                    Server_IP_Address = dto.Server_IP_Address,
                    Server_Port = dto.Server_Port,
                    Client_IP = dto.Client_IP,
                    Client_Port = dto.Server_Port,
                    User_agent = dto.Client_User_Agent,
                    Language = dto.Language,
                    Region = dto.Region,
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