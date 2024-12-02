using System.Net;
using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
using MemeOfTheYear;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Google.Protobuf.WellKnownTypes;
using Microsoft.Extensions.Primitives;

[ApiController]
[Route("[controller]")]
public class FilesaveController(
    IHostEnvironment env,
    ILogger<FilesaveController> logger
) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<IList<UploadResult>>> PostFile([FromForm] IEnumerable<IFormFile> files)
    {
        logger.LogInformation("Post File ...");

        var maxAllowedFiles = 10;
        long maxFileSize = 15 * 1024 * 1024;
        var filesProcessed = 0;
        var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");
        List<UploadResult> uploadResults = [];

        this.Request.Headers.TryGetValue("session", out StringValues sessionId);

        var channel = GrpcChannel.ForAddress("http://localhost:5000");
        var imageService = new ImageService.ImageServiceClient(channel);
        var uploadRequest = new UploadImageRequest
        {
            SessionId = sessionId.Single()
        };

        foreach (var file in files)
        {
            var uploadResult = new UploadResult();
            string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;
            uploadResult.FileName = untrustedFileName;
            var trustedFileNameForDisplay =
                WebUtility.HtmlEncode(untrustedFileName);

            if (filesProcessed < maxAllowedFiles)
            {
                if (file.Length == 0)
                {
                    logger.LogInformation("{FileName} length is 0 (Err: 1)",
                        trustedFileNameForDisplay);
                    uploadResult.ErrorCode = 1;
                }
                else if (file.Length > maxFileSize)
                {
                    logger.LogInformation("{FileName} of {Length} bytes is " +
                        "larger than the limit of {Limit} bytes (Err: 2)",
                        trustedFileNameForDisplay, file.Length, maxFileSize);
                    uploadResult.ErrorCode = 2;
                }
                else
                {
                    try
                    {
                        trustedFileNameForFileStorage = Path.GetRandomFileName();
                        var path = Path.Combine(env.ContentRootPath,
                            env.EnvironmentName, "unsafe_uploads",
                            trustedFileNameForFileStorage);

                        var fileInfo = new FileInfo(path);

                        await using FileStream fs = new(path, FileMode.Create);
                        await file.CopyToAsync(fs);

                        logger.LogInformation("{FileName} saved at {Path}",
                            trustedFileNameForDisplay, path);

                        uploadRequest.Entries.Add(new UploadEntry
                        {
                            Filename = trustedFileNameForFileStorage,
                            MimeType = file.ContentType
                        });

                        uploadResult.Uploaded = true;
                        uploadResult.StoredFileName = trustedFileNameForFileStorage;
                    }
                    catch (IOException ex)
                    {
                        logger.LogError("{FileName} error on upload (Err: 3): {Message}",
                            trustedFileNameForDisplay, ex.Message);
                        uploadResult.ErrorCode = 3;
                    }
                }

                filesProcessed++;
            }
            else
            {
                logger.LogInformation("{FileName} not uploaded because the " +
                    "request exceeded the allowed {Count} of files (Err: 4)",
                    trustedFileNameForDisplay, maxAllowedFiles);
                uploadResult.ErrorCode = 4;
            }

            uploadResults.Add(uploadResult);
        }

        var response = await imageService.UploadImageAsync(uploadRequest);
        logger.LogInformation("Uploaded {}", string.Join(", ", response.Images.Select(x => $"{x.Id} {x.MimeType}")));

        return new CreatedResult(resourcePath, uploadResults);
    }
}