using Microsoft.AspNetCore.Mvc;
using dotnet_user_server.Models.Users;

namespace dotnet_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Load")]
    public class LoadController : ControllerBase
    {
        private readonly ILogger<LoadController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UsersDbC UsersDbC;//EFCore -> Database
        public LoadController(ILogger<LoadController> logger, IConfiguration configuration, UsersDbC context)
        {
            _logger = logger;
            _configuration = configuration;
            UsersDbC = context;
        }

        [HttpGet("All_Users")]
        public async Task<ActionResult<string>> LoadAllUsers()
        {
            try {
                return await Task.FromResult(UsersDbC.Read_Users());
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("User")]
        public async Task<ActionResult<string>> LoadUser([FromBody] DTO dto)
        {
            try
            {
                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(dto.Token);

                if (user_id == 0)
                    return Ok();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return Ok();

                DTO obj = new DTO
                {
                    ID = user_id,
                    Token = dto.Token
                };

                return await Task.FromResult(UsersDbC.Read_User(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("User/Profile")]
        public async Task<ActionResult<string>> LoadUserProfile([FromBody] DTO dto)
        {
            try
            {
                ulong user_id = UsersDbC.Get_User_ID_From_JWToken(dto.Token);

                if (user_id == 0)
                    return Ok();

                if (!UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return Ok();

                DTO obj = new DTO
                {
                    ID = dto.ID,//Targetted User's ID that we use to retrieve data.
                    Token = dto.Token//Authorizing End User Requesting the Targetted ID.
                };

                return await Task.FromResult(UsersDbC.Read_User_Profile(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}