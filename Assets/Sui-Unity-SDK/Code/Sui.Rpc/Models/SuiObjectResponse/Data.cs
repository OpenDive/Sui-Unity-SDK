using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    public abstract class Data
    {
        [JsonProperty("dataType")]
        public string DataType { get; set; }
    }

    public class MoveObjectData : Data
    {
        [JsonProperty("fields")]
        [JsonConverter(typeof(MoveStructJsonConverter))]
        public MoveStruct Fields { get; set; }

        [JsonProperty("hasPublicTransfer")]
        public bool HasPublicTransfer { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        public T ConvertFieldsTo<T>() where T : class
        {
            return Fields.ToObject<T>();
        }
    }

    public class PackageData : Data
    {
        [JsonProperty("disassembled")]
        public Dictionary<string, object> Disassembled { get; set; }
    }
}