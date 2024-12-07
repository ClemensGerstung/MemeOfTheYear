using Grpc.Core;
using Microsoft.Extensions.Logging;
using MemeOfTheYear.Remote;
using MemeOfTheYear.Providers;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Services
{
    public class ResultServiceImpl(
        ILogger<ResultServiceImpl> logger,
        IImageProvider imageProvider,
        ISessionProvider sessionProvider,
        IVoteProvider voteProvider
    ) : ResultService.ResultServiceBase
    {
        public override Task<GetVotedImagesResponse> GetMostLikedImages(GetVotedImagesRequest request, ServerCallContext context)
        {
            if (!sessionProvider.IsAllowed(request.SessionId))
            {
                throw new InvalidOperationException("Session not authenticated!");
            }

            var result = imageProvider.GetAvailableMemes()
                .ToDictionary(x => x, x => voteProvider.GetVoteCount(x.Id, VoteType.Like))
                .OrderByDescending(x => x.Value)
                .ThenByDescending(x => x.Key.UploadCount)
                .Take(request.Count)
                .Select(x => new Remote.Image
                {
                    Id = x.Key.Id,
                    Likes = x.Value,
                    Uploaded = x.Key.UploadCount
                })
                .ToList();

            int index = 1;
            foreach (var img in result)
            {
                logger.LogInformation("{}\t{}\t{}\t{}", index, img.Id, img.Likes, img.Uploaded);
                index++;
            }

            var response = new GetVotedImagesResponse();
            response.Images.AddRange(result);
            return Task.FromResult(response);
        }
    }
}