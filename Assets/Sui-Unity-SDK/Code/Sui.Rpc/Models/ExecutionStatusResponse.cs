using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Sui.Rpc.Models
{
    public enum ExecutionStatus
    {
        Success,
        Failure,
    }

    [JsonObject]
    public class ExecutionStatusResponse
    {
        [JsonProperty("status")]
        [JsonConverter(typeof(StringEnumConverter))]
        public ExecutionStatus Status { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }
    }
}