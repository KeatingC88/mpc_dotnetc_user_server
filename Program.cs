using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Index;
using System.Runtime.InteropServices;
using mpc_dotnetc_user_server.Controllers;
using System.Net;
using DotNetEnv;
//...
string server_origin = "MPC_Users_Server_Origin";
string sqlite3_users_database_path = "";
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

/*
    Load .env file
 */

Env.Load();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

/*
    Select database path.
*/
string environment = builder.Environment.EnvironmentName;
string dir = Directory.GetCurrentDirectory();
IWebHostEnvironment env = builder.Environment;
if (env.IsDevelopment() && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {

    sqlite3_users_database_path = $"{dir}\\bin\\Debug\\net8.0\\mpc_sqlite_users_db\\Users.db";

} else if (env.IsDevelopment() && RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {

    sqlite3_users_database_path = $"{dir}/bin/Debug/net8.0/mpc_sqlite_users_db/Users.db";

} else if (env.EnvironmentName == "Docker") {

    sqlite3_users_database_path = "/app/Users.db";//This must match Dockerfile's COPY Cmd.

} else if (env.IsProduction() && RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {//Linux for AWS Production.

    sqlite3_users_database_path = Path.Combine(dir, "mpc_sqlite_users_db", "Users.db");

}
/*
    Use Database Context for ORM.
 */
builder.Services.AddDbContext<UsersDBC>(options => options.UseSqlite($"Data Source = {sqlite3_users_database_path}"));
builder.Services.AddScoped<IUsersRepository, UsersRepository>();

/*
    Limit Access to this server.
*/
builder.Services.AddCors(options => { 
    options.AddPolicy(name: server_origin, policy => {
        policy.WithOrigins("http://localhost:6499/").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
    });
});
/*
    Initiate Token Service Provider for this server.
 */
builder.Services.AddSingleton<Constants>();
/*
    Initiate Cryptography Class for this server.
*/
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = $"{AES.Process_Encryption(Environment.GetEnvironmentVariable("JWT_ISSUER_KEY"))}",
        ValidIssuer = $"{AES.Process_Encryption(Environment.GetEnvironmentVariable("JWT_CLIENT_KEY"))}",
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGN_KEY")))
    };
});

static string get_local_machine_ip_address()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
        // Filter out the loopback address and ensure it's an IPv4 address
        if (ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !IPAddress.IsLoopback(ip))
        {
            return ip.ToString();
        }
    }
    return "IP Address not found.";
}

string local_network_ip_address = get_local_machine_ip_address();

builder.Services.AddHttpContextAccessor();

var app = builder.Build();
Network.Configure(app.Services.GetRequiredService<IHttpContextAccessor>());
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(server_origin);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapGet("/", () => @$"Users Server: Ready on Network IP Address {local_network_ip_address} and Port Number 5177");
//app.Urls.Add(@$"http://{local_network_ip_address}:5177");//do not remove -- use on host
app.Run();

public class Constants
{
    public string JWT_ISSUER_KEY { get; set; } = Environment.GetEnvironmentVariable("JWT_ISSUER_KEY");
    public string JWT_CLIENT_KEY { get; set; } = Environment.GetEnvironmentVariable("JWT_CLIENT_KEY");
    public string JWT_SECURITY_KEY { get; set; } = Environment.GetEnvironmentVariable("JWT_SIGN_KEY");
    //public string JWT_CLAIM_WEBPAGE { get; set; } = "http://192.168.0.102:6499";//do not remove -- use on host
    public string JWT_CLAIM_WEBPAGE { get; set; } = "http://localhost:6499";
}