using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Index;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using mpc_dotnetc_user_server.Controllers;
//...
string server_origin = "MPC_Users_Server_Origin";
string sqlite3_users_database_path = "";
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
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
AES AES = new AES();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = $"{AES.Process_Encryption("JWT-Servicing-MPC-Client-As-Audience")}",
        ValidIssuer = $"{AES.Process_Encryption("JWT-Authentication-MPC-User-Server-As-Issuer")}",
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes("9!5@a$59#%8^7MPC]1MPC999587)($@!53DataMonkey78912345645447890#%^2345vvcczxxedddg!#$%132577979798dA&*($##$$%@!^&*DFGGFFFFA^%YHBFSSDFTYG"))
    };
});

var app = builder.Build();
// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(server_origin);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();