using BlazorFurniture.Application.Common.Models.Email;

namespace BlazorFurniture.Application.Common.Options;

public class EmailOptions
{
    public const string NAME = "SmtpConfig";

    public required string Host { get; set; }
    public required int Port { get; set; }
    public bool UseSsl { get; set; }
    public bool Authentication { get; set; }
    public HashSet<EmailCredential> Accounts { get; set; } = [];
}

public class EmailCredential
{
    public required string Username { get; set; }
    public required string Password { get; set; }
    public EmailAccountTypes Type { get; set; } = EmailAccountTypes.NoReply;

    public override bool Equals( object? obj )
    {
        return obj is EmailCredential other && Type == other.Type;
    }

    public override int GetHashCode()
    {
        return Type.GetHashCode();
    }
}
