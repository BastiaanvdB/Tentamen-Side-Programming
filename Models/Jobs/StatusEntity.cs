using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.WindowsAzure.Storage.Table;

namespace Tentamen_Server_Side_Programming.Models.Jobs
{
    public class StatusEntity : TableEntity
    {
        public int Status { get; set; }
        public string StationRegio { get; set; }
        public string StationName { get; set; }

        public StatusEntity()
        {
            
        }

        public StatusEntity(string jobId, Status status)
        {
            PartitionKey = jobId;
            RowKey = $"{jobId}:General";
            Status = (int)status;
            StationName = "General";
            StationRegio = "General";
        }

        public StatusEntity(string jobId, string stationName, string stationRegio, int stationId, Status status)
        {
            PartitionKey = jobId;
            RowKey = $"{jobId}:{stationId}";
            Status = (int)status;
            StationName = stationName;
            StationRegio = stationRegio;
        }
    }
}
