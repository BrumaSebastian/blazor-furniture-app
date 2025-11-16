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
    private HttpClient? adminHttpClient;

    public string Authority { get; private set; } = string.Empty;
    public string AdminUsername { get; } = "admin";
    public string AdminPassword { get; } = "admin";
    public string RealmName { get; } = "main";
    public string TestClient { get; set; } = "integration-test-client";
    public string BaseUrl { get; private set; } = string.Empty;
    internal Endpoints Endpoints { get; set; } = default!;
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
            .WithPortBinding(5432, true)
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
            .WithPortBinding(8080, true)
            .WithPortBinding(9000, true)
            .WithCommand("start-dev", "--import-realm")
            .WithWaitStrategy(Wait.ForUnixContainer()
                .UntilHttpRequestIsSucceeded(r => r.ForPort(9000).ForPath("/health/ready")))
            .Build();

        await keycloakContainer.StartAsync();

        var keycloakPort = keycloakContainer.GetMappedPublicPort(8080);
        BaseUrl = $"http://localhost:{keycloakPort}";
        Authority = $"http://localhost:{keycloakPort}/realms/{RealmName}";

        Endpoints = new Endpoints(RealmName);
        adminHttpClient = new HttpClient { BaseAddress = new Uri(BaseUrl) };
    }

    public async Task<string> CreateUserAsync( string username, string email, string password, string? firstName = null, string? lastName = null, bool enabled = true )
    {
        var accessToken = await GetAdminAccessTokenAsync();

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

    private async Task<string> GetAdminAccessTokenAsync()
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

        // Set authorization header for subsequent requests
        adminHttpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

        return accessToken;
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
