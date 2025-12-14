using mpc_dotnetc_user_server.Interfaces.Social;
using mpc_dotnetc_user_server.Models.Users.Authentication.Login.Twitch;
using System.Net;
using System.Net.Http.Headers;
using System.Text.Json;

namespace mpc_dotnetc_user_server.Services.Social.Media
{
    public class Twitch : ITwitch
    {
        private static readonly Constants Constants = new Constants();
        private readonly string twitch_client_id = Environment.GetEnvironmentVariable("TWITCH_CLIENT_ID") ?? string.Empty;
        private readonly string twitch_client_secret = Environment.GetEnvironmentVariable("TWITCH_CLIENT_SECRET") ?? string.Empty;

        public Twitch() { }

        public async Task<string> Get_Client_Credentials_Flow_Access_Token() 
        {
            using var client = new HttpClient();

            var request = new HttpRequestMessage(HttpMethod.Post, "https://id.twitch.tv/oauth2/token");
            request.Content = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", twitch_client_id),
                new KeyValuePair<string, string>("client_secret", twitch_client_secret),
                new KeyValuePair<string, string>("grant_type", "client_credentials"),
            });

            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();

            var token_response = JsonSerializer.Deserialize<Twitch_Token_Response>(json);
            return token_response.AccessToken ?? string.Empty;
        }
        public async Task<string?> Authorization_Code_Flow_Access_Token(string code)
        {
            using HttpClient http_client = new HttpClient();

            var http_response = await http_client.PostAsync("https://id.twitch.tv/oauth2/token", new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "client_id", twitch_client_id },
                    { "client_secret", twitch_client_secret },
                    { "code", code },
                    { "grant_type", "authorization_code" },
                    { "redirect_uri", Environment.GetEnvironmentVariable("TWITCH_CLIENT_REDIRECT_URI") ?? string.Empty }
                }));

            var content = await http_response.Content.ReadAsStringAsync();

            var token_response = JsonSerializer.Deserialize<Twitch_Token_Response>(content);

            if (token_response == null || token_response.AccessToken == null)
            {
                return null;
            }

            return await Task.FromResult(token_response.AccessToken);
        }
        public async Task<Twitch_User_Response?> Get_User_Data(string twitch_access_token)
        {
            HttpClient http_client = new HttpClient();
            http_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", twitch_access_token);
            http_client.DefaultRequestHeaders.Add("Client-Id", twitch_client_id);

            var twitch_response = await http_client.GetAsync("https://api.twitch.tv/helix/users");

            if (!twitch_response.IsSuccessStatusCode)
            {
                throw new Exception("Invalid Twitch_Access_Token");
            }

            var twitch_end_user_json = await twitch_response.Content.ReadAsStringAsync();

            var twitch_end_user_data = JsonSerializer.Deserialize<Twitch_User_Response>(twitch_end_user_json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (twitch_end_user_data == null)
                return null;

            return await Task.FromResult(twitch_end_user_data);
        }
    }
}