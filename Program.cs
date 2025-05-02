using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using study_buddys_backend_v2.Context;
using study_buddys_backend_v2.Hubs;
using study_buddys_backend_v2.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Study Buddys API", Version = "v1" });

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' followed by your token.\r\n\r\nExample: Bearer abc.def.ghi"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<CommunityService>();

builder.Services.AddSignalR(); // Add this
builder.Services.AddSingleton<UserConnectionManager>(); // And this

var connectionString = builder.Configuration.GetConnectionString("DatabaseConnection");
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connectionString));

builder.Services.AddCors(Options =>
{
    Options.AddPolicy("AllowAll",
        policy =>
        {
            policy.WithOrigins("http://localhost:3001/", "http://localhost:3000/", "https://study-buddies-frontend-seven.vercel.app/")
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

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

app.MapHub<CommunityHub>("/communityHub"); // Add this line to map the hub
app.MapHub<DirectMessageHub>("/directMessageHub"); // Add this line to map the hub
app.MapControllers();

app.Run();