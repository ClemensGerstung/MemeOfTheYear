using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using MemeOfTheYear.Remote;
using MemeOfTheYear.Providers;
using MemeOfTheYear.Types;
using Google.Protobuf;

namespace MemeOfTheYear.Services
{
    public class ImageStreamServiceImpl(
        ILogger<ImageServiceImpl> logger,
        ILocalStorageProvider localStorageProvider,
        IImageProvider imageProvider,
        ISessionProvider sessionProvider
    ) : ImageStreamService.ImageStreamServiceBase
    {
        public override async Task GetImageData(GetImageDataRequest request, IServerStreamWriter<GetImageDataResponse> responseStream, ServerCallContext context)
        {
            if (!sessionProvider.IsAllowed(request.SessionId))
            {
                throw new InvalidOperationException("Session not authenticated!");
            }

            var image = imageProvider.GetImageById(request.ImageId);
            var filename = $"{image.Id}{imageProvider.MimeTypeToExtension(image.MimeType)}";
            logger.LogDebug("try get {}", filename);

            await foreach (var chunk in localStorageProvider.GetFileContentStream(filename))
            {
                if(context.CancellationToken.IsCancellationRequested)
                {
                    logger.LogInformation("Cancellation requested, stopping stream for image {}", request.ImageId);
                    break;
                }

                await responseStream.WriteAsync(new GetImageDataResponse
                {
                    ChunkData = ByteString.CopyFrom(chunk)
                });
            }
        }
    }
}