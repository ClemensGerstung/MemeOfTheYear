using Grpc.Core;
using MemeOfTheYear.Backend.Database;

namespace MemeOfTheYear.Backend.Server
{
    public class ImageServer : ImageService.ImageServiceBase
    {
        private readonly IMemeOfTheYearContext _context;

        public ImageServer(IMemeOfTheYearContext context)
        {
            _context = context;
        }

        public override async Task<GetImageResponse> GetImage(GetImageRequest request, ServerCallContext context)
        {
            if (!await _context.CheckSession(request.SessionId))
            {
                throw new Exception($"unknown session {request.SessionId}");
            }

            var imageId = request.ImageId;
            if (string.IsNullOrEmpty(request.ImageId))
            {
                imageId = "default";
            }

            var content = await _context.GetImageData(imageId);

            return new GetImageResponse { ImageContent = content };
        }

        public override async Task<GetImagesResponse> GetAllImages(GetImagesRequest request, ServerCallContext context)
        {
            if (!await _context.CheckSession(request.SessionId))
            {
                throw new Exception($"unknown session {request.SessionId}");
            }

            var result = await _context.GetAllImages();

            var response = new GetImagesResponse();
            response.ImageIds.AddRange(result.Select(x => x.Id));

            return response;
        }
    }
}