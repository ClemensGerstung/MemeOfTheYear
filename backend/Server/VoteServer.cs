using Grpc.Core;
using MemeOfTheYear.Backend.Database;
using MemeOfTheYear.Backend.Types;

namespace MemeOfTheYear.Backend.Server
{
    public class VoteServer : VoteService.VoteServiceBase
    {
        private readonly IMemeOfTheYearContext _context;

        public VoteServer(IMemeOfTheYearContext context)
        {
            _context = context;
        }

        public override async Task<SessionIdResponse> Init(SessionIdRequest request, ServerCallContext context)
        {
            var session = await _context.GetNewSession();
            var image = await _context.GetRandomImage();

            return new SessionIdResponse { SessionId = session.Id, ImageId = image.Id };
        }

        public override async Task<VoteResponse> Like(VoteRequest request, ServerCallContext context)
        {
            await _context.Vote(request.SessionId, request.ImageId, VoteType.Like);
            var nextImageId = await _context.GetNextRandomImage(request.SessionId);

            return new VoteResponse { NextImageId = nextImageId.Id };
        }

        public override async Task<VoteResponse> Dislike(VoteRequest request, ServerCallContext context)
        {
            await _context.Vote(request.SessionId, request.ImageId, VoteType.Dislike);
            var nextImageId = await _context.GetNextRandomImage(request.SessionId);

            return new VoteResponse { NextImageId = nextImageId.Id };
        }

        public override async Task<VoteResponse> Skip(VoteRequest request, ServerCallContext context)
        {
            if (!string.IsNullOrEmpty(request.ImageId))
            {
                await _context.Vote(request.SessionId, request.ImageId, VoteType.Skip);
            }

            var nextImageId = await _context.GetNextRandomImage(request.SessionId);

            return new VoteResponse { NextImageId = nextImageId.Id };
        }
    }
}