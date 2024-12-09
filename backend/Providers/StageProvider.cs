using System.Text.Json;
using Microsoft.Extensions.Logging;
using MemeOfTheYear.Types;
using MemeOfTheYear.Database;

namespace MemeOfTheYear.Providers
{
    public class StageProvider(
        ILogger<StageProvider> logger,
        ILocalStorageProvider localStorageProvider,
        IImageProvider imageProvider,
        IVoteProvider voteProvider
    ) : IStageProvider
    {
        private int _stageIndex = 0;
        private TimeSpan _waitTimeSpan;

        public List<Stage> Stages { get; private set; } = new();

        public Stage CurrentStage { get; private set; } = new();

        private async Task CheckStages()
        {
            var now = DateTime.Now;
            logger.LogDebug("Check {} against {}", now, CurrentStage);

            if (now >= CurrentStage.EndsAt)
            {
                var oldStage = CurrentStage;
                _stageIndex++;
                CurrentStage = Stages[_stageIndex];
                logger.LogInformation("Advanced to next stage {}", CurrentStage);

                if (CurrentStage.Extras.TryGetValue("MaxImages", out object? obj))
                {
                    int maxImages = imageProvider.Images.Count;
                    if (obj is not null)
                    {
                        logger.LogDebug("MaxImages: {} {}", obj, obj.GetType());
                        maxImages = ((JsonElement)obj).GetInt32();
                    }

                    logger.LogInformation("Current Stage has a limit of {} allowed images", maxImages);

                    // TODO: merge with ResultServiceImpl!
                    var images = imageProvider.Images
                                        .Where(x => x.IsEnabled)
                                        .Select(x => new
                                        {
                                            Image = x,
                                            Count = voteProvider.GetVoteCount(x.Id, oldStage, VoteType.Like),
                                            Dislikes = voteProvider.GetVoteCount(x.Id, oldStage, VoteType.Dislike)
                                        })
                                        .OrderByDescending(x => x.Count)
                                        .ThenBy(x => x.Dislikes)
                                        .ThenByDescending(x => x.Image.UploadCount)
                                        .Take(maxImages)
                                        .Select(x => x.Image)
                                        .ToList();
                    logger.LogDebug("Images to keep active: {}", JsonSerializer.Serialize(images));
                    var imagesToDisable = imageProvider.Images.Where(x => x.IsEnabled).Except(images).ToList();
                    logger.LogDebug("Images to disable: {}", JsonSerializer.Serialize(imagesToDisable));

                    foreach (var imageToDisable in imagesToDisable)
                    {
                        logger.LogDebug("Disable image {}", imageToDisable);
                        imageToDisable.IsEnabled = false;
                        await imageProvider.UpdateImage(imageToDisable);
                    }
                }
            }

            await Task.Delay(_waitTimeSpan);
            await CheckStages();
        }

        public async Task StartTracking()
        {
            Stages = localStorageProvider.GetConfig<List<Types.Stage>>("stages.json");
            logger.LogDebug("{}", JsonSerializer.Serialize(Stages));

            CurrentStage = Stages.Where(x => x.EndsAt >= DateTime.Now).First();
            _stageIndex = Stages.IndexOf(CurrentStage);
            _waitTimeSpan = TimeSpan.FromMinutes(1);

            logger.LogDebug("{}", CurrentStage);

            _ = Task.Run(async () =>
            {
                await Task.Delay(_waitTimeSpan);
                await CheckStages();
            });
        }

        public Stage GetStageById(int id)
        {
            return Stages.First(x => x.Id == id);
        }
    }
}