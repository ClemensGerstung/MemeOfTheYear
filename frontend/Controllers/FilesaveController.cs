using System.Net;
using Microsoft.AspNetCore.Mvc;
using Grpc.Net.Client;
using MemeOfTheYear;
using Microsoft.Extensions.Primitives;

[ApiController]
[Route("[controller]")]
public class FilesaveController(
    ILogger<FilesaveController> logger,
    IGrpcClientProvider grpcClient
) : ControllerBase
{
    private string _uploadPath = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_UPLOAD") ?? "/tmp/upload";

    [HttpPost]
    public async Task<ActionResult<IList<UploadResult>>> PostFile([FromForm] IEnumerable<IFormFile> files)
    {
        logger.LogInformation("Post File ...");

        var maxAllowedFiles = 10;
        long maxFileSize = 1 * 1024 * 1024;
        var filesProcessed = 0;
        var resourcePath = new Uri($"{Request.Scheme}://{Request.Host}/");

        this.Request.Headers.TryGetValue("session", out StringValues sessionId);

        var imageService = grpcClient.GetClient<ImageService.ImageServiceClient>();
        var uploadRequest = new UploadImageRequest
        {
            SessionId = sessionId.Single()
        };

        foreach (var file in files)
        {
            string trustedFileNameForFileStorage;
            var untrustedFileName = file.FileName;
            var trustedFileNameForDisplay = WebUtility.HtmlEncode(untrustedFileName);

            if (filesProcessed < maxAllowedFiles)
            {
                if (file.Length == 0)
                {
                    logger.LogInformation("{} length is 0 (Err: 1)",
                        trustedFileNameForDisplay);
                }
                else if (file.Length > maxFileSize)
                {
                    logger.LogError("{} of {} bytes is larger than the limit of {} bytes (Err: 2)",
                        trustedFileNameForDisplay, file.Length, maxFileSize);
                }
                else
                {
                    try
                    {
                        trustedFileNameForFileStorage = Path.GetRandomFileName();

                        var path = Path.Combine(_uploadPath, trustedFileNameForFileStorage);
                        var fileInfo = new FileInfo(path);

                        await using FileStream fs = new(path, FileMode.Create);
                        await file.CopyToAsync(fs);

                        logger.LogInformation("{} saved at {}",
                            trustedFileNameForDisplay, path);

                        uploadRequest.Entries.Add(new UploadEntry
                        {
                            Filename = trustedFileNameForFileStorage,
                            MimeType = file.ContentType
                        });
                    }
                    catch (IOException ex)
                    {
                        logger.LogError("{} error on upload (Err: 3): {}",
                            trustedFileNameForDisplay, ex.Message);
                    }
                }

                filesProcessed++;
            }
            else
            {
                logger.LogInformation("{FileName} not uploaded because the " +
                    "request exceeded the allowed {Count} of files (Err: 4)",
                    trustedFileNameForDisplay, maxAllowedFiles);
            }
        }

        var response = await imageService.UploadImageAsync(uploadRequest);
        logger.LogInformation("Uploaded {}", string.Join(", ", response.Images.Select(x => $"{x.Id} {x.MimeType}")));

        List<UploadResult> uploadResults = response.Images
                                                    .Select(x => new UploadResult
                                                    {
                                                        ImageId = x.Id
                                                    })
                                                    .ToList();

        return new CreatedResult(resourcePath, uploadResults);
    }
}