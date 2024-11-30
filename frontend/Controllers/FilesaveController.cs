using System.Net;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Security.Cryptography;

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

        using SHA256 hasher = SHA256.Create();

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
                        using MemoryStream raw = new MemoryStream();
                        await file.CopyToAsync(raw);

                        raw.Seek(0, SeekOrigin.Begin);
                        var hash = await hasher.ComputeHashAsync(raw);
                        logger.LogInformation($"computed hash ({hash.Length}b) {string.Join("", hash.Select(x => x.ToString("X2")))}");

                        var ext = file.ContentType switch
                        {
                            "image/jpeg" => ".jpg",
                            "image/png" => ".png",
                            "image/gif" => ".gif",
                            _ => throw new Exception("AAAAAA")
                        };
                        trustedFileNameForFileStorage = string.Join("", hash.Select(x => x.ToString("X2"))) + ext;
                        var path = Path.Combine(env.ContentRootPath,
                            env.EnvironmentName, "unsafe_uploads",
                            trustedFileNameForFileStorage);

                        var fileInfo = new FileInfo(path);
                        if (fileInfo.Exists)
                        {
                            logger.LogWarning($"{fileInfo} already exists, skipping!");
                        }
                        else
                        {
                            await using FileStream fs = new(path, FileMode.Create);

                            raw.Seek(0, SeekOrigin.Begin);
                            await raw.CopyToAsync(fs);

                            logger.LogInformation("{FileName} saved at {Path}",
                                trustedFileNameForDisplay, path);
                        }

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

        return new CreatedResult(resourcePath, uploadResults);
    }
}