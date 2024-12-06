using Microsoft.Extensions.Logging;

public interface IImageProvider
{
    List<Meme> Images { get; }

    Meme? GetImageByHash(string hash);

    Meme GetImageById(string id);

    Task<Meme> CreateNewImage(string hash, string mimeType, Session session);

    Task UpdateImage(Meme meme);

    // TODO: does not belong here...
    string MimeTypeToExtension(string mime);

    List<Meme> GetAvailableMemes();
}

class ImageProvider : IImageProvider
{
    public List<Meme> Images { get; }

    private readonly ILogger<ImageProvider> _logger;
    private readonly IContext _context;

    public ImageProvider(ILogger<ImageProvider> logger, IContext context)
    {
        _logger = logger;
        _context = context;

        Images = [.. _context.Memes];
    }

    public async Task<Meme> CreateNewImage(string hash, string mimeType, Session session)
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
        await _context.AddImage(meme);

        return meme;
    }

    public List<Meme> GetAvailableMemes()
    {
        return Images.Where(x => x.IsEnabled).ToList();
    }

    public Meme? GetImageByHash(string hash)
    {
        return Images.FirstOrDefault(x => x.Hash == hash);
    }

    public Meme GetImageById(string id)
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

    public async Task UpdateImage(Meme meme)
    {
        var index = Images.FindIndex(x => x.Id == meme.Id);
        Images[index] = meme;

        await _context.UpdateMeme(meme);
    }
}