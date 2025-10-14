using BlazorFurniture.Application.Common.Models.Email;

namespace BlazorFurniture.Application.Common.Interfaces;

public interface IEmailNotificationService
{
    Task SendWelcomeEmail( WelcomeEmailModel model, CancellationToken ct );
}
