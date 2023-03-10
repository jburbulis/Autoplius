using AutopliusReader.Services;
using Microsoft.Extensions.Hosting;

namespace ConsoleApp1;

public class Class3 : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Start");
            Class1.CheckCars();
           
            await Task.Delay(Timmer.Delay(), cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}