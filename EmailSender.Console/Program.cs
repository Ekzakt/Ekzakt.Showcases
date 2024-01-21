using Microsoft.Extensions.DependencyInjection;
using Ekzakt.EmailSender.Smtp.Configuration;
using Ekzakt.EmailSender.Core.Contracts;
using Ekzakt.EmailSender.Core.Models;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;


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
        services.AddSmtpEmailSender();
    })
    .Build();

IEmailSenderService _emailService = host.Services.GetService<IEmailSenderService>();

int counter = 0;

while (true)
{
    Console.WriteLine("Would you like to send an email? (Y)es (N)o");
    ConsoleKeyInfo yesNo = Console.ReadKey(true);

    if (yesNo.Key == ConsoleKey.N)
    {
        Console.WriteLine();
        Console.WriteLine("Exiting...");
        break;
    }

    counter++;

    var request = new SendEmailRequest();

    request.Tos.Add(new EmailAddress("mail@ericjansen.com", "eric"));
    request.Tos.Add(new EmailAddress("nickverelst@yahoo.com", "nick"));
    request.Subject = $"Nr. {counter}: Send from Console Application";
    request.Body.Html = "<h1>Console Application</h1>";
    request.Body.Html += "<p>This email was sent from the EmailSender.Console application.</p>";
    request.Body.PlainText = "Console Application\n\rThis email was sent from the EmailSender.Console application.";

    try
    {
        _ = await _emailService!.SendAsync(request);
    }
    catch (Exception ex)
    {
        Console.WriteLine(ex.ToString());
    }

    Console.WriteLine();

}




