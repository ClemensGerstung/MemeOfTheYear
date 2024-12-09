using Microsoft.Extensions.Logging;
using Grpc.Core;
using MemeOfTheYear.Remote;
using MemeOfTheYear.Providers;
using MemeOfTheYear.Types;
using System.Text.Json;

namespace MemeOfTheYear.Services
{
    class VoteServiceImpl(
        ILogger<VoteServiceImpl> logger,
        ISessionProvider sessionProvider,
        IVoteProvider voteProvider,
        IImageProvider imageProvider,
        IStageProvider stageProvider
    ) : VoteService.VoteServiceBase
    {

        public override async Task<SessionIdResponse> Init(SessionIdRequest request, ServerCallContext context)
        {
            logger.LogInformation("Init: {}", request);
            string sessionId = request.SessionId;
            string nextImageId = string.Empty;
            var stage = stageProvider.CurrentStage;
            var type = stage.Type switch
            {
                StageType.Nominate => Remote.Stage.Types.Type.Nominate,
                StageType.Vote => Remote.Stage.Types.Type.Vote,
                StageType.Result => Remote.Stage.Types.Type.Result,
                _ => throw new NotImplementedException(),
            };
            var session = sessionProvider.GetSession(sessionId);
            logger.LogInformation("Init: current session {}", session);

            if (session == null)
            {
                session = await sessionProvider.CreateNew();
            }
            else
            {
                var meme = voteProvider.GetNextRandomImage(session, stage);
                nextImageId = meme?.Id ?? String.Empty;
            }

            logger.LogInformation("Init: current session {}", session.Id);

            return new SessionIdResponse
            {
                SessionId = session.Id,
                IsAuthenticated = session.IsAuthenticated,
                ImageId = nextImageId,
                Stage = new Remote.Stage
                {
                    Type = type,
                    Id = stage.Id
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
            var stage = stageProvider.GetStageById(request.StageId);

            if (!string.IsNullOrWhiteSpace(request.ImageId))
            {
                var image = imageProvider.GetImageById(request.ImageId);

                await voteProvider.SetVoting(session, image, stage, type);
            }

            stageProvider.CurrentStage.Extras.TryGetValue("MaxVotes", out object? maxVotes);
            
            var allowedMaxVotes = (maxVotes as JsonElement?)?.GetInt32() ?? int.MaxValue;
            var sessionVotes = voteProvider.GetSessionVotes(request.SessionId, stage);
            logger.LogInformation("Session {} has voted {} times and is allowed {} votes", request.SessionId, sessionVotes, allowedMaxVotes);

            string nextImageId = string.Empty;

            if (sessionVotes < allowedMaxVotes)
            {
                var nextImage = voteProvider.GetNextRandomImage(session, stage);
                nextImageId = nextImage?.Id ?? string.Empty;
            }

            return new VoteResponse
            {
                Finished = string.IsNullOrWhiteSpace(nextImageId),
                NextImageId = nextImageId,
                RemainingLikes = allowedMaxVotes - sessionVotes
            };
        }
    }
}