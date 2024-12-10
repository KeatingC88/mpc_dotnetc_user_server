using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;


namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Load")]
    public class LoadController : ControllerBase
    {
        private readonly ILogger<LoadController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;
        
        public LoadController(ILogger<LoadController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }

        [HttpGet("All_Users")]
        public async Task<ActionResult<string>> LoadAllUsers()
        {
            try {
                return await _UsersRepository.Read_Users();
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("User")]
        public async Task<ActionResult<string>> LoadUser([FromBody] DTO dto)
        {
            try
            {
                ulong user_id = _UsersRepository.Read_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                DTO obj = new DTO
                {
                    ID = user_id,
                    Token = dto.Token
                };

                return await _UsersRepository.Read_User(obj.ID);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("User/Profile")]
        public async Task<ActionResult<string>> LoadUserProfile([FromBody] DTO dto)
        {
            try
            {
                ulong user_id = _UsersRepository.Read_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                return await _UsersRepository.Read_User_Profile(new DTO
                {
                    ID = dto.ID,//Targetted User's ID that we use to retrieve data.
                    Token = dto.Token//Authorizing End User Requesting the Targetted ID.
                });
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}