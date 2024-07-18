using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users;
using mpc_dotnetc_user_server.Models.Users.Chat;


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

            ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(token).Result;

            if (user_id == 0)
                return BadRequest();

            DTO obj = new DTO
            {
                ID = user_id,
                Token = token
            };

            return await Task.FromResult(_UsersRepository.Read_End_User_Web_Socket_Data(obj).Result);
        }

        [HttpPost("Approve")]
        public async Task<ActionResult<bool>> UpdateApprovedChatWebSocketLogTbl(DTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest();

            ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

            if (user_id == 0)
                return BadRequest();

            DTO Approver = new DTO
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.ID,//Approved by this person.
                Sent_from = dto.Send_to,//Approving the person made the request prior to now.
                Requested = 0,
                Approved = 1,
                Blocked = 0
            };

            DTO Recipient = new DTO
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.Send_to,//Updating other chatter's data too.
                Sent_from = user_id,
                Requested = 0,
                Approved = 1,
                Blocked = 0
            };

            await _UsersRepository.Update_Chat_Web_Socket_Log_Tbl(Recipient);

            return await Task.FromResult(_UsersRepository.Update_Chat_Web_Socket_Log_Tbl(Approver).Result);
        }

        [HttpPost("Block")]
        public async Task<ActionResult<bool>> UpdateBlockedChatWebSocketLogTbl(DTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest();

            ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

            if (user_id == 0)
                return BadRequest();

            DTO Person_Blocking_Their_Chat_From_Someone_Else = new DTO
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.ID,//Blocked by this person which is the End User.
                Sent_from = dto.Send_to,//Person being BLocked.
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            DTO Blocked_Recipient = new DTO
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.Send_to,//Updating other chatter's data too.
                Sent_from = user_id,//Updated by
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            await _UsersRepository.Update_Chat_Web_Socket_Log_Tbl(Blocked_Recipient);

            return await Task.FromResult(_UsersRepository.Update_Chat_Web_Socket_Log_Tbl(Person_Blocking_Their_Chat_From_Someone_Else).Result);
        }

        [HttpPut("Spam")]
        public async Task<ActionResult<bool>> UpdateSpamChatWebSocketLogTbl(DTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest();

            ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

            if (user_id == 0)
                return BadRequest();

            DTO obj = new DTO
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.ID,//Approved by this person.
                Sent_from = dto.Send_to,//Approving the person making the request.
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            //Todo: Send Player Report

            return await Task.FromResult(_UsersRepository.Update_Chat_Web_Socket_Log_Tbl(obj).Result);
        }

        [HttpPut("Abusive")]
        public async Task<ActionResult<bool>> UpdateAbusiveChatWebSocketLogTbl(DTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token))
                return BadRequest();

            ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

            if (user_id == 0)
                return BadRequest();

            DTO obj = new DTO
            {
                ID = user_id,//End User that is using this API POST derrived from JWToken.
                Sent_to = dto.ID,//Approved by this person.
                Sent_from = dto.Send_to,//Approving the person making the request.
                Requested = 0,
                Approved = 0,
                Blocked = 1
            };

            //Todo: Send Player Report

            return await Task.FromResult(_UsersRepository.Update_Chat_Web_Socket_Log_Tbl(obj).Result);
        }

        [HttpPost("Authorize_Users")]
        public async Task<ActionResult<string>> CheckingBothEndUsersAuthorizationToChatTogether([FromBody] DTO dto)
        {
            if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                string.IsNullOrEmpty(dto.Message) || string.IsNullOrWhiteSpace(dto.Message) ||
                string.IsNullOrEmpty(dto.Send_to.ToString()) || string.IsNullOrWhiteSpace(dto.Send_to.ToString()))
                return BadRequest();

            ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

            if (user_id == 0)
                return BadRequest();

            if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result || !_UsersRepository.ID_Exists_In_Users_Tbl(dto.Send_to).Result)
                return BadRequest();

            dto.ID = user_id;

            return await Task.FromResult(_UsersRepository.Read_Chat_Web_Socket_Log_Record(dto)).Result;
        }
    }//Controller.
}