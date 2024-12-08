
using System.Text.Json;

namespace MemeOfTheYear.Types
{
    public class Image
    {
        public string Id { get; set; } = "";

        public string Hash { get; set; } = "";

        public bool IsEnabled { get; set; } = true;

        public string MimeType { get; set; } = "";

        public int UploadCount { get; set; }

        public Session Uploader { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}