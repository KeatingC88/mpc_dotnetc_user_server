using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users._Index;
using mpc_dotnetc_user_server.Models.Users.Authentication.Report;
using mpc_dotnetc_user_server.Models.Users.Authentication.JWT;
using mpc_dotnetc_user_server.Controllers;
using System.Text.Json;


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
                dto.Client_time = AES.Process_Decryption(dto.Client_time);
                dto.Login_type = AES.Process_Decryption(dto.Login_type);

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.User_agent = AES.Process_Decryption(dto.User_agent);
                var user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

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
                ulong user_id_from_jwt = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (user_agent == "error" || dto.User_agent != user_agent)
                {
                    await _UsersRepository.Insert_Report_Failed_User_Agent_HistoryTbl(new Report_Failed_User_Agent_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "User-Agent Client-Server Mismatch",
                        Controller = "Load",
                        Action = "All_Users",
                        Server_User_Agent = user_agent,
                        Client_User_Agent = dto.User_agent,
                        User_id = dto.Client_id,
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
                        Device_ram_gb = dto.Device_ram_gb
                    });
                    return Conflict();
                }

                if (dto.JWT_issuer_key != _Constants.JWT_ISSUER_KEY ||
                    dto.JWT_client_key != _Constants.JWT_CLIENT_KEY ||
                    dto.JWT_client_address != _Constants.JWT_CLAIM_WEBPAGE)
                {
                    await _UsersRepository.Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Client_id = dto.Client_id,
                        JWT_id = dto.JWT_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "JWT Client-Server Mismatch",
                        Controller = "Load",
                        Action = "All_Users",
                        User_id = dto.JWT_id,
                        JWT_issuer_key = dto.JWT_issuer_key,
                        JWT_client_key = dto.JWT_client_key,
                        JWT_client_address = dto.JWT_client_address,
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
                        Token = dto.Token
                    });
                    return Conflict();
                }

                if (dto.Client_id != dto.JWT_id)
                {
                    await _UsersRepository.Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Client_id = dto.Client_id,
                        JWT_id = dto.JWT_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "JWT Client-ID Mismatch",
                        Controller = "Load",
                        Action = "All_Users",
                        User_id = dto.JWT_id,
                        JWT_issuer_key = dto.JWT_issuer_key,
                        JWT_client_key = dto.JWT_client_key,
                        JWT_client_address = dto.JWT_client_address,
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
                        Token = dto.Token
                    });
                    return Conflict();
                }

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(dto.JWT_id).Result)
                {
                    await _UsersRepository.Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Client_id = dto.Client_id,
                        JWT_id = dto.JWT_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "JWT ID DNE Users_IDTbl",
                        Controller = "Load",
                        Action = "All_Users",
                        User_id = dto.JWT_id,
                        JWT_issuer_key = dto.JWT_issuer_key,
                        JWT_client_key = dto.JWT_client_key,
                        JWT_client_address = dto.JWT_client_address,
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
                        Token = dto.Token
                    });
                    return Conflict();
                }

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(dto.Client_id).Result)
                {
                    await _UsersRepository.Insert_Report_Failed_Logout_HistoryTbl(new Report_Failed_Logout_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "Client ID DNE Users_IDTbl",
                        Controller = "Load",
                        Action = "All_Users",
                        User_id = dto.Client_id,
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
                        Token = dto.Token,
                    });
                    return Conflict();
                }

                dto.User_id = dto.JWT_id;

                switch (dto.Login_type.ToUpper()) {
                    case "EMAIL":
                        return _UsersRepository.Read_Email_User_Data_By_ID(dto.User_id).Result;
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
                dto.Client_time = AES.Process_Decryption(dto.Client_time);

                dto.Client_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                dto.JWT_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                dto.User_agent = AES.Process_Decryption(dto.User_agent);
                var user_agent = Request.Headers["User-Agent"].ToString() ?? "error";

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
                ulong user_id_from_jwt = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (user_agent == "error" || dto.User_agent != user_agent)
                {
                    await _UsersRepository.Insert_Report_Failed_User_Agent_HistoryTbl(new Report_Failed_User_Agent_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "User-Agent Client-Server Mismatch",
                        Controller = "Load",
                        Action = "All_Users",
                        Server_User_Agent = user_agent,
                        Client_User_Agent = dto.User_agent,
                        User_id = dto.Client_id,
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
                        Device_ram_gb = dto.Device_ram_gb
                    });
                    return Conflict();
                }

                if (dto.JWT_issuer_key != _Constants.JWT_ISSUER_KEY ||
                    dto.JWT_client_key != _Constants.JWT_CLIENT_KEY ||
                    dto.JWT_client_address != _Constants.JWT_CLAIM_WEBPAGE)
                {
                    await _UsersRepository.Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Client_id = dto.Client_id,
                        JWT_id = dto.JWT_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "JWT Client-Server Mismatch",
                        Controller = "Load",
                        Action = "All_Users",
                        User_id = dto.JWT_id,
                        JWT_issuer_key = dto.JWT_issuer_key,
                        JWT_client_key = dto.JWT_client_key,
                        JWT_client_address = dto.JWT_client_address,
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
                        Token = dto.Token
                    });
                    return Conflict();
                }

                if (dto.Client_id != dto.JWT_id)
                {
                    await _UsersRepository.Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Client_id = dto.Client_id,
                        JWT_id = dto.JWT_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "JWT Client-ID Mismatch",
                        Controller = "Load",
                        Action = "All_Users",
                        User_id = dto.JWT_id,
                        JWT_issuer_key = dto.JWT_issuer_key,
                        JWT_client_key = dto.JWT_client_key,
                        JWT_client_address = dto.JWT_client_address,
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
                        Token = dto.Token
                    });
                    return Conflict();
                }

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(dto.JWT_id).Result)
                {
                    await _UsersRepository.Insert_Report_Failed_JWT_HistoryTbl(new Report_Failed_JWT_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Client_id = dto.Client_id,
                        JWT_id = dto.JWT_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = dto.Client_time,
                        Reason = "JWT ID DNE Users_IDTbl",
                        Controller = "Load",
                        Action = "All_Users",
                        User_id = dto.JWT_id,
                        JWT_issuer_key = dto.JWT_issuer_key,
                        JWT_client_key = dto.JWT_client_key,
                        JWT_client_address = dto.JWT_client_address,
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
                        Token = dto.Token
                    });
                    return Conflict();
                }

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(dto.Client_id).Result)
                {
                    await _UsersRepository.Insert_Report_Failed_Logout_HistoryTbl(new Report_Failed_Logout_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_agent = user_agent,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "Client ID DNE Users_IDTbl",
                        Controller = "Load",
                        Action = "All_Users",
                        User_id = dto.Client_id,
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
                        Token = dto.Token,
                    });
                    return Conflict();
                }

                dto.User_id = dto.JWT_id;

                return await _UsersRepository.Read_Users();
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

/*        [HttpPost("User")]
        public async Task<ActionResult<string>> LoadUser([FromBody] UserDTO dto)
        {
            try
            {
                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return Ok();

                UserDTO obj = new UserDTO
                {
                    ID = user_id,
                    Token = dto.Token
                };

                return await _UsersRepository.Read_Email_User_Data_By_ID(obj.ID);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }*/

/*        [HttpPost("User/Profile")]
        public async Task<ActionResult<string>> LoadUserProfile([FromBody] Read_User_ProfileDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong client_given_user_id = ulong.Parse(AES.Process_Decryption(dto.ID));
                ulong jwt_given_user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;
                dto.JWT_issuer_key = AES.Process_Decryption(dto.JWT_issuer_key);
                dto.JWT_client_key = AES.Process_Decryption(dto.JWT_client_key);
                dto.JWT_client_address = AES.Process_Decryption(dto.JWT_client_address);

                if (dto.JWT_issuer_key != _Constants.JWT_ISSUER_KEY ||
                    dto.JWT_client_key != _Constants.JWT_CLIENT_KEY ||
                    dto.JWT_client_address != _Constants.JWT_CLAIM_WEBPAGE)
                {
                    dto.Language = AES.Process_Decryption(dto.Language);
                    dto.Region = AES.Process_Decryption(dto.Region);
                    dto.Location = AES.Process_Decryption(dto.Location);
                    dto.Client_time = AES.Process_Decryption(dto.Client_time);
                    await _UsersRepository.Insert_Report_Failed_Load_Users_HistoryTbl(new Report_Failed_Load_Users_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_ID = client_given_user_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "JWT Mismatch for Load_User_Profile"
                    });
                    return Conflict();
                }

                if (client_given_user_id != jwt_given_user_id)
                {
                    dto.Language = AES.Process_Decryption(dto.Language);
                    dto.Region = AES.Process_Decryption(dto.Region);
                    dto.Location = AES.Process_Decryption(dto.Location);
                    dto.Client_time = AES.Process_Decryption(dto.Client_time);
                    await _UsersRepository.Insert_Report_Failed_User_ID_HistoryTbl(new Report_Failed_User_ID_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_id = client_given_user_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "User Client ID and JWT Mismatch Load_User_Profile."
                    });
                    return Conflict();
                }

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(client_given_user_id).Result)
                {
                    dto.Language = AES.Process_Decryption(dto.Language);
                    dto.Region = AES.Process_Decryption(dto.Region);
                    dto.Location = AES.Process_Decryption(dto.Location);
                    dto.Client_time = AES.Process_Decryption(dto.Client_time);
                    await _UsersRepository.Insert_Report_Failed_User_ID_HistoryTbl(new Report_Failed_User_ID_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_id = client_given_user_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "User Client ID is Not Found for Load_User_Profile."
                    });
                    return NotFound();
                }

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(jwt_given_user_id).Result)
                {
                    await _UsersRepository.Insert_Report_Failed_User_ID_HistoryTbl(new Report_Failed_User_ID_HistoryDTO
                    {
                        Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                        Client_Networking_Port = HttpContext.Connection.RemotePort,
                        Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                        Server_Networking_Port = HttpContext.Connection.LocalPort,
                        User_id = jwt_given_user_id,
                        Language = dto.Language,
                        Region = dto.Region,
                        Location = dto.Location,
                        Client_time = ulong.Parse(dto.Client_time),
                        Reason = "User JWT ID NotFound for Load_User_Profile."
                    });
                    return NotFound();
                }

                return await _UsersRepository.Read_User_Profile_By_ID(client_given_user_id);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }*/
    }//Controller.
}