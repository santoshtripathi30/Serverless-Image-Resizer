using ImageResizeFunction.Services;

using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using System;
using System.IO;

namespace ImageResizeFunction
{

    [StorageAccount("AzureWebJobsStorage")]

    public class ImageResizeFunction
    {
        private readonly IImageResizer _imageResizer;
        public ImageResizeFunction(IImageResizer imageResizer)
        {
            _imageResizer = imageResizer;
        }

        [FunctionName("ImageResizeFunction")]
        public void Run([BlobTrigger("normal-size/{name}", Connection = "")] Stream inputBlob,
            [Blob("reduced-size/{name}", FileAccess.Write)] Stream outputBlob,
            string name, ILogger log)
        {
            log.LogInformation($"Blob trigger function started blob\n Name:{name} \n Size: {inputBlob.Length} Bytes");

            try
            {
                this._imageResizer.Resize(inputBlob, outputBlob);
                log.LogInformation($"Blob trigger function Processed blob\n Name:{name}");

            }
            catch (Exception e)
            {
                log.LogError("Resize fail ", e);
            }



        }
    }
}
