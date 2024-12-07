namespace MemeOfTheYear.Types
{
    public class Vote
    {
        public int Id { get; set; }

        public Session Session { get; set; } = new();

        public Image Image { get; set; } = new();

        public VoteType Type { get; set; }
    }
}