using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public interface IVoteProvider
    {
        Task SetVoting(Session session, Image image, VoteType type);

        Image? GetNextRandomImage(Session session);

        int GetVoteCount(string imageId, VoteType type);

        int GetSessionVotes(string sessionId);
    }
}