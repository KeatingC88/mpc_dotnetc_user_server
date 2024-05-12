using Microsoft.AspNetCore.Mvc;
using dotnet_user_server.Models.Users.Chat;
using dotnet_user_server.Models.Users;

namespace dotnet_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/WebSocket")]
    public class WebSocketController : ControllerBase
    {
        private readonly ILogger<WebSocketController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UsersDbC UsersDbC;//EFCore -> Database
        public WebSocketController(ILogger<WebSocketController> logger, IConfiguration configuration, UsersDbC context)
        {
            _logger = logger;
            _configuration = configuration;
            UsersDbC = context;
        }

        [HttpGet("End_User/{token}")]
        public async Task<ActionResult<string>> Get_EndUser_Websocket_Data(string token)
        {
            if (string.IsNullOrEmpty(token) || string.IsNullOrWhiteSpace(token))
                return BadRequest();

            ulong user_id = UsersDbC.Get_User_ID_From_JWToken(token);

            if (user_id == 0)
                return BadRequest();

            Chat_WebSocketLogDTO obj = new Chat_WebSocketLogDTO
            {
                ID = user_id,
                Token = token
            };

            return await Task.FromResult(UsersDbC.Read_End_User_Web_Socket_Data(obj));
        }

        [HttpPost("Approve")]
        public async Task<ActionResult<bool>> UpdateApprovedChatWebSocketLogTbl(Chat_WebSocketLogDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest();

            ulong user_id = UsersDbC.Get_User_ID_From_JWToken(dto.Token);

            if (user_id == 0)
                return BadRequest();

            Update_ChatWebSocketLogTbl Approver = new Update_ChatWebSocketLogTbl
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.ID,//Approved by this person.
                Sent_from = dto.Send_to,//Approving the person made the request prior to now.
                Requested = 0,
                Approved = 1,
                Blocked = 0
            };

            Update_ChatWebSocketLogTbl Recipient = new Update_ChatWebSocketLogTbl
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.Send_to,//Updating other chatter's data too.
                Sent_from = user_id,
                Requested = 0,
                Approved = 1,
                Blocked = 0
            };

            UsersDbC.Update_Chat_Web_Socket_Log_Tbl(Recipient);

            return await Task.FromResult(UsersDbC.Update_Chat_Web_Socket_Log_Tbl(Approver));
        }

        [HttpPost("Block")]
        public async Task<ActionResult<bool>> UpdateBlockedChatWebSocketLogTbl(Chat_WebSocketLogDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest();

            ulong user_id = UsersDbC.Get_User_ID_From_JWToken(dto.Token);

            if (user_id == 0)
                return BadRequest();

            Update_ChatWebSocketLogTbl Person_Blocking_Their_Chat_From_Someone_Else = new Update_ChatWebSocketLogTbl
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.ID,//Blocked by this person which is the End User.
                Sent_from = dto.Send_to,//Person being BLocked.
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            Update_ChatWebSocketLogTbl Blocked_Recipient = new Update_ChatWebSocketLogTbl
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.Send_to,//Updating other chatter's data too.
                Sent_from = user_id,//Updated by
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            UsersDbC.Update_Chat_Web_Socket_Log_Tbl(Blocked_Recipient);

            return await Task.FromResult(UsersDbC.Update_Chat_Web_Socket_Log_Tbl(Person_Blocking_Their_Chat_From_Someone_Else));
        }

        [HttpPut("Spam")]
        public async Task<ActionResult<bool>> UpdateSpamChatWebSocketLogTbl(Chat_WebSocketLogDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest();

            ulong user_id = UsersDbC.Get_User_ID_From_JWToken(dto.Token);

            if (user_id == 0)
                return BadRequest();

            Update_ChatWebSocketLogTbl obj = new Update_ChatWebSocketLogTbl
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.ID,//Approved by this person.
                Sent_from = dto.Send_to,//Approving the person making the request.
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            //Todo: Send Player Report

            return await Task.FromResult(UsersDbC.Update_Chat_Web_Socket_Log_Tbl(obj));
        }

        [HttpPut("Abusive")]
        public async Task<ActionResult<bool>> UpdateAbusiveChatWebSocketLogTbl(Chat_WebSocketLogDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest();

            ulong user_id = UsersDbC.Get_User_ID_From_JWToken(dto.Token);

            if (user_id == 0)
                return BadRequest();

            Update_ChatWebSocketLogTbl obj = new Update_ChatWebSocketLogTbl
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.ID,//Approved by this person.
                Sent_from = dto.Send_to,//Approving the person making the request.
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            //Todo: Send Player Report

            return await Task.FromResult(UsersDbC.Update_Chat_Web_Socket_Log_Tbl(obj));
        }

        [HttpPost("Authorize_Users")]
        public async Task<ActionResult<string>> CheckingBothEndUsersAuthorizationToChatTogether([FromBody] Chat_WebSocketLogDTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                string.IsNullOrEmpty(dto.Message) || string.IsNullOrWhiteSpace(dto.Message) ||
                string.IsNullOrEmpty(dto.Send_to.ToString()) || string.IsNullOrWhiteSpace(dto.Send_to.ToString()))
                return BadRequest();

            ulong user_id = UsersDbC.Get_User_ID_From_JWToken(dto.Token);

            if (user_id == 0)
                return BadRequest();

            if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id) || !UsersDbC.ID_Exist_In_Users_Tbl(dto.Send_to))
                return BadRequest();

            dto.ID = user_id;

            return await Task.FromResult(UsersDbC.Read_Chat_Web_Socket_Log_Record(dto));
        }
    }//Controller.
}