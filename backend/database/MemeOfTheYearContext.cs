using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public interface IContext
{
    DbSet<Session> Sessions { get; }
    DbSet<Meme> Memes { get; }
    DbSet<Vote> Votes { get; }

    Task AddSession(Session session);
    Task UpdateSession(Session session);
    Task AddVote(Vote vote);
    Task UpdateVote(Vote vote);
    Task AddImage(Meme meme);
    Task UpdateMeme(Meme meme);
}

public class MemeOfTheYearContext : DbContext, IContext
{
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Meme> Memes { get; set; }
    public DbSet<Vote> Votes { get; set; }

    public MemeOfTheYearContext()
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var server = Environment.GetEnvironmentVariable("MYSQL_SERVER") ?? "localhost";
        var username = Environment.GetEnvironmentVariable("MYSQL_USERNAME") ?? "demo";
        var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password";

        var connectionString = $"server={server};user={username};password={password};database=memeOfTheYear";
        Console.WriteLine(connectionString);

        optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
            .LogTo(Console.WriteLine, LogLevel.Information)
            .EnableSensitiveDataLogging()
            .EnableDetailedErrors();
    }

    private async Task RunInTransaction(Func<Task> action)
    {
        using var transaction = await Database.BeginTransactionAsync();

        await action();

        await transaction.CommitAsync();

        await SaveChangesAsync();
    }

    public async Task AddSession(Session session)
    {
        await RunInTransaction(async () =>
        {
            await Sessions.AddAsync(session);
        });
    }

    public async Task UpdateSession(Session session)
    {
        await RunInTransaction(() =>
        {
            Sessions.Update(session);

            return Task.CompletedTask;
        });
    }

    public async Task AddVote(Vote vote)
    {
        await RunInTransaction(async () =>
        {
            await Votes.AddAsync(vote);
        });
    }

    public async Task UpdateVote(Vote vote)
    {
        await RunInTransaction(() =>
        {
            Votes.Update(vote);

            return Task.CompletedTask;
        });
    }

    public async Task AddImage(Meme meme)
    {
        await RunInTransaction(async () =>
        {
            await Memes.AddAsync(meme);
        });
    }

    public async Task UpdateMeme(Meme meme)
    {
        await RunInTransaction(() =>
        {
            Memes.Update(meme);

            return Task.CompletedTask;
        });
    }
}