using Microsoft.AspNetCore.Http;

namespace mpc_dotnetc_user_server.Controllers
{
    public class Network : INetwork
    {
        private static IHttpContextAccessor _httpContextAccessor = default!;

        public static void Configure(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<string> Get_Client_Remote_Internet_Protocol_Address()
        {
            return await Task.Run(() => {
                if (_httpContextAccessor == null)
                    return "error";

                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                    return "error";

                var remoteIpAddress = httpContext.Connection.RemoteIpAddress?.ToString();
                var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].ToString();

                if (!string.IsNullOrEmpty(forwardedFor))
                {
                    var ipList = forwardedFor.Split(',');

                    if (remoteIpAddress == null)
                    {
                        return "error";
                    }

                    return ipList.Length > 0 ? ipList[0].Trim() : remoteIpAddress;
                }

                return remoteIpAddress ?? "error";
            });
        }

        public async Task<int> Get_Client_Remote_Internet_Protocol_Port()
        {
            return await Task.Run(() =>
            {
                if (_httpContextAccessor == null)
                    return 0;

                var httpContext = _httpContextAccessor.HttpContext;

                if (httpContext == null)
                    return 0;

                var forwardedPort = httpContext.Request.Headers["X-Forwarded-Port"].ToString();

                if (!string.IsNullOrEmpty(forwardedPort))
                {
                    return int.Parse(forwardedPort.Trim());
                }

                var hostPort = httpContext.Request.Host.Port;

                return hostPort.HasValue ? hostPort.Value : 0;
            });


        }

        public async Task<string> Get_Client_Internet_Protocol_Address()
        {
            using var httpClient = new HttpClient();
            return await httpClient.GetStringAsync("https://api64.ipify.org");
        }

        public async Task<int> Get_Client_Internet_Protocol_Port()
        {
            return await Task.Run(() =>
            {
                var urls = Environment.GetEnvironmentVariable("ASPNETCORE_URLS") ?? null;

                if (string.IsNullOrEmpty(urls))
                {
                    return 0;
                }

                foreach (var url in urls.Split(';'))
                {
                    try
                    {
                        Uri uri = new Uri(url);
                        if (uri.Port != 0)
                        {
                            return uri.Port;
                        }
                    }
                    catch (UriFormatException)
                    {
                        continue;
                    }
                }

                return 0;
            });
        }
    }
}