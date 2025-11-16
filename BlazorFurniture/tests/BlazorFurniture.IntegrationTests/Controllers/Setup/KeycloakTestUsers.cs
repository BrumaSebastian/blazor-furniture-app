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
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public required string Password { get; set; }
}
