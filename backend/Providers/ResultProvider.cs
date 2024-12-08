using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    class ResultProvider(
        IImageProvider imageProvider,
        IVoteProvider voteProvider
    ) : IResultProvider
    {
        public List<Image> GetMostVotedImages(int count)
        {
            var result = imageProvider.Images
                .Where(x => x.IsEnabled)
                .Select(x => new { Image = x, Count = voteProvider.GetVoteCount(x.Id, VoteType.Like) })
                .OrderByDescending(x => x.Count)
                .ThenByDescending(x => x.Image.UploadCount)
                .Take(count)
                .Select(x => x.Image)
                .ToList();

            return result;
        }
    }
}