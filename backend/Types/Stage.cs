using System.Text.Json;

namespace MemeOfTheYear.Types
{
    public class Stage
    {
        public StageType Type { get; set; }

        public DateTime EndsAt { get; set; }

        public Dictionary<string, object> Extras { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}