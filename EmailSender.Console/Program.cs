using Microsoft.Extensions.DependencyInjection;
using Ekzakt.EmailSender.Smtp.Configuration;
using Ekzakt.EmailSender.Core.Contracts;
using Ekzakt.EmailSender.Core.Models;

using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

IServiceCollection services = new ServiceCollection();

services.AddSmtpEmailSender();

var host = Host
    .CreateDefaultBuilder(args)
    .ConfigureAppConfiguration(config => 
        config.AddJsonFile(
            path: "appsettings.Development.json",
            optional: true, 
            reloadOnChange: true)
        )
    .ConfigureServices((context, services) =>
    {
        services.AddSmtpEmailSender(options =>
        {
            options.FromDisplayName = "Console App";
            options.FromAddress = "info@smoothsensation.net";
            options.UserName = "info@smoothsensation.net";
            options.Password = "hTioin_2nB";
            options.Host = "smtp-auth.mailprotect.be";
            options.Port = 587;
        });         
    })
    .Build();

IEmailSenderService _emailService = host.Services.GetService<IEmailSenderService>();

var request = new SendEmailRequest();

request.Tos.Add(new EmailAddress("mail@ericjansen.com"));
request.Subject = "Send from Console Application";
request.HtmlBody = "<h1>Console Applications</h1>";

var result = await _emailService!.SendAsync(request);

Console.WriteLine(result.ServerResponse);
