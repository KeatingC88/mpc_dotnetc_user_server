using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Interfaces.IUsers_Respository;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.WebSocket.Chat;
using mpc_dotnetc_user_server.Repositories.SQLite.Users_Repository;
using System.Net.Http;


namespace mpc_dotnetc_user_server.Controllers.Users.WebSocket
{
    [ApiController]
    [Route("api/WebSocket")]
    public class WebSocketController : ControllerBase
    {
        private readonly ILogger<WebSocketController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers_Repository Users_Repository;
        private readonly IUsers_Repository_Read Users_Repository_Read;
        private readonly IUsers_Repository_Delete Users_Repository_Delete;
        private readonly IUsers_Repository_Update Users_Repository_Update;
        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly INetwork Network;
        public WebSocketController(
            ILogger<WebSocketController> logger, 
            IConfiguration configuration, 
            IUsers_Repository users_repository,
            IUsers_Repository_Read users_repository_read,
            IUsers_Repository_Delete users_repository_delete,
            IUsers_Repository_Update users_repository_update,
            IJWT jwt,
            IAES aes,
            INetwork network
            )
        {
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
            Users_Repository_Read = users_repository_read;
            Users_Repository_Delete = users_repository_delete;
            Users_Repository_Update = users_repository_update;
            JWT = jwt;
            AES = aes;
            Network = network;
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
            dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption(dto.Client_time));
            dto.Login_type = AES.Process_Decryption(dto.Login_type);

            dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID));
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

            if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
            {
                Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Port = HttpContext.Connection.LocalPort,
                Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                JWT_client_address = dto.JWT_client_address,
                JWT_client_key = dto.JWT_client_key,
                JWT_issuer_key = dto.JWT_issuer_key,
                Token = dto.Token,
                Client_id = dto.Client_id,
                JWT_id = dto.JWT_id,
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
                Controller = "WebSocket",
                Action = "Chat_Requests/Participant"
            }).Result)
                return Conflict();

           

            return await Task.FromResult(Users_Repository_Read.Read_End_User_WebSocket_Received_Chat_Requests(dto.JWT_id).Result);
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
            dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption(dto.Client_time));
            dto.Login_type = AES.Process_Decryption(dto.Login_type);

            dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID));
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
            
            if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
            {
                Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Port = HttpContext.Connection.LocalPort,
                JWT_client_address = dto.JWT_client_address,
                JWT_client_key = dto.JWT_client_key,
                JWT_issuer_key = dto.JWT_issuer_key,
                Token = dto.Token,
                Client_id = dto.Client_id,
                JWT_id = dto.JWT_id,
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
                Controller = "WebSocket",
                Action = "Chat_Blocks/Participant"
            }).Result)
                return Conflict();

           

            return await Task.FromResult(Users_Repository_Read.Read_End_User_WebSocket_Received_Chat_Blocks(dto.JWT_id).Result);
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
            dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption(dto.Client_time));
            dto.Login_type = AES.Process_Decryption(dto.Login_type);

            dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID));
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

            if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
            {
                Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Port = HttpContext.Connection.LocalPort,
                Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                JWT_client_address = dto.JWT_client_address,
                JWT_client_key = dto.JWT_client_key,
                JWT_issuer_key = dto.JWT_issuer_key,
                Token = dto.Token,
                Client_id = dto.Client_id,
                JWT_id = dto.JWT_id,
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
                Controller = "WebSocket",
                Action = "Chat_Approvals/Participant"
            }).Result)
                return Conflict();

           

            return await Task.FromResult(Users_Repository_Read.Read_End_User_WebSocket_Received_Chat_Approvals(dto.JWT_id).Result);
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
            dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption(dto.Client_time));
            dto.Login_type = AES.Process_Decryption(dto.Login_type);

            dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID));
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

            if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
            {
                Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Port = HttpContext.Connection.LocalPort,
                Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                JWT_client_address = dto.JWT_client_address,
                JWT_client_key = dto.JWT_client_key,
                JWT_issuer_key = dto.JWT_issuer_key,
                Token = dto.Token,
                Client_id = dto.Client_id,
                JWT_id = dto.JWT_id,
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
                Controller = "WebSocket",
                Action = "Chat_Requests/End_User"
            }).Result)
                return Conflict();

           

            return await Task.FromResult(Users_Repository_Read.Read_End_User_WebSocket_Sent_Chat_Requests(dto.JWT_id).Result);
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
            dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption(dto.Client_time));
            dto.Login_type = AES.Process_Decryption(dto.Login_type);

            dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID));
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

            if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
            {
                Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Port = HttpContext.Connection.LocalPort,
                Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                JWT_client_address = dto.JWT_client_address,
                JWT_client_key = dto.JWT_client_key,
                JWT_issuer_key = dto.JWT_issuer_key,
                Token = dto.Token,
                Client_id = dto.Client_id,
                JWT_id = dto.JWT_id,
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
                Controller = "WebSocket",
                Action = "Chat_Blocks/End_User"
            }).Result)
                return Conflict();

           

            return await Task.FromResult(Users_Repository_Read.Read_End_User_WebSocket_Sent_Chat_Blocks(dto.JWT_id).Result);
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
            dto.Client_Time_Parsed = long.Parse(AES.Process_Decryption(dto.Client_time));
            dto.Login_type = AES.Process_Decryption(dto.Login_type);

            dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID));
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

            if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
            {
                Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Port = HttpContext.Connection.LocalPort,
                Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                JWT_client_address = dto.JWT_client_address,
                JWT_client_key = dto.JWT_client_key,
                JWT_issuer_key = dto.JWT_issuer_key,
                Token = dto.Token,
                Client_id = dto.Client_id,
                JWT_id = dto.JWT_id,
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
                Controller = "WebSocket",
                Action = "Chat_Approves/End_User"
            }).Result)
                return Conflict();

           

            return await Task.FromResult(Users_Repository_Read.Read_End_User_WebSocket_Sent_Chat_Approvals(dto.JWT_id).Result);
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

            dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID));
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

            dto.Participant_ID = long.Parse(AES.Process_Decryption(dto.User));

            if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
            {
                Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Port = HttpContext.Connection.LocalPort,
                Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                JWT_client_address = dto.JWT_client_address,
                JWT_client_key = dto.JWT_client_key,
                JWT_issuer_key = dto.JWT_issuer_key,
                Token = dto.Token,
                Client_id = dto.Client_id,
                JWT_id = dto.JWT_id,
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
                Controller = "WebSocket",
                Action = "Approve_Invite"
            }).Result)
                return Conflict();

           

            return await Task.FromResult(Users_Repository_Update.Update_Chat_Web_Socket_Permissions(new WebSocket_Chat_Permission
            {
                End_User_ID = dto.Participant_ID,
                Participant_ID = dto.JWT_id,
                Requested = false,
                Approved = true,
                Blocked = false
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

            dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID));
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

            dto.Participant_ID = long.Parse(AES.Process_Decryption(dto.User));

            if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
            {
                Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Port = HttpContext.Connection.LocalPort,
                Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                JWT_client_address = dto.JWT_client_address,
                JWT_client_key = dto.JWT_client_key,
                JWT_issuer_key = dto.JWT_issuer_key,
                Token = dto.Token,
                Client_id = dto.Client_id,
                JWT_id = dto.JWT_id,
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
                Controller = "WebSocket",
                Action = "Reject_Invite"
            }).Result)
                return Conflict();

            return await Task.FromResult(Users_Repository_Delete.Delete_From_Web_Socket_Chat_Permissions(new WebSocket_Chat_Permission
            {
                End_User_ID = dto.Participant_ID,
                Participant_ID = dto.JWT_id,
                Requested = false,
                Approved = false,
                Blocked = false,
                Deleted_by = dto.JWT_id
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

            dto.Client_id = long.Parse(AES.Process_Decryption(dto.End_User_ID));
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
            dto.Participant_ID = long.Parse(AES.Process_Decryption(dto.User));

            if (!Users_Repository.Validate_Client_With_Server_Authorization(new Report_Failed_Authorization_History
            {
                Remote_IP = Network.Get_Client_Remote_Internet_Protocol_Address().Result,
                Remote_Port = Network.Get_Client_Remote_Internet_Protocol_Port().Result,
                Server_IP = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                Server_Port = HttpContext.Connection.LocalPort,
                Client_IP = Network.Get_Client_Internet_Protocol_Address().Result,
                Client_Port = Network.Get_Client_Internet_Protocol_Port().Result,
                JWT_client_address = dto.JWT_client_address,
                JWT_client_key = dto.JWT_client_key,
                JWT_issuer_key = dto.JWT_issuer_key,
                Token = dto.Token,
                Client_id = dto.Client_id,
                JWT_id = dto.JWT_id,
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
                Controller = "WebSocket",
                Action = "Authorize_Users"
            }).Result)
                return Conflict();

           

            if (!Users_Repository_Read.Read_ID_Exists_In_Users_ID(dto.JWT_id).Result || 
                !Users_Repository_Read.Read_ID_Exists_In_Users_ID(dto.Participant_ID).Result)
                return BadRequest();

            return await Task.FromResult(Users_Repository_Read.Read_WebSocket_Permission_Record_For_Both_End_Users(new WebSocket_Chat_Permission
            {
                End_User_ID = dto.Participant_ID,
                Participant_ID = dto.JWT_id,
                Requested = false,
                Approved = false,
                Blocked = false,
                Deleted_by = dto.JWT_id
            })).Result;
        }
    }
}