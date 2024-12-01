using Microsoft.Extensions.Logging;

interface ISessionProvider {
    Session? GetSession(string id);

    Session CreateNew();

    bool IsAllowed(string id);

    void Authenticate(string id);
}

class SessionProvider(
    ILogger<SessionProvider> logger
) : ISessionProvider
{
    private Dictionary<string, Session> _sessions = new();

    public void Authenticate(string id)
    {
        _sessions[id].IsAuthenticated = true;
    }

    public Session CreateNew()
    {
        var session = new Session {
            Id = Guid.NewGuid().ToString(),
            IsAuthenticated = false
        };
        _sessions.Add(session.Id, session);

        return session;
    }

    public Session? GetSession(string id)
    {
        if(_sessions.TryGetValue(id, out Session session))
        {
            return session;
        }

        return null;
    }

    public bool IsAllowed(string id)
    {
        return GetSession(id)?.IsAuthenticated ?? false;
    }
}