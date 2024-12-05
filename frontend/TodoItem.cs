public class TodoItem
{
    public int QuestionId { get; set; }
    public string Answer { get; set; } = "";
    public string SessionId { get; set; } = "";

    public override string ToString()
    {
        return $"QuestionId: {QuestionId}, SessionId: {SessionId}, Answer: {Answer}";
    }
}