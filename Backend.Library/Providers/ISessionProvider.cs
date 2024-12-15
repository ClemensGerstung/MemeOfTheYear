using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public interface ISessionProvider
    {
        Session? GetSession(string id);

        Task<Session> CreateNew(string id);

        bool IsAllowed(string id);

        Task<Session> Authenticate(string id);
    }
}