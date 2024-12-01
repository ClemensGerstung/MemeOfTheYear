using Microsoft.Extensions.Logging;

public interface IVoteProvider
{
    void SetVoting(Session session, Meme image, VoteType type);

    Meme? GetNextRandomImage(Session session);
}

class VoteProvider : IVoteProvider
{
    private ILogger<VoteProvider> _logger;
    private IImageProvider _imageProvider;
    private List<Vote> _votes = new();

    public VoteProvider(ILogger<VoteProvider> logger, IImageProvider imageProvider)
    {
        _logger = logger;
        _imageProvider = imageProvider;
    }

    public void SetVoting(Session session, Meme image, VoteType type)
    {
        // TODO: add staging?
        var index = _votes.FindIndex(x => x.Session == session && x.Meme == image);

        if (index == -1)
        {
            _votes.Add(new Vote
            {
                Session = session,
                Meme = image,
                Type = type
            });
        }
        else
        {
            // todo: check, this may only be available on previous type == Skip
            _votes[index].Type = type;
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
}