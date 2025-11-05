using Microsoft.Extensions.Logging;
using MemeOfTheYear.Database;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public class ImageProvider : IImageProvider
    {
        public List<Image> Images { get; }

        private readonly ILogger<ImageProvider> _logger;
        private readonly ILocalStorageProvider _localStorageProvider;
        private readonly IContext _context;

        public ImageProvider(ILogger<ImageProvider> logger, ILocalStorageProvider localStorageProvider, IContext context)
        {
            _logger = logger;
            _localStorageProvider = localStorageProvider;
            _context = context;

            Images = [.. _context.Images];
        }

        public async Task<Image> CreateNewImage(string hash, string mimeType, Session session)
        {
            var image = new Image
            {
                Id = Guid.NewGuid().ToString(),
                MimeType = mimeType,
                Hash = hash,
                UploadCount = 1,
                Uploader = session,
                IsEnabled = true
            };

            Images.Add(image);
            await _context.AddImage(image);

            return image;
        }

        public List<Image> GetAvailableMemes()
        {
            return Images.Where(x => x.IsEnabled).ToList();
        }

        public Image? GetImageByHash(string hash)
        {
            return Images.FirstOrDefault(x => x.Hash == hash);
        }

        public Image GetImageById(string id)
        {
            return Images.First(x => x.Id == id);
        }

        public string MimeTypeToExtension(string mime)
        {
            return mime switch
            {
                "image/jpeg" => ".jpg",
                "image/png" => ".png",
                "image/gif" => ".gif",
                "video/mp4" => ".mp4",
                "video/opus" => ".opus",
                _ => string.Empty
            };
        }

        public async Task UpdateImage(Image image)
        {
            var index = Images.FindIndex(x => x.Id == image.Id);
            if (index >= 0)
            {
                Images[index] = image;

                await _context.UpdateMeme(image);
            }
        }

        public async Task SetupByExistingData()
        {
            var existingImages = await _localStorageProvider.GetExistingImages();
            var missingDatabaseImages = existingImages.Except(Images).ToList();
            var session = new Session
            {
                Id = Guid.Empty.ToString(),
                IsAuthenticated = true
            };

            if (!_context.Sessions.Contains(session))
            {
                await _context.AddSession(session);
            }

            foreach (var image in missingDatabaseImages)
            {
                image.Uploader = session;

                _logger.LogInformation("Has local image {} but not on database", image);
                await _context.AddImage(image);
            }
        }
    }
}