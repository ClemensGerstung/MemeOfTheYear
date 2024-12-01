public class Question
{
    public int Id { get; set; }

    public string QuestionText { get; set; } = "";

    public List<string> Answers { get; set; } = new();
}