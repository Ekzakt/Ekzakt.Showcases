using Ekzakt.EmailTemplateProvider.Core;
using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Ekzakt.EmailTemplateProvider.Core.Extensions;
using Ekzakt.EmailTemplateProvider.Core.Requests;
using Ekzakt.Templates.Console.Utilities;
using Ekzakt.Utilities;

namespace Ekzakt.EmailTemplateProvider.Console;

public class TaskRunner(
    ConsoleHelpers c,
    IEkzaktEmailTemplateProvider templateProvider)
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
                var request = new EmailTemplateRequest
                {
                    TemplateName = templateName,
                    CultureName = cultureName
                };

                var result = await templateProvider!.GetEmailTemplateAsync(request);
                
                if (result is not null && result.IsSuccess)
                {
                    var stringReplacer = new StringReplacer();
                    stringReplacer.AddReplacement("IpAddress", "\"****************___MyAddedIpAddress___\"****************");
                    stringReplacer.AddReplacement("ContactName", "****************___Rixke___****************");

                    var x = result!.EmailTemplateInfo!.ApplyReplacements(stringReplacer);

                    c.WriteSuccess(x);
                }
                else
                { 
                    c.WriteError(false.ToString());
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
