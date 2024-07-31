using System.Numerics;
using Newtonsoft.Json;
using Sui.Types;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class CoinPage
    {
        [JsonProperty("data")]
        public CoinDetails[] Data { get; internal set; }

        [JsonProperty("nextCursor", NullValueHandling = NullValueHandling.Include)]
        public AccountAddress NextCursor { get; internal set; }

        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; internal set; }
    }

    [JsonObject]
    public class CoinDetails
    {
        [JsonProperty("coinType")]
        public SuiStructTag CoinType { get; internal set; }

        [JsonProperty("coinObjectId")]
        public AccountAddress CoinObjectID { get; internal set; }

        [JsonProperty("version")]
        public BigInteger Version { get; internal set; }

        [JsonProperty("digest")]
        public string Digest { get; internal set; }

        [JsonProperty("balance")]
        public BigInteger Balance { get; internal set; }

        [JsonProperty("previousTransaction")]
        public string PreviousTransaction { get; internal set; }

        public ObjectData ToSuiObjectData()
            => new ObjectData
            (
                object_id: this.CoinObjectID,
                digest: this.Digest,
                previous_transaction: this.PreviousTransaction,
                version: this.Version
            );

        public Types.SuiObjectRef ToSuiObjectRef()
            => new Types.SuiObjectRef
            (
                this.CoinObjectID,
                this.Version,
                this.Digest
            );
    }
}