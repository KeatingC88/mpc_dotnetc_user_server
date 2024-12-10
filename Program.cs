using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using mpc_dotnetc_user_server.Models.Users.Index;
using System.Runtime.InteropServices;
//...
string server_origin = "MPC_Users_Server_Origin";
string sqlite3_users_database_path = "";
WebApplicationBuilder builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*
    Select database path that depends on environment.
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
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))//This will break in local-production, but works if hard-coded the JWTKEY.
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