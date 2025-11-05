using System.Security.Cryptography;
using System.Text.Json;
using MemeOfTheYear.Types;
using Microsoft.Extensions.Logging;

namespace MemeOfTheYear.Providers
{
    public class LocalStorageProvider : ILocalStorageProvider
    {
        private readonly ILogger<LocalStorageProvider> _logger;
        private readonly string _uploadPath;
        private readonly string _configPath;

        public LocalStorageProvider(ILogger<LocalStorageProvider> logger)
        {
            _logger = logger;
            ImagePath = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_IMAGES") ?? "/tmp/images";
            _uploadPath = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_UPLOAD") ?? "/tmp/upload";
            _configPath = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_CONFIG") ?? "/tmp/config";

            if (!Directory.Exists(ImagePath))
            {
                Directory.CreateDirectory(ImagePath);
            }

            if (!Directory.Exists(_uploadPath))
            {
                Directory.CreateDirectory(_uploadPath);
            }
        }

        public string ImagePath { get; }

        public async Task<T> GetConfigAsync<T>(string configName)
        {
            var file = new FileInfo(Path.Combine(_configPath, configName));
            if (!file.Exists)
            {
                throw new FileNotFoundException($"Config {configName} does not exist");
            }

            using var stream = file.OpenRead();
            var result = await JsonSerializer.DeserializeAsync<T>(stream);
            stream.Close();

            if (result is null)
            {
                throw new Exception("Could not deserialize config");
            }

            return result;
        }

        public T GetConfig<T>(string configName)
        {
            return GetConfigAsync<T>(configName)
                    .GetAwaiter().GetResult();
        }

        public async Task<string> GetFileContent(string filename)
        {
            var file = new FileInfo(Path.Combine(ImagePath, filename));
            using var stream = file.OpenRead();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            var content = Convert.ToBase64String(memoryStream.ToArray());

            return content;
        }

        public void MoveFile(string orig, string save)
        {
            string oldFileName = Path.Combine(_uploadPath, orig);
            string newFileName = Path.Combine(ImagePath, save);

            _logger.LogDebug($"MoveFile: from {oldFileName} to {newFileName}");
            File.Move(oldFileName, newFileName);
        }

        public Stream OpenRead(string orig)
        {
            return File.OpenRead(Path.Combine(_uploadPath, orig));
        }

        public async Task<List<Image>> GetExistingImages()
        {
            var directory = new DirectoryInfo(ImagePath);
            var images = new List<Image>();
            using SHA256 hasher = SHA256.Create();

            foreach (var file in directory.GetFiles())
            {
                var extension = Path.GetExtension(file.Name);
                var mimeType = extension switch
                {
                    ".jpg" => "image/jpeg",
                    ".png" => "image/png",
                    ".gif" => "image/gif",
                    ".mp4" => "video/mp4",
                    ".opus" => "video/opus",
                    _ => string.Empty
                };

                using var stream = file.OpenRead();
                var rawHash = await hasher.ComputeHashAsync(stream);
                var hash = Convert.ToBase64String(rawHash);
                stream.Close();

                var image = new Image
                {
                    Id = Path.GetFileNameWithoutExtension(file.Name),
                    MimeType = mimeType,
                    IsEnabled = true,
                    UploadCount = 1,
                    Hash = hash
                };

                images.Add(image);
            }

            return images;
        }

        public async IAsyncEnumerable<byte[]> GetFileContentStream(string filename)
        {
            var filePath = Path.Combine(ImagePath, filename);
            var bufferSize = 64 * 1024; // 64KB
            var buffer = new byte[bufferSize];

            using var filestream = File.OpenRead(filePath);

            while (true)
            {
                var bytesRead = filestream.Read(buffer, 0, bufferSize);
                if (bytesRead == 0)
                {
                    break;
                }

                if (bytesRead < bufferSize)
                {
                    var lastBuffer = new byte[bytesRead];
                    Array.Copy(buffer, lastBuffer, bytesRead);
                    yield return lastBuffer;
                }
                else
                {
                    yield return buffer;
                    buffer = new byte[bufferSize]; // Allocate a new buffer for the next read
                }
            }

            filestream.Close();
        }
    }
}