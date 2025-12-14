using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;
using mpc_dotnetc_user_server.Models.Users.Authentication.Register.Email_Address;
using mpc_dotnetc_user_server.Models.Users.Index;
using System.Text;

namespace mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository
{
    public class Users_Repository_Integrate : IUsers_Repository_Integrate
    {
        private long TimeStamp() => DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        private readonly Users_Database_Context _UsersDBC;
        private readonly Constants _Constants;

        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly IPassword Password;

        public Users_Repository_Integrate(
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
                Password = Password.Create_Password_Salted_Hash_Bytes(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{dto.Email_Address}{_Constants.JWT_SECURITY_KEY}")),
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

        public async Task Integrate_Account_By_Discord(Complete_Discord_Integration dto)
        {
            await _UsersDBC.Discord_IDsTbl.AddAsync(new Discord_IDsTbl
            {
                End_User_ID = dto.End_User_ID,
                Discord_ID = dto.Discord_ID,
                Discord_User_Name = dto.Discord_User_Name,
                Updated_on = TimeStamp(),
                Created_on = TimeStamp(),
                Created_by = dto.End_User_ID,
                Updated_by = dto.End_User_ID
            });

            await _UsersDBC.Discord_Email_AddressTbl.AddAsync(new Discord_Email_AddressTbl
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

    }
}
