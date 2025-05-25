namespace BlazorFurniture.Modules.Keycloak.Models;

public class CreateRealmRequest
{
    public string Name { get; set; }
}


//public class CreateRealmRequest
//{
//    public string Realm { get; set; }
//    public bool Enabled { get; set; } = true;
//    public string DisplayName { get; set; }
//    public string DisplayNameHtml { get; set; }
//    public bool RegistrationAllowed { get; set; } = false;
//    public bool RegistrationEmailAsUsername { get; set; } = false;
//    public bool RememberMe { get; set; } = true;
//    public bool VerifyEmail { get; set; } = false;
//    public bool LoginWithEmailAllowed { get; set; } = true;
//    public bool DuplicateEmailsAllowed { get; set; } = false;
//    public bool ResetPasswordAllowed { get; set; } = true;
//    public bool EditUsernameAllowed { get; set; } = false;
//    public string SslRequired { get; set; } = "external";
//    public int AccessTokenLifespan { get; set; } = 300;
//    public int AccessTokenLifespanForImplicitFlow { get; set; } = 900;
//    public int SsoSessionIdleTimeout { get; set; } = 1800;
//    public int SsoSessionMaxLifespan { get; set; } = 36000;
//    public int OfflineSessionIdleTimeout { get; set; } = 2592000;
//    public int AccessCodeLifespan { get; set; } = 60;
//    public int AccessCodeLifespanUserAction { get; set; } = 300;
//    public int AccessCodeLifespanLogin { get; set; } = 1800;
//}