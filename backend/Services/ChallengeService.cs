using Grpc.Core;
using MemeOfTheYear;
using Microsoft.Extensions.Logging;

class ChallengeService(
    ILogger<ChallengeService> logger,
    ISessionProvider sessionProvider,
    IChallengeProvider challengeProvider
) : MemeOfTheYear.ChallengeService.ChallengeServiceBase
{
    public override Task<AnswerChallengeResponse> AnswerChallenge(AnswerChallengeRequest request, ServerCallContext context)
    {
        var question = challengeProvider.GetQuestion(request.QuestionId);
        var correct = question?.Answers?.Contains(request.Answer, StringComparer.InvariantCultureIgnoreCase) ?? false;

        sessionProvider.Authenticate(request.SessionId);

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
            QuestionText = question.QuestionText
        });
    }
}