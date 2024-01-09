using Json.Net;
using MemeOfTheYear.Backend.Database;
using MemeOfTheYear.Backend.Server;
using MemeOfTheYear.Backend.Types;

// this sets up the whole webstuff for rRPC
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddGrpc();

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "foo",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                                .AllowAnyHeader()
                                .AllowAnyMethod()
                                .WithExposedHeaders("Grpc-Status", "Grpc-Message", "Grpc-Encoding", "Grpc-Accept-Encoding", "X-Grpc-Web", "User-Agent");
                      });
});

builder.Services.AddScoped<IMemeOfTheYearContext, MemeOfTheYearContext>();

var app = builder.Build();
app.UseCors("foo");
app.UseRouting();
app.UseGrpcWeb();
app.MapGet("/", () => "This gRPC service is gRPC-Web enabled, CORS enabled, and is callable from browser apps uisng the gRPC-Web protocal");

app.MapGrpcService<VoteServer>().EnableGrpcWeb();
app.MapGrpcService<ChallengeServer>().EnableGrpcWeb();
app.MapGrpcService<ImageServer>().EnableGrpcWeb();
app.MapGrpcService<ResultServer>().EnableGrpcWeb();

var dbContext = app.Services.GetService<IMemeOfTheYearContext>();

if (dbContext != null)
{
    // TODO: move to custom handler
    var questionsPath = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_QUESTIONS");
    var file = new FileInfo(questionsPath ?? "/share/questions.json");
    if (file.Exists)
    {
        using var stream = file.OpenRead();
        using var reader = new StreamReader(stream);
        var content = await reader.ReadToEndAsync();
        Console.WriteLine(content);

        var questions = JsonNet.Deserialize<List<Question>>(content);
        await dbContext.AddQuestions(questions);
    }

    // TODO: move to custom handler
    // var imagePath = Environment.GetEnvironmentVariable("MEME_OF_THE_YEAR_IMAGES") ?? "/tmp/images";
    // using var watcher = new FileSystemWatcher(imagePath);

    // watcher.NotifyFilter = NotifyFilters.Attributes
    //                      | NotifyFilters.CreationTime
    //                      | NotifyFilters.DirectoryName
    //                      | NotifyFilters.FileName
    //                      | NotifyFilters.LastAccess
    //                      | NotifyFilters.LastWrite
    //                      | NotifyFilters.Security
    //                      | NotifyFilters.Size;

    // watcher.Changed += HandleFileSystemWatcherEvent;
    // watcher.Created += HandleFileSystemWatcherEvent;
    // watcher.Deleted += HandleFileSystemWatcherEvent;
    // watcher.Renamed += HandleFileSystemWatcherEvent;
    // watcher.Error += (_, e) =>
    // {
    //     Console.WriteLine(e);
    // };

    // watcher.Filter = "*.jpg";
    // watcher.IncludeSubdirectories = false;
    // watcher.EnableRaisingEvents = true;

    await dbContext.InitImages();
}

app.Run();
dbContext?.Dispose();

// async void HandleFileSystemWatcherEvent(object sender, FileSystemEventArgs e)
// {
//     await dbContext.InitImages();
// }