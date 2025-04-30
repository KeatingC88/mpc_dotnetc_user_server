using Xunit;
using FluentAssertions;
using System.IO;
using System.Collections.Generic;

namespace mpc_dotnetc_user_server.tests
{
    public class Env_File_Test
    {

        [Fact]
        public void EnvFileExists_DoesTheFileExist_FileDoesExist()
        {
            // Arrange
            var environment_variable_file_path = Path.Combine(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..")), ".env");
            // Act
            var does_the_file_exist = File.Exists(environment_variable_file_path);
            // Assert
            Assert.True(does_the_file_exist, $"the .env file was not found at this exact location and should look like this: {environment_variable_file_path}\n");
        }

        [Fact]
        public void EnvFileHasAllRequiredKeys_DoAllServerRequiredKeysExists_AllServerKeysExist()
        {
            // Arrange
            string[] env_required_keys = {
                "DOCKER_CONTAINER_NAME",
                "DOCKER_CONTAINER_IMAGE_NAME",
                "SERVER_NETWORK_PORT_NUMBER",
                "SERVER_ORIGIN",
                "ENCRYPTION_KEY",
                "ENCRYPTION_TYPE",
                "ENCRYPTION_FORMAT",
                "ENCRYPTION_BASE",
                "ENCRYPTION_IV",
                "JWT_ISSUER_KEY",
                "JWT_CLIENT_KEY",
                "JWT_SIGN_KEY",
            };

            //Retrieve Keys from .Env file since it exists.
            Dictionary<string, string> LoadEnvFile(string path)
            {
                var lines = File.ReadAllLines(path);
                var dict = new Dictionary<string, string>();

                foreach (var line in lines)
                {
                    var trimmed = line.Trim();

                    // Skip empty lines or comments in .env file
                    if (string.IsNullOrWhiteSpace(trimmed) || trimmed.StartsWith("#"))
                        continue;

                    var parts = trimmed.Split('=', 2);
                    if (parts.Length == 2)
                    {
                        dict[parts[0].Trim()] = parts[1].Trim();
                    }
                }

                return dict;
            }

            //Act
            var envVars = LoadEnvFile(Path.Combine(Path.GetFullPath(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\..")), ".env"));

            //Assert
            foreach (var key in env_required_keys)
            {
                Assert.True(envVars.ContainsKey(key), $"Missing required .env key: {key}");
            }
        }
    }
}