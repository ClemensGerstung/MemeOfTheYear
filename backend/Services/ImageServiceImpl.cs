using Grpc.Core;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using MemeOfTheYear.Remote;
using MemeOfTheYear.Providers;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Services
{
    class ImageServiceImpl(
        ILogger<ImageServiceImpl> logger,
        ILocalStorageProvider localStorageProvider,
        IImageProvider imageProvider,
        ISessionProvider sessionProvider,
        IVoteProvider voteProvider
    ) : ImageService.ImageServiceBase
    {
        public override Task<GetImagesResponse> GetAllImages(GetImagesRequest request, ServerCallContext context)
        {
            var response = new GetImagesResponse();

            foreach (var image in imageProvider.Images)
            {
                var likes = voteProvider.GetVoteCount(image.Id, VoteType.Like);
                var dislikes = voteProvider.GetVoteCount(image.Id, VoteType.Dislike);

                response.Images.Add(new Remote.Image
                {
                    Id = image.Id,
                    Hash = image.Hash,
                    Enabled = image.IsEnabled,
                    MimeType = image.MimeType,
                    Uploaded = image.UploadCount,
                    Likes = likes,
                    Dislikes = dislikes
                });
            }

            return Task.FromResult(response);
        }

        public override async Task<GetImageResponse> GetImage(GetImageRequest request, ServerCallContext context)
        {
            if (!sessionProvider.IsAllowed(request.SessionId))
            {
                throw new InvalidOperationException("Session not authenticated!");
            }

            var image = imageProvider.GetImageById(request.ImageId);
            var filename = $"{image.Id}{imageProvider.MimeTypeToExtension(image.MimeType)}";
            logger.LogDebug("try get {}", filename);
            var content = await localStorageProvider.GetFileContent(filename);

            return new GetImageResponse
            {
                ImageContent = $"data:{image.MimeType};base64, {content}"
            };
        }

        public override async Task<UploadImageResponse> UploadImage(UploadImageRequest request, ServerCallContext context)
        {
            if (!sessionProvider.IsAllowed(request.SessionId))
            {
                throw new InvalidOperationException("Session not authenticated!");
            }

            UploadImageResponse response = new();

            using SHA256 hasher = SHA256.Create();
            Session uploader = sessionProvider.GetSession(request.SessionId)!;

            foreach (var uploadEntry in request.Entries)
            {
                using var fileStream = localStorageProvider.OpenRead(uploadEntry.Filename);
                var rawHash = await hasher.ComputeHashAsync(fileStream);
                var hash = Convert.ToBase64String(rawHash);
                fileStream.Close();

                logger.LogDebug("received new file {} with hash {}", uploadEntry.Filename, hash);

                var existing = imageProvider.GetImageByHash(hash);
                logger.LogInformation("Got {} by hash", existing);
                if (existing == null)
                {
                    existing = await imageProvider.CreateNewImage(hash, uploadEntry.MimeType, uploader);
                    logger.LogInformation("Created new {}", existing);

                    var filename = $"{existing.Id}{imageProvider.MimeTypeToExtension(existing.MimeType)}";
                    localStorageProvider.MoveFile(uploadEntry.Filename, filename);
                }
                else
                {
                    logger.LogDebug("Update upload count to  {}", existing.UploadCount + 1);
                    existing.UploadCount += 1;
                    await imageProvider.UpdateImage(existing);

                    // TODO: delete duplicate file in upload folder
                }

                response.Images.Add(new Remote.Image
                {
                    Id = existing.Id,
                    Hash = existing.Hash,
                    MimeType = existing.MimeType,
                    Uploaded = existing.UploadCount,
                    Likes = 0,
                    Dislikes = 0
                });
            }

            return response;
        }
    }
}