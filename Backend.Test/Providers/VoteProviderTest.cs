using MemeOfTheYear.Database;
using MemeOfTheYear.Providers;
using MemeOfTheYear.Types;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class VoteProviderTest : IAsyncLifetime
{
    private readonly Mock<IContext> context = new();
    private readonly Mock<IImageProvider> imageProvider = new();
    private readonly ILogger<VoteProvider> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<VoteProvider>();

    private IVoteProvider voteProvider;

    public Task InitializeAsync()
    {
        context.SetupGet(x => x.Votes).Returns([]);

        voteProvider = new VoteProvider(logger, imageProvider.Object, context.Object);

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task SetVoting_ValidDataNotVotedYet_VotingAdded()
    {
        // arrange
        var session = new Session
        {
            Id = "asdf",
            IsAuthenticated = true
        };
        var image = new Image
        {
            Id = "fdsa",
            IsEnabled = true,
            Hash = "fghj",
            MimeType = "image/jpeg",
            UploadCount = 1,
            Uploader = session
        };
        var stage = new Stage
        {
            Id = 1,
            Type = StageType.Vote
        };

        // act
        var vote = await voteProvider.SetVoting(session, image, stage, VoteType.Like);

        // assert
        context.Verify(x => x.AddVote(vote), Times.Once());
    }

    [Fact]
    public async Task SetVoting_ValidDataAlreadVoted_VotingUpdated()
    {
        // arrange
        var session = new Session
        {
            Id = "asdf",
            IsAuthenticated = true
        };
        var image = new Image
        {
            Id = "fdsa",
            IsEnabled = true,
            Hash = "fghj",
            MimeType = "image/jpeg",
            UploadCount = 1,
            Uploader = session
        };
        var stage = new Stage
        {
            Id = 1,
            Type = StageType.Vote
        };
        await voteProvider.SetVoting(session, image, stage, VoteType.Skip);

        // act
        var vote = await voteProvider.SetVoting(session, image, stage, VoteType.Like);

        // assert
        context.Verify(x => x.UpdateVote(vote), Times.Once());
    }

    [Fact]
    public void GetNextRandomImage_NoVotes_AvailableImageReturned()
    {
        // arrange 
        var session = new Session
        {
            Id = "asdf",
            IsAuthenticated = true
        };
        var image = new Image
        {
            Id = "fdsa",
            IsEnabled = true,
            Hash = "fghj",
            MimeType = "image/jpeg",
            UploadCount = 1,
            Uploader = session
        };
        var stage = new Stage
        {
            Id = 1,
            Type = StageType.Vote
        };
        imageProvider.Setup(x => x.GetAvailableMemes()).Returns([image]);

        // act
        var next = voteProvider.GetNextRandomImage(session, stage);

        // assert
        Assert.NotNull(next);
        Assert.Equal(image, next);
    }

    [Fact]
    public async Task GetNextRandomImage_ImageVoted_AvailableImageReturned()
    {
        // arrange 
        var session = new Session
        {
            Id = "asdf",
            IsAuthenticated = true
        };
        var image = new Image
        {
            Id = "fdsa",
            IsEnabled = true,
            Hash = "fghj",
            MimeType = "image/jpeg",
            UploadCount = 1,
            Uploader = session
        };
        var stage = new Stage
        {
            Id = 1,
            Type = StageType.Vote
        };
        imageProvider.Setup(x => x.GetAvailableMemes()).Returns([image]);
        await voteProvider.SetVoting(session, image, stage, VoteType.Like);

        // act
        var next = voteProvider.GetNextRandomImage(session, stage);

        // assert
        Assert.Null(next);
    }

    [Fact]
    public async Task GetNextRandomImage_ImageSkipped_AvailableImageReturned()
    {
        // arrange 
        var session = new Session
        {
            Id = "asdf",
            IsAuthenticated = true
        };
        var image = new Image
        {
            Id = "fdsa",
            IsEnabled = true,
            Hash = "fghj",
            MimeType = "image/jpeg",
            UploadCount = 1,
            Uploader = session
        };
        var stage = new Stage
        {
            Id = 1,
            Type = StageType.Vote
        };
        imageProvider.Setup(x => x.GetAvailableMemes()).Returns([image]);
        await voteProvider.SetVoting(session, image, stage, VoteType.Skip);

        // act
        var next = voteProvider.GetNextRandomImage(session, stage);

        // assert
        Assert.NotNull(next);
        Assert.Equal(next, image);
    }

    [Fact]
    public void GetVoteCount_ImageNotVoted_ReturnedZero()
    {
        // arrange

        // act
        var count = voteProvider.GetVoteCount("asdf", VoteType.Like);

        // assert
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetVoteCount_ImageVoted_ReturnedOne()
    {
        // arrange
        var session = new Session
        {
            Id = "asdf",
            IsAuthenticated = true
        };
        var image = new Image
        {
            Id = "fdsa",
            IsEnabled = true,
            Hash = "fghj",
            MimeType = "image/jpeg",
            UploadCount = 1,
            Uploader = session
        };
        var stage = new Stage
        {
            Id = 1,
            Type = StageType.Vote
        };
        await voteProvider.SetVoting(session, image, stage, VoteType.Like);

        // act
        var count = voteProvider.GetVoteCount("fdsa", VoteType.Like);

        // assert
        Assert.Equal(1, count);
    }

    [Fact]
    public async Task GetVoteCount_ImageVotedUnknownStage_ReturnedZero()
    {
        // arrange
        var session = new Session
        {
            Id = "asdf",
            IsAuthenticated = true
        };
        var image = new Image
        {
            Id = "fdsa",
            IsEnabled = true,
            Hash = "fghj",
            MimeType = "image/jpeg",
            UploadCount = 1,
            Uploader = session
        };
        var stage = new Stage
        {
            Id = 1,
            Type = StageType.Vote
        };
        var newStage = new Stage
        {
            Id = 2,
            Type = StageType.Vote
        };
        await voteProvider.SetVoting(session, image, stage, VoteType.Like);

        // act
        var count = voteProvider.GetVoteCount("fdsa", newStage, VoteType.Like);

        // assert
        Assert.Equal(0, count);
    }

    [Fact]
    public async Task GetVoteCount_ImageVotedKnownStage_ReturnedZero()
    {
        // arrange
        var session = new Session
        {
            Id = "asdf",
            IsAuthenticated = true
        };
        var image = new Image
        {
            Id = "fdsa",
            IsEnabled = true,
            Hash = "fghj",
            MimeType = "image/jpeg",
            UploadCount = 1,
            Uploader = session
        };
        var stage = new Stage
        {
            Id = 1,
            Type = StageType.Vote
        };
        await voteProvider.SetVoting(session, image, stage, VoteType.Like);

        // act
        var count = voteProvider.GetVoteCount("fdsa", stage, VoteType.Like);

        // assert
        Assert.Equal(1, count);
    }
}