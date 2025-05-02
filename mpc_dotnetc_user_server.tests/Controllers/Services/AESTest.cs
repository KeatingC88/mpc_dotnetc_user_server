using FluentAssertions;
using mpc_dotnetc_user_server.Controllers.Services;
using mpc_dotnetc_user_server.Controllers.Interfaces;
using System.Security.Cryptography;

namespace mpc_dotnetc_user_server.tests.Controllers.Services
{
    public class AESTest
    {
        private readonly IAES AES;
        private readonly byte[] _key;
        private readonly byte[] _iv;

        public AESTest()
        {
            _key = new byte[32];
            _iv = new byte[16];

            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(_key);
            rng.GetBytes(_iv);

            AES = new AES(_key, _iv);
        }

        [Fact]
        public void AESRoundTripTextTest_EncryptThenDecryptAndCompareValues_ReturnsOriginalText()
        {
            // Arrange
            string originalText = "This is a Test...";

            // Act
            string encrypted = AES.Process_Encryption(originalText);
            string decrypted = AES.Process_Decryption(encrypted);

            // Assert
            encrypted.Should().NotBeNullOrEmpty();
            decrypted.Should().Be(originalText);
        }

    }
}