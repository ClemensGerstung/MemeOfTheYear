using MemeOfTheYear.Database;
using MemeOfTheYear.Providers;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class SessionProviderTest : IAsyncLifetime
{
    private readonly Mock<IContext> context = new();
    private readonly ILogger<SessionProvider> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<SessionProvider>();

    private ISessionProvider sessionProvider;

    public Task InitializeAsync()
    {
        context.SetupGet(x => x.Sessions).Returns([]);

        sessionProvider = new SessionProvider(logger, context.Object);

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateNew_EmptyId_NewSessionCreated()
    {
        // arrange

        // act
        var session = await sessionProvider.CreateNew(string.Empty);

        // assert
        context.Verify(x => x.AddSession(session), Times.Once());
        Assert.False(session.IsAuthenticated);
    }

    [Fact]
    public async Task CreateNew_GivenId_NewSessionCreated()
    {
        // arrange
        string id = Guid.NewGuid().ToString();

        // act
        var session = await sessionProvider.CreateNew(id);

        // assert
        context.Verify(x => x.AddSession(session), Times.Once());
        Assert.False(session.IsAuthenticated);
        Assert.Equal(session.Id, id);
    }

    [Fact]
    public async Task GetSession_KnownId_SessionReturned()
    {
        // arrange
        string id = Guid.NewGuid().ToString();
        await sessionProvider.CreateNew(id);

        // act
        var session = sessionProvider.GetSession(id);

        // assert
        Assert.NotNull(session);
    }

    [Fact]
    public async Task GetSession_UnknownId_NullReturned()
    {
        // arrange
        string id = Guid.NewGuid().ToString();

        // act
        var session = sessionProvider.GetSession(id);

        // assert
        Assert.Null(session);
    }

    [Fact]
    public async Task IsAllowed_KnownIdNotAllowed_NotAllowed()
    {
        // arrange
        string id = Guid.NewGuid().ToString();
        await sessionProvider.CreateNew(id);

        // act
        var allowed = sessionProvider.IsAllowed(id);

        // assert
        Assert.False(allowed);
    }

    [Fact]
    public async Task IsAllowed_UnknownId_NotAllowed()
    {
        // arrange
        string id = Guid.NewGuid().ToString();

        // act
        var allowed = sessionProvider.IsAllowed(id);

        // assert
        Assert.False(allowed);
    }

    [Fact]
    public async Task Authenticate_KnownSession_SessionsUpdated()
    {
        // arrange
        string id = Guid.NewGuid().ToString();
        await sessionProvider.CreateNew(id);

        // act
        var updated = await sessionProvider.Authenticate(id);

        // assert
        context.Verify(x => x.UpdateSession(updated), Times.Once());
        Assert.True(updated.IsAuthenticated);
    }
}