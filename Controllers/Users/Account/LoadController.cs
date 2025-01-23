using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users._Index;
using mpc_dotnetc_user_server.Models.Users.Profile;


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
        public async Task<ActionResult<string>> LoadUser([FromBody] UserDTO dto)
        {
            try
            {
                ulong user_id = JWT.JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                UserDTO obj = new UserDTO
                {
                    ID = user_id,
                    Token = dto.Token
                };

                return await _UsersRepository.Read_Email_User_Data_By_ID(obj.ID);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("User/Profile")]
        public async Task<ActionResult<string>> LoadUserProfile([FromBody] Read_User_ProfileDTO dto)
        {
            try
            {
                ulong user_id = JWT.JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                return await _UsersRepository.Read_User_Profile_By_ID(dto.ID);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}