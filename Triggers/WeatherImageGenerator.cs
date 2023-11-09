using System.Collections.Generic;
using System.IO.Pipes;
using System.Net;
using System.Text;
using Azure.Storage.Queues;
using Azure.Storage.Queues.Models;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tentamen_Server_Side_Programming.Models.Jobs;
using Tentamen_Server_Side_Programming.Services;
using Tentamen_Server_Side_Programming.Services.Interface;

namespace Tentamen_Server_Side_Programming.Triggers
{
    public class WeatherImageGenerator
    {
        private readonly ILogger _logger;
        private readonly IQueueService _queueService;
        private readonly ITableStorageService _tableStorageService;
        private const string StaticUrl = "http://localhost:8080/api/WeatherImageRetriever";

        public WeatherImageGenerator(ILoggerFactory loggerFactory, IQueueService queueService, ITableStorageService tableStorageService)
        {
            _logger = loggerFactory.CreateLogger<WeatherImageGenerator>();
            _queueService = queueService;
            _tableStorageService = tableStorageService;
        }

        [Function("WeatherImageGenerator")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Function, "get")] HttpRequestData req)
        {
            _logger.LogInformation("Weather Image Generator HTTP trigger processed a request.");

            string _connectionString = Environment.GetEnvironmentVariable("QueueStorageConnectionString");

            // init a QueueClient
            _queueService.Init("image-requests");
            
            QueueJob queueJob = new QueueJob();

            queueJob.Id = Guid.NewGuid().ToString();

            // Sending the message to queue
            await _queueService.SendObjectAsync(queueJob);

            // Creating status in Table 
            try
            {
                StatusEntity statusEntity = new StatusEntity(queueJob.Id, Status.Pending);
                await _tableStorageService.CreateStatus(statusEntity);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to push to table storage: {ex.Message}");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            // Creating a response object
            var jobResponse = new
            {
                id = queueJob.Id,
                url = GetUrl(queueJob.Id)
            };

            // Serialize the response object to JSON
            var jsonResponse = JsonConvert.SerializeObject(jobResponse);

            // Creating and set up the response
            var response = req.CreateResponse(HttpStatusCode.OK);
            response.Headers.Add("Content-Type", "application/json; charset=utf-8");
            response.WriteString(jsonResponse);

            return response;
        }

        public static string GetUrl(string id)
        {
            var hostName = Environment.GetEnvironmentVariable("WEBSITE_HOSTNAME");
            var scheme = "https";

            return $"{scheme}://{hostName}/api/WeatherImageRetriever/{id}";
        }
    }
}
