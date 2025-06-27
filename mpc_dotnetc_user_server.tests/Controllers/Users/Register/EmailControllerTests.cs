using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Moq;
using FluentAssertions;
using mpc_dotnetc_user_server.Controllers.Interfaces;
using mpc_dotnetc_user_server.Controllers.Users.Register;
using mpc_dotnetc_user_server.Models.Users.Authentication.Confirmation;
using System.Threading.Tasks;
using mpc_dotnetc_user_server.Controllers.Services;
using System.Security.Cryptography;
using System;
using mpc_dotnetc_user_server.Models.Interfaces;

namespace mpc_dotnetc_user_server.tests.Controllers.Users.Register
{
    public class EmailControllerTests
    {
        private readonly Mock<IUsers_Repository> _usersRepositoryMock = new();
        private readonly Mock<ILogger<EmailController>> _loggerMock = new();
        private readonly Mock<IConfiguration> _configurationMock = new();

        private readonly EmailController _controller;
        private readonly Constants Constants = new Constants();

        private readonly IAES AES = new AES();
        private readonly byte[] _key;
        private readonly byte[] _iv;

        private readonly IValid Valid = new Valid();
        private readonly INetwork Network = new Network();

        public EmailControllerTests()
        {
            _controller = new EmailController(
                _loggerMock.Object,
                _configurationMock.Object,
                _usersRepositoryMock.Object,
                Valid,
                AES,
                Network,
                Constants)
            {
                ControllerContext = new ControllerContext
                {
                    HttpContext = new DefaultHttpContext()
                }
            };

            
            _key = new byte[32];
            _iv = new byte[16];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(_key);
            rng.GetBytes(_iv);
            AES = new AES(_key, _iv);

        }

        /*[Fact] I Think the AES have different keys/ivs and having trouble to test it because of this...
         public async Task Exists_Should_Return_BadRequest_When_Email_Is_Invalid()
        {
            // Arrange
            var encryptedEmail = AES.Process_Encryption("notanemail");
            var dto = new Validate_Email_AddressDTO
            {
                Email_Address = encryptedEmail,
                Language = AES.Process_Encryption("en"),
                Region = AES.Process_Encryption("US"),
                Client_time = AES.Process_Encryption("1721228193"),
                Location = AES.Process_Encryption("USA"),
                JWT_issuer_key = AES.Process_Encryption(Constants.JWT_ISSUER_KEY),
                JWT_client_key = AES.Process_Encryption(Constants.JWT_CLIENT_KEY),
                JWT_client_address = AES.Process_Encryption(Constants.JWT_CLAIM_WEBPAGE),
                User_agent = AES.Process_Encryption("Mozilla"),
                Window_height = AES.Process_Encryption("1080"),
                Window_width = AES.Process_Encryption("1920"),
                Screen_width = AES.Process_Encryption("1920"),
                Screen_height = AES.Process_Encryption("1080"),
                RTT = AES.Process_Encryption("50"),
                Orientation = AES.Process_Encryption("landscape"),
                Data_saver = AES.Process_Encryption("false"),
                Color_depth = AES.Process_Encryption("24"),
                Pixel_depth = AES.Process_Encryption("24"),
                Connection_type = AES.Process_Encryption("wifi"),
                Down_link = AES.Process_Encryption("10"),
                Device_ram_gb = AES.Process_Encryption("8")
            };
            Console.WriteLine(dto);
            // Act
            var result = await _controller.Validating_Email_Exists_In_Login_Email_Address_Tbl(dto);
            Console.WriteLine("Result variable is \n");
            Console.WriteLine(result);
            Console.WriteLine("Resulting Value variable is \n");
            Console.WriteLine(result.Value);

            // Assert
            result.Result.Should().BeOfType<ObjectResult>();
        }*/
    }
}
