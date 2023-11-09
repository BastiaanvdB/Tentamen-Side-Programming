using System;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tentamen_Server_Side_Programming.Models.Jobs;
using Tentamen_Server_Side_Programming.Models.Weather;
using Tentamen_Server_Side_Programming.Services.Interface;

namespace Tentamen_Server_Side_Programming.Triggers
{
    public class ImageRequestQueue
    {
        private readonly ILogger<ImageRequestQueue> _logger;
        private readonly IWeatherForecastService _weatherForecastService;
        private readonly IQueueService _queueService;
        private readonly ITableStorageService _tableStorageService;

        public ImageRequestQueue(ILogger<ImageRequestQueue> logger, IWeatherForecastService weatherForecastService, IQueueService queueService, ITableStorageService tableStorageService)
        {
            _logger = logger;
            _weatherForecastService = weatherForecastService;
            _queueService = queueService;
            _tableStorageService = tableStorageService;
        }

        [Function(nameof(ImageRequestQueue))]
        public async Task RunAsync([QueueTrigger("image-requests", Connection = "AzureWebJobsStorage")] string message)
        {
            if (string.IsNullOrEmpty(message))
            {
                _logger.LogError("Received a null or empty message.");
                return;
            }

            QueueJob incomingJob;

            // deserialize json to Queueobject
            try
            {
                incomingJob = JsonConvert.DeserializeObject<QueueJob>(message);
            }
            catch (JsonException ex)
            {
                _logger.LogError($"Failed to deserialize message. Error: {ex.Message}");
                return;
            }

            _logger.LogInformation($"Image Request Queue trigger processed: {incomingJob.Id}");

            // Init Queue
            _queueService.Init("image-write-requests");

            List<StationMeasurement> stationMeasurements;
            try
            {
                stationMeasurements = await _weatherForecastService.GetForecast();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to fetch station measurements. Error: {ex.Message}");
                return;
            }

            // deleting general status and creating individual statuses
            try
            {
                await _tableStorageService.DeleteStatus(incomingJob.Id, $"{incomingJob.Id}:General");

                foreach (StationMeasurement measurement in stationMeasurements)
                {
                    StatusEntity statusEntity = new StatusEntity(incomingJob.Id, measurement.stationname, measurement.regio, measurement.stationid, Status.Processing);
                    await _tableStorageService.CreateStatus(statusEntity);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to push changes to the table storage: {ex.Message}");
                return;
            }

            foreach (StationMeasurement measurement in stationMeasurements)
            {
                QueueJob newJob = new QueueJob
                {
                    Id = incomingJob.Id,
                    StationMeasurement = measurement
                };

                try
                {
                    await _queueService.SendObjectAsync(newJob);
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to send job to the queue. Job ID: {newJob.Id}. Error: {ex.Message}");
                }
            }
        }
    }
}
