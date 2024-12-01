﻿
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddGrpc();
builder.Services.AddSingleton<ILocalStorageProvider, LocalStorageProvider>();
builder.Services.AddSingleton<ISessionProvider, SessionProvider>();
builder.Services.AddSingleton<IImageProvider, ImageProvider>();
builder.Services.AddSingleton<IChallengeProvider, ChallengeProvider>();
builder.Services.AddSingleton<IVoteProvider, VoteProvider>();

var app = builder.Build();

app.MapGrpcService<ImageService>();
app.MapGrpcService<VoteService>();
app.MapGrpcService<ChallengeService>();
app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client");

app.Run();