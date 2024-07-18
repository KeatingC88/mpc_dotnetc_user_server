using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Integration")]
    public class IntegrationController : ControllerBase
    {
        private readonly ILogger<IntegrationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        public IntegrationController(ILogger<IntegrationController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }

        [HttpPost("Twitch")]
        public async Task<ActionResult<string>> Create_Twitch_Record([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_address) || string.IsNullOrWhiteSpace(obj.Email_address))
                    return BadRequest();

                /* If Twitch Email Matches one our accounts in out database...?
                    if (!UsersDBC.Email_Exists_In_Login_Email_Address_Tbl(obj.Email_address))
                        return NotFound();
                    ulong user_id = UsersDBC.Get_User_ID_By_Email_Address(obj.Email_address);
                    if (user_id == 0 || !_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                        return NotFound();
                    obj.ID = user_id;
                */


                return await Task.FromResult(_UsersRepository.Create_Integration_Twitch_Record(obj)).Result;//return timestamp and id
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//Twitch.
    }//Controller.
}//Namespace.