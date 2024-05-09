using Ekzakt.Queuing.Console;
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
    "Do something."
};


while (true)
{
    var key = ch.WriteMenu(taskList, "What do you want to do?");

    switch (key.Key)
    {
        case ConsoleKey.A:
            await runner.DoSomethingAsync();
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
        })
        .Build();

    return host;
}

