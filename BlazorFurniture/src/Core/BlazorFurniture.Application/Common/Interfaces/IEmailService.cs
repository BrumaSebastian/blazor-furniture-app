using BlazorFurniture.Application.Common.Models.Email;

namespace BlazorFurniture.Application.Common.Interfaces;

public interface IEmailService
{
    Task SendEmailAsync( EmailDetailsModel model, CancellationToken ct );
}
