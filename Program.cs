using System.Runtime.InteropServices;
using System.Net;
using DotNetEnv;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Index;
using mpc_dotnetc_user_server.Controllers.Interfaces;
using mpc_dotnetc_user_server.Controllers.Services;
using mpc_dotnetc_user_server.Models.Interfaces;

string sqlite3_users_database_path = "";
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

Env.Load();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

string environment = builder.Environment.EnvironmentName;
string dir = Directory.GetCurrentDirectory();
IWebHostEnvironment env = builder.Environment;

if (env.IsDevelopment() && RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) {

    sqlite3_users_database_path = $"{dir}\\bin\\Debug\\net8.0\\mpc_sqlite_users_database\\Users.db";

} else if (env.IsDevelopment() && RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {

    sqlite3_users_database_path = $"{dir}/bin/Debug/net8.0/mpc_sqlite_users_database/Users.db";

} else if (env.EnvironmentName == "Docker") {

    sqlite3_users_database_path = "/app/Users.db";//This must match Dockerfile's COPY Cmd.

} else if (env.IsProduction() && RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) {//Linux for AWS Production.

    sqlite3_users_database_path = Path.Combine(dir, "mpc_sqlite_users_database", "Users.db");

}

builder.Services.AddDbContext<Users_Database_Context>(options => options.UseSqlite($"Data Source = {sqlite3_users_database_path}"));

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: Environment.GetEnvironmentVariable("SERVER_ORIGIN") ?? string.Empty, policy =>
    {
        policy.WithOrigins(Environment.GetEnvironmentVariable("REMOTE_ORIGIN") ?? string.Empty).AllowAnyHeader().AllowAnyMethod();
    });
});

builder.Services.AddScoped<IUsers_Repository, Users_Repository>();
builder.Services.AddScoped<INetwork, Network>();

builder.Services.AddSingleton<Constants>();
builder.Services.AddSingleton<IValid, Valid>();
builder.Services.AddSingleton<IAES, AES>();
builder.Services.AddSingleton<IJWT, JWT>();
builder.Services.AddSingleton<IPassword, Password>();

var tempProvider = builder.Services.BuildServiceProvider();
var AES = tempProvider.GetRequiredService<IAES>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = $"{AES.Process_Encryption(Environment.GetEnvironmentVariable("JWT_CLIENT_KEY") ?? string.Empty)}",
        ValidIssuer = $"{AES.Process_Encryption(Environment.GetEnvironmentVariable("JWT_ISSUER_KEY") ?? string.Empty)}",
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(Environment.GetEnvironmentVariable("JWT_SIGN_KEY") ?? string.Empty))
    };
});

static string get_local_machine_ip_address()
{
    var host = Dns.GetHostEntry(Dns.GetHostName());
    foreach (var ip in host.AddressList)
    {
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
app.UseCors(Environment.GetEnvironmentVariable("SERVER_ORIGIN") ?? string.Empty);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

Console.WriteLine("Host IP");
Console.WriteLine(local_network_ip_address);
Console.WriteLine("Remote Audience");
Console.WriteLine(Environment.GetEnvironmentVariable("REMOTE_ORIGIN") ?? string.Empty);
Console.WriteLine("\n");

app.Urls.Add(@$"http://{local_network_ip_address}:{Environment.GetEnvironmentVariable("SERVER_NETWORK_PORT_NUMBER")}");

app.Run();

public class Constants
{
    private static readonly string local_network_ip_address = local_network_ip_address ?? "localhost";
    public string JWT_ISSUER_KEY { get; set; } = Environment.GetEnvironmentVariable("JWT_ISSUER_KEY") ?? string.Empty;
    public string JWT_CLIENT_KEY { get; set; } = Environment.GetEnvironmentVariable("JWT_CLIENT_KEY") ?? string.Empty;
    public string JWT_SECURITY_KEY { get; set; } = Environment.GetEnvironmentVariable("JWT_SIGN_KEY") ?? string.Empty;
    public string JWT_CLAIM_WEBPAGE { get; set; } = Environment.GetEnvironmentVariable("REMOTE_ORIGIN") ?? string.Empty;
}