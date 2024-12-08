using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public interface IStageProvider
    {
        public List<Stage> Stages { get; }

        public Stage CurrentStage { get; }

        void StartTracking();
    }
}