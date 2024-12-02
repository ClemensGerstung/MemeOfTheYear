using Grpc.Core;
using Microsoft.Extensions.Logging;
using MemeOfTheYear;
using System.Security.Cryptography;
using System.Text;

class ImageService(
    ILogger<ImageService> logger,
    ILocalStorageProvider localStorageProvider,
    IImageProvider imageProvider,
    ISessionProvider sessionProvider
) : MemeOfTheYear.ImageService.ImageServiceBase
{
    public override async Task<GetImageResponse> GetImage(GetImageRequest request, ServerCallContext context)
    {
        if (!sessionProvider.IsAllowed(request.SessionId))
        {
            throw new InvalidOperationException("Session not authenticated!");
        }

        var image = imageProvider.GetImageById(request.ImageId);
        var filename = $"{image.Id}{imageProvider.MimeTypeToExtension(image.MimeType)}";
        var content = await localStorageProvider.GetFileContent(filename);

        return new GetImageResponse
        {
            ImageContent = content
        };
    }

    public override async Task<UploadImageResponse> UploadImage(MemeOfTheYear.UploadImageRequest request, ServerCallContext context)
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

            Meme? existing = imageProvider.GetImageByHash(hash);
            if (existing == null)
            {
                existing = await imageProvider.CreateNewImage(hash, uploadEntry.MimeType, uploader);
                var filename = $"{existing.Id}{imageProvider.MimeTypeToExtension(existing.MimeType)}";

                localStorageProvider.MoveFile(uploadEntry.Filename, filename);
            }
            else
            {
                existing.UploadCount += 1;
                await imageProvider.UpdateImage(existing);
            }

            response.Images.Add(new Image
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