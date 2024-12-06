using Microsoft.Extensions.Logging;

public interface IVoteProvider
{
    Task SetVoting(Session session, Meme image, VoteType type);

    Meme? GetNextRandomImage(Session session);

    int GetVoteCount(string imageId, VoteType type);
}

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

    public async Task SetVoting(Session session, Meme image, VoteType type)
    {
        // TODO: add staging?
        var index = _votes.FindIndex(x => x.Session == session && x.Meme == image);

        if (index == -1)
        {
            var vote =new Vote
            {
                Session = session,
                Meme = image,
                Type = type
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

    public Meme? GetNextRandomImage(Session session)
    {
        var available = _imageProvider.GetAvailableMemes();
        var used = _votes.Where(x => x.Session == session).Select(x => x.Meme).ToList();
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
        return _votes.Where(x => x.Type == type).Count(x => x.Meme.Id == imageId);
    }
}