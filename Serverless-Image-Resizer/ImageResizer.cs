using Azure.Storage.Blobs;

using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Processing;

using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;


namespace Serverless_Image_Resizer
{
    public class ImageResizer
    {
        private readonly BlobServiceClient _blobServiceClient;
        private readonly ILogger<ImageResizer> _logger;
        public ImageResizer(BlobServiceClient blobServiceClient, ILogger<ImageResizer> logger)
        {
            _blobServiceClient = blobServiceClient;
            _logger = logger;
        }

        private readonly Dictionary<string, int> ImageSizes = new Dictionary<string, int>
        {
            { "thumbnail", 100 },
            { "medium", 500 },
            { "large", 1000 }
        };


        [Function("ImageResizer")]
        public async Task Run(
         [BlobTrigger("incoming-images/{name}", Connection = "AzureWebJobsStorage")] Stream imageStream,
         string name)
        {
            _logger.LogInformation($"Processing uploaded image: {name}");

            foreach (var size in ImageSizes)
            {
                await ResizeAndSaveImage(imageStream, name, size.Key, size.Value);
            }

            _logger.LogInformation($"Image {name} resized and saved successfully.");
        }

        private async Task ResizeAndSaveImage(Stream imageStream, string fileName, string sizeLabel, int width)
        {
            try
            {
                imageStream.Position = 0; // Reset stream position

                using var image = await Image.LoadAsync(imageStream);
                image.Mutate(x => x.Resize(width, 0));

                using var outputStream = new MemoryStream();
                await image.SaveAsync(outputStream, new JpegEncoder());
                outputStream.Position = 0;

                var outputContainer = _blobServiceClient.GetBlobContainerClient("resized-images");
                var outputBlob = outputContainer.GetBlobClient($"{sizeLabel}/{fileName}");

                await outputBlob.UploadAsync(outputStream, overwrite: true);

                _logger.LogInformation($"Resized image ({sizeLabel}) saved as {fileName}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error resizing {fileName}: {ex.Message}");
            }
        }
    }

}
