using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
	[JsonObject]
	public class ChildObject
	{
        [JsonProperty("objectId")]
		public string ObjectID { get; set; }

        [JsonProperty("sequenceNumber")]
        public string SequenceNumber { get; set; }
    }

    [JsonObject]
    public class ChildObjects
    {
        [JsonProperty("loadedChildObjects")]
        public ChildObject[] LoadedChildObjects { get; set; }
    }
}