using Microsoft.Extensions.Logging;
using MemeOfTheYear.Types;
using MemeOfTheYear.Database;

namespace MemeOfTheYear.Providers
{
    class VoteProvider : IVoteProvider
    {
        private List<Vote> _votes;
        private readonly ILogger<VoteProvider> _logger;
        private readonly IImageProvider _imageProvider;
        private readonly IContext _context;

        public VoteProvider(ILogger<VoteProvider> logger, IImageProvider imageProvider, IContext context)
        {
            _logger = logger;
            _imageProvider = imageProvider;
            _context = context;

            _votes = [.. _context.Votes];
        }

        public async Task SetVoting(Session session, Image image, Stage stage, VoteType type)
        {
            var index = _votes.FindIndex(x => x.Session == session && x.Image == image && x.StageId == stage.Id);

            if (index == -1)
            {
                var vote = new Vote
                {
                    Session = session,
                    Image = image,
                    Type = type,
                    StageId = stage.Id
                };
                _votes.Add(vote);
                await _context.AddVote(vote);
            }
            else
            {
                // todo: check, this may only be available on previous type == Skip
                _votes[index].Type = type;

                await _context.UpdateVote(_votes[index]);
            }
        }

        public Image? GetNextRandomImage(Session session, Stage stage)
        {
            var available = _imageProvider.GetAvailableMemes();
            var used = _votes.Where(x => x.Session == session && x.StageId == stage.Id && x.Type != VoteType.Skip)
                            .Select(x => x.Image)
                            .ToList();
            var sessionAvailable = available.Except(used).ToList();

            if (sessionAvailable.Count > 0)
            {
                var random = new Random();
                var index = random.Next(sessionAvailable.Count);

                return sessionAvailable[index];
            }


            return null;
        }

        public int GetVoteCount(string imageId, VoteType type)
        {
            return _votes.Where(x => x.Type == type).Count(x => x.Image.Id == imageId);
        }

        public int GetVoteCount(string imageId, Stage stage, VoteType type)
        {
            return _votes.Where(x => x.Type == type)
                         .Where(x => x.StageId == stage.Id)
                         .Count(x => x.Image.Id == imageId);
        }

        public int GetSessionVotes(string sessionId, Stage stage)
        {
            return _votes.Count(x => x.Session.Id == sessionId && x.StageId == stage.Id && x.Type != VoteType.Skip);
        }
    }
}