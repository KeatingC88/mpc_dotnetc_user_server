using Microsoft.AspNetCore.Mvc;
//JWT Stuff...
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using System.Text.RegularExpressions;
using dotnet_user_server.Models.Users;

namespace dotnet_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Delete")]
    public class DeactivateUserController : ControllerBase
    {
        private readonly ILogger<DeactivateUserController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UsersDbC UsersDbC;//EFCore -> Database
        public DeactivateUserController(ILogger<DeactivateUserController> logger, IConfiguration configuration, UsersDbC context)
        {
            _logger = logger;
            _configuration = configuration;
            UsersDbC = context;
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

                user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);
                target_id = obj.Target_id;

                if (!string.IsNullOrEmpty(obj.Target_id.ToString()) || 
                    !string.IsNullOrWhiteSpace(obj.Target_id.ToString()) || 
                    !UsersDbC.ID_Exist_In_Users_Tbl(obj.Target_id) ||
                    target_id == 0)
                        return NotFound();

                string email = UsersDbC.Get_User_Email_By_ID(user_id);
                byte[]? user_password_hash_from_database_storage = UsersDbC.Get_User_Password_Hash_By_ID(user_id);
                byte[]? user_password_given_from_end_user_on_gui_client = UsersDbC.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(obj.Password), Encoding.UTF8.GetBytes($"{email}MPCSalt"));
                if (user_password_hash_from_database_storage != null)
                    if (user_id == 0 || !UsersDbC.Compare_Password_Byte_Arrays(user_password_hash_from_database_storage, user_password_given_from_end_user_on_gui_client))
                        return Unauthorized();//Passwords do not match from end user on the gui-client to what's saved in our database or user_id invalid.

                return await Task.FromResult(UsersDbC.Delete_Account_By_User_ID(new DTO { ID = user_id, Target_id = target_id }));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}