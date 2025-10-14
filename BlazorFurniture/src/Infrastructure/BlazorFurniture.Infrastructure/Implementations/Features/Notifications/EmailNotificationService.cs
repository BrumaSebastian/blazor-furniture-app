using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models.Email;
using BlazorFurniture.Templates.Email;
using System.Globalization;

namespace BlazorFurniture.Infrastructure.Implementations.Features.Notifications;

internal class EmailNotificationService( IEmailService emailService ) : IEmailNotificationService
{
    public async Task SendWelcomeEmail( WelcomeEmailModel model, CancellationToken ct )
    {
        var emailDetailsModel = new EmailDetailsModel()
        {
            Template = typeof(WelcomeEmail),
            Parameters = model.ToParameters(),
            CultureToAddresses = new Dictionary<CultureInfo, IEnumerable<string>>()
            {
                { model.Culture, new List<string>() { model.Email } }
            }
        };

        await emailService.SendEmailAsync(emailDetailsModel, ct);
    }
}
