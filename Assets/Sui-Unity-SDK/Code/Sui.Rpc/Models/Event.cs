using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Sui.Rpc.Models
{
	[JsonObject]
	public class Event
	{
		[JsonProperty("id")]
		public EventId ID { get; set; }

		[JsonProperty("packageId")]
		public string PackageID { get; set; }

		[JsonProperty("transactionModule")]
		public string TransactionModule { get; set; }

		[JsonProperty("sender")]
		public string Sender { get; set; }

		[JsonProperty("type")]
		public string Type { get; set; }

		[JsonProperty("parsedJson")]
		public JObject ParsedJson { get; set; }

	[JsonProperty("bcs", Required = Required.Default)]
		public string BCS { get; set; }

		[JsonProperty("timestampMs", Required = Required.Default)]
		public string TimestampMs { get; set; }
    }

	[JsonObject]
	public class EventId
	{
		[JsonProperty("txDigest")]
		public string TransactionDigest { get; set; }

        [JsonProperty("eventSeq")]
        public string EventSequence { get; set; }
    }
}
