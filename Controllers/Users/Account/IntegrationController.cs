using Microsoft.AspNetCore.Mvc;
using dotnet_user_server.Models.Users;
using System.Text;

namespace dotnet_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Integration")]
    public class IntegrationController : ControllerBase
    {
        private readonly ILogger<IntegrationController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UsersDbC UsersDbC;//Database Model

        public IntegrationController(ILogger<IntegrationController> logger, IConfiguration configuration, UsersDbC context)
        {
            _logger = logger;
            _configuration = configuration;
            UsersDbC = context;
        }

        [HttpPost("Twitch")]
        public async Task<ActionResult<string>> Create_Twitch_Record([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_address) || string.IsNullOrWhiteSpace(obj.Email_address))
                    return BadRequest();

                /* If Twitch Email Matches one our accounts in out database...?
                    if (!UsersDbC.Email_Exists_In_Login_EmailAddress_Tbl(obj.Email_address))
                        return NotFound();
                    ulong user_id = UsersDbC.Get_User_ID_By_Email_Address(obj.Email_address);
                    if (user_id == 0 || !UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                        return NotFound();
                    obj.ID = user_id;
                */


                return await Task.FromResult(UsersDbC.Create_Integration_Twitch_Record(obj));//return timestamp and id
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//Twitch.
    }//Controller.
}//Namespace.