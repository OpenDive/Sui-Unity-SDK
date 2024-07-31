//
//  VotingRights.cs
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
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    // TODO: FROM SUI MYSTEN LABS DOCS: the stake and voting power of a validator can be different so
    // in some places when we are actually referring to the voting power, we
    // should use a different type alias, field name, etc.
    /// <summary>
    /// The class that represents the voting rights a validator has within the committee.
    /// </summary>
    [JsonConverter(typeof(VotingRightsConverter))]
    public class VotingRights : ReturnBase
    {
        // TODO: Ask about the type of the string returned
        /// <summary>
        /// The name of the authority represented as an encoded base 64
        /// Public Key.
        /// </summary>
        [JsonProperty("authority_name")]
        public string AuthorityName { get; internal set; }

        /// <summary>
        /// The amount of stake the authority is holding.
        /// This represents their power within the voting system.
        /// </summary>
        [JsonProperty("stake_unit")]
        public BigInteger StakeUnit { get; internal set; }

        public VotingRights
        (
            string authority_name,
            BigInteger stake_unit
        )
        {
            this.AuthorityName = authority_name;
            this.StakeUnit = stake_unit;
        }

        public VotingRights(SuiError error)
        {
            this.Error = error;
        }
    }
}