using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class TransactionBlockEffects
    {
        [JsonProperty("messageVersion")]
        public string MessageVersion { get; set; }

        [JsonProperty("status")]
        public ExecutionStatusResponse Status { get; set; }

        [JsonProperty("executedEpoch")]
        public BigInteger ExecutedEpoch { get; set; }

        [JsonProperty("gasUsed")]
        public GasCostSummary GasUsed { get; set; }

        [JsonProperty("modifiedAtVersions")]
        public List<TransactionBlockEffectsModifiedAtVersions> ModifiedAtVersions { get; set; }

        [JsonProperty("transactionDigest")]
        public string TransactionDigest { get; set; }

        [JsonProperty("mutated")]
        public List<SuiOwnedObjectRef> Mutated { get; set; }

        [JsonProperty("created")]
        public List<SuiOwnedObjectRef> Created { get; set; }

        [JsonProperty("deleted")]
        public List<SuiObjectRef> Deleted { get; set; }

        [JsonProperty("dependencies")]
        public List<string> Dependencies { get; set; }  // TODO: Check in RPC spec if needed

        [JsonProperty("eventsDigest")] 
        public string EventsDigest { get; set; } // TODO: Check in RPC spec if needed

        [JsonProperty("gasObject")]
        public SuiOwnedObjectRef GasObject { get; set; }
        
        public List<SuiObjectRef> SharedObjects { get; set; }  // TODO: Check in RPC spec if needed

        public List<SuiOwnedObjectRef> Unwrapped { get; set; }  // TODO: Check in RPC spec if needed

        public List<SuiObjectRef> UnwrappedThenDeleted { get; set; }  // TODO: Check in RPC spec if needed

        public List<SuiObjectRef> Wrapped { get; set; }  // TODO: Check in RPC spec if needed
    }
}
