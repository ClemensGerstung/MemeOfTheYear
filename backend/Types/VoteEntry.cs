namespace MemeOfTheYear.Backend.Types
{
    public class VoteEntry
    {
        public int Id { get; set; }
        
        public Session Session { get; set; } = new Session();

        public Image Image { get; set; } = new Image();

        public VoteType Vote { get; set; }
    }

}