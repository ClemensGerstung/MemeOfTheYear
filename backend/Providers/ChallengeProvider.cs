using System.Text.Json;
using Microsoft.Extensions.Logging;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    class ChallengeProvider(ILogger<ChallengeProvider> logger, ILocalStorageProvider localStorageProvider) : IChallengeProvider
    {

        private List<Question> _questions = [];

        public async Task SetupQuestions()
        {
            _questions = await localStorageProvider.GetConfigAsync<List<Question>>("questions.json");
            logger.LogInformation("Questions: {}", JsonSerializer.Serialize(_questions));
        }

        public Question? GetQuestion(int id)
        {
            return _questions.FirstOrDefault(q => q.Id == id);
        }

        public Question GetRandomQuestion()
        {
            var random = new Random();
            var index = random.Next(_questions.Count);

            logger.LogDebug("Return random question {}", _questions[index]);

            return _questions[index];
        }
    }
}