//
//  SuiChangeEpoch.cs
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
    /// <para>A system transaction that updates epoch information on-chain (increments the current epoch).
    /// Executed by the system once per epoch, without using gas. Epoch change transactions cannot be
    /// submitted by users, because validators will refuse to sign them.</para>
    ///
    /// <para>This transaction kind is deprecated in favour of `EndOfEpochTransaction`.</para>
    /// </summary>
    [JsonObject]
    public class SuiChangeEpoch : ITransactionKind
    {
        /// <summary>
        /// The next (to become) epoch.
        /// </summary>
        [JsonProperty("epoch")]
        public BigInteger Epoch { get; set; }

        /// <summary>
        /// The total amount of gas charged for storage during the previous epoch (in MIST).
        /// </summary>
        [JsonProperty("storage_charge")]
        public BigInteger StorageCharge { get; set; }

        /// <summary>
        /// The total amount of gas charged for computation during the previous epoch (in MIST).
        /// </summary>
        [JsonProperty("computation_charge")]
        public BigInteger ComputationCharge { get; set; }

        /// <summary>
        /// The SUI returned to transaction senders for cleaning up objects (in MIST).
        /// </summary>
        [JsonProperty("storage_rebate")]
        public BigInteger StorageRebate { get; set; }

        /// <summary>
        /// Time at which the next epoch will start.
        /// </summary>
        [JsonProperty("epoch_start_timestamp_ms")]
        public BigInteger? EpochStartTimestampMs { get; set; }

        public SuiChangeEpoch
        (
            BigInteger epoch,
            BigInteger storage_charge,
            BigInteger computation_charge,
            BigInteger storage_rebate,
            BigInteger? epoch_start_tinmestamp_ms = null
        )
        {
            this.Epoch = epoch;
            this.StorageCharge = storage_charge;
            this.ComputationCharge = computation_charge;
            this.StorageRebate = storage_rebate;
            this.EpochStartTimestampMs = epoch_start_tinmestamp_ms;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU256(this.Epoch);
            serializer.SerializeU256(this.StorageCharge);
            serializer.SerializeU256(this.ComputationCharge);
            serializer.SerializeU256(this.StorageRebate);

            if (this.EpochStartTimestampMs != null)
                serializer.SerializeU256((BigInteger)this.EpochStartTimestampMs);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            BigInteger epoch = deserializer.DeserializeU256().Value;
            BigInteger storage_charge = deserializer.DeserializeU256().Value;
            BigInteger computation_charge = deserializer.DeserializeU256().Value;
            BigInteger storage_rebate = deserializer.DeserializeU256().Value;
            BigInteger? epoch_start_timestamp_ms = ((U256)deserializer.DeserializeOptional(typeof(U256))).Value;

            return new SuiChangeEpoch
            (
                epoch,
                storage_charge,
                computation_charge,
                storage_rebate,
                epoch_start_timestamp_ms
            );
        }
    }
}