using Grpc.Core;
using MemeOfTheYear.Remote;
using MemeOfTheYear.Providers;

namespace MemeOfTheYear.Services
{
    public class AdminServiceImpl(
        ISessionProvider sessionProvider,
        IImageProvider imageProvider
    ) : AdminService.AdminServiceBase
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
}