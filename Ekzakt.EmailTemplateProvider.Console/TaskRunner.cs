using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.Templates.Console.Utilities;

namespace Ekzakt.EmailTemplateProvider.Console;

public class TaskRunner(
    ConsoleHelpers c,
    IEmailTemplateProvider templateProvider)
{
    public async Task GetEmailTemplateAsync()
    {
        while (true)
        {
            c.Clear();

            var templateName = string.Empty;
            var cultureName = string.Empty;


            while (string.IsNullOrEmpty(templateName))
            {
                c.Write("Enter the name of template you want to build:");
                templateName = c.ReadLine();
            }


            while (string.IsNullOrEmpty(cultureName))
            {
                c.Write("Enter the name of the language in which the template should be built:");
                cultureName = c.ReadLine();
            }


            try
            {
                var result = await templateProvider!.GetTemplateAsync(new EmailTemplateRequest
                {
                    TemplateName = templateName,
                    CultureName = cultureName
                });

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


            if (!c.ConfirmYesNo("Would you like to build another template?")) break;

        }
    }




    #region Helpers



    #endregion Helpers
}
