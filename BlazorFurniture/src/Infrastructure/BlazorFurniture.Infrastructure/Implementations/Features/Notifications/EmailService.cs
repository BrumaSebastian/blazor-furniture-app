using BlazorFurniture.Application.Common.Interfaces;
using BlazorFurniture.Application.Common.Options;
using BlazorFurniture.Templates.Email;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace BlazorFurniture.Infrastructure.Implementations.Features.Notifications;

internal class EmailService( IRazorHtmlRenderer renderer, EmailOptions emailOptions, ILogger<EmailService> logger ) : IEmailService
{
    public async Task SendEmailAsync( IEnumerable<string> receivers, string subject, string htmlContent, CancellationToken ct )
    {
        var s = renderer;
        var credentials = emailOptions.Accounts.FirstOrDefault();

        if (emailOptions.Authentication && credentials is null)
        {
            logger.LogError("No email credential found");

            throw new InvalidOperationException($"No email credential found");
        }

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(credentials!.Username, credentials.Username));

        using var client = new SmtpClient();
        try
        {
            await client.ConnectAsync(emailOptions.Host, emailOptions.Port, emailOptions.UseSsl, ct);

            if (emailOptions.Authentication)
            {
                await client.AuthenticateAsync(credentials.Username, credentials.Password, ct);
            }

            foreach (var addresses in receivers)
            {
                message.Subject = subject;
                message.Body = new BodyBuilder()
                {
                    HtmlBody = string.IsNullOrWhiteSpace(htmlContent)
                       ? await RenderWelcomeEmailAsync(renderer, userName: addresses.Split('@')[0], actionUrl: null, ct)
                       : htmlContent
                }.ToMessageBody();

                await SendMessage(message, client, [addresses], ct);
            }
        }
        catch (Exception e)
        {
            logger.LogError(e, $"An exception occured while sending notification {subject}");
        }

        if (client.IsConnected)
        {
            await client.DisconnectAsync(true, ct);
        }
    }

    private async Task SendMessage( MimeMessage message, SmtpClient client, List<string> addresses, CancellationToken cancellationToken )
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

    private static Task<string> RenderWelcomeEmailAsync(
        IRazorHtmlRenderer renderer,
        string userName,
        string? actionUrl,
        CancellationToken ct )
    {
        return renderer.RenderAsync<WelcomeEmail>(new
        {
            UserName = userName,
            ActionUrl = actionUrl
        }, ct);
    }
}
