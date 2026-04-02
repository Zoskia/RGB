using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using RedGreenBlue.Data;
using RedGreenBlue.Dtos;
using RedGreenBlue.Dtos.User;
using RedGreenBlue.Models;
using RedGreenBlue.Services;

namespace RedGreenBlue.IntegrationTests.Infrastructure;

[Collection("Integration")]
public abstract class ApiTestBase : IAsyncLifetime
{
    protected readonly TestWebApplicationFactory Factory;
    protected readonly HttpClient Client;

    protected ApiTestBase(TestWebApplicationFactory factory)
    {
        Factory = factory;
        Client = factory.CreateClient();
    }

    public virtual async Task InitializeAsync()
    {
        await Factory.ResetDatabaseAsync();
    }

    public virtual Task DisposeAsync()
    {
        Client.Dispose();
        return Task.CompletedTask;
    }

    protected static void SetBearerToken(HttpClient client, string token)
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task<string> RegisterAndLoginAsync(string username, string password, TeamColor team)
    {
        var registerResponse = await Client.PostAsJsonAsync("/api/user/register", new RegisterUserDto
        {
            Username = username,
            Password = password,
            Team = team
        });

        registerResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        return await LoginAndGetTokenAsync(username, password);
    }

    protected async Task<string> SeedAdminAndLoginAsync(string username, string password)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var hasher = new CustomPasswordHasher();
        var salt = hasher.GenerateSalt();
        var hash = hasher.HashPassword(password, salt);

        db.Users.Add(new User
        {
            Username = username,
            Password = hash,
            Salt = salt,
            Team = TeamColor.Red,
            IsAdmin = true
        });

        await db.SaveChangesAsync();

        return await LoginAndGetTokenAsync(username, password);
    }

    protected async Task SeedCellsAsync(params Cell[] cells)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        await db.Cells.AddRangeAsync(cells);
        await db.SaveChangesAsync();
    }

    protected async Task<Cell?> FindCellAsync(int q, int r, TeamColor team)
    {
        using var scope = Factory.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        return await db.Cells.FindAsync(q, r, team);
    }

    private async Task<string> LoginAndGetTokenAsync(string username, string password)
    {
        var loginResponse = await Client.PostAsJsonAsync("/api/user/login", new LoginUserDto
        {
            Username = username,
            Password = password
        });

        loginResponse.StatusCode.Should().Be(HttpStatusCode.OK);

        var payload = await loginResponse.Content.ReadFromJsonAsync<LoginResponseDto>();
        payload.Should().NotBeNull();

        if (payload is null)
        {
            throw new InvalidOperationException("Login response payload was null.");
        }

        return payload.Token;
    }
}
