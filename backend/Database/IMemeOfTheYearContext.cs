using MemeOfTheYear.Backend.Types;

namespace MemeOfTheYear.Backend.Database
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

        Task<Image> GetNextRandomImage(string currentImageId, string session);

        Task<Session> GetNewSession(int maxLikes);

        Task<int> Vote(string sessionId, string imageId, VoteType vote);

        Task<List<VoteResult>> GetMostLikedImages(int count);

        Task<List<VoteResult>> GetMostDislikedImages(int count);

        Task<List<VoteResult>> GetMostSkippedImages(int count);

        Task<bool> CheckSession(string sessionId);

        Task<List<Image>> GetAllImages();
    }

}