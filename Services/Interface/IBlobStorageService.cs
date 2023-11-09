using Azure.Storage.Blobs.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tentamen_Server_Side_Programming.Services.Interface
{
    public interface IBlobStorageService
    {
        Task InitializeAsync(string jobId);
        Task CreateContainerAsync();
        Task UploadBlobAsync(string blobName, byte[] blob);
        Task<IEnumerable<string>> GetBlobUrlsAsync();
        Task<string> GetBlobUrlByNameAsync(string blobName);
        Task<UserDelegationKey> RequestUserDelegationKeyAsync();
    }
}
