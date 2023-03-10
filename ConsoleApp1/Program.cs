using System.Diagnostics;
using ConsoleApp1;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using static System.Net.Mime.MediaTypeNames;

Host.CreateDefaultBuilder()
    .ConfigureServices((context, services)=> 
            services.AddHostedService<Class3>()
        )
    .Build()
    .Run();