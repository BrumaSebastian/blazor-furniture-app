using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Models.Email;
using BlazorFurniture.Application.Common.Options;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BlazorFurniture.Infrastructure.Implementations.Features.Notifications;

internal class EmailService( IRazorHtmlRenderer renderer, EmailOptions emailOptions, ILogger<EmailService> logger ) : IEmailService
{
    public async Task SendEmailAsync( EmailDetailsModel model, CancellationToken ct )
    {
        var credentials = emailOptions.Accounts.FirstOrDefault();

        if (emailOptions.Authentication && credentials is null)
        {
            logger.LogError("No email credential found");

            throw new InvalidOperationException($"No email credential found");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(credentials!.Username, credentials.Username));

        using var client = new SmtpClient();

        await client.ConnectAsync(emailOptions.Host, emailOptions.Port, emailOptions.UseSsl, ct);

        if (emailOptions.Authentication)
        {
            await client.AuthenticateAsync(credentials.Username, credentials.Password, ct);
        }

        foreach ((var culture, var addresses) in model.CultureToAddresses)
        {
            message.Subject = renderer.RenderSubject(model.Template, model.Parameters, culture);
            message.Body = new BodyBuilder()
            {
                HtmlBody = await renderer.RenderHtml(model.Template, model.Parameters, culture, ct),
                TextBody = renderer.RenderText(model.Template, model.Parameters, culture)
            }.ToMessageBody();

            await SendMessage(message, client, addresses, ct);
        }

        if (client.IsConnected)
        {
            await client.DisconnectAsync(true, ct);
        }
    }

    private async Task SendMessage( MimeMessage message, SmtpClient client, IEnumerable<string> addresses, CancellationToken cancellationToken )
    {
        foreach (var address in addresses)
        {
            message.To.Clear();
            try
            {
                message.To.Add(InternetAddress.Parse(address));
                await client.SendAsync(message, cancellationToken);
            }
            catch (ParseException e)
            {
                logger.LogError(e, $"Could not parse email address {address}");
            }
            catch (Exception e)
            {
                logger.LogError(e, $"Could not send email to {address}");
            }
        }
    }
}
