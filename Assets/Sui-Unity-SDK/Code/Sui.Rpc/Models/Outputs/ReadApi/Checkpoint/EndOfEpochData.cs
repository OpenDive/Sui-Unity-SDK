//
//  EndOfEpochData.cs
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
    [JsonObject]
    public class EndOfEpochData
    {
        /// <summary>
        /// <para>NextEpochCommittee is not null if and only if the current checkpoint is
        /// the last checkpoint of an epoch.</para>
        /// 
        /// <para>Therefore next_epoch_committee can be used to pick the last checkpoint of an epoch,
        /// which is often useful to get epoch level summary stats like total gas cost of an epoch,
        /// or the total number of transactions from genesis to the end of an epoch.
        /// The committee is stored as a vector of validator pub key and stake pairs. The vector
        /// should be sorted based on the Committee data structure.</para>
        /// </summary>
        [JsonProperty("nextEpochCommittee")]
        public (string, BigInteger)[] NextEpochCommittee { get; internal set; }

        /// <summary>
        /// The protocol version that is in effect during the epoch that starts immediately after this
        /// checkpoint.
        /// </summary>
        [JsonProperty("nextEpochProtocolVersion")]
        public BigInteger NextEpochProtocolVersion { get; internal set; }

        /// <summary>
        /// Commitments to epoch specific state (e.g. live object set).
        /// </summary>
        [JsonProperty("epochCommitments")]
        public string EpochCommitments { get; internal set; }
    }
}