using Grpc.Core;
using MemeOfTheYear.Backend.Database;
using MemeOfTheYear;

namespace MemeOfTheYear.Backend.Server 
{
    public class ResultServer : ResultService.ResultServiceBase 
    {
        private readonly IMemeOfTheYearContext _context;

        public ResultServer(IMemeOfTheYearContext context)
        {
            _context = context;
        }

        public override async Task<GetVotedImagesResponse> GetMostLikedImages(GetVotedImagesRequest request, ServerCallContext context)
        {
            if(!await _context.CheckSession(request.SessionId)) {
                throw new Exception($"unknown session {request.SessionId}");
            }

            var result = await _context.GetMostLikedImages(request.Count);
            var response = new GetVotedImagesResponse();
            response.Entries.AddRange(result.Select(x => new VoteEntry
            {
                ImageId = x.Image.Id,
                Votes = x.Votes
            }));

            return response;
        }

        public override async Task<GetVotedImagesResponse> GetMostDislikedImages(GetVotedImagesRequest request, ServerCallContext context)
        {
            if(!await _context.CheckSession(request.SessionId)) {
                throw new Exception($"unknown session {request.SessionId}");
            }

            var result = await _context.GetMostDislikedImages(request.Count);
            var response = new GetVotedImagesResponse();
            response.Entries.AddRange(result.Select(x => new VoteEntry
            {
                ImageId = x.Image.Id,
                Votes = x.Votes
            }));

            return response;
        }

        public override async Task<GetVotedImagesResponse> GetMostSkippedImages(GetVotedImagesRequest request, ServerCallContext context)
        {
            if(!await _context.CheckSession(request.SessionId)) {
                throw new Exception($"unknown session {request.SessionId}");
            }

            var result = await _context.GetMostSkippedImages(request.Count);
            var response = new GetVotedImagesResponse();
            response.Entries.AddRange(result.Select(x => new VoteEntry
            {
                ImageId = x.Image.Id,
                Votes = x.Votes
            }));

            return response;
        }
    }
}