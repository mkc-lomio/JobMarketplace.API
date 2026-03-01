using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace JobMarketplace.Tests.Integration;

/// <summary>
/// Integration tests that spin up the real API in-memory using WebApplicationFactory.
/// These hit actual endpoints with HttpClient — real middleware, real DI, real database.
/// 
/// PREREQUISITE: The database must exist and be seeded (run the API once first).
/// These tests use the Development appsettings and connect to your local SQL Server.
/// </summary>
public class ApiEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public ApiEndpointTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    // ─── Health Check ────────────────────────────────────────

    [Fact]
    public async Task HealthCheck_ShouldReturnHealthy()
    {
        var response = await _client.GetAsync("/health");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("Healthy");
    }

    // ─── Auth: Unauthenticated Access ────────────────────────

    [Fact]
    public async Task GetJobs_WithoutToken_ShouldReturn401()
    {
        var response = await _client.GetAsync("/api/jobs");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetCompanies_WithoutToken_ShouldReturn401()
    {
        var response = await _client.GetAsync("/api/companies");

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ─── Auth: Login ─────────────────────────────────────────

    [Fact]
    public async Task Login_WithValidCredentials_ShouldReturnToken()
    {
        var loginRequest = new { Email = "admin@jobmarketplace.com", Password = "Admin@123!" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("accessToken");
        content.Should().Contain("refreshToken");
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_ShouldReturn401()
    {
        var loginRequest = new { Email = "admin@jobmarketplace.com", Password = "WrongPassword!" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    // ─── Authenticated Endpoints ─────────────────────────────

    [Fact]
    public async Task GetJobs_WithToken_ShouldReturnPaginatedResult()
    {
        // Arrange — login first
        var token = await GetAccessToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.GetAsync("/api/jobs?pageSize=5&cursor=0");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("data");
        content.Should().Contain("hasMore");
        content.Should().Contain("nextCursor");
        content.Should().Contain("pageSize");
    }

    [Fact]
    public async Task GetCompanies_WithToken_ShouldReturnPaginatedResult()
    {
        var token = await GetAccessToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/companies?pageSize=5&cursor=0");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("data");
        content.Should().Contain("hasMore");
    }

    [Fact]
    public async Task SearchJobs_WithToken_ShouldReturnResults()
    {
        var token = await GetAccessToken();
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await _client.GetAsync("/api/jobs/search?pageSize=5&cursor=0");

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var content = await response.Content.ReadAsStringAsync();
        content.Should().Contain("data");
    }

    // ─── Correlation ID ──────────────────────────────────────

    [Fact]
    public async Task AnyRequest_ShouldReturnCorrelationIdHeader()
    {
        var response = await _client.GetAsync("/health");

        response.Headers.Should().ContainKey("X-Correlation-Id");
        response.Headers.GetValues("X-Correlation-Id").First().Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task AnyRequest_WithCorrelationId_ShouldEchoItBack()
    {
        var customId = "my-custom-trace-id-12345";
        var request = new HttpRequestMessage(HttpMethod.Get, "/health");
        request.Headers.Add("X-Correlation-Id", customId);

        var response = await _client.SendAsync(request);

        response.Headers.GetValues("X-Correlation-Id").First().Should().Be(customId);
    }

    // ─── Helper ──────────────────────────────────────────────

    private async Task<string> GetAccessToken()
    {
        var loginRequest = new { Email = "admin@jobmarketplace.com", Password = "Admin@123!" };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);
        var content = await response.Content.ReadAsStringAsync();

        using var doc = JsonDocument.Parse(content);

        // Try both "accessToken" and "data.accessToken" patterns
        if (doc.RootElement.TryGetProperty("accessToken", out var token))
            return token.GetString()!;

        if (doc.RootElement.TryGetProperty("data", out var data)
            && data.TryGetProperty("accessToken", out var nestedToken))
            return nestedToken.GetString()!;

        throw new Exception($"Could not extract accessToken from login response: {content}");
    }
}
