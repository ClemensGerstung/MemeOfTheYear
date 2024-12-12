using MemeOfTheYear.Database;
using MemeOfTheYear.Providers;
using MemeOfTheYear.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Backend.Test;

public class MyTestFixture : IAsyncLifetime
{
    private Mock<IContext> context;
    private ILogger<ImageProvider> logger;

    private IImageProvider imageProvider;

    private Session session = new Session { Id = "qwert", IsAuthenticated = true };

    public Task InitializeAsync()
    {
        var data = new List<Image>().AsQueryable();
        var mockSet = new Mock<DbSet<Image>>();
        mockSet.As<IQueryable<Image>>().Setup(m => m.Provider).Returns(data.Provider);
        mockSet.As<IQueryable<Image>>().Setup(m => m.Expression).Returns(data.Expression);
        mockSet.As<IQueryable<Image>>().Setup(m => m.ElementType).Returns(data.ElementType);
        mockSet.As<IQueryable<Image>>().Setup(m => m.GetEnumerator()).Returns(() => data.GetEnumerator());

        context = new Mock<IContext>();
        context.SetupGet(x => x.Images).Returns(mockSet.Object);

        logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ImageProvider>();

        imageProvider = new ImageProvider(logger, context.Object);

        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

    [Fact]
    public async Task CreateNewImage_ValidData_NewImageCreatedAndAddedToDatabase()
    {
        // arrange
        string hash = "adsf";
        string mime = "image/jpeg";

        // act 
        var image = await imageProvider.CreateNewImage(hash, mime, session);

        // assert
        Assert.Single(imageProvider.Images);

        Assert.Equal(image.Hash, hash);
        Assert.Equal(image.MimeType, mime);
        Assert.Equal(1, image.UploadCount);
        Assert.Equal(image.Uploader, session);
        Assert.True(image.IsEnabled);

        context.Verify(x => x.AddImage(image), Times.Once());
    }

    
}
