using Microsoft.Extensions.Logging;
using MemeOfTheYear.Database;
using MemeOfTheYear.Types;
using System.Text.Json;

namespace MemeOfTheYear.Providers
{
    public class SessionProvider : ISessionProvider
    {
        private Dictionary<string, Session> _sessions;

        private readonly ILogger<SessionProvider> _logger;
        private readonly IContext _context;

        public SessionProvider(ILogger<SessionProvider> logger, IContext context)
        {
            _logger = logger;
            _context = context;

            _sessions = _context.Sessions.ToDictionary(x => x.Id);
            _logger.LogInformation("Existing Sessions: {}", JsonSerializer.Serialize(_sessions));
        }

        public async Task<Session> Authenticate(string id)
        {
            _sessions[id].IsAuthenticated = true;

            await _context.UpdateSession(_sessions[id]);

            return _sessions[id];
        }

        public async Task<Session> CreateNew(string id)
        {
            _logger.LogInformation("CreateNew(id: {})", id);

            if (string.IsNullOrWhiteSpace(id))
            {
                id = Guid.NewGuid().ToString();
            }

            var session = new Session
            {
                Id = id,
                IsAuthenticated = false
            };
            _logger.LogInformation("Created session {}", session);
            _sessions.Add(session.Id, session);

            await _context.AddSession(session);

            return session;
        }

        public Session? GetSession(string id)
        {
            if (_sessions.TryGetValue(id, out Session? session))
            {
                _logger.LogInformation("Found session for id {}: {}", id, session);
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