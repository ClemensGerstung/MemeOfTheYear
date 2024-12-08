using Microsoft.Extensions.Logging;
using MemeOfTheYear.Database;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    class ImageProvider : IImageProvider
    {
        public List<Image> Images { get; }

        private readonly ILogger<ImageProvider> _logger;
        private readonly IContext _context;
        private readonly IStageProvider _stageProvider;
        private readonly IResultProvider _resultProvider;

        public ImageProvider(ILogger<ImageProvider> logger, IContext context, IStageProvider stageProvider, IResultProvider resultProvider)
        {
            _logger = logger;
            _context = context;
            _stageProvider = stageProvider;
            _resultProvider = resultProvider;

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
            if (_stageProvider.CurrentStage.Extras.TryGetValue("MaxImages", out object? obj))
            {
                int maxImages = obj as int? ?? Images.Count;
                _logger.LogInformation("Current Stage has a limit of {} allowed images", maxImages);

                var images = _resultProvider.GetMostVotedImages(maxImages);
                var imagesToDisable = Images.Where(x => x.IsEnabled).Except(images);

                foreach (var imageToDisable in imagesToDisable)
                {
                    _logger.LogDebug("Disable image {}", imageToDisable);
                    imageToDisable.IsEnabled = false;
                    UpdateImage(imageToDisable).GetAwaiter().GetResult();
                }
            }

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
                _ => string.Empty
            };
        }

        public async Task UpdateImage(Image image)
        {
            var index = Images.FindIndex(x => x.Id == image.Id);
            Images[index] = image;

            await _context.UpdateMeme(image);
        }
    }
}