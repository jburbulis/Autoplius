using Autoplius.Repository;
using AutopliusReader.Services;
using Microsoft.Extensions.Hosting;
using Timer = ConsoleApp1.Utils.Timer;

namespace ConsoleApp1;

public class HostedService : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var carReaderService = new CarReaderService();
        var heartBeat = 1;

        while (!cancellationToken.IsCancellationRequested)
        {
            Console.WriteLine($"----------------------{"Skaitymas pradedamas"}---------------------");
            Console.WriteLine(DateTime.Now.ToString("yyyy-MM-dd HH:mm"));
            Console.WriteLine($"---------------------------------------------------------------");
            carReaderService.CheckCars();

            var delay = Timer.Delay(delayMinutes: true);
            Console.WriteLine($"Delay minutes: {delay / 1000 / 60}");
            await Task.Delay(delay, cancellationToken);

            if (heartBeat==100)
            {
                carReaderService.SendMail("heartBeat", "heartBeat","AutopliusAppOnline");
            }

            heartBeat++;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}