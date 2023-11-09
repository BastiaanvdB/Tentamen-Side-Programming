using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tentamen_Server_Side_Programming.Models.Dtos
{
    public class jobDTO
    {
        [JsonProperty("id")]
        public string? Id { get; set; }

        [JsonProperty("images", NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
        public List<ImageDTO>? Images { get; set; }

        public jobDTO(string id)
        {
            Id = id;
            Images = new List<ImageDTO>();
        }
    }
}
