using Grpc.Core;

namespace MemeOfTheYear
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
            var imageId = request.ImageId;
            if(string.IsNullOrEmpty(request.ImageId)) {
                imageId = "default";
            }

            var content = await _context.GetImageData(imageId);

            return new GetImageResponse { ImageContent = content };
        }

        public override async Task<GetVotedImagesResponse> GetMostLikedImages(GetVotedImagesRequest request, ServerCallContext context)
        {
            var result = await _context.GetMostLikedImages(request.Count);
            var response = new GetVotedImagesResponse();
            response.Entries.AddRange(result.Select(x => new global::VoteEntry
            {
                ImageId = x.Image.Id,
                Votes = x.Votes
            }));

            return response;
        }

        public override async Task<GetVotedImagesResponse> GetMostDislikedImages(GetVotedImagesRequest request, ServerCallContext context)
        {
            var result = await _context.GetMostDislikedImages(request.Count);
            var response = new GetVotedImagesResponse();
            response.Entries.AddRange(result.Select(x => new global::VoteEntry
            {
                ImageId = x.Image.Id,
                Votes = x.Votes
            }));

            return response;
        }

        public override async Task<GetVotedImagesResponse> GetMostSkippedImages(GetVotedImagesRequest request, ServerCallContext context)
        {
            var result = await _context.GetMostSkippedImages(request.Count);
            var response = new GetVotedImagesResponse();
            response.Entries.AddRange(result.Select(x => new global::VoteEntry
            {
                ImageId = x.Image.Id,
                Votes = x.Votes
            }));

            return response;
        }
    }
}