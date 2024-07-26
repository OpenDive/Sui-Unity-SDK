using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a paginated collection of `DynamicFieldInfo` objects, providing information
    /// on the availability of additional pages.
    /// </summary>
    [JsonObject]
    public class DynamicFieldPage
    {
        /// <summary>
        /// A list of `DynamicFieldInfo` objects, each providing detailed information
        /// about a specific dynamic field.
        /// </summary>
        [JsonProperty("data")]
        public DynamicFieldInfo[] Data { get; internal set; }

        /// <summary>
        /// An optional `string` representing the cursor for the next page of dynamic field information.
        /// If `null`, it may indicate that there are no more pages available.
        /// </summary>
        [JsonProperty("nextCursor", NullValueHandling = NullValueHandling.Include)]
        public string NextCursor { get; internal set; }

        /// <summary>
        /// A `bool` indicating whether there is an additional page of dynamic field information available.
        /// </summary>
        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; internal set; }
    }

    /// <summary>
    /// A structure representing information about a dynamic field.
    /// </summary>
    [JsonObject]
    public class DynamicFieldInfo
    {
        /// <summary>
        /// An instance of `DynamicFieldName` representing the name of the dynamic field.
        /// </summary>
        [JsonProperty("name")]
        public DynamicFieldName Name { get; internal set; }

        /// <summary>
        /// A `string` representing the BCS (Binary Canonical Serialization) name of the dynamic field.
        /// </summary>
        [JsonProperty("bcsName")]
        public string BCSName { get; internal set; }

        /// <summary>
        /// An instance of `string` representing the type of the dynamic field.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; internal set; }

        /// <summary>
        /// A `string` representing the type of the object associated with the dynamic field.
        /// </summary>
        [JsonProperty("objectType")]
        public string ObjectType { get; internal set; }

        /// <summary>
        /// A `string` representing the object ID associated with the dynamic field.
        /// </summary>
        [JsonProperty("objectId")]
        public AccountAddress ObjectID { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the version of the dynamic field.
        /// </summary>
        [JsonProperty("version")]
        public BigInteger Version { get; internal set; }

        /// <summary>
        /// A `string` representing the digest of the dynamic field.
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; internal set; }
    }

    /// <summary>
    /// A structure representing the name of a dynamic field.
    /// </summary>
    [JsonObject]
    public class DynamicFieldName
    {
        /// <summary>
        /// A `string` representing the type of dynamic field.
        /// This can help in identifying the nature or category of the field.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; internal set; }

        /// <summary>
        /// A `JValue` representing the actual name or value of the dynamic field.
        /// This is the specific identifier for the field.
        /// </summary>
        [JsonProperty("value")]
        public JValue Value { get; internal set; }

        /// <summary>
        /// Converts an output Dynamic Field Name to an input for the RPC client.
        /// </summary>
        /// <returns>A `DynamicFieldInput` value.</returns>
        public DynamicFieldNameInput ToInput()
            => new DynamicFieldNameInput(this.Type, this.Value.Value.ToString());
    }

    /// <summary>
    /// A structure representing the name of a dynamic field as an input.
    /// </summary>
    [JsonObject]
    public class DynamicFieldNameInput
    {
        /// <summary>
        /// A `string` representing the type of dynamic field.
        /// This can help in identifying the nature or category of the field.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; set; }

        /// <summary>
        /// A `string` representing the actual name or value of the dynamic field.
        /// This is the specific identifier for the field.
        /// </summary>
        [JsonProperty("value")]
        public string Value { get; set; }

        public DynamicFieldNameInput
        (
            string type,
            string value
        )
        {
            this.Type = type;
            this.Value = value;
        }
    }
}