using Microsoft.EntityFrameworkCore;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Database
{
    public interface IContext
    {
        DbSet<Session> Sessions { get; }
        DbSet<Image> Images { get; }
        DbSet<Vote> Votes { get; }

        Task AddSession(Session session);
        Task UpdateSession(Session session);
        Task AddVote(Vote vote);
        Task UpdateVote(Vote vote);
        Task AddImage(Image image);
        Task UpdateMeme(Image image);
    }
}