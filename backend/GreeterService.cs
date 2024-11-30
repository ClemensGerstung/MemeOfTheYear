using Grpc.Core;
using GrpcGreeter;
using Microsoft.Extensions.Logging;

class GreeterService(ILogger<GreeterService> logger) : GrpcGreeter.Greeter.GreeterBase
{
    static int count = 0;

    public override async Task SayHello(HelloRequest request, IServerStreamWriter<HelloReply> responseStream, ServerCallContext context)
    {
        logger.LogInformation("Received request");

        while (!context.CancellationToken.IsCancellationRequested)
        {
            count++;

            logger.LogInformation($"Hello World {count}");
            await responseStream.WriteAsync(new HelloReply { Message = $"Hello World {count}" });
            await Task.Delay(TimeSpan.FromSeconds(1), context.CancellationToken);
        }
    }
}