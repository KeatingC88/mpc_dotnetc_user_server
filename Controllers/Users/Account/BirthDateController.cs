using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users.BirthDate;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Birth")]
    public class BirthDateController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        public BirthDateController(ILogger<IdentityController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }

        [HttpPost("Date")]
        public async Task<ActionResult<string>> Process_End_User_Birth_Date([FromBody] Birth_DateDTO dto)
        {
            try
            {
                ulong user_id = JWT.JWT.Read_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Conflict();

                return await Task.FromResult(_UsersRepository.Update_End_User_Birth_Date(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}