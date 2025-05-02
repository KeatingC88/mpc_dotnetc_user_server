using mpc_dotnetc_user_server.Controllers.Interfaces;
using FluentAssertions;
using System;

namespace mpc_dotnetc_user_server.Controllers.Services
{
    public class ValidTest
    {
        private readonly IValid Valid;

        public ValidTest()
        {
            Valid = new Valid();
        }

        [Theory]
        [InlineData("plainaddress")]
        [InlineData("missingatsign.com")]
        [InlineData("missingdomain@")]
        [InlineData("@missinglocal.com")]
        [InlineData("a@b.c")]
        [InlineData("two@@signs.com")]
        [InlineData("endswithdot.@domain.com")]
        [InlineData("user@.com")]
        [InlineData("user@domain.")]
        [InlineData(" user@domain.com ")]
        [InlineData("")]
        [InlineData(null)]
        public void Email_Should_ReturnFalse_ForMalformedEmails(string malformed_email)
        {
            //Arrange @ Param
            // Act
            var result = Valid.Email(malformed_email);
            // Assert
            result.Should().BeFalse();
        }
    }
}
