using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository;


namespace mpc_dotnetc_user_server.Services.Security
{
    public class System_Tampering : ISystem_Tampering
    {

        private readonly Constants _Constants;
        private readonly IUsers_Repository_Read Users_Repository_Read;
        private readonly IUsers_Repository_Create Users_Repository_Create;

        public System_Tampering(
            Constants constants,
            IAES aes,
            IJWT jwt,
            IPassword password,
            IUsers_Repository_Read iuser_repository_read,
            IUsers_Repository_Create iuser_repository_create
        )
        {
            _Constants = constants;
            Users_Repository_Read = iuser_repository_read;
            Users_Repository_Create = iuser_repository_create;
        }

        public async Task<bool> Validate_Client_With_Server_Authorization(Report_Failed_Authorization_History dto)
        {

            if (dto.Server_User_Agent == "error" || dto.Client_User_Agent != dto.Server_User_Agent)
            {
                await Users_Repository_Create.Insert_Report_Failed_User_Agent_History(new Report_Failed_User_Agent_History
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
                await Users_Repository_Create.Insert_Report_Failed_JWT_History_Record(new Report_Failed_JWT_History
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
                await Users_Repository_Create.Insert_Report_Failed_JWT_History_Record(new Report_Failed_JWT_History
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

            if (dto.JWT_id != 0 && !Users_Repository_Read.Read_ID_Exists_In_Users_ID(dto.JWT_id).Result)
            {
                await Users_Repository_Create.Insert_Report_Failed_JWT_History_Record(new Report_Failed_JWT_History
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

            if (dto.Client_id != 0 && !Users_Repository_Read.Read_ID_Exists_In_Users_ID(dto.Client_id).Result)
            {
                await Users_Repository_Create.Insert_Report_Failed_Client_ID_History_Record(new Report_Failed_Client_ID_History
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