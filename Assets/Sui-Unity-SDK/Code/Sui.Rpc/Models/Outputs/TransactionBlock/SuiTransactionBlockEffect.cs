//
//  SuiTransactionBlockEffects.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using System.Numerics;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// The effects that occured during transaction block execution.
    /// </summary>
    [JsonObject]
    public class SuiTransactionBlockEffects
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
