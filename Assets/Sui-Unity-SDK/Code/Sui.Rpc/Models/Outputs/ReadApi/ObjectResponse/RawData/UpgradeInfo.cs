//
//  UpgradeInfo.cs
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
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Upgraded package info for the linkage table.
    /// </summary>
    public class UpgradeInfo
    {
        /// <summary>
        /// ID of the upgraded packages.
        /// </summary>
        [JsonProperty("upgraded_id")]
        public AccountAddress UpgradedID { get; internal set; }

        /// <summary>
        /// Version of the upgraded package.
        /// </summary>
        [JsonProperty("upgraded_version")]
        public BigInteger UpgradedVersion { get; internal set; }
    }
}