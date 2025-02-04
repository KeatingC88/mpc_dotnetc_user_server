using Microsoft.AspNetCore.Mvc;
using mpc_dotnetc_user_server.Models.Users.Identity;
using mpc_dotnetc_user_server.Models.Users.Index;

namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Identity")]
    public class IdentityController : ControllerBase
    {
        private readonly ILogger<IdentityController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        public IdentityController(ILogger<IdentityController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }

        [HttpPost("FirstName")]
        public async Task<ActionResult<string>> Process_End_User_First_Name([FromBody] IdentityDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return Conflict();

                return await Task.FromResult(_UsersRepository.Update_End_User_First_Name(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("LastName")]
        public async Task<ActionResult<string>> Process_End_User_Last_Name([FromBody] IdentityDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return Conflict();

                return await Task.FromResult(_UsersRepository.Update_End_User_Last_Name(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("MiddleName")]
        public async Task<ActionResult<string>> Process_End_User_Middle_Name([FromBody] IdentityDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return Conflict();

                return await Task.FromResult(_UsersRepository.Update_End_User_Middle_Name(dto)).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("MaidenName")]
        public async Task<ActionResult<string>> Process_End_User_Maiden_Name([FromBody] IdentityDTO dto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return Conflict();

                return await Task.FromResult(_UsersRepository.Update_End_User_Maiden_Name(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Gender")]
        public async Task<ActionResult<string>> Process_End_User_Gender([FromBody] IdentityDTO dto)
        {
            try
            {
                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return Conflict();

                return await Task.FromResult(_UsersRepository.Update_End_User_Gender(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Ethnicity")]
        public async Task<ActionResult<string>> Process_End_User_Ethnicity([FromBody] IdentityDTO dto)
        {
            try
            {
                if (ModelState.IsValid)
                    return BadRequest();

                ulong user_id = JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;

                if (!_UsersRepository.ID_Exists_In_Users_IDTbl(user_id).Result)
                    return Conflict();

                return await Task.FromResult(_UsersRepository.Update_End_User_Ethnicity(dto)).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }
}