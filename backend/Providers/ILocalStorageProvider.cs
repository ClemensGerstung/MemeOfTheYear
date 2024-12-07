using System.Text.Json;
using Microsoft.Extensions.Logging;

public interface ILocalStorageProvider
{
    string ImagePath { get; }

    void MoveFile(string orig, string save);

    Task<string> GetFileContent(string filename);

    Stream OpenRead(string orig);

    Task<T> GetConfigAsync<T>(string configName);

    T GetConfig<T>(string configName);
}

class LocalStorageProvider : ILocalStorageProvider
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
}