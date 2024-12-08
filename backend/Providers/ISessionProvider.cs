using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public interface ISessionProvider
    {
        Session? GetSession(string id);

        Task<Session> CreateNew();

        bool IsAllowed(string id);

        Task Authenticate(string id);
    }
}