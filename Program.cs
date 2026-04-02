using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RedGreenBlue.Data;
using RedGreenBlue.Helpers;
using RedGreenBlue.Repositories;
using RedGreenBlue.Repositories.Interfaces;
using RedGreenBlue.Services;
using RedGreenBlue.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Configure camelCase JSON payloads for JS/TS clients.
builder.Services.AddControllers()
    .AddJsonOptions(o =>
    {
        o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        o.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));

// Configure EF Core with SQLite.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register repository and application services.
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IPasswordHasher, CustomPasswordHasher>();
builder.Services.AddScoped<IGridRepository, GridRepository>();
builder.Services.AddScoped<IGridService, GridService>();
builder.Services.AddScoped<IJwtService, JwtService>();

// Configure JWT bearer authentication.
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>();
if (jwtSettings is null || string.IsNullOrWhiteSpace(jwtSettings.Key))
{
    throw new InvalidOperationException(
        "JWT key is missing. Set Jwt:Key via dotnet user-secrets or environment variable Jwt__Key.");
}
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

// Allow requests from the Angular dev server during local development.
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

var runMigrationsAtStartup = builder.Configuration.GetValue<bool?>("Database:RunMigrationsAtStartup")
    ?? app.Environment.IsDevelopment();
var seedAtStartup = builder.Configuration.GetValue<bool?>("Database:SeedAtStartup")
    ?? app.Environment.IsDevelopment();

if (runMigrationsAtStartup || seedAtStartup)
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

    // Run migrations first so optional seed data can target existing tables.
    await db.Database.MigrateAsync();

    if (seedAtStartup)
    {
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
        await DbInitializer.SeedDefaultUsersAsync(db, passwordHasher);
        await DbInitializer.SeedRectangleForAllTeamsAsync(db);
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("AllowAngularApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

public partial class Program { }
