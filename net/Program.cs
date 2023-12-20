using MemeOfTheYear;

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

_ = app.UseEndpoints(endpoints =>
{
    endpoints.MapGrpcService<VoteServer>().EnableGrpcWeb();
    endpoints.MapGrpcService<ChallengeServer>().EnableGrpcWeb();
    endpoints.MapGrpcService<ImageServer>().EnableGrpcWeb();
});

var dbContext = app.Services.GetService<IMemeOfTheYearContext>();

if (dbContext != null)
{
    await dbContext.AddQuestion(1, "Hello World", ["Hello", "World"]);
    await dbContext.AddQuestion(2, "Test Frage", ["420", "69"]);

    await dbContext.InitImages();
}



app.Run();
dbContext?.Dispose();