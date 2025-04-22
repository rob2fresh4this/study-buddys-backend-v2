using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using study_buddys_backend_v2.Context;
using study_buddys_backend_v2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ✅ Your services
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CommunityService>();

// ✅ Register your SignalR and UserConnectionManager
builder.Services.AddSignalR(); // Add this
builder.Services.AddSingleton<UserConnectionManager>(); // And this

// ✅ DB Context
var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

// ✅ CORS Policy
builder.Services.AddCors(Options =>
{
    Options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

// ✅ JWT Authentication
var secretKey = builder.Configuration["Jwt:Key"];
var signingCredentials = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

string serverUrl = "https://study-buddys-backend.azurewebsites.net/";
string serverUrl2 = "https://studybuddies-g9bmedddeah6aqe7.westus-01.azurewebsites.net/";
string localHostUrl = "https://localhost:5233/";

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidIssuers = new List<string> { serverUrl, localHostUrl, serverUrl2 },
        ValidAudiences = new List<string> { serverUrl, localHostUrl, serverUrl2 },
        IssuerSigningKey = signingCredentials
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// ✅ You’ll plug in this line after we make the DirectMessageHub in next step
// app.MapHub<DirectMessageHub>("/hubs/direct");

app.Run();
