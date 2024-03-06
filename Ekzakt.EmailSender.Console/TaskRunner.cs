
using Ekzakt.EmailSender.Core.Contracts;
using Ekzakt.EmailSender.Core.Models;
using Ekzakt.EmailSender.Smtp.Configuration;
using Ekzakt.Templates.Console.Utilities;
using Microsoft.Extensions.Options;

namespace Ekzakt.EmailSender.Console;

public class TaskRunner(ConsoleHelpers c, IEmailSenderService emailSenderService, IOptions<SmtpEmailSenderOptions> options)
{
    public async Task SendEmailAsync()
    {
        int counter = 0;

        c.Clear();

        while (true)
        {
            c.Write($"Executing '{nameof(SendEmailAsync)}'...");
            c.Write($"Send test-email to {options.Value.SenderDisplayName} ({options.Value.SenderAddress})");

            c.Write();

            counter++;

            var request = new SendEmailRequest();

            request.Tos.Add(new EmailAddress("mail@ericjansen.com", "Eric"));
            request.Subject = $"Nr. {counter}: Send from Console Application";
            request.Body.Html = "<h1>Console Application</h1>";
            request.Body.Html += "<p>This email was sent from the EmailSender.Console application.</p>";
            request.Body.PlainText = "Console Application\n\rThis email was sent from the EmailSender.Console application.";

            try
            {
                var result = await emailSenderService!.SendAsync(request);

                if (result.IsSuccess)
                {
                    c.WriteSuccess(result);
                }
                else
                {
                    c.WriteError(result);
                }
            }
            catch (Exception ex)
            {
                c.WriteError(ex.ToString());
            }

            if (!c.ConfirmYesNo("Would you like to try again?")) break;
        }
    }
}
