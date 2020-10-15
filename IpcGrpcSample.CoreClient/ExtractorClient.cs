using Google.Protobuf.WellKnownTypes;
using Grpc.Net.Client;
using IpcGrpcSample.Protocol.Extractor;
using System.Net.Http;
using System.Threading.Tasks;

namespace IpcGrpcSample.CoreClient
{
    class ExtractorClient
    {
        private readonly ExtractorRpcService.ExtractorRpcServiceClient client;

        public ExtractorClient()
        {
            //AppContext.SetSwitch("System.Net.Http.SocketsHttpHandler.Http2UnencryptedSupport", true); //без этого кода невозможна коммуникация через http/2 без TLS        
            var httpClientHandler = new HttpClientHandler
            {
                ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator // заглушка для самоподписанного сертификата
            };
            var httpClient = new HttpClient(httpClientHandler);
            var channel = GrpcChannel.ForAddress("https://localhost:7001", new GrpcChannelOptions { HttpClient = httpClient });
            client = new ExtractorRpcService.ExtractorRpcServiceClient(channel);
        }

        public async Task<bool> StartAsync()
        {
            var response = await client.StartAsync(new Empty());
            return response.Success;
        }
    }
}
