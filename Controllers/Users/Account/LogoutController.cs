using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Email;
using mpc_dotnetc_user_server.Models.Users.Selected.Status;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Authentication.Logout;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Account")]
    public class LogoutController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<LogoutController> _logger;
        private static IConfiguration? _configuration;
        private readonly ISystem_Tampering System_Tampering;
        private readonly IUsers_Repository_Update Users_Repository_Update;
        private readonly IUsers_Repository_Create Users_Repository_Create;

        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly INetwork Network;
        private readonly IPassword Password;

        public LogoutController(
            ILogger<LogoutController> logger,
            IConfiguration configuration,
            ISystem_Tampering system_tampering,
            IUsers_Repository_Create users_repository_create,
            Constants constants,
            IAES aes,
            IJWT jwt,
            INetwork network,
            IPassword password
            )
        {
            _logger = logger;
            _configuration = configuration;
            System_Tampering = system_tampering;
            _Constants = constants;
            AES = aes;
            JWT = jwt;
            Network = network;
            Password = password;
        }

        [HttpPut("Logout")]
        public async Task<ActionResult<string>> Logout([FromBody] LogoutDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Locked = AES.Process_Decryption(dto.Locked);
                dto.Alignment = AES.Process_Decryption(dto.Alignment);
                dto.Text_alignment = AES.Process_Decryption(dto.Text_alignment);
                dto.Theme = AES.Process_Decryption(dto.Theme);
                dto.Grid_type = AES.Process_Decryption(dto.Grid_type);
                dto.Online_status = AES.Process_Decryption(dto.Online_status);

                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption(dto.Client_time));

                dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID.ToString()));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = dto.Client_user_agent;

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
    
                dto.Screen_width = AES.Process_Decryption(dto.Screen_width);
                dto.Screen_height = AES.Process_Decryption(dto.Screen_height);
                dto.RTT = AES.Process_Decryption(dto.RTT);
                dto.Orientation = AES.Process_Decryption(dto.Orientation);
                dto.Data_saver = AES.Process_Decryption(dto.Data_saver);
                dto.Color_depth = AES.Process_Decryption(dto.Color_depth);
                dto.Pixel_depth = AES.Process_Decryption(dto.Pixel_depth);
                dto.Connection_type = AES.Process_Decryption(dto.Connection_type);
                dto.Down_link = AES.Process_Decryption(dto.Down_link);
                dto.Device_ram_gb = AES.Process_Decryption(dto.Device_ram_gb);

                if (!System_Tampering.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
                {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    Token = dto.Token,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Client_time = dto.Client_Time_Parsed,
                    Server_User_Agent = dto.Server_user_agent,
                    Client_User_Agent = dto.Client_user_agent,
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
                    Controller = "Selected",
                    Action = "Status"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                await Users_Repository_Update.Update_End_User_Selected_Status(new Selected_Status
                {
                    End_User_ID = dto.End_User_ID,
                    Status = 0
                });

                HttpContext.Session.Remove(dto.End_User_ID.ToString());
                Response.Cookies.Delete(Environment.GetEnvironmentVariable("SERVER_COOKIE_NAME") ?? ".AspNetCore.Session");

                await Users_Repository_Create.Insert_End_User_Logout_History_Record(new Logout_Time_Stamp {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    User_agent = dto.Server_user_agent,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Client_time = dto.Client_Time_Parsed,
                    End_User_ID = dto.JWT_id,
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

                await Users_Repository_Update.Update_End_User_Logout(new Logout_Time_Stamp {
                    Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                    Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                    Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Port = HttpContext.Connection.LocalPort,
                    User_agent = dto.Server_user_agent,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Client_time = dto.Client_Time_Parsed,
                    End_User_ID = dto.JWT_id,
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

                return "Successfully Logged out.";
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}