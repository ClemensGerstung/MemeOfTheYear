using Microsoft.EntityFrameworkCore;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Database
{
    public interface IContext
    {
        List<Session> Sessions { get; }
        List<Image> Images { get; }
        List<Vote> Votes { get; }

        Task AddSession(Session session);
        Task UpdateSession(Session session);
        Task AddVote(Vote vote);
        Task UpdateVote(Vote vote);
        Task AddImage(Image image);
        Task UpdateMeme(Image image);
    }
}