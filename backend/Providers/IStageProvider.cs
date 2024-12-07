using System.ComponentModel;
using System.Text.Json;
using Microsoft.Extensions.Logging;

public enum StageType
{
    Nominate,
    Vote,
    Result
}

public class Stage
{
    public StageType Type { get; set; }

    public DateTime EndsAt { get; set; }

    public Dictionary<string, object> Extras { get; set; } = new();

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}

public interface IStageProvider
{
    public List<Stage> Stages { get; }

    public Stage CurrentStage { get; }

    void StartTracking();
}

public class StageProvider : IStageProvider
{
    private ILogger<StageProvider> _logger;
    private int _stageIndex = 0;
    private TimeSpan _waitTimeSpan;

    public List<Stage> Stages { get; }

    public Stage CurrentStage { get; private set; }

    public StageProvider(ILogger<StageProvider> logger,
                         ILocalStorageProvider localStorageProvider)
    {
        _logger = logger;

        Stages = localStorageProvider.GetConfig<List<Stage>>("stages.json");
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