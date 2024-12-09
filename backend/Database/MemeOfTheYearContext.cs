using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Database
{
    class MemeOfTheYearContext(ILogger<MemeOfTheYearContext> logger) : DbContext, IContext
    {
        public DbSet<Session> Sessions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Vote> Votes { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var server = Environment.GetEnvironmentVariable("MYSQL_SERVER") ?? "localhost";
            var username = Environment.GetEnvironmentVariable("MYSQL_USERNAME") ?? "user";
            var password = Environment.GetEnvironmentVariable("MYSQL_PASSWORD") ?? "password";

            var connectionString = $"server={server};user={username};password={password};database=memeOfTheYear";
            logger.LogDebug("Connect to {}", connectionString);

            optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
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

        public async Task AddImage(Image image)
        {
            await RunInTransaction(async () =>
            {
                await Images.AddAsync(image);
            });
        }

        public async Task UpdateMeme(Image image)
        {
            await RunInTransaction(() =>
            {
                Images.Update(image);

                return Task.CompletedTask;
            });
        }
    }
}