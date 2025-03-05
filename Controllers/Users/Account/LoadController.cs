using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using mpc_dotnetc_user_server.Models.Users.Profile;
using mpc_dotnetc_user_server.Models.Users.Community;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Load")]
    public class LoadController : ControllerBase
    {
        private readonly Constants _Constants;
        private readonly ILogger<LoadController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        public LoadController(ILogger<LoadController> logger, IConfiguration configuration, IUsersRepository UsersRepository, Constants constants)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
            _Constants = constants;
        }

        [HttpPost("Token")]
        public async Task<ActionResult<string>> Renew_Token([FromBody] Renew_JWTDTO dto)
        {
            try {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption(dto.Client_time));
                dto.Login_type = AES.Process_Decryption(dto.Login_type);

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                dto.ID = AES.Process_Decryption(dto.ID);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Token = dto.Token,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
                    Server_User_Agent = dto.Server_user_agent,
                    Client_User_Agent = dto.Client_user_agent,
                    End_User_ID = dto.Client_id,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Load",
                    Action = "Token"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                switch (dto.Login_type.ToUpper()) {
                    case "EMAIL":
                        return _UsersRepository.Read_Email_User_Data_By_ID(dto.End_User_ID).Result;
                }

                return "Token Error";
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("All_Users")]
        public async Task<ActionResult<string>> Load_All_Users([FromBody] Load_All_UsersDTO dto)
        {
            try {

                if (!ModelState.IsValid)
                    return BadRequest();

                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption(dto.Client_time));

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                dto.ID = AES.Process_Decryption(dto.ID);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Token = dto.Token,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                        Client_Time_Parsed = dto.Client_Time_Parsed,
                    Server_User_Agent = dto.Server_user_agent,
                    Client_User_Agent = dto.Client_user_agent,
                    End_User_ID = dto.Client_id,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Load",
                    Action = "All_Users"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await _UsersRepository.Read_Users();
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("User/Profile")]
        public async Task<ActionResult<string>> LoadUserProfile([FromBody] Read_User_ProfileDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);

                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Location = AES.Process_Decryption(dto.Location);
                dto.Client_Time_Parsed = ulong.Parse(AES.Process_Decryption(dto.Client_time));
                dto.Login_type = AES.Process_Decryption(dto.Login_type);

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.Client_user_agent = AES.Process_Decryption(dto.User_agent);
                dto.Server_user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

                dto.Window_height = AES.Process_Decryption(dto.Window_height);
                dto.Window_width = AES.Process_Decryption(dto.Window_width);
                dto.Screen_extend = AES.Process_Decryption(dto.Screen_extend);
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

                dto.ID = AES.Process_Decryption(dto.ID);
                dto.Profile_id = AES.Process_Decryption(dto.Profile_id);

                if (!_UsersRepository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_HistoryDTO
                {
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                    JWT_client_address = dto.JWT_client_address,
                    JWT_client_key = dto.JWT_client_key,
                    JWT_issuer_key = dto.JWT_issuer_key,
                    Token = dto.Token,
                    Client_id = dto.Client_id,
                    JWT_id = dto.JWT_id,
                    Language = dto.Language,
                    Region = dto.Region,
                    Location = dto.Location,
                    Client_Time_Parsed = dto.Client_Time_Parsed,
                    Server_User_Agent = dto.Server_user_agent,
                    Client_User_Agent = dto.Client_user_agent,
                    End_User_ID = dto.Client_id,
                    Window_height = dto.Window_height,
                    Window_width = dto.Window_width,
                    Screen_extend = dto.Screen_extend,
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
                    Controller = "Load",
                    Action = @$"User Profile ID # {dto.Profile_id}"
                }).Result)
                    return Conflict();

                dto.End_User_ID = dto.JWT_id;

                return await _UsersRepository.Read_User_Profile_By_ID(ulong.Parse(dto.Profile_id));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}