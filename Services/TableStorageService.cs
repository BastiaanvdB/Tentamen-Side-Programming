using Microsoft.WindowsAzure.Storage.Table;
using Microsoft.WindowsAzure.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tentamen_Server_Side_Programming.Services.Interface;
using static Azure.Core.HttpHeader;
using Tentamen_Server_Side_Programming.Models.Jobs;

namespace Tentamen_Server_Side_Programming.Services
{
    public class TableStorageService : ITableStorageService
    {
        private readonly CloudStorageAccount _storageAccount;
        protected CloudTableClient _tableClient;
        protected CloudTable _table;

        public TableStorageService()
        {
            _storageAccount = CloudStorageAccount.Parse(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
            _tableClient = _storageAccount.CreateCloudTableClient();

            _table = _tableClient.GetTableReference("ImageStatuses");
            _table.CreateIfNotExistsAsync().GetAwaiter().GetResult();
        }

        public async Task<StatusEntity> GetStatusByKeys(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException(nameof(partitionKey), "The partition key cannot be null or whitespace.");
            }
            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException(nameof(rowKey), "The row key cannot be null or whitespace.");
            }

            TableOperation retrieveOperation = TableOperation.Retrieve<StatusEntity>(partitionKey, rowKey);

            TableResult result = await _table.ExecuteAsync(retrieveOperation);

            StatusEntity statusEntity = result.Result as StatusEntity;

            return statusEntity;
        }

        public async Task<IEnumerable<StatusEntity>> GetAllStatusEntitiesByPartitionKey(string jobId)
        {
            if (string.IsNullOrWhiteSpace(jobId))
            {
                throw new ArgumentNullException(nameof(jobId), "The partition key cannot be null or whitespace.");
            }

            var query = new TableQuery<StatusEntity>().Where(TableQuery.GenerateFilterCondition("PartitionKey", QueryComparisons.Equal, jobId));

            var entities = new List<StatusEntity>();
            TableContinuationToken token = null;
            do
            {
                var queryResult = await _table.ExecuteQuerySegmentedAsync(query, token);
                entities.AddRange(queryResult.Results);
                token = queryResult.ContinuationToken;
            } while (token != null); 

            return entities;
        }

        public async Task CreateStatus(StatusEntity status)
        {
            TableOperation insertOperation = TableOperation.Insert(status);
            await _table.ExecuteAsync(insertOperation);
        }

        public async Task UpdateStatus(StatusEntity entityToUpdate)
        {
            if (entityToUpdate == null)
            {
                throw new ArgumentNullException(nameof(entityToUpdate), "The entity to update cannot be null.");
            }
            entityToUpdate.ETag = "*";
            TableOperation updateOperation = TableOperation.Replace(entityToUpdate);

            await _table.ExecuteAsync(updateOperation);
        }

        public async Task DeleteStatus(string partitionKey, string rowKey)
        {
            if (string.IsNullOrWhiteSpace(partitionKey))
            {
                throw new ArgumentNullException(nameof(partitionKey), "The partition key cannot be null or whitespace.");
            }
            if (string.IsNullOrWhiteSpace(rowKey))
            {
                throw new ArgumentNullException(nameof(rowKey), "The row key cannot be null or whitespace.");
            }

            TableOperation retrieveOperation = TableOperation.Retrieve<StatusEntity>(partitionKey, rowKey);
            TableResult retrieveResult = await _table.ExecuteAsync(retrieveOperation);
            var deleteEntity = retrieveResult.Result as StatusEntity;

            if (deleteEntity == null) return;

            TableOperation deleteOperation = TableOperation.Delete(deleteEntity);
            deleteEntity.ETag = "*"; 

            await _table.ExecuteAsync(deleteOperation);
        }
    }
}
