using System.Text.Json;
using Microsoft.Extensions.Logging;
using MemeOfTheYear.Types;

namespace MemeOfTheYear.Providers
{
    public class StageProvider : IStageProvider
    {
        private ILogger<StageProvider> _logger;
        private readonly IImageProvider _imageProvider;
        private readonly IVoteProvider _voteProvider;
        private int _stageIndex = 0;
        private TimeSpan _waitTimeSpan;

        public List<Stage> Stages { get; }

        public Stage CurrentStage { get; private set; }

        public StageProvider(ILogger<StageProvider> logger,
                             ILocalStorageProvider localStorageProvider,
                             IImageProvider imageProvider,
                             IVoteProvider voteProvider)
        {
            _logger = logger;
            _imageProvider = imageProvider;
            _voteProvider = voteProvider;

            Stages = localStorageProvider.GetConfig<List<Types.Stage>>("stages.json");
            _logger.LogInformation("{}", JsonSerializer.Serialize(Stages));

            CurrentStage = Stages[_stageIndex];

            _waitTimeSpan = TimeSpan.FromHours(1);
            _logger.LogDebug("{}", CurrentStage);
        }

        private async Task CheckStages()
        {
            var now = DateTime.Now;
            _logger.LogDebug("Check {} against {}", now, CurrentStage);

            if (now >= CurrentStage.EndsAt)
            {
                _stageIndex++;
                CurrentStage = Stages[_stageIndex];
                _logger.LogInformation("Advanced to next stage {}", CurrentStage);

                if (CurrentStage.Extras.TryGetValue("MaxImages", out object? obj))
                {
                    int maxImages = obj as int? ?? +_imageProvider.Images.Count;
                    _logger.LogInformation("Current Stage has a limit of {} allowed images", maxImages);

                    // TODO: merge with ResultServiceImpl!
                    var images = _imageProvider.Images.Where(x => x.IsEnabled)
                                        .Select(x => new
                                        {
                                            Image = x,
                                            Count = _voteProvider.GetVoteCount(x.Id, VoteType.Like),
                                            Dislikes = _voteProvider.GetVoteCount(x.Id, VoteType.Dislike)
                                        })
                                        .OrderByDescending(x => x.Count)
                                        .ThenBy(x => x.Dislikes)
                                        .ThenByDescending(x => x.Image.UploadCount)
                                        .Take(maxImages)
                                        .Select(x => x.Image)
                                        .ToList();
                    var imagesToDisable = _imageProvider.Images.Where(x => x.IsEnabled).Except(images);

                    foreach (var imageToDisable in imagesToDisable)
                    {
                        _logger.LogDebug("Disable image {}", imageToDisable);
                        imageToDisable.IsEnabled = false;
                        await _imageProvider.UpdateImage(imageToDisable);
                    }
                }
            }

            await Task.Delay(_waitTimeSpan);
            await CheckStages();
        }

        public void StartTracking()
        {
            _ = Task.Run(async delegate
            {
                await Task.Delay(_waitTimeSpan);
                await CheckStages();
            });
        }
    }
}