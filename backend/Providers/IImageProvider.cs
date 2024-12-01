using Microsoft.Extensions.Logging;

interface IImageProvider
{
    List<Meme> Images { get; }

    Meme? GetImageByHash(string hash);

    Meme GetImageById(string id);

    Meme CreateNewImage(string hash, string mimeType, Session session);

    void UpdateImage(Meme meme);

    // TODO: does not belong here...
    string MimeTypeToExtension(string mime);

    List<Meme> GetAvailableMemes();
}

class ImageProvider(
    ILogger<ImageProvider> logger
) : IImageProvider
{
    public List<Meme> Images {get;} = new();

    public Meme CreateNewImage(string hash, string mimeType, Session session)
    {
        var meme = new Meme
        {
            Id = Guid.NewGuid().ToString(),
            MimeType = mimeType,
            Hash = hash,
            UploadCount = 1,
            Uploader = session,
            IsEnabled = true
        };

        Images.Add(meme);

        return meme;
    }

    public List<Meme> GetAvailableMemes()
    {
        return Images.Where(x => x.IsEnabled).ToList();
    }

    public Meme? GetImageByHash(string hash)
    {
        throw new NotImplementedException();
    }

    public Meme GetImageById(string id)
    {
        throw new NotImplementedException();
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

    public void UpdateImage(Meme meme)
    {
        throw new NotImplementedException();
    }
}