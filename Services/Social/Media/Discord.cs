using mpc_dotnetc_user_server.Interfaces.Social;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Discord;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Services.Social.Media
{
    public class Discord : IDiscord
    {
        private static readonly Constants Constants = new Constants();
        private readonly string discord_client_id = Environment.GetEnvironmentVariable("discord_client_id") ?? string.Empty;
        private readonly string discord_client_secret = Environment.GetEnvironmentVariable("discord_client_secret") ?? string.Empty;

        public Discord() { }

        public async Task<string> Get_Client_Credentials_Flow_Access_Token() 
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://discord.com/api/oauth2/token");
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", discord_client_id),
                new KeyValuePair<string, string>("client_secret", discord_client_secret),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            });

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var token_response = JsonSerializer.Deserialize<Discord_Token_Response>(json);
            return token_response.AccessToken ?? string.Empty;
        }
        public async Task<string?> Authorization_Code_Flow_Access_Token(string code)
        {
            using HttpClient http_client = new HttpClient();

            var http_response = await http_client.PostAsync("https://discord.com/api/oauth2/token", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", discord_client_id },
                    { "client_secret", discord_client_secret },
                    { "code", code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", Environment.GetEnvironmentVariable("Discord_CLIENT_REDIRECT_URI") ?? string.Empty }
                }));

            var content = await http_response.Content.ReadAsStringAsync();

            var token_response = JsonSerializer.Deserialize<Discord_Token_Response>(content);

            if (token_response == null || token_response.AccessToken == null)
            {
                return null;
            }

            return await Task.FromResult(token_response.AccessToken);
        }
        public async Task<Discord_User_Response?> Get_User_Data(string Discord_access_token)
        {
            HttpClient http_client = new HttpClient();
            http_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Discord_access_token);
            http_client.DefaultRequestHeaders.Add("Client-Id", discord_client_id);

            var discord_response = await http_client.GetAsync("https://discord.com/api/users/@me");

            if (!discord_response.IsSuccessStatusCode)
            {
                throw new Exception("Invalid Discord_Access_Token");
            }

            var discord_end_user_json = await discord_response.Content.ReadAsStringAsync();

            var discord_end_user_data = JsonSerializer.Deserialize<Discord_User_Response>(discord_end_user_json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (discord_end_user_data == null)
                return null;

            return await Task.FromResult(discord_end_user_data);
        }
    }
}