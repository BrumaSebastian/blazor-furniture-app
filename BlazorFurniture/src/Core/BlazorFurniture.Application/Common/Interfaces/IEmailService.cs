namespace BlazorFurniture.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync( IEnumerable<string> receivers, string subject, string htmlContent, CancellationToken ct );
}
