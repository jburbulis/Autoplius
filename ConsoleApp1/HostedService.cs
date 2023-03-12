using AutopliusReader.Services;
using Microsoft.Extensions.Hosting;
using Timer = ConsoleApp1.Utils.Timer;

namespace ConsoleApp1;

public class HostedService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var carReaderService = new CarReaderService();

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine("Skaitymas pradedamas");
            carReaderService.CheckCars();

            var delay = Timer.Delay(delayMinutes: true);
            Console.WriteLine($"Delay minutes: {delay / 1000 / 60}");
            await Task.Delay(delay, cancellationToken);
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}