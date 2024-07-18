using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users;

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
        public async Task<ActionResult<bool>> EmailRegister([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.URL) || string.IsNullOrWhiteSpace(obj.URL))
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Broken_Link_Record(obj).Result);
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Contact_Us")]
        public async Task<ActionResult<bool>> ContactUsRegister([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Subject_line) || string.IsNullOrWhiteSpace(obj.Subject_line) ||
                    string.IsNullOrEmpty(obj.Summary) || string.IsNullOrWhiteSpace(obj.Summary))
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Contact_Us_Record(obj)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Discord_Bot_Bug")]
        public async Task<ActionResult<bool>> DiscordBotBugRegisterRegister([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Location) || string.IsNullOrWhiteSpace(obj.Location) ||
                    string.IsNullOrEmpty(obj.Detail) || string.IsNullOrWhiteSpace(obj.Detail))
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Discord_Bot_Bug_Record(obj)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Comment_Box")]
        public async Task<ActionResult<bool>> Comment_BoxRegister([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Comment) || string.IsNullOrWhiteSpace(obj.Comment))
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Comment_Box_Record(obj)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("User")]
        public async Task<ActionResult<string>> ReportUserRegister([FromBody] DTO obj)
        {
            try
            {
                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Reported_User_Record(obj)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Website_Bug")]
        public async Task<ActionResult<bool>> Website_BugRegister([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.URL) || string.IsNullOrWhiteSpace(obj.URL) ||
                    string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Detail) || string.IsNullOrWhiteSpace(obj.Detail))
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Website_Bug_Record(obj)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}//NameSpace.