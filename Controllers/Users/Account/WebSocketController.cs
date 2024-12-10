using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Authentication;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Index;


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

        [HttpGet("End_User/{token}")]
        public async Task<ActionResult<string>> Get_EndUser_Websocket_Data(string token)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
                return BadRequest();

            ulong user_id = _UsersRepository.Read_User_ID_By_JWToken(token).Result;
            return await Task.FromResult(_UsersRepository.Read_End_User_Web_Socket_Data(user_id).Result);
        }

        [HttpPost("Approve")]
        public async Task<ActionResult<string>> Update_Approve_for_WebSocket_PermissionTbl(Websocket_Chat_PermissionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            ulong user_id = _UsersRepository.Read_User_ID_By_JWToken(dto.Token).Result;

            Websocket_Chat_PermissionTbl Approver = new Websocket_Chat_PermissionTbl
            {
                User_A_id = user_id,
                User_B_id = dto.User_B_id,
                Requested = 0,
                Approved = 1,
                Blocked = 0
            };

            Websocket_Chat_PermissionTbl Recipient = new Websocket_Chat_PermissionTbl
            {
                User_A_id = dto.User_B_id,
                User_B_id = user_id,
                Requested = 0,
                Approved = 1,
                Blocked = 0
            };

            await _UsersRepository.Update_Chat_Web_Socket_Permissions_Tbl(Recipient);

            return await Task.FromResult(_UsersRepository.Update_Chat_Web_Socket_Permissions_Tbl(Approver).Result);
        }

        [HttpPost("Block")]
        public async Task<ActionResult<string>> Update_Block_for_WebSocket_PermissionTbl(Websocket_Chat_PermissionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            ulong user_id = _UsersRepository.Read_User_ID_By_JWToken(dto.Token).Result;

            Websocket_Chat_PermissionTbl permission_obj = new Websocket_Chat_PermissionTbl
            {
                User_A_id = user_id,
                User_B_id = dto.User_B_id,
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            return await Task.FromResult(_UsersRepository.Update_Chat_Web_Socket_Permissions_Tbl(permission_obj).Result);
        }

        [HttpPut("Abusive")]
        public async Task<ActionResult<string>> Report_Abusive_User(Reported_WebSocket_AbuseDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            ulong user_id = _UsersRepository.Read_User_ID_By_JWToken(dto.Token).Result;

            Reported_WebSocket_AbuseDTO abuse_obj = new Reported_WebSocket_AbuseDTO
            {
                User_id = user_id,
                Abuser = dto.Abuser,
                Abuse_type = dto.Abuse_type,
                Reason = dto.Reason
            };

            Websocket_Chat_PermissionTbl permission_obj = new Websocket_Chat_PermissionTbl
            {
                User_A_id = user_id,
                User_B_id = dto.Abuser,
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            await Task.FromResult(_UsersRepository.Create_Reported_WebSocket_Abuse_Record(abuse_obj).Result);

            return await Task.FromResult(_UsersRepository.Update_Chat_Web_Socket_Permissions_Tbl(permission_obj).Result);
        }

        [HttpPost("Authorize_Users")]
        public async Task<ActionResult<string>> Authorizating_End_Users_Chat_Permission([FromBody] Websocket_Chat_PermissionDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            ulong user_id = _UsersRepository.Read_User_ID_By_JWToken(dto.Token).Result;

            if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result || 
                !_UsersRepository.ID_Exists_In_Users_Tbl(dto.User_B_id).Result)
                return BadRequest();

            dto.User_A_id = user_id;

            return await Task.FromResult(_UsersRepository.Read_WebSocket_Permission_Record(dto)).Result;
        }
    }//Controller.
}