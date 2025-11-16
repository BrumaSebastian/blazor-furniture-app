namespace BlazorFurniture.IntegrationTests.Controllers.Setup;

public static class KeycloakTestUsers
{
    public static KeycloakUser PlatformAdmin { get; set; } = new KeycloakUser
    {
        Username = "platformadmin",
        Email = "platformadmin@test.com",
        Password = "Test123@"
    };

    public static KeycloakUser GroupManager { get; set; } = new KeycloakUser
    {
        Username = "groupmanager",
        Email = "groupmanager@test.com",
        Password = "Test123@"
    };
}

public class KeycloakUser
{
    public Guid Id { get; set; }
    public string Username { get; set; } = $"test-user{Guid.NewGuid()}";
    public string? Email { get; set; }
    public string Password { get; set; } = "Test123@";
}
