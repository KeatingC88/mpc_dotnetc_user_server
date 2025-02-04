using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Authentication.Report;
using mpc_dotnetc_user_server.Models.Users.Feedback;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users._Index;

namespace mpc_dotnetc_user_server.Controllers.Users.Feedback
{
    [ApiController]
    [Route("api/Report")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        

        public ReportController(ILogger<ReportController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }
        [HttpPost("Broken_Link")]
        public async Task<ActionResult<bool>> EmailRegister([FromBody] Reported_Broken_LinkDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.URL) || string.IsNullOrWhiteSpace(dto.URL))
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return NotFound();

                dto.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Broken_Link_Record(dto).Result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Contact_Us")]
        public async Task<ActionResult<bool>> ContactUsRegister([FromBody] Contact_UsDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.Subject_line) || string.IsNullOrWhiteSpace(dto.Subject_line) ||
                    string.IsNullOrEmpty(dto.Summary) || string.IsNullOrWhiteSpace(dto.Summary))
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return NotFound();

                dto.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Contact_Us_Record(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Discord_Bot_Bug")]
        public async Task<ActionResult<bool>> DiscordBotBugRegisterRegister([FromBody] Reported_Discord_Bot_BugDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.Location) || string.IsNullOrWhiteSpace(dto.Location) ||
                    string.IsNullOrEmpty(dto.Detail) || string.IsNullOrWhiteSpace(dto.Detail))
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return NotFound();

                dto.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Discord_Bot_Bug_Record(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Comment_Box")]
        public async Task<ActionResult<bool>> Comment_BoxRegister([FromBody] Comment_BoxDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return NotFound();

                dto.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Comment_Box_Record(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("User")]
        public async Task<ActionResult<string>> ReportUserProfile([FromBody] Reported_ProfileDTO dto)
        {
            try
            {
                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return NotFound();

                dto.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Reported_User_Profile_Record(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Website_Bug")]
        public async Task<ActionResult<bool>> Website_BugRegister([FromBody] Reported_Website_BugDTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.URL) || string.IsNullOrWhiteSpace(dto.URL) ||
                    string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.Detail) || string.IsNullOrWhiteSpace(dto.Detail))
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return NotFound();

                dto.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Website_Bug_Record(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}//NameSpace.