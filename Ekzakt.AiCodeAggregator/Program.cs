using Ekzakt.AiCodeAggregator;
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
    "Aggregate project files."
};


while (true)
{
    var key = ch.WriteMenu(taskList, "What do you want to do?");

    switch (key.Key)
    {
        case ConsoleKey.A:
            await runner.AggregateProjectFiles();
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
        })
        .Build();

    return host;
}

