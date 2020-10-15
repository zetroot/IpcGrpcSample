using Grpc.Core;
using IpcGrpcSample.Protocol.Thermocycler;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace IpcGrpcSample.NetServer
{
    internal class ThermocyclerServiceImpl : ThermocyclerRpcService.ThermocyclerRpcServiceBase
    {
        private readonly ILogger<ThermocyclerServiceImpl> logger;

        public ThermocyclerServiceImpl(ILogger<ThermocyclerServiceImpl> logger)
        {
            this.logger = logger;
        }

        public override async Task Start(StartRequest request, IServerStreamWriter<StartResponse> responseStream, ServerCallContext context)
        {
            logger.LogInformation("Эксперимент начинается");
            var rand = new Random(42);
            for(int i = 1; i <= request.CycleCount; ++i)
            {
                logger.LogInformation($"Отправка цикла {i}");
                var plate = new PlateRead { ExperimentalData = $"Эксперимент {request.ExperimentName}, шаг {i} из {request.CycleCount}: {rand.Next(100, 500000)}" };
                await responseStream.WriteAsync(new StartResponse { CycleNumber = i, Plate = plate });
                var status = new StatusMessage { PlateTemperature = rand.Next(25, 95) };
                await responseStream.WriteAsync(new StartResponse { CycleNumber = i, Status = status });
                await Task.Delay(500);
            }
            logger.LogInformation("Эксперимент завершен");
        }
    }
}