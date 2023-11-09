using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tentamen_Server_Side_Programming.Models.Jobs;
using Tentamen_Server_Side_Programming.Models.Weather;
using Tentamen_Server_Side_Programming.Services;
using Tentamen_Server_Side_Programming.Services.Interface;

namespace Tentamen_Server_Side_Programming.Triggers
{
    public class ImageTextWriteQueue
    {
        private readonly ILogger<ImageTextWriteQueue> _logger;
        private readonly IImageWriteService _imageWriteService;
        private readonly IImageService _imageService;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ITableStorageService _tableStorageService;

        public ImageTextWriteQueue(ILogger<ImageTextWriteQueue> logger, IImageWriteService imageWriteService, IImageService imageService, IBlobStorageService blobStorageService, ITableStorageService tableStorageService)
        {
            _logger = logger;
            _imageWriteService = imageWriteService;
            _imageService = imageService;
            _blobStorageService = blobStorageService;
            _tableStorageService = tableStorageService;
        }

        [Function(nameof(ImageTextWriteQueue))]
        public async Task RunAsync([QueueTrigger("image-write-requests", Connection = "AzureWebJobsStorage")] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                _logger.LogError("Received a null or empty message.");
                return;
            }

            QueueJob incomingJob;
            try
            {
                incomingJob = JsonConvert.DeserializeObject<QueueJob>(message);
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Failed to deserialize message. Error: {ex.Message}");
                return;
            }


            _logger.LogInformation($"Image Write Queue trigger processed: {incomingJob.Id}");

            // Download image
            byte[] image = await _imageService.GetImageAsync();

            // Write text on image
            byte[] imageWithText = _imageWriteService.DrawImage(image, incomingJob.StationMeasurement);

            // Storing it in blob storage
            await _blobStorageService.InitializeAsync(incomingJob.Id);

            // Creating container
            await _blobStorageService.CreateContainerAsync();

            // Uploading to container
            await _blobStorageService.UploadBlobAsync($"{incomingJob.Id}:{incomingJob.StationMeasurement.stationid}", imageWithText);


            // Update the statuses
            try
            {
                StatusEntity statusEntity = await _tableStorageService.GetStatusByKeys(incomingJob.Id, $"{incomingJob.Id}:{incomingJob.StationMeasurement.stationid}");
                statusEntity.Status = (int)Status.Processed;
                await _tableStorageService.UpdateStatus(statusEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to push to table storage: {ex.Message}");
                return;
            }

        }
    }
}
