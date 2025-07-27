namespace BlazorFurniture.Modules.Keycloak.Models;

public class UpdatePasswordRequest
{
    public string CurrentPassword { get; set; }
    public string NewPassword { get; set; }
}
