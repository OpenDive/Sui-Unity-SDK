using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class CoinPage
    {
        [JsonProperty("data")]
        public List<CoinDetails> Data { get; set; }

        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; set; }
    }

    [JsonObject]
    public class CoinDetails
    {
        [JsonProperty("coinType")]
        public string CoinType { get; set; }

        [JsonProperty("coinObjectId")]
        public string CoinObjectId { get; set; }

        [JsonProperty("version")]
        public string Version { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        [JsonProperty("balance")]
        public string Balance { get; set; }

        [JsonProperty("previousTransaction")]
        public string PreviousTransaction { get; set; }

        public ObjectData ToSuiObjectData()
        {
            ObjectData res = new ObjectData();
            res.Digest = this.Digest;
            res.ObjectId = this.CoinObjectId;
            res.PreviousTransaction = this.PreviousTransaction;
            res.Version = BigInteger.Parse(this.Version);
            return res;
        }

        public Types.SuiObjectRef ToSuiObjectRef()
        {
            return new Types.SuiObjectRef
            (
                this.CoinObjectId,
                this.Version,
                this.Digest
            );
        }
    }
}