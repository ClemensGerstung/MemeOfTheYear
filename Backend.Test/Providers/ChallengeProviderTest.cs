using MemeOfTheYear.Providers;
using MemeOfTheYear.Types;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class ChallengeProviderTest : IAsyncLifetime
{
    private readonly Mock<ILocalStorageProvider> localStorageProvider = new();
    private readonly ILogger<ChallengeProvider> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ChallengeProvider>();

    private IChallengeProvider challengeProvider;

    public Task InitializeAsync()
    {
        localStorageProvider.Setup(x => x.GetConfigAsync<List<Question>>("questions.json"))
                            .ReturnsAsync([
                                new() {
                                    Id = 1,
                                    Text = "asdf",
                                    Answers = ["asdf"]
                                },
                                new() {
                                    Id = 2,
                                    Text = "fdsa",
                                    Answers = ["fdsa"]
                                }
                            ]);

        challengeProvider = new ChallengeProvider(logger, localStorageProvider.Object);

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task GetQuestion_KnownId_QuestionReturned()
    {
        // arrange
        await challengeProvider.SetupQuestions();

        // act
        var question = challengeProvider.GetQuestion(1);

        // assert
        Assert.NotNull(question);
        Assert.Equal("asdf", question.Text);
        Assert.Single(question.Answers);
    }

    [Fact]
    public async Task GetQuestion_UnknownId_QuestionReturned()
    {
        // arrange
        await challengeProvider.SetupQuestions();

        // act
        var question = challengeProvider.GetQuestion(1337);

        // assert
        Assert.Null(question);
    }
}