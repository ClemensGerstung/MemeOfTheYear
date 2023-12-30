using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace MemeOfTheYear
{

    public interface IMemeOfTheYearContext : IDisposable
    {
        Task AddQuestion(int id, string question, List<string> answers);
        
        Task AddQuestions(IEnumerable<Question> questions);

        Task<Question> GetRandomQuestion();

        Task<bool> CheckQuestionAnswer(int id, string answer);

        Task InitImages();

        Task<string> GetImageData(string imageId);

        Task<Image> GetRandomImage();

        Task<Image> GetNextRandomImage(string session);

        Task<Session> GetNewSession();

        Task Vote(string sessionId, string imageId, Vote vote);

        Task<List<VoteResult>> GetMostLikedImages(int count);

        Task<List<VoteResult>> GetMostDislikedImages(int count);

        Task<List<VoteResult>> GetMostSkippedImages(int count);
    }

    public class MemeOfTheYearContext : DbContext, IMemeOfTheYearContext
    {
        private readonly ILogger _logger;

        public DbSet<Question> Questions { get; set; }
        public DbSet<Image> Images { get; set; }
        public DbSet<Session> Sessions { get; set; }
        public DbSet<VoteEntry> VoteEntries { get; set; }

        public string DbPath { get; }

        public string ImagePath { get; }

        public MemeOfTheYearContext(ILogger<MemeOfTheYearContext> logger)
        {
            _logger = logger;

            var folder = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_DB") 
                ?? Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            
            DbPath = System.IO.Path.Join(folder, "memeoftheyear.db");
            ImagePath = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_IMAGES") ?? "/tmp/images";

            _logger.LogInformation("Use DbPath: {0}", DbPath);
            _logger.LogInformation("Use ImagesPath: {0}", ImagePath);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source={DbPath}");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<VoteEntry>()
                        .Property(f => f.Id)
                        .ValueGeneratedOnAdd();
        }

        public async Task AddQuestion(int id, string question, List<string> answers)
        {
            var exisiting = await Questions.FindAsync(id);
            if (exisiting != null)
            {
                return;
            }

            await Questions.AddAsync(new Question { Id = id, Text = question, Answers = answers });
            await SaveChangesAsync();
        }

        public async Task AddQuestions(IEnumerable<Question> questions) 
        {
            foreach (var question in questions)
            {
                await AddQuestion(question.Id, question.Text, question.Answers);
            }
        }

        public async Task<Question> GetRandomQuestion()
        {
            var size = await Questions.CountAsync();
            var random = new Random();

            var index = random.Next(size);

            return await Questions.ElementAtAsync(index);
        }

        public async Task<bool> CheckQuestionAnswer(int id, string answer)
        {
            var question = await Questions.FindAsync(id);

            return question?.Answers?.Contains(answer, StringComparer.InvariantCultureIgnoreCase) ?? false;
        }

        public async Task InitImages()
        {
            Images.RemoveRange(Images);
            await SaveChangesAsync();

            var directory = new DirectoryInfo(ImagePath);
            foreach (var item in directory.GetFiles())
            {
                var name = Path.GetFileNameWithoutExtension(item.Name);

                if (await Images.FindAsync(name) == null)
                {
                    _logger.LogInformation("Add new Image with Id {0}", name);
                    await Images.AddAsync(new Image { Id = name });
                }
                else
                {
                    _logger.LogWarning("Image with Id {0} already exists", name);
                }
            }

            await SaveChangesAsync();
        }

        public async Task<string> GetImageData(string imageId)
        {
            _logger.LogInformation("GetImageData(imageId: {0})", imageId);

            var image = new FileInfo(Path.Combine(ImagePath, imageId + ".jpg"));
            using var stream = image.OpenRead();
            using var memoryStream = new MemoryStream();
            await stream.CopyToAsync(memoryStream);

            var data = "data:image/jpg;base64, " + Convert.ToBase64String(memoryStream.ToArray());
            // _logger.LogInformation("data: {0}", data);

            return data;
        }

        public Task<Image> GetRandomImage()
        {
            var realImages = Images.Where(x => x.Id != "default").ToArray();
            var count = realImages.Length;
            var random = new Random();
            var index = random.Next(count);

            return Task.FromResult(realImages[index]);
        }

        public Task<Image> GetNextRandomImage(string sessionId)
        {
            var unvotedImages = Images.Where(x => x.Id != "default")
                                      .Except(VoteEntries.Where(x => x.Session.Id == sessionId)
                                                         .Select(x => x.Image))
                                      .ToArray();
            if(unvotedImages.Length == 0)
            {
                _logger.LogWarning($"no more images left for session {sessionId}");
                return Task.FromResult(new Image {Id = string.Empty});
            }

            _logger.LogInformation($"remaining images: {string.Join(',', unvotedImages.Select(x => x.Id))}");

            var count = unvotedImages.Length;
            var random = new Random();
            var index = random.Next(count);

            _logger.LogInformation($"select index {index} in {count} => {unvotedImages[index].Id}");

            return Task.FromResult(unvotedImages[index]);
        }

        public async Task<Session> GetNewSession()
        {
            var guid = Guid.NewGuid();
            var session = new Session { Id = guid.ToString() };
            await Sessions.AddAsync(session);
            await SaveChangesAsync();

            return session;
        }

        public async Task Vote(string sessionId, string imageId, Vote vote)
        {
            var session = await Sessions.FindAsync(sessionId);
            _ = session ?? throw new Exception($"Unknown Session {sessionId}");
            
            var image = await Images.FindAsync(imageId);
            _ = image ?? throw new Exception($"Unknown Session {imageId}");

            var entry = new VoteEntry {
                Session = session,
                Image = image,
                Vote = vote
            };

            _logger.LogInformation($"Voted {sessionId} for image {imageId}: {vote}");

            await VoteEntries.AddAsync(entry);
            await SaveChangesAsync();
        }

        public Task<List<VoteResult>> GetMostLikedImages(int count) 
        {
            var result = VoteEntries.GroupBy(x => x.Image, x => x.Vote)
                        .Select(x => new VoteResult {
                            Image = x.Key, 
                            Votes = x.Count(y => y == MemeOfTheYear.Vote.Like)
                        })
                        .OrderByDescending(x => x.Votes)
                        .Take(count)
                        .ToList();

            return Task.FromResult(result);
        }

        public Task<List<VoteResult>> GetMostDislikedImages(int count) 
        {
            var result = VoteEntries.GroupBy(x => x.Image, x => x.Vote)
                        .Select(x => new VoteResult {
                            Image = x.Key, 
                            Votes = x.Count(y => y == MemeOfTheYear.Vote.Dislike)
                        })
                        .OrderByDescending(x => x.Votes)
                        .Take(count)
                        .ToList();

            return Task.FromResult(result);
        }

        public Task<List<VoteResult>> GetMostSkippedImages(int count) 
        {
            var result = VoteEntries.GroupBy(x => x.Image, x => x.Vote)
                        .Select(x => new VoteResult {
                            Image = x.Key, 
                            Votes = x.Count(y => y == MemeOfTheYear.Vote.Skip)
                        })
                        .OrderByDescending(x => x.Votes)
                        .Take(count)
                        .ToList();

            return Task.FromResult(result);
        }
    }

    public class VoteResult 
    {
        public Image Image { get; set; }

        public int Votes { get; set; }
    }

    public class Question
    {
        public int Id { get; set; }

        public string Text { get; set; }

        public List<string> Answers { get; set; } = new List<string>();
    }

    public class Image
    {
        public string Id { get; set; }
    }

    public class Session
    {
        public string Id { get; set; }
    }

    public enum Vote
    {
        Like,
        Dislike,
        Skip
    }

    public class VoteEntry
    {
        public int Id { get; set; }
        public Session Session { get; set; }

        public Image Image { get; set; }

        public Vote Vote { get; set; }
    }

}