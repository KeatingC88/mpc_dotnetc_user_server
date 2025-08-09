using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Interfaces;
using mpc_dotnetc_user_server.Models.Users.Integration.Twitch;

namespace mpc_dotnetc_user_server.Controllers.Users.Integration
{
    [ApiController]
    [Route("api/Integration")]
    public class IntegrationController : ControllerBase
    {
        private readonly ILogger<IntegrationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsers_Repository Users_Repository;

        public IntegrationController(
            ILogger<IntegrationController> logger,
            IConfiguration configuration,
            IUsers_Repository users_repository
        )
        {
            _logger = logger;
            _configuration = configuration;
            Users_Repository = users_repository;
        }

        [HttpPost("Twitch")]
        public async Task<ActionResult<string>> Create_Twitch_Record([FromBody] Integration_TwitchDTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_Address) || string.IsNullOrWhiteSpace(obj.Email_Address))
                    return BadRequest();

                /* If Twitch Email Matches one our accounts in out database...?
                    if (!Users_Database_Context.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address))
                        return NotFound();
                    ulong user_id = Users_Database_Context.Read_User_id_By_Email_Address(obj.Email_Address);
                    if (user_id == 0 || !Users_Repository.ID_Exists_In_Users_IDTbl(user_id).Result)
                        return NotFound();
                    obj.ID = user_id;
                */


                return await Task.FromResult(Users_Repository.Create_Integration_Twitch_Record(obj)).Result;//return timestamp and id
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }//Twitch.

        [HttpPost("Email")]
        public async Task<ActionResult<string>> Create_Enail_Record([FromBody] Integration_TwitchDTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_Address) || string.IsNullOrWhiteSpace(obj.Email_Address))
                    return BadRequest();

                /* If Twitch Email Matches one our accounts in out database...?
                    if (!Users_Database_Context.Email_Exists_In_Login_Email_AddressTbl(obj.Email_Address))
                        return NotFound();
                    ulong user_id = Users_Database_Context.Read_User_id_By_Email_Address(obj.Email_Address);
                    if (user_id == 0 || !Users_Repository.ID_Exists_In_Users_IDTbl(user_id).Result)
                        return NotFound();
                    obj.ID = user_id;
                */


                return await Task.FromResult(Users_Repository.Create_Integration_Twitch_Record(obj)).Result;//return timestamp and id
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }//Email.
    }//Controller.
}//Namespace.