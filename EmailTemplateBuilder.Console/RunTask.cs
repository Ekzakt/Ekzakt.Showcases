using Ekzakt.EmailTemplateProvider.Core.Contracts;
using System.Text.Json;

namespace EmailTemplateProvider.Console;

public class RunTask
{
    private readonly IEmailTemplateProvider _emailProvider;

    public RunTask(IEmailTemplateProvider emailProvider)
    {
        _emailProvider = emailProvider;
    }


    public async Task GetTemplate()
    {
        Clear();

        while (true)
        {
            var templateName = string.Empty;
            var language = string.Empty;


            while (string.IsNullOrEmpty(templateName))
            {
                System.Console.WriteLine("Enter the name of template you want to build:");
                templateName = System.Console.ReadLine();
            }


            while (string.IsNullOrEmpty(language))
            {
                WriteEmpty("Enter the name of the language in which the template should be built:");
                language = System.Console.ReadLine();
            }


            try
            {
                var template = await _emailProvider!.GetTemplateAsync(templateName, language);
                var jsonString = JsonSerializer.Serialize(template, new JsonSerializerOptions
                {
                    WriteIndented = true
                });

                WriteEmpty("Template build:");
                WriteEmpty(jsonString);
            }
            catch (Exception ex)
            {
                WriteError(ex);
            }


            WriteLine("Would you like to build another template? (Y)es (N)o");
            ConsoleKeyInfo yesNo = System.Console.ReadKey(true);

            Clear();

            if (yesNo.Key == ConsoleKey.N)
            {
                break;
            }
        }
    }


    public async Task ListTemplates()
    {
        Clear();

        try
        {
            WriteEmpty("Getting list of files...");

            var list = await _emailProvider.ListAllFilesAsync();
            var jsonString = JsonSerializer.Serialize(list, new JsonSerializerOptions
            {
                WriteIndented = true
            });

            WriteLine(jsonString);

        }
        catch (Exception ex)
        {
            WriteError(ex);
        }
    }




    #region Helpers

    private void WriteEmpty(string? message = "")
    {
        System.Console.WriteLine(message);
    }


    private void WriteLine(string? message = "")
    {
        WriteLine();
        System.Console.WriteLine();

    }


    private void WriteError(Exception ex)
    {
        System.Console.ForegroundColor = ConsoleColor.Red;
        WriteLine(ex.ToString());
        System.Console.ForegroundColor = ConsoleColor.White;
    }


    private void Clear()
    {
        System.Console.Clear();
    }


    #endregion Helpers
}

