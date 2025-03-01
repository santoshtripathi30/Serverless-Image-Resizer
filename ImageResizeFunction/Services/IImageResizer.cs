using Microsoft.Extensions.Logging;

using System.IO;

namespace ImageResizeFunction.Services
{
    public interface IImageResizer
    {
        void Resize(Stream input, Stream output);

    }
}
