using Grpc.Core;
using Grpc.Net.Client;
using IpcGrpcSample.Protocol.Thermocycler;
using OneOf;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace IpcGrpcSample.CoreClient
{
    class ThermocyclerClient
    {
        private readonly ThermocyclerRpcService.ThermocyclerRpcServiceClient client;

        public ThermocyclerClient()
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); //без этого кода невозможна коммуникация через http/2 без TLS        
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator // заглушка для самоподписанного сертификата
            };
            var httpClient = new HttpClient(httpClientHandler);
            var channel = GrpcChannel.ForAddress("https://localhost:7001", new GrpcChannelOptions { HttpClient = httpClient });
            client = new ThermocyclerRpcService.ThermocyclerRpcServiceClient(channel);
        }

        public async IAsyncEnumerable<OneOf<string, int>> StartAsync(string experimentName, int cycleCount)
        {
            var request = new StartRequest { ExperimentName = experimentName, CycleCount = cycleCount };
            using var call = client.Start(request, new CallOptions().WithDeadline(DateTime.MaxValue)); // настройка времени ожидания
            while (await call.ResponseStream.MoveNext())
            {
                var message = call.ResponseStream.Current;
                switch (message.ContentCase)
                {
                    case StartResponse.ContentOneofCase.Plate:
                        yield return message.Plate.ExperimentalData;
                        break;
                    case StartResponse.ContentOneofCase.Status:
                        yield return message.Status.PlateTemperature;
                        break;
                    default:
                        break;
                };
            }
        }
    }
}
