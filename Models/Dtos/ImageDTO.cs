using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tentamen_Server_Side_Programming.Models.Jobs;

namespace Tentamen_Server_Side_Programming.Models.Dtos
{
    public class ImageDTO
    {
        [JsonProperty("region")]
        public string? Regio {  get; set; }

        [JsonProperty("stationName")]
        public string? StationName { get; set; }

        [JsonProperty("imageStatus")]
        public string? Status { get; set; }

        [JsonProperty("url", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public string? Url {  get; set; }
    }
}
