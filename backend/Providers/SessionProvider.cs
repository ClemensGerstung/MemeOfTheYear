using Microsoft.Extensions.Logging;
using MemeOfTheYear.Database;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    class SessionProvider : ISessionProvider
    {
        private Dictionary<string, Session> _sessions;

        private readonly ILogger<SessionProvider> _logger;
        private readonly IContext _context;

        public SessionProvider(ILogger<SessionProvider> logger, IContext context)
        {
            _logger = logger;
            _context = context;

            _sessions = _context.Sessions.ToDictionary(x => x.Id);
        }

        public async Task Authenticate(string id)
        {
            _sessions[id].IsAuthenticated = true;

            await _context.UpdateSession(_sessions[id]);
        }

        public async Task<Session> CreateNew()
        {
            var session = new Session
            {
                Id = Guid.NewGuid().ToString(),
                IsAuthenticated = false
            };
            _sessions.Add(session.Id, session);

            await _context.AddSession(session);

            return session;
        }

        public Session? GetSession(string id)
        {
            if (_sessions.TryGetValue(id, out Session? session))
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
}