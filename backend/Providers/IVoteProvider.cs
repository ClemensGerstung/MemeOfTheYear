using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public interface IVoteProvider
    {
        Task SetVoting(Session session, Image image, Stage stage, VoteType type);

        Image? GetNextRandomImage(Session session, Stage stage);

        int GetVoteCount(string imageId, VoteType type);
        
        int GetVoteCount(string imageId, Stage stage, VoteType type);

        int GetSessionVotes(string sessionId, Stage stage);
    }
}