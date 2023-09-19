using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Sui.Rpc
{
    [JsonObject(NamingStrategyType = typeof(CamelCaseNamingStrategy),
        ItemNullValueHandling = NullValueHandling.Ignore)]
    public abstract class IRequest
    {
        public string Jsonrpc { get => "2.0"; }

        public int Id { get; set; }
    }
}