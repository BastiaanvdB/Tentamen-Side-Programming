using System.Collections;
using System.Collections.Generic;
using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Tentamen_Server_Side_Programming.Models.Dtos;
using Tentamen_Server_Side_Programming.Models.Jobs;
using Tentamen_Server_Side_Programming.Services;
using Tentamen_Server_Side_Programming.Services.Interface;

namespace Tentamen_Server_Side_Programming.Triggers
{
    public class WeatherImageRetriever
    {
        private readonly ILogger _logger;
        private readonly IBlobStorageService _blobStorageService;
        private readonly ITableStorageService _tableStorageService;

        public WeatherImageRetriever(ILoggerFactory loggerFactory, IBlobStorageService blobStorageService, ITableStorageService tableStorageService)
        {
            _logger = loggerFactory.CreateLogger<WeatherImageRetriever>();
            _blobStorageService = blobStorageService;
            _tableStorageService = tableStorageService;
        }

        [Function("WeatherImageRetriever")]
        public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "WeatherImageRetriever/{id}")] HttpRequestData req, string id)
        {
            _logger.LogInformation("Weather Image Retriever HTTP trigger processed a request.");

            IEnumerable<StatusEntity> statusEntities = Enumerable.Empty<StatusEntity>();
            IEnumerable<string> blobUrls = Enumerable.Empty<string>();

            if (string.IsNullOrWhiteSpace(id))
            {
                return req.CreateResponse(HttpStatusCode.BadRequest);
            }

            // retrieving statuses with job id
            try
            {       
                statusEntities = await _tableStorageService.GetAllStatusEntitiesByPartitionKey(id);
            }catch (Exception ex)
            {
                _logger.LogError($"Failed to retrieve entities from table storage: {ex.Message}");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            // check if job status is still pending
            if (statusEntities.Any(se => se.Status == (int)Status.Pending))
            {
                var pendingResponse = req.CreateResponse(HttpStatusCode.NotFound);
                var message = new { id = id, Status = Status.Pending.ToString(), message = "The request for images is still pending." };
                await pendingResponse.WriteAsJsonAsync(message);

                return pendingResponse;
            }

            // Check if jobid is valid
            if (!statusEntities.Any())
            {
                _logger.LogInformation($"Invalid token used: {id}");

                var invalidBlobRequest = req.CreateResponse(HttpStatusCode.NotFound);
                var message = new { Message = "Invalid token" };
                await invalidBlobRequest.WriteAsJsonAsync(message);

                return invalidBlobRequest;
            }


            // initialize container and get all blob urls
            try
            {
                await _blobStorageService.InitializeAsync(id);
                blobUrls = await _blobStorageService.GetBlobUrlsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to use Blobstorage: {ex.Message}");
                return req.CreateResponse(HttpStatusCode.InternalServerError);
            }

            jobDTO jobResponse = new jobDTO(id);

            // sorting status entities and image bloburls
            foreach (StatusEntity statusEntity in statusEntities)
            {
                foreach (string blobUrl in blobUrls)
                {
                    string[] splittedBlobUrl = blobUrl.Split("%3A");
                    string[] splittedRowKey = statusEntity.RowKey.Split(':');

                    if (splittedBlobUrl[1] == splittedRowKey[1])
                    {
                        ImageDTO imageDTO = new ImageDTO
                        {
                            Regio = statusEntity.StationRegio,
                            StationName = statusEntity.StationName,
                            Status = ((Status)statusEntity.Status).ToString(),
                            Url = blobUrl
                        };
                        jobResponse.Images.Add(imageDTO);
                    }
                }
            }

            // sending response
            var response = req.CreateResponse(HttpStatusCode.OK);
            await response.WriteAsJsonAsync(jobResponse);

            return response;
        }
    }
}
