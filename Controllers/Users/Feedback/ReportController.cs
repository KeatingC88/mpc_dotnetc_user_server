using Microsoft.AspNetCore.Mvc;
using dotnet_user_server.Models.Users;
using dotnet_user_server.Models.Users.Feedback;
using System.Text.RegularExpressions;

namespace dotnet_user_server.Controllers.Users.Feedback
{
    [ApiController]
    [Route("api/Report")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UsersDbC UsersDbC;//EFCore -> Database
        public ReportController(ILogger<ReportController> logger, IConfiguration configuration, UsersDbC context)
        {
            _logger = logger;
            _configuration = configuration;
            UsersDbC = context;
        }
        [HttpPost("Broken_Link")]
        public async Task<ActionResult<bool>> EmailRegister([FromBody] Broken_LinkDTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.URL) || string.IsNullOrWhiteSpace(obj.URL))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Create_Broken_Link_Record(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Contact_Us")]
        public async Task<ActionResult<bool>> ContactUsRegister([FromBody] Contact_UsDTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Subject_Line) || string.IsNullOrWhiteSpace(obj.Subject_Line) ||
                    string.IsNullOrEmpty(obj.Summary) || string.IsNullOrWhiteSpace(obj.Summary))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Create_Contact_Us_Record(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Discord_Bot_Bug")]
        public async Task<ActionResult<bool>> DiscordBotBugRegisterRegister([FromBody] Discord_Bot_BugDTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Location) || string.IsNullOrWhiteSpace(obj.Location) ||
                    string.IsNullOrEmpty(obj.Detail) || string.IsNullOrWhiteSpace(obj.Detail))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Create_Discord_Bot_Bug_Record(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Comment_Box")]
        public async Task<ActionResult<bool>> Comment_BoxRegister([FromBody] Comment_BoxDTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Comment) || string.IsNullOrWhiteSpace(obj.Comment))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Create_Comment_Box_Record(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("User")]
        public async Task<ActionResult<string>> ReportUserRegister([FromBody] Reported_DTO obj)
        {
            try
            {
                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Create_Reported_User_Record(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Website_Bug")]
        public async Task<ActionResult<bool>> Website_BugRegister([FromBody] Reported_WebsiteBugDTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.URL) || string.IsNullOrWhiteSpace(obj.URL) ||
                    string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Detail) || string.IsNullOrWhiteSpace(obj.Detail))
                    return BadRequest();

                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);

                if (user_id == 0)
                    return Unauthorized();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                obj.ID = user_id;

                return await Task.FromResult(UsersDbC.Create_Website_Bug_Record(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}//NameSpace.