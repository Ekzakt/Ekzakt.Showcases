using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using EmailTemplateProvider.Console;
using System.Net.Http.Headers;

var services = new ServiceCollection();
var c = new ConsoleHelpers();

var host = BuildHost(services);


TaskRunner runner = new TaskRunner();


List<string> taskList = new()
{
    "Do something."
};



while (true)
{
    var key = runner.WriteTaskList(taskList);

    switch (key.Key)
    {
        case ConsoleKey.A:
            await runner.DoSomething();
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
            //services.AddSomething();
        })
        .Build();

    return host;
}

