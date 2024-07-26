
using System;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Client;
using Sui.Transactions.Builder;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a paginated response containing a list of `string` objects
    /// that represent the resolved names associated with the inputted ID.
    /// </summary>
    public class NameServicePage
    {
        /// <summary>
        /// A list of `string` instances, each representing the associated name
        /// service associated with the inputted ID
        /// </summary>
        [JsonProperty("data")]
        public string[] Data { get; internal set; }

        /// <summary>
        /// An optional string representing the cursor for the next page of the name service.
        /// `null` if there are no more pages available.
        /// </summary>
        [JsonProperty("nextCursor")]
        public string NextCursor { get; internal set; }

        /// <summary>
        /// A `bool` value indicating whether there are more pages of name services available to be retrieved.
        /// `true` if there are more pages available, otherwise `false`.
        /// </summary>
        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; internal set; }
    }

    /// <summary>
    /// Represents a paginated response containing a list of `TransactionBlockResponsePage` instances,
    /// which may represent transaction blocks retrieved from the Sui blockchain, along with
    /// pagination information.
    /// </summary>
    [JsonObject]
    public class TransactionBlockResponsePage
    {
        /// <summary>
        /// A list of `TransactionBlockResponse` instances, each representing the response
        /// for a specific transaction block, possibly containing transaction data.
        /// </summary>
        [JsonProperty("data")]
        public TransactionBlockResponse[] Data { get; internal set; }

        /// <summary>
        /// An optional string representing the cursor for the next page of transaction blocks.
        /// `null` if there are no more pages available.
        /// </summary>
        [JsonProperty("nextCursor")]
        public string NextCursor { get; internal set; }

        /// <summary>
        /// A `bool` value indicating whether there are more pages of transaction blocks available to be retrieved.
        /// `true` if there are more pages available, otherwise `false`.
        /// </summary>
        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; internal set; }
    }

    /// <summary>
    /// The resulting values after execution of the transaction block.
    /// </summary>
    [JsonObject]
    public class TransactionBlockResponse
    {
        /// <summary>
        /// An optional `TransactionBlockEffects` representing the effects of the transaction.
        /// </summary>
        [JsonProperty("effects", NullValueHandling = NullValueHandling.Include)]
        public TransactionBlockEffects Effects { get; internal set; }

        /// <summary>
        /// An optional list of `Event` objects representing the events occurred during the transaction.
        /// </summary>
        [JsonProperty("events", NullValueHandling = NullValueHandling.Include)]
        public Event[] Events { get; internal set; }

        /// <summary>
        /// An optional list of `ObjectChange` representing the object changes occurred during the transaction.
        /// </summary>
        [JsonProperty("objectChanges", NullValueHandling = NullValueHandling.Include)]
        public ObjectChange[] ObjectChanges { get; internal set; }

        /// <summary>
        /// An optional list of `BalanceChange` representing the balance changes occurred during the transaction.
        /// </summary>
        [JsonProperty("balanceChanges", NullValueHandling = NullValueHandling.Include)]
        public BalanceChange[] BalanceChanges { get; internal set; }

        /// <summary>
        /// An optional `string` representing the timestamp of the transaction block response in milliseconds.
        /// </summary>
        [JsonProperty("timestampMs", NullValueHandling = NullValueHandling.Include)]
        public string TimestampMs { get; internal set; }

        /// <summary>
        /// A `string` representing the digest of the transaction block response.
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; internal set; }

        /// <summary>
        /// An optional `SuiTransactionBlock` representing the transaction block in the response.
        /// </summary>
        [JsonProperty("transaction", NullValueHandling = NullValueHandling.Include)]
        public SuiTransactionBlock Transaction { get; internal set; }
    }

    /// <summary>
    /// The class representing the Sui transaction block within a block response call
    /// </summary>
    [JsonObject]
    public class SuiTransactionBlock
    {
        /// <summary>
        /// A `SuiTransactionBlockData` object representing the data of the block
        /// </summary>
        [JsonProperty("data")]
        public SuiTransactionBlockData Data { get; internal set; }

        /// <summary>
        /// A list of `string` objects representing the transaction signatures of the transactions executed within the block.
        /// </summary>
        [JsonProperty("txSignature")]
        public string[] TransactionSignature { get; internal set; }
    }

    /// <summary>
    /// The class representing the data within the transaction block.
    /// </summary>
    [JsonObject]
    public class SuiTransactionBlockData
    {
        /// <summary>
        /// A `string` representing the message version of the transaction block data.
        /// </summary>
        [JsonProperty("messageVersion")]
        public string MessageVersion { get; internal set; }

        /// <summary>
        /// An `AccountAddress` representing the sender of the transaction block.
        /// </summary>
        [JsonProperty("sender")]
        public AccountAddress Sender { get; internal set; }

        /// <summary>
        /// A `GasData` representing the gas data associated with the transaction block.
        /// </summary>
        [JsonProperty("gasData")]
        public GasData GasData { get; internal set; }

        /// <summary>
        /// A `TransactionBlockKind` representing the kind or type of the transaction block.
        /// </summary>
        [JsonProperty("transaction")]
        public Transactions.Kinds.TransactionBlockKind Transaction { get; internal set; }
    }

    /// <summary>
    /// A class representing the results of Dev Inspect call.
    /// </summary>
    [JsonObject]
    public class DevInspectResponse
    {
        /// <summary>
        /// Summary of effects that likely would be generated if the
        /// transaction is actually run. Note however, that not all dev-
        /// inspect transactions are actually usable as transactions so it
        /// might not be possible actually generate these effects from a
        /// normal transaction.
        /// </summary>
        [JsonProperty("effects")]
        public TransactionBlockEffects Effects { get; internal set; }

        /// <summary>
        /// Events that likely would be generated if the transaction is actually run.
        /// </summary>
        [JsonProperty("events")]
        public Event[] Events { get; internal set; }

        /// <summary>
        /// The raw effects of the transaction that was dev inspected.
        /// </summary>
        [JsonProperty("rawEffects", NullValueHandling = NullValueHandling.Include)]
        public string[] RawEffects { get; internal set; }

        /// <summary>
        /// The raw transaction data that was dev inspected.
        /// </summary>
        [JsonProperty("rawTxnData", NullValueHandling = NullValueHandling.Include)]
        public string[] RawTransactionData { get; internal set; }

        /// <summary>
        /// An optional list of `ExecutionResultType` representing the results of the execution.
        /// This will be `null` if there are no execution results to represent.
        /// </summary>
        [JsonProperty("results", NullValueHandling = NullValueHandling.Include)]
        public ExecutionResultType[] Results { get; internal set; }

        /// <summary>
        /// An optional `string` representing any error that occurred during the inspection.
        /// This will be `null` if there is no error to report.
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Include)]
        public string Error { get; internal set; }
    }

    /// <summary>
    /// Represents the type of execution result, which includes mutable reference outputs
    /// and return values after the execution of a function or operation.
    /// </summary>
    [JsonObject]
    public class ExecutionResultType
    {
        /// <summary>
        /// An optional list of `MutableReferenceOutput` representing the mutable
        /// reference outputs obtained as a result of the execution. If `null`, it may indicate
        /// that there were no mutable reference outputs produced during the execution.
        /// </summary>
        [JsonProperty("mutableReferenceOutputs", NullValueHandling = NullValueHandling.Include)]
        public MutableReferenceOutput[] MutableReferenceOutputs { get; internal set; }

        /// <summary>
        /// An optional list of `ReturnValue` representing the return values obtained
        /// as a result of the execution. If `null`, it may indicate that there were no return
        /// values produced during the execution.
        /// </summary>
        [JsonProperty("returnValues", NullValueHandling = NullValueHandling.Include)]
        public ReturnValue[] ReturnValues { get; internal set; }
    }

    [JsonConverter(typeof(ReturnValueConverter))]
    public class ReturnValue : ReturnBase
    {
        public byte[] ObjectID { get; internal set; }

        public SuiStructTag Type { get; internal set; }

        public ReturnValue
        (
            byte[] object_id,
            SuiStructTag type
        )
        {
            this.ObjectID = object_id;
            this.Type = type;
        }

        public ReturnValue(SuiError error)
        {
            this.Error = error;
        }
    }

    [JsonConverter(typeof(MutableReferenceOutputConverter))]
    public class MutableReferenceOutput : ReturnBase
    {
        public string Name { get; internal set; }

        public byte[] ObjectID { get; internal set; }

        public SuiStructTag Type { get; internal set; }

        public MutableReferenceOutput
        (
            string name,
            byte[] object_id,
            SuiStructTag type
        )
        {
            this.Name = name;
            this.ObjectID = object_id;
            this.Type = type;
        }

        public MutableReferenceOutput(SuiError error)
        {
            this.Error = error;
        }
    }

    public class ReturnValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ReturnValue);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray return_value = (JArray)JToken.ReadFrom(reader);

                return new ReturnValue
                (
                    ((JArray)return_value[0]).Select(val => val.Value<byte>()).ToArray(),
                    new SuiStructTag(return_value[1].Value<string>())
                );
            }

            return new ReturnValue(new SuiError(0, "Unable to convert JSON to ReturnValue.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartArray();

                ReturnValue return_value = (ReturnValue)value;

                writer.WriteValue(return_value.ObjectID);
                writer.WriteValue(return_value.Type.ToString());

                writer.WriteEndArray();
            }
        }
    }

    public class MutableReferenceOutputConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(MutableReferenceOutput);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray mutable_reference_output = (JArray)JToken.ReadFrom(reader);

                return new MutableReferenceOutput
                (
                    mutable_reference_output[0].Value<string>(),
                    ((JArray)mutable_reference_output[1]).Select(val => val.Value<byte>()).ToArray(),
                    new SuiStructTag(mutable_reference_output[2].Value<string>())
                );
            }

            return new MutableReferenceOutput(new SuiError(0, "Unable to convert JSON to MutableReferenceOutput.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartArray();

                MutableReferenceOutput mutable_reference_output = (MutableReferenceOutput)value;

                writer.WriteValue(mutable_reference_output.Name);
                writer.WriteValue(mutable_reference_output.ObjectID);
                writer.WriteValue(mutable_reference_output.Type.ToString());

                writer.WriteEndArray();
            }
        }
    }
}