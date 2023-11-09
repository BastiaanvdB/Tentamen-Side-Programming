using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tentamen_Server_Side_Programming.Models.Weather;

namespace Tentamen_Server_Side_Programming.Models.Jobs
{
    public class QueueJob
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("stationMeasurement", NullValueHandling = NullValueHandling.Ignore)]
        public StationMeasurement? StationMeasurement { get; set; }
    }
}
