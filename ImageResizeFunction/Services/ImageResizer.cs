using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

using System;
using System.IO;

namespace ImageResizeFunction.Services
{
    public class ImageResizer : IImageResizer
    {
        public void Resize(Stream input, Stream output)
        {
            try
            {
                using (Image image = Image.Load(input))
                {

                    // Ensure new dimensions are at least 1 pixel
                    int newWidth = Math.Max(1, image.Width / 2);
                    int newHeight = Math.Max(1, image.Height / 2);

                    image.Mutate(x => x.Resize(newWidth, newHeight));
                    image.Save(output, new JpegEncoder());

                }
            }
            catch (Exception )
            {
             
                throw;
            }

        }
    }

}
