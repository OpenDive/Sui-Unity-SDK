//
//  GasCostSummary.cs
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
    /// Represents a summary of the gas costs in a blockchain transaction or operation.
    /// Gas is a unit that measures the amount of computational effort required to execute operations,
    /// like making transactions or running dApps.
    /// </summary>
    [JsonObject]
    public class GasCostSummary
    {
        /// <summary>
        /// A `BigInteger` representing the cost of computation in the transaction or operation.
        /// This cost is associated with the execution of smart contract code or transaction processing.
        /// </summary>
        [JsonProperty("computationCost")]
        public BigInteger ComputationCost { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the cost associated with storing data in the transaction or operation.
        /// This can include costs related to saving data on the blockchain.
        /// </summary>
        [JsonProperty("storageCost")]
        public BigInteger StorageCost { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the rebate for storage in the transaction or operation.
        /// This can include any reductions or refunds on the overall storage cost.
        /// </summary>
        [JsonProperty("storageRebate")]
        public BigInteger StorageRebate { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing any non-refundable storage fee in the transaction or operation.
        /// This refers to the portion of the storage cost that cannot be recovered or refunded.
        /// </summary>
        [JsonProperty("nonRefundableStorageFee")]
        public BigInteger NonRefundableStorageFee { get; internal set; }
    }
}