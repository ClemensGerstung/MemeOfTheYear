using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public interface IStageProvider
    {
        public List<Stage> Stages { get; }

        public Stage CurrentStage { get; }

        public Task BackgroundTask { get; }

        public TimeSpan WaitDuration { get; }

        Task StartTracking();

        public Stage GetStageById(int id);
    }
}