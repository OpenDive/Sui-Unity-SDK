//
//  ProtocolConfig.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents protocol config table for the given version number.
    /// If the version number is not specified, If none is specified,
    /// the node uses the version of the latest epoch it has processed.
    /// </summary>
    public class ProtocolConfig
    {
        /// <summary>
        /// A `BigInteger` representing the minimum supported protocol version.
        /// </summary>
        [JsonProperty("minSupportedProtocolVersion")]
        public BigInteger MinSupportedProtocolVersion { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the maximum supported protocol version.
        /// </summary>
        [JsonProperty("maxSupportedProtocolVersion")]
        public BigInteger MaxSupportedProtocolVersion { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the current protocol version.
        /// </summary>
        [JsonProperty("protocolVersion")]
        public BigInteger ProtocolVersion { get; internal set; }

        /// <summary>
        /// A dictionary containing feature flags related to the protocol configuration.
        /// The keys are strings representing the name of the feature flag,
        /// and the values are Booleans representing whether the feature is enabled (`true`) or disabled (`false`).
        /// </summary>
        [JsonProperty("featureFlags")]
        public Dictionary<string, bool> FeatureFlags { get; internal set; }

        /// <summary>
        /// A dictionary containing attributes related to the protocol configuration.
        /// The keys are strings representing the name of the attribute,
        /// and the values are optional `AttributeValue` instances representing the value of the attribute.
        /// </summary>
        [JsonProperty("attributes")]
        public Dictionary<string, AttributeValue> Attributes { get; internal set; }
    }
}