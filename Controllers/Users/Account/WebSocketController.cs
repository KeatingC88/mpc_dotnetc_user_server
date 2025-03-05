using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.WebSocket_Chat;


namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/WebSocket")]
    public class WebSocketController : ControllerBase
    {
        private readonly ILogger<WebSocketController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        public WebSocketController(ILogger<WebSocketController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }

        [HttpPost("Chat_Requests/Participant")]
        public async Task<ActionResult<string>> Get_All_Participant_WebSocket_Requests(WebSocket_Chat_PermissionDTO dto)
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
                Controller = "WebSocket",
                Action = "Chat_Requests/Participant"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            return await Task.FromResult(_UsersRepository.Read_All_End_User_WebSocket_Received_Chat_Requests(dto.End_User_ID).Result);
        }

        [HttpPost("Chat_Blocks/Participant")]
        public async Task<ActionResult<string>> Get_All_Participant_WebSocket_Blocks(WebSocket_Chat_PermissionDTO dto)
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
                Controller = "WebSocket",
                Action = "Chat_Blocks/Participant"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            return await Task.FromResult(_UsersRepository.Read_All_End_User_WebSocket_Received_Chat_Blocks(dto.End_User_ID).Result);
        }

        [HttpPost("Chat_Approvals/Participant")]
        public async Task<ActionResult<string>> Get_All_Participant_WebSocket_Approvals(WebSocket_Chat_PermissionDTO dto)
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
                Controller = "WebSocket",
                Action = "Chat_Approvals/Participant"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            return await Task.FromResult(_UsersRepository.Read_All_End_User_WebSocket_Received_Chat_Approvals(dto.End_User_ID).Result);
        }

        [HttpPost("Chat_Requests/End_User")]
        public async Task<ActionResult<string>> Get_All_End_User_WebSocket_Requests(WebSocket_Chat_PermissionDTO dto)
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
                Controller = "WebSocket",
                Action = "Chat_Requests/End_User"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            return await Task.FromResult(_UsersRepository.Read_All_End_User_WebSocket_Sent_Chat_Requests(dto.End_User_ID).Result);
        }

        [HttpPost("Chat_Blocks/End_User")]
        public async Task<ActionResult<string>> Get_All_End_User_WebSocket_Blocks(WebSocket_Chat_PermissionDTO dto)
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
                Controller = "WebSocket",
                Action = "Chat_Blocks/End_User"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            return await Task.FromResult(_UsersRepository.Read_All_End_User_WebSocket_Sent_Chat_Blocks(dto.End_User_ID).Result);
        }

        [HttpPost("Chat_Approvals/End_User")]
        public async Task<ActionResult<string>> Get_All_End_User_WebSocket_Approvals(WebSocket_Chat_PermissionDTO dto)
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
                Controller = "WebSocket",
                Action = "Chat_Approves/End_User"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            return await Task.FromResult(_UsersRepository.Read_All_End_User_WebSocket_Sent_Chat_Approvals(dto.End_User_ID).Result);
        }

        [HttpPost("Approve_Invite")]
        public async Task<ActionResult<string>> Update_Approve_for_WebSocket_PermissionTbl(WebSocket_Chat_PermissionDTO dto)
        {
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

            dto.Participant_ID = ulong.Parse(AES.Process_Decryption(dto.User));

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
                Controller = "WebSocket",
                Action = "Approve_Invite"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            return await Task.FromResult(_UsersRepository.Update_Chat_Web_Socket_Permissions_Tbl(new WebSocket_Chat_PermissionTbl
            {
                User_ID = dto.Participant_ID,
                Participant_ID = dto.End_User_ID,
                Requested = 0,
                Approved = 1,
                Blocked = 0
            }).Result);
        }

        [HttpPost("Reject_Invite")]
        public async Task<ActionResult<string>> Update_Block_for_WebSocket_PermissionTbl(WebSocket_Chat_PermissionDTO dto)
        {
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

            dto.Participant_ID = ulong.Parse(AES.Process_Decryption(dto.User));

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
                Controller = "WebSocket",
                Action = "Reject_Invite"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            return await Task.FromResult(_UsersRepository.Delete_Chat_Web_Socket_Permissions_Tbl(new WebSocket_Chat_PermissionTbl
            {
                User_ID = dto.Participant_ID,
                Participant_ID = dto.End_User_ID,
                Requested = 0,
                Approved = 0,
                Blocked = 0,
                Deleted_by = dto.End_User_ID
            }).Result);
        }

        [HttpPost("Report_User")]
        public async Task<ActionResult<string>> Report_User(Reported_WebSocketDTO dto)
        {
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

            dto.Participant_ID = ulong.Parse(AES.Process_Decryption(dto.user));
            dto.Report_type = AES.Process_Decryption(dto.Report_type);

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
                Controller = "WebSocket",
                Action = "Report_User"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            await Task.FromResult(_UsersRepository.Create_Reported_WebSocket_Records(dto).Result);

            return await Task.FromResult(_UsersRepository.Update_Chat_Web_Socket_Permissions_Tbl(new WebSocket_Chat_PermissionTbl
            {
                User_ID = dto.End_User_ID,
                Participant_ID = dto.Participant_ID,
                Requested = 0,
                Approved = 0,
                Blocked = 1
            }).Result);
        }

        [HttpPost("Authorize_Users")]
        public async Task<ActionResult<string>> Authorization_Between_End_Users_Chat_Permissions([FromBody] WebSocket_Chat_PermissionDTO dto)
        {
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
            dto.Participant_ID = ulong.Parse(AES.Process_Decryption(dto.User));

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
                Controller = "WebSocket",
                Action = "Authorize_Users"
            }).Result)
                return Conflict();

            dto.End_User_ID = dto.JWT_id;

            if (!_UsersRepository.ID_Exists_In_Users_IDTbl(dto.End_User_ID).Result || 
                !_UsersRepository.ID_Exists_In_Users_IDTbl(dto.Participant_ID).Result)
                return BadRequest();

            return await Task.FromResult(_UsersRepository.Read_WebSocket_Permission_Record_For_Both_End_Users(dto)).Result;
        }
    }
}