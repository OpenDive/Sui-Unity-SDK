using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// The effects that occured during transaction block execution.
    /// </summary>
    [JsonObject]
    public class TransactionBlockEffects
    {
        /// <summary>
        /// Represents the message version of the transaction effect.
        /// </summary>
        [JsonProperty("messageVersion")]
        public string MessageVersion { get; internal set; }

        /// <summary>
        /// Represents the execution status of the transaction effect.
        /// </summary>
        [JsonProperty("status")]
        public ExecutionStatusResponse Status { get; internal set; }

        /// <summary>
        /// Represents the epoch in which the transaction was executed.
        /// </summary>
        [JsonProperty("executedEpoch")]
        public BigInteger ExecutedEpoch { get; internal set; }

        /// <summary>
        /// An optional list representing versions at which the transaction was modified.
        /// </summary>
        [JsonProperty("modifiedAtVersions", NullValueHandling = NullValueHandling.Include)]
        public TransactionBlockEffectsModifiedAtVersions[] ModifiedAtVersions { get; internal set; }

        /// <summary>
        /// Represents a summary of the gas used by the transaction.
        /// </summary>
        [JsonProperty("gasUsed")]
        public GasCostSummary GasUsed { get; internal set; }

        /// <summary>
        /// An optional list representing shared objects associated with the transaction effect.
        /// </summary>
        [JsonProperty("sharedObjects", NullValueHandling = NullValueHandling.Include)]
        public Sui.Types.SuiObjectRef[] SharedObjects { get; internal set; }

        /// <summary>
        /// Represents the digest of the transaction.
        /// </summary>
        [JsonProperty("transactionDigest")]
        public string TransactionDigest { get; internal set; }

        /// <summary>
        /// An optional list representing objects created by the transaction.
        /// </summary>
        [JsonProperty("created", NullValueHandling = NullValueHandling.Include)]
        public SuiOwnedObjectRef[] Created { get; internal set; }

        /// <summary>
        /// An optional list representing objects mutated by the transaction.
        /// </summary>
        [JsonProperty("mutated", NullValueHandling = NullValueHandling.Include)]
        public SuiOwnedObjectRef[] Mutated { get; internal set; }

        /// <summary>
        /// An optional list representing objects unwrapped by the transaction.
        /// </summary>
        [JsonProperty("unwrapped", NullValueHandling = NullValueHandling.Include)]
        public SuiOwnedObjectRef[] Unwrapped { get; internal set; }

        /// <summary>
        /// An optional list representing objects deleted by the transaction.
        /// </summary>
        [JsonProperty("deleted", NullValueHandling = NullValueHandling.Include)]
        public Sui.Types.SuiObjectRef[] Deleted { get; internal set; }

        /// <summary>
        /// An optional list representing objects that were unwrapped then deleted by the transaction.
        /// </summary>
        [JsonProperty("unwrappedThenDeleted", NullValueHandling = NullValueHandling.Include)]
        public Sui.Types.SuiObjectRef[] UnwrappedThenDeleted { get; internal set; }

        /// <summary>
        /// An optional list representing objects wrapped by the transaction.
        /// </summary>
        [JsonProperty("wrapped", NullValueHandling = NullValueHandling.Include)]
        public Sui.Types.SuiObjectRef[] Wrapped { get; internal set; }

        /// <summary>
        /// Represents the gas object associated with the transaction effect.
        /// </summary>
        [JsonProperty("gasObject")]
        public SuiOwnedObjectRef GasObject { get; internal set; }

        /// <summary>
        /// An optional digest of the events related to the transaction.
        /// </summary>
        [JsonProperty("eventsDigest", NullValueHandling = NullValueHandling.Include)] 
        public string EventsDigest { get; internal set; }

        /// <summary>
        /// An optional list representing the dependencies of the transaction.
        /// </summary>
        [JsonProperty("dependencies", NullValueHandling = NullValueHandling.Include)]
        public string[] Dependencies { get; internal set; }
    }
}
