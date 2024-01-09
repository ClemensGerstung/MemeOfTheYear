namespace MemeOfTheYear.Backend.Types
{
    public class VoteResult 
    {
        public Image Image { get; set; } = new Image();

        public int Votes { get; set; }
    }

}