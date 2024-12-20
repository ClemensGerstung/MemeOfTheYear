using System.Text.Json;

namespace MemeOfTheYear.Types
{
    public class Question
    {
        public int Id { get; set; }

        public string Text { get; set; } = "";

        public List<string> Answers { get; set; } = new();

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}