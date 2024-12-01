public class Vote
{
    public int Id { get; set; }
    
    public Session Session { get; set; } = new();

    public Meme Meme { get; set; } = new();

    public VoteType Type { get; set; }
}

public enum VoteType
{
    Like,
    Dislike,
    Skip
}