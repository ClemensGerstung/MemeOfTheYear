using Grpc.Core;
using MemeOfTheYear;
using Microsoft.Extensions.Logging;

public class ResultService(
    ILogger<ResultService> logger,
    IImageProvider imageProvider,
    ISessionProvider sessionProvider,
    IVoteProvider voteProvider
) : MemeOfTheYear.ResultService.ResultServiceBase
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
            .Select(x => new Image
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