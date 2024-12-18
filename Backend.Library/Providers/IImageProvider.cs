using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public interface IImageProvider
    {
        List<Image> Images { get; }

        Image? GetImageByHash(string hash);

        Image GetImageById(string id);

        Task<Image> CreateNewImage(string hash, string mimeType, Session session);

        Task UpdateImage(Image image);

        // TODO: does not belong here...
        string MimeTypeToExtension(string mime);

        List<Image> GetAvailableMemes();

        Task SetupByExistingData();
    }
}