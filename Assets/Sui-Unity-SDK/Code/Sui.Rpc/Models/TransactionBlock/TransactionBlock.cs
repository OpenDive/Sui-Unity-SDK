using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class TransactionBlock
    {
        [JsonProperty("data")]
        public TransactionBlockData Data { get; set; }

        [JsonProperty("txSignatures")]
        public List<string> TxSignatures { get; set; }
    }
}