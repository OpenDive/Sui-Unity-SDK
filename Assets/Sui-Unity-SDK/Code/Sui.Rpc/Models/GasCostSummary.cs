using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class GasCostSummary
    {
        [JsonProperty("computationCost")]
        public BigInteger ComputationCost { get; set; }

        [JsonProperty("storageCost")]
        public BigInteger StorageCost { get; set; }

        [JsonProperty("storageRebate")]
        public BigInteger StorageRebate { get; set; }

        [JsonProperty("nonRefundableStorageFee")]
        public BigInteger NonRefundableStorageFee { get; set; }
    }
}