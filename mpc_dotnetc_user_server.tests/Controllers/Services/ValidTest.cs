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

        [Theory]
        [InlineData("Password1!")]    
        [InlineData("A1b@cdef")]      
        [InlineData("P@ssw0rd123")] 
        public void IsPasswordInvalid_Should_ReturnFalse_ForValidPasswords(string password)
        {
            var result = Valid.Password(password);

            result.Should().BeFalse(); 
        }

        [Theory]
        [InlineData("password")]      
        [InlineData("PASSWORD1")]  
        [InlineData("Passw1")]    
        [InlineData("Password")]  
        [InlineData("12345678")]   
        [InlineData("Password1")] 
        public void IsPasswordInvalid_Should_ReturnTrue_ForInvalidPasswords(string password)
        {
            var result = Valid.Password(password);

            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("EN")]
        [InlineData("de")]
        [InlineData("Fr")]
        [InlineData("ZH")]
        public void Language_Code_ValidInputs_ShouldReturnTrue(string input)
        {
            var result = Valid.Language_Code(input);
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("EN1")]
        [InlineData("en-us")]
        [InlineData("123")]
        [InlineData("")]
        [InlineData("deu")]
        public void Language_Code_InvalidInputs_ShouldReturnFalse(string input)
        {
            var result = Valid.Language_Code(input);
            result.Should().BeFalse();
        }

        [Theory]
        [InlineData("US")]
        [InlineData("CDO")]
        [InlineData("TW")]
        [InlineData("BE")]
        [InlineData("GAN")]
        public void Region_Code_ValidInputs_ShouldReturnTrue(string input)
        {
            var result = Valid.Region_Code(input);
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData("us1")]
        [InlineData("us-CA")]
        [InlineData("")]
        [InlineData("123")]
        [InlineData("usa")]
        public void Region_Code_InvalidInputs_ShouldReturnFalse(string input)
        {
            var result = Valid.Region_Code(input);
            result.Should().BeFalse();
        }
    }
}