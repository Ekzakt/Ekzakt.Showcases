using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ekzakt.EmailTemplateProvider.Io.Configuration;
using Ekzakt.EmailTemplateProvider.Console;
using Ekzakt.Templates.Console.Utilities;
using Ekzakt.FileManager.AzureBlob.Configuration;
using Microsoft.Extensions.Azure;
using Azure.Identity;
using Ekzakt.FileManager.Core.Options;

var services = new ServiceCollection();

var host = BuildHost(services);

var runner = host.Services.GetRequiredService<TaskRunner>();
var ch = host.Services.GetRequiredService<ConsoleHelpers>();


List<string> taskList = new()
{
    "Get EmailTemplate"
};


while (true)
{
    var key = ch.WriteMenu(taskList, "What do you want to do?");

    switch (key.Key)
    {
        case ConsoleKey.A:
            await runner.GetEmailTemplateAsync();
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
                       .AddBlobServiceClient(context.Configuration.GetSection(FileManagerOptions.SectionName).GetSection(AzureStorageOptions.SectionName));
                   clientBuilder
                       .ConfigureDefaults(context.Configuration.GetSection("Azure:Defaults"));
               });

            services.AddAzureBlobFileManager();
            services.AddEmailTemplateProviderIo();
        })
        .Build();

    return host;
}