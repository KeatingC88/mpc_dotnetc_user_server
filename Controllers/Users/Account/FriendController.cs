using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Interfaces;
using mpc_dotnetc_user_server.Models.Report;
using mpc_dotnetc_user_server.Models.Users.Report;
using mpc_dotnetc_user_server.Models.Users.Friends;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Friend")]
    public class FriendController : ControllerBase
    {
        private readonly ILogger<FriendController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers_Repository Users_Repository;
        private readonly IAES AES;
        private readonly IJWT JWT;
        private readonly INetwork Network;
        public FriendController(
            ILogger<FriendController> logger,
            IConfiguration configuration,
            IUsers_Repository users_repository,
            IJWT jwt,
            IAES aes,
            INetwork network
            )
        {
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
            JWT = jwt;
            AES = aes;
            Network = network;
        }

        [HttpPost("Permissions")]
        public async Task<ActionResult<string>> Read_End_User_Friend_Permission_Data(Friends_PermissionDTO dto)
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

            dto.End_User_ID_Parsed = dto.JWT_id;

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
                Client_Time_Parsed = dto.Client_Time_Parsed,
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
                Controller = "Friend",
                Action = "Chat_Requests/Participant"
            }).Result)
                return Conflict();

            return await Task.FromResult(Users_Repository.Read_End_User_Friend_Permissions_By_ID(dto.End_User_ID_Parsed).Result);
        }

        [HttpPut("Approve")]
        public async Task<ActionResult<string>> Update_Friend_Approve_Permission(Friends_PermissionDTO dto)
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

            dto.Participant_ID_Parsed = long.Parse(AES.Process_Decryption(dto.Participant_ID));
            dto.End_User_ID_Parsed = dto.JWT_id;

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
                Client_Time_Parsed = dto.Client_Time_Parsed,
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
                Controller = "Friend",
                Action = "Approve_Invite"
            }).Result)
                return Conflict();

            return await Task.FromResult(Users_Repository.Update_Friend_Permissions(new Friends_Permission
            {
                End_User_ID = dto.End_User_ID_Parsed,
                Participant_ID = dto.Participant_ID_Parsed,
                Approved = true
            }).Result);
        }

        [HttpPut("Reject")]
        public async Task<ActionResult<string>> Update_Friend_Reject_Permission(Friends_PermissionDTO dto)
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

            dto.Participant_ID_Parsed = long.Parse(AES.Process_Decryption(dto.Participant_ID));
            dto.End_User_ID_Parsed = dto.JWT_id;

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
                Client_Time_Parsed = dto.Client_Time_Parsed,
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
                Controller = "Friend",
                Action = "Reject_Invite"
            }).Result)
                return Conflict();

            return await Task.FromResult(Users_Repository.Update_Friend_Permissions(new Friends_Permission
            {
                End_User_ID = dto.End_User_ID_Parsed,
                Participant_ID = dto.Participant_ID_Parsed,
                Deleted = true
            }).Result);
        }

        [HttpPost("Request")]
        public async Task<ActionResult<string>> Update_Friend_Request_Permission(Friends_PermissionDTO dto)
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

            dto.Participant_ID_Parsed = long.Parse(AES.Process_Decryption(dto.Participant_ID));
            dto.End_User_ID_Parsed = dto.JWT_id;

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
                Client_id = dto.End_User_ID_Parsed,
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
                Controller = "Friend",
                Action = "Request"
            }).Result)
                return Conflict();

            return await Task.FromResult(AES.Process_Encryption(Users_Repository.Update_Friend_Permissions(new Friends_Permission
            {
                End_User_ID = dto.End_User_ID_Parsed,
                Participant_ID = dto.Participant_ID_Parsed,
                Requested = true
            }).Result));
        }

        [HttpPut("Block")]
        public async Task<ActionResult<string>> Update_Friend_Block_Permission(Friends_PermissionDTO dto)
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

            dto.Participant_ID_Parsed = long.Parse(AES.Process_Decryption(dto.Participant_ID));
            dto.End_User_ID_Parsed = dto.JWT_id;

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
                Client_Time_Parsed = dto.Client_Time_Parsed,
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
                Controller = "Friend",
                Action = "Block"
            }).Result)
                return Conflict();

            return await Task.FromResult(Users_Repository.Update_Friend_Permissions(new Friends_Permission
            {
                End_User_ID = dto.End_User_ID_Parsed,
                Participant_ID = dto.Participant_ID_Parsed,
                Blocked = true
            }).Result);
        }
    }
}