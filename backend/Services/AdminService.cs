using Grpc.Core;
using MemeOfTheYear;

public class AdminService(
    ISessionProvider sessionProvider,
    IImageProvider imageProvider
) : MemeOfTheYear.AdminService.AdminServiceBase
{
    public override async Task<SetImageEnabledResponse> SetImageEnabled(SetImageEnabledRequest request, ServerCallContext context)
    {
        if (!sessionProvider.IsAllowed(request.SessionId))
        {
            throw new InvalidOperationException("Session not authenticated!");
        }

        var image = imageProvider.GetImageById(request.ImageId);
        image.IsEnabled = request.Enabled;

        await imageProvider.UpdateImage(image);

        return new SetImageEnabledResponse();
    }
}