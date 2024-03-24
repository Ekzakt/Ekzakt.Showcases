using Ekzakt.EmailSender.Console;
using Ekzakt.EmailSender.Smtp.Configuration;
using Ekzakt.EmailTemplateProvider.Core.Contracts;
using Ekzakt.Templates.Console.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Ekzakt.FileManager.AzureBlob.Configuration;
using Ekzakt.EmailTemplateProvider.Io.Services;
using Microsoft.Extensions.Azure;
using Azure.Identity;
using Ekzakt.FileManager.Core.Options;
using Ekzakt.EmailTemplateProvider.Io.Configuration;

var services = new ServiceCollection();

var host = BuildHost(services);

var runner = host.Services.GetRequiredService<TaskRunner>();
var ch = host.Services.GetRequiredService<ConsoleHelpers>();


List<string> taskList = new()
{
    "Send an e-mail."
};


while (true)
{
    var key = ch.WriteMenu(taskList, "What do you want to do?");

    switch (key.Key)
    {
        case ConsoleKey.A:
            await runner.SendEmailAsync();
            break;
        default:
            break;
    }

    if (key.Key.Equals(ConsoleKey.Escape))
    {
        break;
    }
}

IHost BuildHost(ServiceCollection serviceCollection)
{
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
            services.AddScoped<TaskRunner>();
            services.AddScoped<ConsoleHelpers>();

            services
               .AddAzureClients(clientBuilder => {
                   clientBuilder
                       .UseCredential(new DefaultAzureCredential());
                   clientBuilder
                       .AddBlobServiceClient(context.Configuration.GetSection(FileManagerOptions.SectionName).GetSection(AzureStorageBlobsOptions.SectionName));
               });

            services.AddEkzaktSmtpEmailSender();
            services.AddAzureBlobFileManager();
            services.AddEkzaktEmailTemplateProviderIo();

            services.AddScoped<IEkzaktEmailTemplateProvider, EkzaktEmailTemplateProviderIo>();
        })
        .Build();

    return host;
}

