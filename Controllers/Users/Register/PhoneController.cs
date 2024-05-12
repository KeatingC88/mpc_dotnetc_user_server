using Microsoft.AspNetCore.Mvc;
using dotnet_user_server.Controllers;
using dotnet_user_server.Models.Users;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace dotnet_user_server.Controllers.Users.Register
{
    [ApiController]
    [Route("api/Phone")]
    public class PhoneController : ControllerBase
    {
        private readonly ILogger<PhoneController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UsersDbC UsersDbC;//EFCore -> Database
        public PhoneController(ILogger<PhoneController> logger, IConfiguration configuration, UsersDbC context)
        {
            _logger = logger;
            _configuration = configuration;
            UsersDbC = context;
        }

        [HttpPost("Confirmation")]
        public ActionResult<string> Phone_Confirmation([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Phone.ToString()) || 
                    string.IsNullOrWhiteSpace(obj.Phone.ToString()) ||
                    string.IsNullOrEmpty(obj.Code) || 
                    string.IsNullOrWhiteSpace(obj.Code) ||
                    string.IsNullOrEmpty(obj.Carrier) || 
                    string.IsNullOrWhiteSpace(obj.Carrier) ||
                    string.IsNullOrEmpty(obj.Country.ToString()) || 
                    string.IsNullOrWhiteSpace(obj.Country.ToString()) ||
                    !Valid.Phone(obj.Phone) || UsersDbC.Phone_Exists_In_Login_Telephone_Tbl(obj.Phone) ||
                    !UsersDbC.Phone_Exists_In_Telephone_Not_Confirmed_Tbl(obj.Phone))
                    return BadRequest();

                return StatusCode(200, JsonSerializer.Serialize(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }

        [HttpPost("Register")]
        public async Task<ActionResult<string>> Phone_Register([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Phone.ToString()) ||
                    string.IsNullOrWhiteSpace(obj.Phone.ToString()) ||
                    string.IsNullOrEmpty(obj.Code) || 
                    string.IsNullOrWhiteSpace(obj.Code) ||
                    string.IsNullOrEmpty(obj.Carrier) || 
                    string.IsNullOrWhiteSpace(obj.Carrier) ||
                    string.IsNullOrEmpty(obj.Country.ToString()) || 
                    string.IsNullOrWhiteSpace(obj.Country.ToString()) ||
                    !Valid.Phone(obj.Phone) || 
                    UsersDbC.Phone_Exists_In_Login_Telephone_Tbl(obj.Phone))
                    return BadRequest();

                if (UsersDbC.Phone_Exists_In_Telephone_Not_Confirmed_Tbl(obj.Phone))
                    return await Task.FromResult(UsersDbC.Update_Unconfirmed_Phone(obj));

                return await Task.FromResult(UsersDbC.Create_Unconfirmed_Phone(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Submit")]
        public async Task<ActionResult<string>> PhoneSubmit([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Phone.ToString()) || string.IsNullOrWhiteSpace(obj.Phone.ToString()) ||
                    string.IsNullOrEmpty(obj.Password) || string.IsNullOrWhiteSpace(obj.Password) ||
                    string.IsNullOrEmpty(obj.Code) || string.IsNullOrWhiteSpace(obj.Code) ||
                    string.IsNullOrEmpty(obj.Carrier) || string.IsNullOrWhiteSpace(obj.Carrier) ||
                    string.IsNullOrEmpty(obj.Country.ToString()) || string.IsNullOrWhiteSpace(obj.Country.ToString()) ||
                    string.IsNullOrEmpty(obj.Password) || string.IsNullOrWhiteSpace(obj.Password) ||
                    !Valid.Phone(obj.Phone) || !Valid.Password(obj.Password) ||
                    !Valid.LanguageRegion(obj.Language))
                    return BadRequest();

                return await Task.FromResult(UsersDbC.Create_Account_By_Phone(obj));
            }
            catch (Exception e)
            {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}//NameSpace.