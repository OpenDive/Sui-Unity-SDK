using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a paginated list of Sui Move Events.
    /// </summary>
    [JsonObject]
	public class EventPage
	{
        /// <summary>
        /// A list of `Event` objects representing the events in the current page.
        /// </summary>
        [JsonProperty("data")]
		public Event[] Data { get; internal set; }

        /// <summary>
        /// An `EventId` object representing the cursor to the next page of events.
        /// </summary>
        [JsonProperty("nextCursor")]
        public EventId NextCursor { get; internal set; }

        /// <summary>
        /// A `bool` value indicating whether there is a next page of events available.
        /// </summary>
        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; internal set; }
    }

    /// <summary>
    /// A move event that happens on the Sui Network.
    /// </summary>
	[JsonObject]
	public class Event
	{
        /// <summary>
        /// The unique identifier of the event.
        /// </summary>
		[JsonProperty("id")]
		public EventId ID { get; internal set; }

        /// <summary>
        /// The identifier of the package associated with the event.
        /// </summary>
		[JsonProperty("packageId")]
		public AccountAddress PackageID { get; internal set; }

        /// <summary>
        /// A `string` representing the module associated with the transaction.
        /// </summary>
		[JsonProperty("transactionModule")]
		public string TransactionModule { get; internal set; }

        /// <summary>
        /// An `AccountAddress` representing the sender of the event.
        /// </summary>
		[JsonProperty("sender")]
		public AccountAddress Sender { get; internal set; }

        /// <summary>
        /// A `string` representing the type of the event.
        /// </summary>
		[JsonProperty("type")]
		public string Type { get; internal set; }

        /// <summary>
        /// A `JObject` object representing the parsed JSON associated with the event.
        /// </summary>
		[JsonProperty("parsedJson")]
		public JObject ParsedJson { get; internal set; }

        /// <summary>
        /// An optional `string` representing the BCS (Binary Canonical Serialization) of the event.
        /// </summary>
		[JsonProperty("bcs", NullValueHandling = NullValueHandling.Include)]
		public string BCS { get; internal set; }

        /// <summary>
        /// An optional `string` representing the timestamp of the event in milliseconds.
        /// </summary>
		[JsonProperty("timestampMs", NullValueHandling = NullValueHandling.Include)]
		public string TimestampMs { get; internal set; }
    }

    /// <summary>
    /// Represents a unique identifier for an event, typically associated with a transaction.
    /// </summary>
    [JsonObject]
	public class EventId
	{
        /// <summary>
        /// The digest of the transaction.
        /// </summary>
        [JsonProperty("txDigest")]
		public string TransactionDigest { get; internal set; }

        /// <summary>
        /// The sequence number of the event.
        /// </summary>
        [JsonProperty("eventSeq")]
        public string EventSequence { get; internal set; }
    }
}
