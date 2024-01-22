using Ekzakt.FileManager.AzureBlob.Configuration;
using Ekzakt.FileManager.AzureBlob.Services;
using Ekzakt.FileManager.Console;
using Ekzakt.FileManager.Core.Contracts;
using Ekzakt.Templates.Console.Utilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var services = new ServiceCollection();

var host = BuildHost(services);

var runner = host.Services.GetRequiredService<TaskRunner>();
var ch = host.Services.GetRequiredService<ConsoleHelpers>();


List<string> taskList = new()
{
    "A save file.",
    "B delete file.",
    "C download file."
};


while (true)
{
    var key = ch.WriteTaskList(taskList);

    switch (key.Key)
    {
        case ConsoleKey.A:
            await runner.SaveFile();
            break;
        default:
            break;
    }

    if (key.Key.Equals(ConsoleKey.Q))
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
            services.AddAzureBlobFileManager();
        })
        .Build();

    return host;
}

