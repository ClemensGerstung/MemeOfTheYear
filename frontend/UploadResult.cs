using System.Text.Json;

public class UploadResult
{
    public string ImageId { get; set; } = "";

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}