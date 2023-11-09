using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tentamen_Server_Side_Programming.Services.Interface;

namespace Tentamen_Server_Side_Programming.Services
{
    public class BlobStorageService : IBlobStorageService
    {
        private readonly BlobServiceClient _blobServiceClient;
        private BlobContainerClient _containerClient;
        private BlobClient _blobClient;

        public BlobStorageService()
        {
            _blobServiceClient = new BlobServiceClient(Environment.GetEnvironmentVariable("AzureWebJobsStorage"));
        }

        public async Task InitializeAsync(string jobId)
        {
            _containerClient = _blobServiceClient.GetBlobContainerClient(jobId);
        }


        public async Task CreateContainerAsync()
        {
            if (_containerClient == null)
                throw new InvalidOperationException("Blob container client is not initialized.");

            var response = await _containerClient.CreateIfNotExistsAsync();

            if (response != null)
            { 
                await _containerClient.SetAccessPolicyAsync(PublicAccessType.Blob);
            }
        }

        public async Task UploadBlobAsync(string blobName, byte[] blob)
        {
            if (_containerClient == null)
            {
                throw new InvalidOperationException("Blob container not initialized. Call InitBlobAsync first.");
            }

            _blobClient = _containerClient.GetBlobClient(blobName);
            using (var stream = new MemoryStream(blob))
            {
                await _blobClient.UploadAsync(stream, true);
            }
        }

        public async Task<IEnumerable<string>> GetBlobUrlsAsync()
        {
            if (_containerClient == null)
            {
                throw new InvalidOperationException("Blob container not initialized. Call InitBlobAsync first.");
            }

            var urls = new List<string>();
            await foreach (BlobItem blobItem in _containerClient.GetBlobsAsync())
            {
                urls.Add(_containerClient.GetBlobClient(blobItem.Name).Uri.ToString());
            }
            return urls;
        }

        public async Task<string> GetBlobUrlByNameAsync(string blobName)
        {
            if (_containerClient == null)
            {
                throw new InvalidOperationException("Blob container not initialized. Call InitializeAsync first.");
            }

            _blobClient = _containerClient.GetBlobClient(blobName);

            bool exists = await _blobClient.ExistsAsync();
            if (!exists)
            {
                throw new InvalidOperationException($"Blob with name {blobName} does not exist.");
            }

            return _blobClient.Uri.ToString();
        }

        public async Task<UserDelegationKey> RequestUserDelegationKeyAsync()
        {
            return await _blobServiceClient.GetUserDelegationKeyAsync(
                DateTimeOffset.UtcNow,
                DateTimeOffset.UtcNow.AddDays(1));
        }
    }
}
