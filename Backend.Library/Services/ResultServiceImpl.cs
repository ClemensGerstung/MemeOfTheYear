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
                .Select(x => new
                {
                    Image = x,
                    Likes = voteProvider.GetVoteCount(x.Id, VoteType.Like),
                    Dislikes = voteProvider.GetVoteCount(x.Id, VoteType.Dislike)
                })
                .OrderByDescending(x => x.Likes)
                .ThenBy(x => x.Dislikes)
                .ThenByDescending(x => x.Image.UploadCount)
                .Take(request.Count)
                .Select(x => new Remote.Image
                {
                    Id = x.Image.Id,
                    Likes = x.Likes,
                    Dislikes = x.Dislikes,
                    Uploaded = x.Image.UploadCount
                })
                .ToList();

            int index = 1;
            foreach (var img in result)
            {
                logger.LogInformation("{}\t{}\t{}\t{}\t{}", index, img.Id, img.Likes, img.Dislikes, img.Uploaded);
                index++;
            }

            var response = new GetVotedImagesResponse();
            response.Images.AddRange(result);
            return Task.FromResult(response);
        }
    }
}