using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Ekzakt.EmailTemplateProvider.AzureBlob.Configuration;
using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Microsoft.Extensions.DependencyInjection;
using EmailTemplateProvider.Console;
using Microsoft.AspNetCore.Http.Features;


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
        services.AddEmailTemplateProvider();
    })
    .Build();


IEmailTemplateProvider _emailProvider = host.Services.GetService<IEmailTemplateProvider>();


RunTask tasks = new RunTask(_emailProvider);

while (true)
{
    Console.Clear();
    Console.WriteLine("Which tasks do you want to run?");
    Console.WriteLine($"A = {nameof(RunTask.GetTemplate)}");
    Console.WriteLine($"B = {nameof(RunTask.ListTemplates)}");

    ConsoleKeyInfo key = Console.ReadKey(true);
    switch (key.Key)
    {
        case ConsoleKey.A:
            await tasks.GetTemplate(); break;
        case ConsoleKey.B:
            await tasks.ListTemplates(); break;
        default:
            Console.WriteLine();
            Console.WriteLine("You chose an invalid option.");
            Console.WriteLine();
            break;
    }

    Console.WriteLine("Do you want to start again? (Y)es (N)o");


    ConsoleKeyInfo yesNo = System.Console.ReadKey(true);
    if (yesNo.Key == ConsoleKey.N)
    {
        break;
    }

    Console.Clear();
}


