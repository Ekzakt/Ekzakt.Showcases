using Ekzakt.EmailSender.Core.Contracts;
using Ekzakt.EmailSender.Core.EventArguments;
using Ekzakt.EmailSender.Core.Models;
using Ekzakt.EmailSender.Core.Models.Requests;
using Ekzakt.Templates.Console.Utilities;

namespace Ekzakt.EmailSender.Console;

public class TaskRunner(
    ConsoleHelpers c, 
    IEkzaktEmailSenderService emailSenderService) : IDisposable
{
    public async Task SendEmailAsync()
    {
        c.Clear();

        emailSenderService.BeforeEmailSentAsync += OnBeforeEmailSentAsync;
        emailSenderService.AfterEmailSentAsync += OnAfterEmailSentAsync;

        while (true)
        {
            c.Write($"Executing '{nameof(SendEmailAsync)}'...");

            var request = new SendEmailRequest();
            request.Email.Tos.Add(new EmailAddress("mail@ericjansen.com", "Eric"));
            request.Email.Body.Html = "<h1>Body.Html</h1>";
            request.Email.Body.Text = "Body.Text";
            request.Email.Subject = "Email.Subject";

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
