
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using MemeOfTheYear.Providers;
using MemeOfTheYear.Services;
using MemeOfTheYear.Database;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc(opt =>
    {
        opt.EnableDetailedErrors = true;
    });
builder.Services.AddSingleton<ILocalStorageProvider, LocalStorageProvider>();
builder.Services.AddSingleton<ISessionProvider, SessionProvider>();
builder.Services.AddSingleton<IImageProvider, ImageProvider>();
builder.Services.AddSingleton<IChallengeProvider, ChallengeProvider>();
builder.Services.AddSingleton<IVoteProvider, VoteProvider>();
builder.Services.AddSingleton<IStageProvider, StageProvider>();

builder.Services.AddDbContext<IContext, MemeOfTheYearContext>(ServiceLifetime.Singleton, ServiceLifetime.Singleton);

var app = builder.Build();
var challenge = app.Services.GetService<IChallengeProvider>();
if (challenge is not null)
{
    await challenge.SetupQuestions();
}

var stages = app.Services.GetService<IStageProvider>();
if (stages is not null)
{
    await stages.StartTracking();
}

var images = app.Services.GetService<IImageProvider>();
if (images is not null)
{
    await images.SetupByExistingData();
}

app.MapGrpcService<ImageServiceImpl>();
app.MapGrpcService<VoteServiceImpl>();
app.MapGrpcService<ChallengeServiceImpl>();
app.MapGrpcService<AdminServiceImpl>();
app.MapGrpcService<ResultServiceImpl>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client");

app.Run();