using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Authentication.Completed.Email;
using mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.TimeStamps;
using mpc_dotnetc_user_server.Models.Users.Authentication.Reported;
using mpc_dotnetc_user_server.Models.Users.Index;

namespace mpc_dotnetc_user_server.Controllers.Users.Feedback
{
    [ApiController]
    [Route("api/Report")]
    public class ReportController : ControllerBase
    {
        private readonly ILogger<ReportController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        AES AES = new AES();

        public ReportController(ILogger<ReportController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }
        [HttpPost("Broken_Link")]
        public async Task<ActionResult<bool>> EmailRegister([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.URL) || string.IsNullOrWhiteSpace(dto.URL))
                    return BadRequest();

                ulong user_id =JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
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
        public async Task<ActionResult<bool>> ContactUsRegister([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.Subject_line) || string.IsNullOrWhiteSpace(dto.Subject_line) ||
                    string.IsNullOrEmpty(dto.Summary) || string.IsNullOrWhiteSpace(dto.Summary))
                    return BadRequest();

                ulong user_id =JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
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
        public async Task<ActionResult<bool>> DiscordBotBugRegisterRegister([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.Location) || string.IsNullOrWhiteSpace(dto.Location) ||
                    string.IsNullOrEmpty(dto.Detail) || string.IsNullOrWhiteSpace(dto.Detail))
                    return BadRequest();

                ulong user_id =JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
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
        public async Task<ActionResult<bool>> Comment_BoxRegister([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.Comment) || string.IsNullOrWhiteSpace(dto.Comment))
                    return BadRequest();

                ulong user_id =JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
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
        public async Task<ActionResult<string>> ReportUserProfile([FromBody] DTO dto)
        {
            try
            {
                ulong user_id =JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
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
        public async Task<ActionResult<bool>> Website_BugRegister([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrEmpty(dto.URL) || string.IsNullOrWhiteSpace(dto.URL) ||
                    string.IsNullOrEmpty(dto.Token) || string.IsNullOrWhiteSpace(dto.Token) ||
                    string.IsNullOrEmpty(dto.Detail) || string.IsNullOrWhiteSpace(dto.Detail))
                    return BadRequest();

                ulong user_id = JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Unauthorized();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                dto.ID = user_id;

                return await Task.FromResult(_UsersRepository.Create_Website_Bug_Record(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Email/Reregistration/Attempt")]
        public async Task<ActionResult<string>> Record_Email_Reregistration_Attempt([FromBody] Report_Email_RegistrationDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                dto.Email_Address = AES.Process_Decryption(dto.Email_Address);
                dto.Language = AES.Process_Decryption(dto.Language);
                dto.Region = AES.Process_Decryption(dto.Region);
                dto.Client_time = AES.Process_Decryption(dto.Client_time);
                dto.Location = AES.Process_Decryption(dto.Location);

                if (!Valid.Email(dto.Email_Address) ||
                    !Valid.Language_Code(dto.Language) ||
                    !Valid.Region_Code(dto.Region))
                    return BadRequest();

                return await Task.FromResult(_UsersRepository.Insert_Report_Email_RegistrationTbl(new Report_Email_RegistrationDTO
                {
                    User_id = _UsersRepository.Read_User_ID_By_Email_Address(dto.Email_Address).Result,
                    Client_time = dto.Client_time,
                    Location = dto.Location,
                    Language = dto.Language,
                    Region = dto.Region,
                    Email_Address = dto.Email_Address,
                    Client_Networking_IP_Address = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "error",
                    Client_Networking_Port = HttpContext.Connection.RemotePort,
                    Server_Networking_IP_Address = HttpContext.Connection.LocalIpAddress?.ToString() ?? "error",
                    Server_Networking_Port = HttpContext.Connection.LocalPort,
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}//NameSpace.