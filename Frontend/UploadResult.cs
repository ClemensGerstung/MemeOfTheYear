using System.Text.Json;

public class UploadResult
{
    public string ImageId { get; set; } = "";

    public string MimeType { get; set; } = "";

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}