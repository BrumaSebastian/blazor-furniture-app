

using System.Globalization;

namespace BlazorFurniture.Application.Common.Models.Email;

public class WelcomeEmailModel( string name, string email, CultureInfo culture ) : EmailModel
{
    public string Name { get; set; } = name;

    public string Email { get; set; } = email;
    public CultureInfo Culture { get; set; } = culture;

    public override Dictionary<string, string> ToParameters()
        => new()
        {
            { nameof(Name), Name }
        };
}
