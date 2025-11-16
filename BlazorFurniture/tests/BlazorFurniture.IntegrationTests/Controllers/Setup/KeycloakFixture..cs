using BlazorFurniture.Domain.Entities.Keycloak;
using BlazorFurniture.Domain.Enums;
using BlazorFurniture.Infrastructure.External;
using BlazorFurniture.Infrastructure.External.Keycloak.Configurations;
using DotNet.Testcontainers.Builders;
using DotNet.Testcontainers.Containers;
using DotNet.Testcontainers.Networks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace BlazorFurniture.IntegrationTests.Controllers.Setup;

public class KeycloakFixture : IAsyncLifetime
{
    private INetwork? network;
    private IContainer? postgresContainer;
    private IContainer? keycloakContainer;
    private HttpClient adminHttpClient = null!;

    private const int KeycloakPort = 18080;
    private const int KeycloakHealthPort = 19000;
    private const int PostgresPort = 15432;

    public string Authority { get; private set; } = string.Empty;
    public string AdminUsername { get; } = "admin";
    public string AdminPassword { get; } = "admin";
    public string RealmName { get; } = "main";
    public string TestClient { get; set; } = "integration-test-client";
    public string BaseUrl { get; private set; } = string.Empty;
    internal Endpoints Endpoints { get; set; } = default!;
    public KeycloakUser PlatformAdmin { get; set; } = new KeycloakUser
    {
        Username = "platformadmin",
        Email = "platformadmin@test.com",
        Password = "Test123@"
    };

    public async Task InitializeAsync()
    {
        // Create a shared Docker network for container-to-container communication
        network = new NetworkBuilder()
            .Build();

        await network.CreateAsync();

        // Start PostgreSQL for Keycloak
        postgresContainer = new ContainerBuilder()
            .WithImage("postgres:16")
            .WithNetwork(network)
            .WithNetworkAliases("postgres")
            .WithEnvironment("POSTGRES_DB", "keycloak")
            .WithEnvironment("POSTGRES_USER", "keycloak")
            .WithEnvironment("POSTGRES_PASSWORD", "password")
            .WithPortBinding(PostgresPort, 5432)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilInternalTcpPortIsAvailable(5432))
            .Build();

        await postgresContainer.StartAsync();

        // Get the directory where test assembly is running
        var setupDirectory = Path.Combine(AppContext.BaseDirectory, "Controllers", "Setup");

        // Build custom Keycloak image with SPIs
        var keycloakImage = new ImageFromDockerfileBuilder()
            .WithDockerfileDirectory(setupDirectory)
            .WithDockerfile("Keycloak.dockerfile")
            .WithName("keycloak-with-custom-spi:test")
            .WithCleanUp(true)
            .Build();

        await keycloakImage.CreateAsync();

        // Start Keycloak with custom image
        // Use the network alias "postgres" and internal port 5432 for container-to-container communication
        keycloakContainer = new ContainerBuilder()
            .WithImage("keycloak-with-custom-spi:test")
            .WithNetwork(network)
            .WithEnvironment("KEYCLOAK_ADMIN", AdminUsername)
            .WithEnvironment("KEYCLOAK_ADMIN_PASSWORD", AdminPassword)
            .WithEnvironment("KC_DB", "postgres")
            .WithEnvironment("KC_DB_URL", "jdbc:postgresql://postgres:5432/keycloak")
            .WithEnvironment("KC_DB_USERNAME", "keycloak")
            .WithEnvironment("KC_DB_PASSWORD", "password")
            .WithEnvironment("KC_HOSTNAME_STRICT", "false")
            .WithEnvironment("KC_HTTP_ENABLED", "true")
            .WithEnvironment("KC_HTTP_ENABLED", "true")
            .WithEnvironment("KC_HEALTH_ENABLED", "true")
            .WithPortBinding(KeycloakPort, 8080)
            .WithPortBinding(KeycloakHealthPort, 9000)
            .WithCommand("start-dev", "--import-realm")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r.ForPort(9000).ForPath("/health/ready")))
            .Build();

        await keycloakContainer.StartAsync();

        BaseUrl = $"http://localhost:{KeycloakPort}";
        Authority = $"{BaseUrl}/realms/{RealmName}";
        Endpoints = new Endpoints(RealmName);
        adminHttpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };

        // Create platform admin user
        var adminId = await CreateUser(PlatformAdmin.Username, PlatformAdmin.Email, PlatformAdmin.Password);

        // Assign admin realm role to platform admin user
        var platformAdminRole = PlatformRoles.Admin.ToString().ToLower();
        var adminRole = await GetRole(platformAdminRole);
        await AssignRole(adminId, adminRole);
    }

    public async Task<string> CreateUser( KeycloakUser user )
    {
        return await CreateUser(user.Username, user.Email, user.Password);
    }

    public async Task<string> CreateUser( string username, string email, string password, string? firstName = null, string? lastName = null, bool enabled = true )
    {
        await GetAndSetAdminAccessToken();

        var userRepresentation = new
        {
            username,
            email,
            firstName = firstName ?? username,
            lastName = lastName ?? username,
            enabled,
            emailVerified = true,
            credentials = new[]
            {
                new
                {
                    type = "password",
                    value = password,
                    temporary = false
                }
            }
        };

        var response = await adminHttpClient!.PostAsJsonAsync(
            $"/admin/realms/{RealmName}/users",
            userRepresentation,
            new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

        response.EnsureSuccessStatusCode();

        // Get the user ID from the Location header
        var locationHeader = response.Headers.Location?.ToString();
        var userId = locationHeader?.Split('/').Last();

        return userId ?? throw new InvalidOperationException("Failed to retrieve user ID");
    }

    public async Task<string> GetUserToken( HttpClient httpClient, string username, string password )
    {
        var tokenRequest = HttpRequestMessageBuilder.Create(httpClient!, HttpMethod.Post)
                .WithPath(Endpoints.AccessToken())
                .WithFormParams(new Dictionary<string, string>
                {
                    { OpenIdConnectParameterNames.GrantType, OpenIdConnectGrantTypes.Password },
                    { OpenIdConnectParameterNames.ClientId, TestClient },
                    { OpenIdConnectParameterNames.Username, username },
                    { OpenIdConnectParameterNames.Password, password }
                })
                .Build();

        var tokenResponse = await httpClient.SendAsync(tokenRequest);
        tokenResponse.EnsureSuccessStatusCode();

        var token = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
        return token.GetProperty("access_token").GetString()!;
    }

    public async Task<string> GetAndSetUserToken( HttpClient httpClient, string username, string password )
    {
        var accessToken = await GetUserToken(httpClient, username, password);
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

        return accessToken;
    }

    private async Task AssignRole( string userId, RoleRepresentation role )
    {
        await GetAndSetAdminAccessToken();
        var request = HttpRequestMessageBuilder.Create(adminHttpClient!, HttpMethod.Post)
                .WithPath($"/admin/realms/{RealmName}/users/{userId}/role-mappings/realm")
                .WithContent(new List<RoleRepresentation> { role })
                .Build();
        var response = await adminHttpClient!.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }

    private async Task<List<RoleRepresentation>> GetRoles( string? search = null )
    {
        var request = HttpRequestMessageBuilder.Create(adminHttpClient!, HttpMethod.Get)
                .WithPath($"/admin/realms/{RealmName}/roles")
                .AddQueryParam("search", search)
                .Build();

        var tokenResponse = await adminHttpClient!.SendAsync(request);
        tokenResponse.EnsureSuccessStatusCode();

        var roles = await tokenResponse.Content.ReadFromJsonAsync<List<RoleRepresentation>>();

        return roles ?? new List<RoleRepresentation>();
    }

    private async Task<RoleRepresentation> GetRole( string roleName )
    {
        var request = HttpRequestMessageBuilder.Create(adminHttpClient!, HttpMethod.Get)
                .WithPath($"/admin/realms/{RealmName}/roles/{roleName}")
                .Build();

        var tokenResponse = await adminHttpClient!.SendAsync(request);
        tokenResponse.EnsureSuccessStatusCode();

        var role = await tokenResponse.Content.ReadFromJsonAsync<RoleRepresentation>();

        return role ?? throw new Exception($"Failed to obtain role {roleName}");
    }

    private async Task<string> GetAndSetAdminAccessToken()
    {
        var accessToken = await GetAdminAccessToken();
        adminHttpClient!.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

        return accessToken;
    }

    private async Task<string> GetAdminAccessToken()
    {
        var tokenRequest = HttpRequestMessageBuilder.Create(adminHttpClient!, HttpMethod.Post)
                .WithPath($"realms/master/protocol/openid-connect/token")
                .WithFormParams(new Dictionary<string, string>
                {
                    { OpenIdConnectParameterNames.GrantType, OpenIdConnectGrantTypes.Password },
                    { OpenIdConnectParameterNames.ClientId, "admin-cli" },
                    { OpenIdConnectParameterNames.Username, AdminUsername },
                    { OpenIdConnectParameterNames.Password, AdminPassword }
                })
                .Build();

        var tokenResponse = await adminHttpClient!.SendAsync(tokenRequest);
        tokenResponse.EnsureSuccessStatusCode();

        var tokenData = await tokenResponse.Content.ReadFromJsonAsync<JsonElement>();
        var accessToken = tokenData.GetProperty("access_token").GetString()
            ?? throw new InvalidOperationException("Failed to obtain access token");

        return accessToken;
    }

    async ValueTask IAsyncLifetime.InitializeAsync()
    {
        await InitializeAsync();
    }

    async ValueTask IAsyncDisposable.DisposeAsync()
    {
        if (keycloakContainer != null)
            await keycloakContainer.DisposeAsync();

        if (postgresContainer != null)
            await postgresContainer.DisposeAsync();

        if (network != null)
            await network.DeleteAsync();
    }
}
