using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tentamen_Server_Side_Programming.Models.Weather
{
    public class StationMeasurement
    {
        [JsonProperty("stationid")]
        public int stationid { get; set; }

        [JsonProperty("stationname")]
        public string? stationname { get; set; }

        [JsonProperty("lat")]
        public double lat { get; set; }

        [JsonProperty("lon")]
        public double lon { get; set; }

        [JsonProperty("regio")]
        public string? regio { get; set; }

        [JsonProperty("timestamp")]
        public DateTime timestamp { get; set; }

        [JsonProperty("weatherdescription")]
        public string? weatherdescription { get; set; }

        [JsonProperty("winddirection")]
        public string? winddirection { get; set; }

        [JsonProperty("temperature")]
        public double temperature { get; set; }

        [JsonProperty("groundtemperature")]
        public double groundtemperature { get; set; }

        [JsonProperty("feeltemperature")]
        public double feeltemperature { get; set; }

        [JsonProperty("windgusts")]
        public double windgusts { get; set; }

        [JsonProperty("windspeed")]
        public double windspeed { get; set; }

        [JsonProperty("windspeedBft")]
        public int windspeedBft { get; set; }

        [JsonProperty("humidity")]
        public double humidity { get; set; }

        [JsonProperty("precipitation")]
        public double precipitation { get; set; }

        [JsonProperty("sunpower")]
        public double sunpower { get; set; }

        [JsonProperty("rainFallLast24Hour")]
        public double rainFallLast24Hour { get; set; }

        [JsonProperty("rainFallLastHour")]
        public double rainFallLastHour { get; set; }

        [JsonProperty("winddirectiondegrees")]
        public double winddirectiondegrees { get; set; }
    }
}
