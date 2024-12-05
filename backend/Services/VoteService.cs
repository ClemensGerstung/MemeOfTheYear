using Microsoft.Extensions.Logging;
using MemeOfTheYear;
using Grpc.Core;

class VoteService(
    ILogger<VoteService> logger,
    ISessionProvider sessionProvider,
    IVoteProvider voteProvider,
    IImageProvider imageProvider,
    IStageProvider stageProvider
) : MemeOfTheYear.VoteService.VoteServiceBase
{

    public override async Task<SessionIdResponse> Init(SessionIdRequest request, ServerCallContext context)
    {
        logger.LogInformation("Init: {}", request);
        string sessionId = request.SessionId;
        string nextImageId = string.Empty;
        var stage = stageProvider.CurrentStage;
        var type = stage.Type switch
        {
            StageType.Nominate => MemeOfTheYear.Stage.Types.Type.Nominate,
            StageType.Vote => MemeOfTheYear.Stage.Types.Type.Vote,
            StageType.Result => MemeOfTheYear.Stage.Types.Type.Result,
            _ => throw new NotImplementedException(),
        };
        var session = sessionProvider.GetSession(sessionId);
        Console.WriteLine($"Init: current session {session}");

        if (session == null)
        {
            session = await sessionProvider.CreateNew();
        }
        else
        {
            var meme = voteProvider.GetNextRandomImage(session);
            nextImageId = meme?.Id ?? String.Empty;
        }

        Console.WriteLine($"Init: current session {session.Id}");

        return new SessionIdResponse
        {
            SessionId = session.Id,
            IsAuthenticated = session.IsAuthenticated,
            ImageId = nextImageId,
            Stage = new MemeOfTheYear.Stage {
                Type = type
            }
        };
    }

    public override Task<VoteResponse> Like(VoteRequest request, ServerCallContext context)
    {
        return SetVoting(request, VoteType.Like);
    }

    public override Task<VoteResponse> Dislike(VoteRequest request, ServerCallContext context)
    {
        return SetVoting(request, VoteType.Dislike);
    }

    public override Task<VoteResponse> Skip(VoteRequest request, ServerCallContext context)
    {
        return SetVoting(request, VoteType.Skip);
    }

    private async Task<VoteResponse> SetVoting(VoteRequest request, VoteType type)
    {
        if (!sessionProvider.IsAllowed(request.SessionId))
        {
            throw new InvalidOperationException("Session not authenticated!");
        }

        var session = sessionProvider.GetSession(request.SessionId)!;
        var image = imageProvider.GetImageById(request.ImageId);

        await voteProvider.SetVoting(session, image, type);
        var nextImage = voteProvider.GetNextRandomImage(session);

        return new VoteResponse
        {
            Finished = nextImage == null,
            NextImageId = nextImage?.Id ?? string.Empty,
            RemainingLikes = int.MaxValue
        };
    }
}