using MemeOfTheYear.Types;
using Microsoft.Extensions.Logging;

namespace MemeOfTheYear.Providers
{
    public interface IResultProvider
    {
        List<Image> GetMostVotedImages(int count);
    }
}