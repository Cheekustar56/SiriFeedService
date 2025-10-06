using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SiriFeedService;

public class Worker : BackgroundService
{
    private readonly FeedProcessor _processor;
    private readonly ILogger<Worker> _logger;

    public Worker(FeedProcessor processor, ILogger<Worker> logger)
    {
        _processor = processor;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            _logger.LogInformation("Worker running at: {time}, cycle: 0", DateTimeOffset.Now);
            Console.WriteLine("Starting feed processing...");

            await _processor.ProcessFeedsAsync(stoppingToken); // ✅ Make sure this line exists!

            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // simulate polling
        }
    }
}

