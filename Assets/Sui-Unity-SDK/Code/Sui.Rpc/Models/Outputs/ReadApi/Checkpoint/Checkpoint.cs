//
//  Checkpoint.cs
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

using Newtonsoft.Json;
using System.Numerics;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Checkpoints contain finalized transactions and are used for node synchronization
    /// and global transaction ordering.
    /// </summary>
    [JsonObject]
    public class Checkpoint
    {
        /// <summary>
        /// Commitments to checkpoint-specific state (e.g. txns in checkpoint, objects read/written in
        /// checkpoint).
        /// </summary>
        [JsonProperty("checkpointCommitments")]
        public string[] CheckpointCommitments { get; internal set; }

        /// <summary>
        /// A 32-byte hash that uniquely identifies the checkpoint contents, encoded in Base58. This
        /// hash can be used to verify checkpoint contents by checking signatures against the committee,
        /// Hashing contents to match digest, and checking that the previous checkpoint digest matches.
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; internal set; }

        /// <summary>
        /// Present only on the final checkpoint of the epoch.
        /// </summary>
        [JsonProperty("endOfEpochData", NullValueHandling = NullValueHandling.Include)]
        public EndOfEpochData EndOfEpochData { get; internal set; }

        /// <summary>
        /// The epoch this checkpoint is part of.
        /// </summary>
        [JsonProperty("epoch")]
        public BigInteger Epoch { get; internal set; }

        /// <summary>
        /// The running total gas costs of all transactions included in the current epoch so far
        /// until this checkpoint.
        /// </summary>
        [JsonProperty("epochRollingGasCostSummary")]
        public GasCostSummary EpochRollingGasCostSummary { get; internal set; }

        /// <summary>
        /// Total number of transactions committed since genesis, including those in this
        /// checkpoint.
        /// </summary>
        [JsonProperty("networkTotalTransactions")]
        public BigInteger NetworkTotalTransactions { get; internal set; }

        /// <summary>
        /// The digest of the checkpoint at the previous sequence number.
        /// </summary>
        [JsonProperty("previousDigest", NullValueHandling = NullValueHandling.Include)]
        public string PreviousDigest { get; internal set; }

        /// <summary>
        /// This checkpoint's position in the total order of finalized checkpoints, agreed upon by
        /// consensus.
        /// </summary>
        [JsonProperty("sequenceNumber")]
        public BigInteger SequenceNumber { get; internal set; }

        /// <summary>
        /// Timestamp of the checkpoint - number of milliseconds from the Unix epoch
        /// Checkpoint timestamps are monotonic, but not strongly monotonic - subsequent
        /// checkpoints can have same timestamp if they originate from the same underlining consensus commit
        /// </summary>
        [JsonProperty("timestampMs")]
        public BigInteger TimestampMs { get; internal set; }

        /// <summary>
        /// Transactions in this checkpoint.
        /// </summary>
        [JsonProperty("transactions")]
        public string[] Transactions { get; internal set; }

        /// <summary>
        /// This is an aggregation of signatures from a quorum of validators for the checkpoint
        /// proposal.
        /// </summary>
        [JsonProperty("validatorSignature")]
        public string ValidatorSignature { get; internal set; }

        /// <summary>
        /// <para>CheckpointSummary is not an evolvable structure - it must be readable by any version of the
        /// code. Therefore, in order to allow extensions to be added to CheckpointSummary, we allow
        /// opaque data to be added to checkpoints which can be deserialized based on the current
        /// protocol version.</para>
        ///
        /// <para>This is implemented with BCS-serialized `CheckpointVersionSpecificData`.</para>
        /// </summary>
        [JsonProperty("versionSpecificData")]
        public byte[] VersionSpecificData { get; internal set; }
    }
}