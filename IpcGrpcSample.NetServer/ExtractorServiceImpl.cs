using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using IpcGrpcSample.Protocol.Extractor;
using System.Threading.Tasks;

namespace IpcGrpcSample.NetServer
{
    internal class ExtractorServiceImpl : ExtractorRpcService.ExtractorRpcServiceBase
    {
        private static bool success = true;
        public override Task<StartResponse> Start(Empty request, ServerCallContext context)
        {
            success = !success;
            return Task.FromResult(new StartResponse { Success = success });
        }
    }
}