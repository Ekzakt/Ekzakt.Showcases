using Ekzakt.EmailSender.Core.Contracts;
using Ekzakt.EmailSender.Core.EventArguments;
using Ekzakt.EmailSender.Core.Models;
using Ekzakt.Templates.Console.Utilities;

namespace Ekzakt.EmailSender.Console;

public class TaskRunner(
    ConsoleHelpers c, 
    IEmailSenderService emailSenderService) : IDisposable
{
    public async Task SendEmailAsync()
    {
        int counter = 0;

        c.Clear();

        emailSenderService.BeforeEmailSentAsync += OnBeforeEmailSentAsync;
        emailSenderService.AfterEmailSentAsync += OnAfterEmailSentAsync;

        while (true)
        {
            c.Write($"Executing '{nameof(SendEmailAsync)}'...");

            counter++;

            var request = new SendEmailRequest();

            request.Tos.Add(new EmailAddress("mail@ericjansen.com", "Eric"));
            request.Tos.Add(new EmailAddress("mail@johndoe.com", "John"));
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




    #region Helpers

    public void Dispose()
    {
        emailSenderService.BeforeEmailSentAsync -= OnBeforeEmailSentAsync;
        emailSenderService.AfterEmailSentAsync -= OnAfterEmailSentAsync;
    }


    private async Task OnBeforeEmailSentAsync(BeforeSendEmailEventArgs e)
    {
        await Task.Delay(10);
        c.WriteSuccess($"{nameof(OnBeforeEmailSentAsync)} fired.");
        c.WriteSuccess(e);
    }


    private async Task OnAfterEmailSentAsync(AfterSendEmailEventArgs e)
    {
        await Task.Delay(10);
        c.WriteSuccess($"{nameof(OnAfterEmailSentAsync)} fired.");
        c.WriteSuccess(e);
    }

    #endregion Helpers
}
