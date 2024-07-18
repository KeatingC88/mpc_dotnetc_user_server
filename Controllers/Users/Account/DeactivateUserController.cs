using Microsoft.AspNetCore.Mvc;
using System.Text;
using mpc_dotnetc_user_server.Models.Users;


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
        public async Task<ActionResult<string>> DeactivateUser([FromBody] DTO obj)
        {
            try
            {
                ulong target_id = 0;
                ulong user_id = 0;

                if (string.IsNullOrEmpty(obj.ID.ToString()) || string.IsNullOrWhiteSpace(obj.ID.ToString()) ||
                    string.IsNullOrEmpty(obj.Token) || string.IsNullOrWhiteSpace(obj.Token) ||
                    string.IsNullOrEmpty(obj.Language) || string.IsNullOrWhiteSpace(obj.Language) ||
                    string.IsNullOrEmpty(obj.Password) || string.IsNullOrWhiteSpace(obj.Password))
                    return BadRequest();

                user_id = _UsersRepository.Get_User_ID_From_JWToken(obj.Token).Result;
                target_id = obj.Target_ID;

                if (!string.IsNullOrEmpty(obj.Target_ID.ToString()) || 
                    !string.IsNullOrWhiteSpace(obj.Target_ID.ToString()) || 
                    !_UsersRepository.ID_Exists_In_Users_Tbl(obj.Target_ID).Result ||
                    target_id == 0)
                        return NotFound();

                string? email = _UsersRepository.Get_User_Email_By_ID(user_id).Result;
                byte[]? user_password_hash_from_database_storage = _UsersRepository.Get_User_Password_Hash_By_ID(user_id).Result;
                byte[]? user_password_given_from_end_user_on_gui_client = _UsersRepository.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(obj.Password), Encoding.UTF8.GetBytes($"{email}MPCSalt")).Result;
                if (user_password_hash_from_database_storage != null)
                    if (user_id == 0 || !_UsersRepository.Compare_Password_Byte_Arrays(user_password_hash_from_database_storage, user_password_given_from_end_user_on_gui_client))
                        return Unauthorized();//Passwords do not match from end user on the gui-client to what's saved in our database or user_id invalid.

                return await Task.FromResult(_UsersRepository.Delete_Account_By_User_ID(new DTO { ID = user_id, Target_ID = target_id }).Result);
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}