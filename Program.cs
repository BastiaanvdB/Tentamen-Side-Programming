using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Tentamen_Server_Side_Programming.Services;
using Tentamen_Server_Side_Programming.Services.Interface;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(services =>
    {
        services.AddTransient<IQueueService, QueueService>();
        services.AddTransient<IWeatherForecastService, WeatherForecastService>();
        services.AddTransient<IImageWriteService, ImageWriteService>();
        services.AddTransient<IImageService, ImageService>();
        services.AddTransient<IBlobStorageService, BlobStorageService>();
        services.AddTransient<ITableStorageService, TableStorageService>();
    })
    .Build();

host.Run();
