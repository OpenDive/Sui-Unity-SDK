//
//  MoveObjectRawData.cs
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
    /// Represents a raw Move Object in the Move programming language,
    /// containing its BCS (Binary Canonical Serialization) representation, type,
    /// version, and whether it has public transfer enabled.
    /// </summary>
    public class MoveObjectRawData : ARawData
    {
        /// <summary>
        /// BCS bytes of a Move struct value.
        /// </summary>
        [JsonProperty("bcsBytes")]
        public string BCSBytes { get; internal set; }

        /// <summary>
        /// (DEPRECATED) this field is no longer used to determine whether a tx can transfer this
        /// object. Instead, it is always calculated from the objects type when loaded in execution.
        /// </summary>
        [JsonProperty("hasPublicTransfer")]
        public bool HasPublicTransfer { get; internal set; }

        /// <summary>
        /// The type of this object. Immutable.
        /// </summary>
        [JsonProperty("type")]
        public string ObjectType { get; internal set; }

        public MoveObjectRawData
        (
            string bcs_bytes,
            bool has_public_transfer,
            string object_type,
            BigInteger version
        ) : base(version)
        {
            this.BCSBytes = bcs_bytes;
            this.HasPublicTransfer = has_public_transfer;
            this.ObjectType = object_type;
        }
    }
}