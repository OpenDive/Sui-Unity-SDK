//
//  SharedObjectRef.cs
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
using Sui.Accounts;

namespace Sui.Types
{
    /// <summary>
    /// A shared object is a Sui object that can be accessed by others.
    /// This type of object can also be mutated is allowed.
    /// </summary>
    [JsonObject]
    public class SharedObjectRef : IObjectRef
    {
        [JsonProperty("objectId")]
        public string ObjectIDString { get; set; }

        /// <summary>
        /// The version the object was shared at.
        /// </summary>
        [JsonProperty("initialSharedVersion")]
        public BigInteger InitialSharedVersion { get; set; }

        /// <summary>
        /// Whether reference is mutable.
        /// </summary>
        [JsonProperty("mutable")]
        public bool Mutable { get; set; }

        public AccountAddress ObjectID
        {
            get => AccountAddress.FromHex(this.ObjectIDString);
            set => this.ObjectIDString = value.ToHex();
        }

        public SharedObjectRef(AccountAddress objectId, BigInteger initialSharedVersion, bool mutable)
        {
            this.ObjectIDString = objectId.ToHex();
            this.InitialSharedVersion = initialSharedVersion;
            this.Mutable = mutable;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.ObjectID);
            serializer.SerializeU64((ulong)this.InitialSharedVersion);
            serializer.SerializeBool(this.Mutable);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
            => new SharedObjectRef
               (
                   (AccountAddress)AccountAddress.Deserialize(deserializer),
                   new BigInteger(deserializer.DeserializeU64().Value),
                   Bool.Deserialize(deserializer).Value
               );
    }
}