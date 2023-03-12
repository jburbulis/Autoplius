using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

Host.CreateDefaultBuilder()
    .ConfigureServices((_, services) => services.AddHostedService<HostedService>())
    .Build()
    .Run();