using System.Data.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using RedGreenBlue.Data;

namespace RedGreenBlue.IntegrationTests.Infrastructure;

public sealed class TestWebApplicationFactory : WebApplicationFactory<Program>
{
    private const string JwtKey = "integration-tests-super-secret-key-1234567890";
    private const string JwtIssuer = "https://issuer.integration.test";
    private const string JwtAudience = "https://audience.integration.test";

    private SqliteConnection? _connection;

    public TestWebApplicationFactory()
    {
        // Program.cs validates Jwt:Key before middleware/service setup finishes.
        // Environment variables guarantee the values exist at host bootstrap time.
        Environment.SetEnvironmentVariable("Jwt__Key", JwtKey);
        Environment.SetEnvironmentVariable("Jwt__Issuer", JwtIssuer);
        Environment.SetEnvironmentVariable("Jwt__Audience", JwtAudience);
        Environment.SetEnvironmentVariable("Jwt__ExpiresInMinutes", "60");
        Environment.SetEnvironmentVariable("Database__RunMigrationsAtStartup", "false");
        Environment.SetEnvironmentVariable("Database__SeedAtStartup", "false");
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");

        builder.ConfigureAppConfiguration((_, configBuilder) =>
        {
            var testConfig = new Dictionary<string, string?>
            {
                ["Jwt:Key"] = JwtKey,
                ["Jwt:Issuer"] = JwtIssuer,
                ["Jwt:Audience"] = JwtAudience,
                ["Jwt:ExpiresInMinutes"] = "60",
                ["Database:RunMigrationsAtStartup"] = "false",
                ["Database:SeedAtStartup"] = "false",
                ["ConnectionStrings:DefaultConnection"] = "Data Source=:memory:"
            };

            configBuilder.AddInMemoryCollection(testConfig);
        });

        builder.ConfigureServices(services =>
        {
            services.RemoveAll<DbContextOptions<ApplicationDbContext>>();
            services.RemoveAll<ApplicationDbContext>();
            services.RemoveAll<DbConnection>();

            _connection?.Dispose();
            _connection = new SqliteConnection("Data Source=:memory:");
            _connection.Open();

            services.AddSingleton<DbConnection>(_connection);
            services.AddDbContext<ApplicationDbContext>((sp, options) =>
            {
                var connection = sp.GetRequiredService<DbConnection>();
                options.UseSqlite(connection);
            });
        });
    }

    public async Task ResetDatabaseAsync()
    {
        using var scope = Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Database.EnsureDeletedAsync();
        await db.Database.EnsureCreatedAsync();
    }

    protected override void Dispose(bool disposing)
    {
        base.Dispose(disposing);

        if (disposing)
        {
            _connection?.Dispose();

            Environment.SetEnvironmentVariable("Jwt__Key", null);
            Environment.SetEnvironmentVariable("Jwt__Issuer", null);
            Environment.SetEnvironmentVariable("Jwt__Audience", null);
            Environment.SetEnvironmentVariable("Jwt__ExpiresInMinutes", null);
            Environment.SetEnvironmentVariable("Database__RunMigrationsAtStartup", null);
            Environment.SetEnvironmentVariable("Database__SeedAtStartup", null);
        }
    }
}
