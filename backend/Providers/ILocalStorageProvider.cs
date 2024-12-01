using Microsoft.Extensions.Logging;

interface ILocalStorageProvider
{
    string ImagePath { get; }

    void MoveFile(string orig, string save);

    Task<string> GetFileContent(string filename);

    Stream OpenRead(string orig);
}

class LocalStorageProvider : ILocalStorageProvider
{
    private readonly ILogger<LocalStorageProvider> _logger;
    private readonly string _uploadPath;

    public LocalStorageProvider(ILogger<LocalStorageProvider> logger)
    {
        _logger = logger;
        ImagePath = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_IMAGES") ?? "/tmp/images";
        _uploadPath = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_UPLOAD") ?? "/tmp/upload";
    }

    public string ImagePath { get; }

    public async Task<string> GetFileContent(string filename)
    {
        var file = new FileInfo(Path.Combine(ImagePath, filename));
        using var stream = file.OpenRead();
        using var reader = new StreamReader(stream);

        var content = await reader.ReadToEndAsync();

        reader.Close();

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
}