﻿using Ekzakt.EmailSender.Core.Contracts;
using Ekzakt.EmailSender.Core.EventArguments;
using Ekzakt.EmailSender.Core.Models.Requests;
using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Ekzakt.EmailTemplateProvider.Core.Models;
using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.Templates.Console.Utilities;
using System.Text.Json;

namespace Ekzakt.EmailSender.Console;

public class TaskRunner(
    ConsoleHelpers c, 
    IEkzaktEmailSenderService emailSenderService,
    IEkzaktEmailTemplateProvider templateProvider) : IDisposable
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

            var templatesResponse = await templateProvider.GetTemplateAsync(new EmailTemplateRequest
            {
                CultureName = "en-us",
                TemplateName = "sample"
            });

            var templates = templatesResponse.Templates;

            if (!templatesResponse.IsSuccess)
            {
                c.WriteError(templatesResponse);
            }
            
            foreach (var template in templates ?? new List<EmailTemplate>())
            { 
                counter++;

                var request = new SendEmailRequest();

                request.TemplateName = "Contact";
                request.RecipientType = "User";

                request.Email.Tos.Add(new Core.Models.EmailAddress("mail@ericjansen.com", "Eric"));
                request.Email.Subject = template.Subject;
                request.Email.Body.Html = template.Body.Html;
                request.Email.Body.Text = template.Body.Text;

                var templateString = JsonSerializer.Serialize(template, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

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
