using MemeOfTheYear.Database;
using MemeOfTheYear.Providers;
using MemeOfTheYear.Types;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class ImageProviderTest : IAsyncLifetime
{
    private readonly Mock<IContext> context = new();
    private readonly ILogger<ImageProvider> logger = LoggerFactory.Create(builder => builder.AddConsole()).CreateLogger<ImageProvider>();

    private readonly List<Image> _images = [];
    private IImageProvider imageProvider;

    private readonly Session session = new() { Id = "qwert", IsAuthenticated = true };

    public Task InitializeAsync()
    {
        context.SetupGet(x => x.Images).Returns(_images);

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

    [Fact]
    public async Task GetAvailableMemes_EnabledImagesAvailable_EnabledImagesReturned()
    {
        // arrange
        string hash = "adsf";
        string mime = "image/jpeg";
        var image = await imageProvider.CreateNewImage(hash, mime, session);

        // act 
        var images = imageProvider.GetAvailableMemes();

        // assert
        Assert.Single(images);
        Assert.Equal(images[0], image);
    }

    [Fact]
    public async Task GetImageByHash_ImagesAvailable_ImageReturned()
    {
        // arrange
        string hash = "adsf";
        string mime = "image/jpeg";
        var image = await imageProvider.CreateNewImage(hash, mime, session);

        // act 
        var imageByHash = imageProvider.GetImageByHash(hash);

        // assert
        Assert.Equal(imageByHash, image);
    }

    [Fact]
    public void GetImageByHash_NoImagesAvailable_NullReturned()
    {
        // arrange
        string hash = "adsf";

        // act 
        var image = imageProvider.GetImageByHash(hash);

        // assert
        Assert.Null(image);
    }

    [Fact]
    public async Task GetImageById_ImagesAvailable_ImageReturned()
    {
        // arrange
        string hash = "adsf";
        string mime = "image/jpeg";
        var image = await imageProvider.CreateNewImage(hash, mime, session);

        // act 
        var imageById = imageProvider.GetImageById(image.Id);

        // assert
        Assert.Equal(imageById, image);
    }

    [Fact]
    public void GetImageById_NoImagesAvailable_ExceptionThrown()
    {
        // arrange
        // act 
        var imageById = () => imageProvider.GetImageById("adsf");

        // assert
        Assert.Throws<InvalidOperationException>(imageById);
    }

    [Fact]
    public async Task UpdateImage_NoImagesAvailable_NothingUpdated()
    {
        // arrange
        var image = new Image
        {
            Id = "adfdd"
        };

        // act 
        await imageProvider.UpdateImage(image);

        // assert
        context.Verify(x => x.UpdateMeme(It.IsAny<Image>()), Times.Never());
    }

    [Fact]
    public async Task UpdateImage_NoImageWithIdAvailable_NothingUpdated()
    {
        // arrange
        var image = new Image
        {
            Id = "adfdd"
        };
        string hash = "adsf";
        string mime = "image/jpeg";
        await imageProvider.CreateNewImage(hash, mime, session);

        // act 
        await imageProvider.UpdateImage(image);

        // assert
        context.Verify(x => x.UpdateMeme(It.IsAny<Image>()), Times.Never());
    }

    [Fact]
    public async Task UpdateImage_ImageWithIdAvailable_ImageUpdated()
    {
        // arrange
        string hash = "adsf";
        string mime = "image/jpeg";
        var image = await imageProvider.CreateNewImage(hash, mime, session);
        image.Hash = "fdsa";

        // act 
        await imageProvider.UpdateImage(image);

        // assert
        context.Verify(x => x.UpdateMeme(image), Times.Once());
        Assert.Equal(imageProvider.Images.First(), image);
    }

    [Theory]
    [InlineData("image/jpeg", ".jpg")]
    [InlineData("image/png", ".png")]
    [InlineData("image/gif", ".gif")]
    [InlineData("video/mp4", "")]
    public void MimeTypeToExtension(string mimeType, string extension)
    {
        // arrange
        // act
        var val = imageProvider.MimeTypeToExtension(mimeType);

        // assert
        Assert.Equal(val, extension);
    }
}
