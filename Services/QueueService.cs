using Azure.Storage.Queues;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tentamen_Server_Side_Programming.Services.Interface;

namespace Tentamen_Server_Side_Programming.Services
{
    public class QueueService : IQueueService
    {
        private readonly QueueServiceClient _queueServiceClient;
        private QueueClient _queueClient;

        public QueueService()
        {
            _queueServiceClient = new QueueServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        }

        public void Init(string queueName)
        {
            _queueClient = _queueServiceClient.GetQueueClient(queueName);
            _queueClient.CreateIfNotExists();
        }

        public async Task SendStringAsync(string message)
        {
            try
            {
                var bytes = Encoding.UTF8.GetBytes(message);
                await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
            }
            catch (Exception ex)
            {

            }
        }


        public async Task SendObjectAsync<T>(T obj)
        {
            try
            {
                var serializedObject = JsonConvert.SerializeObject(obj);
                var bytes = Encoding.UTF8.GetBytes(serializedObject);
                await _queueClient.SendMessageAsync(Convert.ToBase64String(bytes));
            }
            catch (Exception ex)
            {
                
            }
        }
    }
}
