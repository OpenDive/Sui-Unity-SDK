//
//  SuiConsensusCommitPrologue.cs
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
using OpenDive.BCS;

namespace Sui.Transactions
{
    /// <summary>
    /// Other transaction kinds are usually represented by directly wrapping their native
    /// representation. This kind has two native versions in the protocol, so the same cannot be done.
    /// V2 has all the fields of V1 and one extra (consensus commit digest). The GraphQL representation
    /// of this type is a struct containing all the common fields, as they are in the native type, and
    /// an optional `consensus_commit_digest`.
    /// </summary>
    [JsonObject]
    public class SuiConsensusCommitPrologue : ITransactionKind
    {
        /// <summary>
        /// Epoch of the commit prologue transaction.
        /// </summary>
        [JsonProperty("epoch")]
        public BigInteger Epoch { get; set; }

        /// <summary>
        /// Consensus round of the commit.
        /// </summary>
        [JsonProperty("round")]
        public BigInteger Round { get; set; }

        /// <summary>
        /// Unix timestamp from consensus.
        /// </summary>
        [JsonProperty("commit_timestamp_ms")]
        public BigInteger CommitTimestampMs { get; set; }

        public SuiConsensusCommitPrologue
        (
            BigInteger epoch,
            BigInteger round,
            BigInteger commit_timestamp_ms
        )
        {
            this.Epoch = epoch;
            this.Round = round;
            this.CommitTimestampMs = commit_timestamp_ms;
        }

        public SuiConsensusCommitPrologue() { }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU64((ulong)this.Epoch);
            serializer.SerializeU64((ulong)this.Round);
            serializer.SerializeU64((ulong)this.CommitTimestampMs);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
            => new SuiConsensusCommitPrologue
               (
                   deserializer.DeserializeU64().Value,
                   deserializer.DeserializeU64().Value,
                   deserializer.DeserializeU64().Value
               );
    }
}