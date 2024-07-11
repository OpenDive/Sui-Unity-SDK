using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    public class DisplayFieldsResponse
    {
        [JsonProperty("data")]
        public Dictionary<string, string> Data { get; set; }

        [JsonProperty("error")]
        public ObjectResponseError Error { get; set; }
    }
}