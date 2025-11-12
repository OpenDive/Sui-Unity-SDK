//
//  Genesis.cs
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
using OpenDive.BCS;
using Sui.Accounts;
using System.Linq;

namespace Sui.Transactions
{
    /// <summary>
    /// System transaction that initializes the network and writes the initial set of objects on-chain.
    /// </summary>
    [JsonObject]
    public class Genesis : ITransactionKind
    {
        /// <summary>
        /// Objects to be created during genesis.
        /// </summary>
        [JsonProperty("objects")]
        public AccountAddress[] Objects { get; set; }

        public Genesis(AccountAddress[] objects)
        {
            this.Objects = objects;
        }

        public void Serialize(Serialization serializer)
            => serializer.Serialize(new Sequence(this.Objects));

        public static ISerializable Deserialize(Deserialization deserializer)
            => new Genesis
               (
                   deserializer.DeserializeSequence(typeof(AccountAddress)).Values.Cast<AccountAddress>().ToArray()
               );
    }
}