using Microsoft.AspNetCore.Mvc;
using dotnet_user_server.Controllers;
using dotnet_user_server.Models.Users;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace dotnet_user_server.Controllers.Users.Register
{
    [ApiController]
    [Route("api/Email")]
    public class EmailController : ControllerBase
    {
        private readonly ILogger<EmailController> _logger;
        private readonly IConfiguration _configuration;
        private readonly UsersDbC UsersDbC;//EFCore -> Database
        public EmailController(ILogger<EmailController> logger, IConfiguration configuration, UsersDbC context)
        {
            _logger = logger;
            _configuration = configuration;
            UsersDbC = context;
        }
        [HttpPost("Confirmation")]
        public ActionResult<string> Email_Account_Confirmation([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_address) || 
                    string.IsNullOrWhiteSpace(obj.Email_address) ||
                    string.IsNullOrEmpty(obj.Code) || 
                    string.IsNullOrWhiteSpace(obj.Code) ||
                    !Valid.Email(obj.Email_address) || 
                    !UsersDbC.Email_Exists_In_Not_Confirmed_Registered_Email_Tbl(obj.Email_address) ||
                    UsersDbC.Email_Exists_In_Login_EmailAddress_Tbl(obj.Email_address) ||
                    !UsersDbC.Confirmation_Code_Exists_In_Not_Confirmed_Email_Address_Tbl(obj.Code))
                    return BadRequest();
                
                return StatusCode(200, JsonSerializer.Serialize(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Register")]
        public async Task<ActionResult<string>> Email_Account_Register([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_address) || 
                    string.IsNullOrWhiteSpace(obj.Email_address) ||
                    string.IsNullOrWhiteSpace(obj.Language) ||
                    string.IsNullOrEmpty(obj.Code) || 
                    string.IsNullOrWhiteSpace(obj.Code) ||
                    !Valid.Email(obj.Email_address) ||
                    !Valid.LanguageRegion(obj.Language))
                    return BadRequest();

                if(UsersDbC.Email_Exists_In_Login_EmailAddress_Tbl(obj.Email_address))
                    return StatusCode(409);
               
                if (UsersDbC.Email_Exists_In_Not_Confirmed_Registered_Email_Tbl(obj.Email_address))
                    return await Task.FromResult(UsersDbC.Update_Unconfirmed_Email(obj));

                return await Task.FromResult(UsersDbC.Create_Unconfirmed_Email(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
        [HttpPost("Submit")]
        public async Task<ActionResult<string>> Submit_Email_Password([FromBody] DTO obj)
        {
            try
            {
                if (string.IsNullOrEmpty(obj.Email_address) || 
                    string.IsNullOrWhiteSpace(obj.Email_address) ||
                    string.IsNullOrEmpty(obj.Password) || 
                    string.IsNullOrWhiteSpace(obj.Password) || 
                    string.IsNullOrEmpty(obj.Language) || 
                    string.IsNullOrWhiteSpace(obj.Language) || 
                    UsersDbC.Email_Exists_In_Login_EmailAddress_Tbl(obj.Email_address) ||
                    !Valid.Email(obj.Email_address) || 
                    !Valid.Password(obj.Password) ||
                    !Valid.LanguageRegion(obj.Language))
                    return BadRequest();

                return await Task.FromResult(UsersDbC.Create_Account_By_Email(obj));
            } catch (Exception e) {
                return StatusCode(500, $"{e.Message}");
            }
        }
    }//Controller.
}//NameSpace.