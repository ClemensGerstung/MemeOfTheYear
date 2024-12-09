using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace MemeOfTheYear.Types
{
    public class Stage
    {
        public int Id { get; set; }
        
        public StageType Type { get; set; }

        public DateTime EndsAt { get; set; }

        [NotMapped]
        public Dictionary<string, object> Extras { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}