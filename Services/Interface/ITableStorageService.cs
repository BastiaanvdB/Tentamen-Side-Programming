using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tentamen_Server_Side_Programming.Models.Jobs;

namespace Tentamen_Server_Side_Programming.Services.Interface
{
    public interface ITableStorageService
    {
        Task CreateStatus(StatusEntity status);

        Task<StatusEntity> GetStatusByKeys(string partitionKey, string rowKey);
        Task<IEnumerable<StatusEntity>> GetAllStatusEntitiesByPartitionKey(string jobId);

        Task UpdateStatus(StatusEntity status);
        Task DeleteStatus(string partitionKey, string rowKey);
    }
}
