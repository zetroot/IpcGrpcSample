using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace IpcGrpcSample.NetServer
{
    class Program
    {
        static Task Main(string[] args) => CreateHostBuilder(args).Build().RunAsync();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
            .UseWindowsService()
            .ConfigureServices(services =>
            {
                services.AddLogging(loggingBuilder =>
                {
                    loggingBuilder.ClearProviders();
                    loggingBuilder.SetMinimumLevel(LogLevel.Trace);                    
                    loggingBuilder.AddConsole();
                });
                services.AddTransient<ExtractorServiceImpl>(); // регистрация зависимостей - сервисов с логикой управления приборами
                services.AddTransient<ThermocyclerServiceImpl>();
                services.AddHostedService<GrpcServer>(); // регистрация GRPC сервера как HostedService
            });
    }
}
