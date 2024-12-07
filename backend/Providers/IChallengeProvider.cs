using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public interface IChallengeProvider
    {
        Task SetupQuestions();

        Question GetRandomQuestion();

        Question? GetQuestion(int id);
    }
}