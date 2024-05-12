using Microsoft.AspNetCore.Mvc;
using dotnet_user_server.Models.Users;
using System.Reflection.Metadata.Ecma335;
using System.Text;

namespace dotnet_user_server.Controllers.Users.Account
{
    [ApiController]
    [Route("api/Authenticate")]
    public class AuthenticateController : ControllerBase
    {
        private readonly ILogger<AuthenticateController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UsersDbC UsersDbC;//Database Model

        public AuthenticateController(ILogger<AuthenticateController> logger, IConfiguration configuration, UsersDbC context)
        {
            _logger = logger;
            _configuration = configuration;
            UsersDbC = context;
        }
        [HttpPut("Login/Email")]
        public async Task<ActionResult<string>> LoginEmailPassword([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_address) || string.IsNullOrWhiteSpace(obj.Email_address) ||
                    string.IsNullOrEmpty(obj.Password) || string.IsNullOrWhiteSpace(obj.Password))
                    return BadRequest();

                if (!UsersDbC.Email_Exists_In_Login_EmailAddress_Tbl(obj.Email_address))
                    return NotFound();

                ulong user_id = UsersDbC.Get_User_ID_By_Email_Address(obj.Email_address);

                if (user_id == 0 || !UsersDbC.ID_Exist_In_Users_Tbl(user_id))
                    return NotFound();

                byte[]? user_id_password_hash_in_the_database = UsersDbC.Get_User_Password_Hash_By_ID(user_id);
                byte[]? end_user_given_password_that_becomes_hash_to_compare_db_hash = UsersDbC.Create_Salted_Hash_String(Encoding.UTF8.GetBytes(obj.Password), Encoding.UTF8.GetBytes($"{obj.Email_address}MPCSalt"));

                if (user_id_password_hash_in_the_database != null)
                    if (!UsersDbC.Compare_Password_Byte_Arrays(user_id_password_hash_in_the_database, end_user_given_password_that_becomes_hash_to_compare_db_hash))
                        return Unauthorized();

                obj.ID = user_id;

                byte end_user_selected_status = UsersDbC.Read_End_User_Selected_Status(obj);
                if (end_user_selected_status != 1)
                {//This is here incase user has selected "hidden" and they are signing into the server.
                    obj.Online_status = 2;//User came online.
                    UsersDbC.Update_User_Selected_Status(obj);//Update Database Status Table.
                }

                return await Task.FromResult(UsersDbC.Update_User_Login(obj));//return timestamp and id
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }//Login.
        [HttpPut("Logout")]
        public async Task<ActionResult<bool>> Logout([FromBody] DTO obj)
        {
            try
            {
                ulong end_user_id = UsersDbC.Get_User_ID_From_JWToken(obj.Token);
                if (end_user_id == 0 || !UsersDbC.ID_Exist_In_Users_Tbl(end_user_id))
                    return Unauthorized();
                
                obj.ID = end_user_id;
                switch (obj.Online_status) {
                    case 1:
                        obj.Online_status = 10;//1-0 i.e. like save status as hidden-offline since end user is trying to logout.
                        UsersDbC.Update_User_Selected_Status(obj);
                        break;
                    case 2:
                        obj.Online_status = 20;//2-0 (online-offline status saved)
                        UsersDbC.Update_User_Selected_Status(obj);
                        break;
                    case 3:
                        obj.Online_status = 30;//3-0 (away-offline status saved)
                        UsersDbC.Update_User_Selected_Status(obj);
                        break;
                    case 4:
                        obj.Online_status = 40;//2-0 (dnd-offline status saved)
                        UsersDbC.Update_User_Selected_Status(obj);
                        break;
                    case 5:
                        obj.Online_status = 50;//2-0 (custom-offline status saved)
                        UsersDbC.Update_User_Selected_Status(obj);
                        break;
                }//Locate End User's Submitted Status Protocol.

                return await Task.FromResult(UsersDbC.Update_User_Logut(obj.ID));//return true if successful
            } catch (Exception e) {//Except
                return StatusCode(500, $"{e.Message}");//Error
            }//Try
        }//PUT End User into Logout State
    }//Controller.
}//Namespace.