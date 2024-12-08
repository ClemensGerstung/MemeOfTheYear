using Grpc.Core;
using MemeOfTheYear.Remote;
using MemeOfTheYear.Providers;

namespace MemeOfTheYear.Services
{
    class ChallengeServiceImpl(
        ISessionProvider sessionProvider,
        IChallengeProvider challengeProvider
    ) : ChallengeService.ChallengeServiceBase
    {
        public override Task<AnswerChallengeResponse> AnswerChallenge(AnswerChallengeRequest request, ServerCallContext context)
        {
            var question = challengeProvider.GetQuestion(request.QuestionId);
            var correct = question?.Answers?.Contains(request.Answer, StringComparer.InvariantCultureIgnoreCase) ?? false;

            if (correct)
            {
                sessionProvider.Authenticate(request.SessionId);
            }

            return Task.FromResult(new AnswerChallengeResponse
            {
                Success = correct
            });
        }

        public override Task<GetChallengeResponse> GetChallenge(GetChallengeRequest request, ServerCallContext context)
        {
            var question = challengeProvider.GetRandomQuestion();

            return Task.FromResult(new GetChallengeResponse
            {
                QuestionId = question.Id,
                QuestionText = question.Text
            });
        }
    }
}