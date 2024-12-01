using Microsoft.Extensions.Logging;

public interface IChallengeProvider
{
    Question GetRandomQuestion();

    Question? GetQuestion(int id);
}

class ChallengeProvider : IChallengeProvider
{
private readonly ILogger<ChallengeProvider> _logger;

private readonly List<Question> _questions;

    public ChallengeProvider(ILogger<ChallengeProvider> logger)
    {
        _logger = logger;

        _questions = [
            new Question {
                Id = 1,
                QuestionText = "asdf?",
                Answers = [
                    "asdf"
                ]
            },
            new Question {
                Id = 2,
                QuestionText = "fdsa?",
                Answers = [
                    "fdsa"
                ]
            }
        ];
    }

    public Question? GetQuestion(int id)
    {
        return _questions.FirstOrDefault(q => q.Id == id);
    }

    public Question GetRandomQuestion()
    {
        var random = new Random();
        var index = random.Next(_questions.Count);

        return _questions[index];
    }
}