using Ekzakt.EmailTemplateProvider.Core.Contracts;
using System.Text.Json;

namespace EmailTemplateProvider.Console;

public class RunTask
{
    private ConsoleHelpers c = new();
    private readonly IEmailTemplateProvider _emailProvider;


    public RunTask(IEmailTemplateProvider emailProvider)
    {
        _emailProvider = emailProvider;
    }


    public async Task GetTemplate()
    {
        c.Clear();

        while (true)
        {
            var templateName = string.Empty;
            var language = string.Empty;


            while (string.IsNullOrEmpty(templateName))
            {
                c.Write("Enter the name of template you want to build:");
                templateName = c.ReadLine();
            }


            while (string.IsNullOrEmpty(language))
            {
                c.Write("Enter the name of the language in which the template should be built:");
                language = c.ReadLine();
            }


            try
            {
                var template = await _emailProvider!.GetTemplateAsync(templateName, language);

                c.WriteResult(c.WriteJson(template));

            }
            catch (Exception ex)
            {
                c.WriteError(ex);
            }


            if (!c.ConfirmYes("Would you like to build another template?")) break;

        }
    }


    public async Task ListTemplates()
    {
        c.Clear();

        try
        {
            c.Write("Getting list of files...");

            var list = await _emailProvider.ListAllFilesAsync();
            
            c.WriteResult(c.WriteJson(list));

        }
        catch (Exception ex)
        {
            c.WriteError(ex);
        }
    }
}

