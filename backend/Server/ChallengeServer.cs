using Grpc.Core;
using MemeOfTheYear.Backend.Database;

namespace MemeOfTheYear.Backend.Server
{
    public class ChallengeServer : ChallengeService.ChallengeServiceBase
    {
        private readonly IMemeOfTheYearContext _context;

        public ChallengeServer(IMemeOfTheYearContext context)
        {
            _context = context;
        }

        public override async Task<GetChallengeResponse> GetChallenge(GetChallengeRequest request, ServerCallContext context)
        {
            var question = await _context.GetRandomQuestion();

            return new GetChallengeResponse { QuestionId = question.Id, QuestionText = question.Text };
        }

        public override async Task<AnswerChallengeResponse> AnswerChallenge(AnswerChallengeRequest request, ServerCallContext context)
        {
            bool result = await _context.CheckQuestionAnswer(request.QuestionId, request.Answer);

            return new AnswerChallengeResponse { Success = result };
        }
    }
}