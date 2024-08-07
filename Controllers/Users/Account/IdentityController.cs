using Microsoft.AspNetCore.Mvc;
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
        public async Task<ActionResult<string>> Process_End_User_First_Name([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.First_name) ||
                    dto.First_name.Length > 19)
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                return await Task.FromResult(_UsersRepository.Save_End_User_First_Name(new DTO
                {
                    ID = user_id,
                    Token = dto.Token,
                    First_name = dto.First_name
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//

        [HttpPost("LastName")]
        public async Task<ActionResult<string>> Process_End_User_Last_Name([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Last_name) ||
                    dto.Last_name.Length > 19)
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                return await Task.FromResult(_UsersRepository.Save_End_User_Last_Name(new DTO
                {
                    ID = user_id,
                    Token = dto.Token,
                    Last_name = dto.Last_name
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//

        [HttpPost("MiddleName")]
        public async Task<ActionResult<string>> Process_End_User_Middle_Name([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Middle_name) ||
                    dto.Middle_name.Length > 19)
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                return await Task.FromResult(_UsersRepository.Save_End_User_Middle_Name(new DTO{
                    ID = user_id,
                    Token = dto.Token,
                    Middle_name = dto.Middle_name
                })).Result;
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }//

        [HttpPost("MaidenName")]
        public async Task<ActionResult<string>> Process_End_User_Maiden_Name([FromBody] DTO dto)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(dto.Maiden_name) ||
                    dto.Maiden_name.Length > 19)
                    return BadRequest();

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                return await Task.FromResult(_UsersRepository.Save_End_User_Maiden_Name(new DTO
                {
                    ID = user_id,
                    Token = dto.Token,
                    Maiden_name = dto.Maiden_name
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//

        [HttpPost("Gender")]
        public async Task<ActionResult<string>> Process_End_User_Gender([FromBody] DTO dto)
        {
            try
            {
                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return NotFound();

                return await Task.FromResult(_UsersRepository.Update_End_User_Gender(new DTO {
                    ID = user_id,
                    Token = dto.Token,
                    Gender = dto.Gender
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//

        [HttpPost("Ethnicity")]
        public async Task<ActionResult<string>> Process_End_User_Ethnicity([FromBody] DTO dto)
        {
            try
            {

                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                return await Task.FromResult(_UsersRepository.Save_End_User_Ethnicity(new DTO
                {
                    ID = user_id,
                    Token = dto.Token,
                    Ethnicity = dto.Ethnicity
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//

        [HttpPost("Birth/Date")]
        public async Task<ActionResult<string>> Process_End_User_Birth_Date([FromBody] DTO dto)
        {
            try
            {
                
                ulong user_id = _UsersRepository.Get_User_ID_From_JWToken(dto.Token).Result;

                if (user_id == 0)
                    return Ok();

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(user_id).Result)
                    return Ok();

                return await Task.FromResult(_UsersRepository.Save_End_User_Birth_Date(new DTO{
                    ID = user_id,
                    Token = dto.Token,
                    Month = dto.Month,
                    Day = dto.Day,
                    Year = dto.Year
                })).Result;
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//
    }//Controller.
}//Namespace.