using Grpc.Core;
using Grpc.Net.Client;

public interface IGrpcClientProvider
{
    T GetClient<T>() where T : ClientBase<T>;
}

class GrpcClientProvider : IGrpcClientProvider
{
    private readonly ILogger<GrpcClientProvider> _logger;

    private readonly GrpcChannel _channel;

    public GrpcClientProvider(ILogger<GrpcClientProvider> logger)
    {
        _logger = logger;

        var grpcHost = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_BACKEND") ?? "http://localhost:5000";
        _logger.LogInformation("Connect to gRPC Host {}", grpcHost);
        _channel = GrpcChannel.ForAddress(grpcHost);
    }

    public T GetClient<T>() 
        where T : ClientBase<T>
    {
        return (T) Activator.CreateInstance(typeof(T), _channel)!;
    }
}