using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using dotnet_user_server.Models.Users;

var MPCServerOrigins = "MPCServerOrigins";
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
//Database
string path = Directory.GetCurrentDirectory();
builder.Services.AddDbContext<UsersDbC>(options => options.UseSqlite($"Data Source = {path}\\Users.db"));
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MPCServerOrigins,
        policy =>
        {
            policy.WithOrigins("*").AllowAnyHeader().AllowAnyHeader().AllowAnyMethod();//Unrestricted/Security Breach
            //policy.WithOrigins("https://localhost:7179", "http://localhost:44459").AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
/*builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
    };
});*/
var app = builder.Build();
// Configure the HTTP request pipeline.
app.MapGet("/", () => "Root Called");
app.MapGet("/api", () => "API Called");
app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseCors(MPCServerOrigins);
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();