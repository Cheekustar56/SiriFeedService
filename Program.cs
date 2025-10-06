using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SiriFeedService;

class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices((hostContext, services) =>
            {
                // Load config
                var config = ConfigLoader.Load("ServiceConfig.xml"); // or .xml if you implemented that

                if (config == null)
                    throw new InvalidOperationException("Service configuration not found or invalid.");

                // Register config as singleton
                services.AddSingleton(config);

                // Register FeedProcessor with DI
                services.AddSingleton<FeedProcessor>();

                // Register the Worker service
                services.AddHostedService<Worker>();
            });
}
