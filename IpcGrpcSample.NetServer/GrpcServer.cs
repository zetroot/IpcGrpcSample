using Grpc.Core;
using IpcGrpcSample.Protocol.Extractor;
using IpcGrpcSample.Protocol.Thermocycler;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace IpcGrpcSample.NetServer
{
    /// <summary>
    /// GRPC сервер будет работать как размещаемая служба
    /// </summary>
    internal class GrpcServer : IHostedService
    {
        private readonly ILogger<GrpcServer> logger;
        private readonly Server server;
        private readonly ExtractorServiceImpl extractorService;
        private readonly ThermocyclerServiceImpl thermocyclerService;

        public GrpcServer(ExtractorServiceImpl extractorService, ThermocyclerServiceImpl thermocyclerService, ILogger<GrpcServer> logger)
        {
            this.logger = logger;
            this.extractorService = extractorService;
            this.thermocyclerService = thermocyclerService;
            var credentials = BuildSSLCredentials(); // строим креды из сертификата и приватного ключа. 
            server = new Server //создаем объект сервера
            {
                Ports = { new ServerPort("localhost", 7001, credentials) }, // биндим сервер к адресу и порту
                Services = // прописываем службы которые будут доступны на сервере
                {
                    ExtractorRpcService.BindService(this.extractorService),
                    ThermocyclerRpcService.BindService(this.thermocyclerService)
                }
            };            
        }

        /// <summary>
        /// Вспомогательный метод генерации серверных кредов из сертификата
        /// </summary>
        private ServerCredentials BuildSSLCredentials()
        {
            var cert = File.ReadAllText("cert\\server.crt");
            var key = File.ReadAllText("cert\\server.key");

            var keyCertPair = new KeyCertificatePair(cert, key);
            return new SslServerCredentials(new[] { keyCertPair });
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Запуск GRPC сервера");
            server.Start();
            logger.LogInformation("GRPC сервер запущен");
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Останов GRPC сервера");
            await server.ShutdownAsync();
            logger.LogInformation("GRPC сервер остановлен");
        }
    }
}