using Microsoft.AspNetCore.Mvc;
using System.Text;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Models.Users._Index;
using mpc_dotnetc_user_server.Controllers.Users.JWT;


namespace mpc_dotnetc_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Delete")]
    public class DeactivateUserController : ControllerBase
    {
        private readonly ILogger<DeactivateUserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IUsersRepository _UsersRepository;

        public DeactivateUserController(ILogger<DeactivateUserController> logger, IConfiguration configuration, IUsersRepository UsersRepository)
        {
            _logger = logger;
            _configuration = configuration;
            _UsersRepository = UsersRepository;
        }
        [HttpDelete("User")]
        public async Task<ActionResult<string>> DeactivateUser([FromBody] Delete_UserDTO dto)
        {
            try
            {
                ulong target_id = 0;
                ulong user_id = 0;

                if (!ModelState.IsValid)
                    return BadRequest();

                user_id = _JWT.Read_Email_Account_User_ID_By_JWToken(dto.Token).Result;
                target_id = dto.Target_User;

                if (!_UsersRepository.ID_Exists_In_Users_Tbl(dto.Target_User).Result)
                        return NotFound();

                string? email = _UsersRepository.Read_User_Email_By_ID(user_id).Result;
                byte[]? user_password_hash_from_database_storage = _UsersRepository.Read_User_Password_Hash_By_ID(user_id).Result;
                byte[]? user_password_given_from_end_user_on_gui_client = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(dto.Password), Encoding.UTF8.GetBytes($"{email}MPCSalt")).Result;
                if (user_password_hash_from_database_storage != null)
                    if (!_UsersRepository.Compare_Password_Byte_Arrays(user_password_hash_from_database_storage, user_password_given_from_end_user_on_gui_client))
                        return Unauthorized();//Passwords do not match from end user on the gui-client to what's saved in our database or user_id invalid.

                return await Task.FromResult(_UsersRepository.Delete_Account_By_User_id(new Delete_UserDTO { ID = user_id, Target_User = target_id }).Result);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}