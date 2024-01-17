using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

IServiceCollection services = new ServiceCollection();

//services.AddSmtpEmailSender();

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config =>
        config.AddJsonFile(
            path: "appsettings.Development.json",
            optional: false,
            reloadOnChange: true)
        )
    .ConfigureServices((context, services) =>
    {
        //services.AddSmtpEmailSender();
    })
    .Build();



while (true)
{
    var templateName = string.Empty;
    var language = string.Empty;

    while (string.IsNullOrEmpty(templateName))
    {
        Console.WriteLine("Which template would you like to build?");
        templateName = Console.ReadLine();
    }

    while (string.IsNullOrEmpty(language))
    { 
        Console.WriteLine("In which language would you like the template to be built?");
        language = Console.ReadLine();
    }

    Console.WriteLine("Build template? (Y)es (N)o");
    ConsoleKeyInfo yesNo = Console.ReadKey(true);

    if (yesNo.Key == ConsoleKey.N)
    {
        Console.WriteLine("Exiting...");
        break;
    }

    Console.WriteLine("Template build:");
    Console.WriteLine("Response");

    Console.WriteLine("Would you like to build another template?");
    yesNo = Console.ReadKey(true);

    if (yesNo.Key == ConsoleKey.N)
    {
        Console.WriteLine("Exiting...");
        break;
    }
}