using Microsoft.Extensions.Logging;
using MemeOfTheYear.Database;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public class ImageProvider : IImageProvider
    {
        public List<Image> Images { get; }

        private readonly ILogger<ImageProvider> _logger;
        private readonly IContext _context;

        public ImageProvider(ILogger<ImageProvider> logger, IContext context)
        {
            _logger = logger;
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
    }
}