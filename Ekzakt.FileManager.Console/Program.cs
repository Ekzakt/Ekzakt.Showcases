using Azure.Identity;
using Ekzakt.FileManager.AzureBlob.Configuration;
using Ekzakt.FileManager.Console;
using Ekzakt.FileManager.Core.Options;
using Ekzakt.Templates.Console.Utilities;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var services = new ServiceCollection();

var host = BuildHost(services);

var runner = host.Services.GetRequiredService<TaskRunner>();
var c = host.Services.GetRequiredService<ConsoleHelpers>();

List<string> taskList =
[
    "Save file.",
    "Save file in chunks.",
    "Save file using stream.",
    "List files.",
    "Download SasToken for file.",
    "Delete file.",
    "Read file as string."
];


while (true)
{
    var key = c.WriteMenu(taskList);

    switch (key.Key)
    {
        case ConsoleKey.A:
            await runner.SaveFileAsync();
            break;
        case ConsoleKey.B:
            await runner.SaveFileAsync(true);
            break;
        case ConsoleKey.C:
            await runner.SaveFileAsync(false, true);
            break;
        case ConsoleKey.D:
            await runner.ListFilesAsync();
            break;
        case ConsoleKey.E:
            await runner.DownloadFileAsync();
            break;
        case ConsoleKey.F:
            await runner.DeleteFileAsync();
            break;
        case ConsoleKey.G:
            await runner.ReadFileAsStringAsync();
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
            services
               .Configure<FileManagerOptions>
               (
                   context.Configuration
                       .GetSection(FileManagerOptions.SectionName)
               );

            services.AddScoped<TaskRunner>();
            services.AddScoped<ConsoleHelpers>();

            services
                .AddAzureClients(clientBuilder => {
                    clientBuilder
                        .UseCredential(new DefaultAzureCredential());
                    clientBuilder
                        .AddBlobServiceClient(context.Configuration.GetSection(AzureStorageBlobsOptions.SectionName));
                    clientBuilder
                        .ConfigureDefaults(context.Configuration.GetSection("Azure:Defaults"));
                });

            services.AddAzureBlobFileManager();

        })
        .Build();

    return host;
}

