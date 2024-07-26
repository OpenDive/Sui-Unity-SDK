using System.Numerics;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a paginated response containing a list of `ObjectDataResponse` instances,
    /// which may represent objects retrieved from the Sui Network, along with
    /// pagination information.
    /// </summary>
    [JsonObject]
    public class PaginatedObjectsResponse
    {
        /// <summary>
        /// A list of `ObjectDataResponse` instances, each representing the response
        /// for a specific object, possibly containing object data or errors.
        /// </summary>
        [JsonProperty("data")]
        public ObjectDataResponse[] Data { get; internal set; }

        /// <summary>
        /// A `bool` value indicating whether there are more pages of objects available to be retrieved.
        /// `true` if there are more pages available, otherwise `false`.
        /// </summary>
        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; internal set; }

        /// <summary>
        /// An optional string representing the cursor for the next page of objects.
        /// `null` if there are no more pages available.
        /// </summary>
        [JsonProperty("nextCursor", NullValueHandling = NullValueHandling.Include)]
        public string NextCursor { get; internal set; }
    }

    /// <summary>
    /// A structure representing the response from a request for an `ObjectData`,
    /// containing either the object data or an error.
    /// </summary>
    [JsonObject]
    public class ObjectDataResponse
    {
        /// <summary>
        /// An optional `ObjectData` representing the data of the requested SuiObject, if the request was successful.
        /// </summary>
        [JsonProperty("data", NullValueHandling = NullValueHandling.Include)]
        public ObjectData Data { get; internal set; }

        /// <summary>
        /// An optional `ObjectResponseError` representing any error that occurred during the request for the SuiObject.
        /// </summary>
        [JsonProperty("error", NullValueHandling = NullValueHandling.Include)]
        public ObjectResponseError Error { get; internal set; }

        /// <summary>
        /// Retrieves the initial version of the shared object, if applicable.
        /// This method will return `null` if the `data` is `null` or if the owner of the object is not shared.
        /// </summary>
        /// <returns>An optional `int` representing the initial version of the shared object.</returns>
        public int? GetSharedObjectInitialVersion()
        {
            if (this.Data == null)
                return null;

            Owner owner = this.Data.Owner;

            if (owner == null || owner.Shared == null)
                return null;

            return owner.Shared.InitialSharedVersion;
        }

        /// <summary>
        /// Constructs and retrieves a `SuiObjectRef` reference from the object data, if available.
        /// This method will return `null` if the `data` is `null`.
        /// </summary>
        /// <returns>An optional `SuiObjectRef` representing the object reference constructed from the object data.</returns>
        public Sui.Types.SuiObjectRef GetObjectReference()
        {
            if (this.Data == null || this.Data.Version == null)
                return null;

            return new Sui.Types.SuiObjectRef
            (
                this.Data.ObjectID,
                this.Data.Version,
                this.Data.Digest
            );
        }
    }

    /// <summary>
    /// A structure representing ObjectData,
    /// containing various information about an object on the Sui Network.
    /// </summary>
    [JsonObject]
    public class ObjectData
    {
        /// <summary>
        /// A `string` representing the object ID.
        /// </summary>
        [JsonProperty("objectId")]
        public AccountAddress ObjectID { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the version of the object.
        /// </summary>
        [JsonProperty("version", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? Version { get; internal set; }

        /// <summary>
        /// A `string` representing the digest of the object.
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; internal set; }

        /// <summary>
        /// An optional `string` representing the type of the object.
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Include)]
        public SuiStructTag Type { get; internal set; }

        /// <summary>
        /// An optional `Owner` representing the owner of the object.
        /// </summary>
        [JsonProperty("owner", NullValueHandling = NullValueHandling.Include)]
        public Owner Owner { get; internal set; }

        /// <summary>
        /// An optional `string` representing the previous transaction of the object.
        /// </summary>
        [JsonProperty("previousTransaction", NullValueHandling = NullValueHandling.Include)]
        public string PreviousTransaction { get; internal set; }

        /// <summary>
        /// An optional `BigInteger` representing the storage rebate of the object.
        /// </summary>
        [JsonProperty("storageRebate", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? StorageRebate { get; internal set; }

        /// <summary>
        /// An optional `Data` representing the parsed data of the object.
        /// </summary>
        [JsonProperty("content", NullValueHandling = NullValueHandling.Include)]
        public Data Content { get; internal set; }

        /// <summary>
        /// An optional `RawData` representing the Binary Canonical Serialization (BCS) of the object.
        /// </summary>
        [JsonProperty("bcs", NullValueHandling = NullValueHandling.Include)]
        public RawData BCS { get; internal set; }

        /// <summary>
        /// An optional `DisplayFieldsResponse` representing the display fields of the object.
        /// </summary>
        [JsonProperty("display", NullValueHandling = NullValueHandling.Include)]
        public DisplayFieldsResponse Display { get; internal set; }

        public ObjectData
        (
            AccountAddress object_id,
            string digest,
            BigInteger? version = null,
            SuiStructTag type = null,
            Owner owner = null,
            string previous_transaction = null,
            BigInteger? storage_rebate = null,
            RawData bcs = null,
            Data content = null,
            DisplayFieldsResponse display = null
        )
        {
            this.ObjectID = object_id;
            this.Digest = digest;
            this.Version = version;
            this.Type = type;
            this.Owner = owner;
            this.PreviousTransaction = previous_transaction;
            this.StorageRebate = storage_rebate;
            this.BCS = bcs;
            this.Content = content;
            this.Display = display;
        }
    }
}