//
//  SuiObjectRef.cs
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
using NBitcoin.DataEncoders;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Types
{
    /// <summary>
    /// A Sui object can be immutable or owned (`ImmOrOwned`).
    /// `ObjectDigest` is a Base 58 BCS serialized string.
    /// </summary>
    [JsonObject]
    public class SuiObjectRef : ReturnBase, IObjectRef
    {
        [JsonProperty("objectId")]
        public string ObjectIDString { get; set; }

        /// <summary>
        /// The version of the object.
        /// </summary>
        [JsonProperty("version")]
        public BigInteger Version { get; set; }

        /// <summary>
        /// The object's digest.
        /// </summary>
        [JsonProperty("digest")]
        public string Digest { get; set; }

        public AccountAddress ObjectID
        {
            get => AccountAddress.FromHex(this.ObjectIDString);
            set => this.ObjectIDString = value.ToHex();
        }

        [JsonConstructor]
        public SuiObjectRef(string object_id, string version, string digest)
        {
            this.ObjectIDString = object_id;
            this.Version = BigInteger.Parse(version);
            this.Digest = digest;
        }

        public SuiObjectRef(AccountAddress object_id, BigInteger? version, string digest)
        {
            if (version == null)
            {
                this.SetError<SuiError>("Version is null.");
                return;
            }

            this.ObjectIDString = object_id.ToHex();
            this.Version = (BigInteger)version;
            this.Digest = digest;
        }

        public void Serialize(Serialization serializer)
        {
            Base58Encoder decoder = new Base58Encoder();
            byte[] decoded_digest = decoder.DecodeData(this.Digest);

            serializer.Serialize(this.ObjectID);
            serializer.SerializeU64((ulong)this.Version);
            serializer.Serialize(decoded_digest);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            Base58Encoder decoder = new Base58Encoder();

            return new SuiObjectRef
            (
                (AccountAddress)AccountAddress.Deserialize(deserializer),
                new BigInteger(deserializer.DeserializeU64().Value),
                decoder.EncodeData(deserializer.ToBytes())
            );
        }
    }
}