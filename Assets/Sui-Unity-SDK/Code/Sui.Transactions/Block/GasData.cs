//
//  GasData.cs
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

using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Accounts;

namespace Sui.Transactions
{
    /// <summary>
    /// The gas data used for the transaction.
    /// </summary>
    [JsonConverter(typeof(GasDataConverter))]
    public class GasData : ISerializable
    {
        /// <summary>
        /// The budget set with the gas data.
        /// </summary>
        [JsonProperty("budget", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? Budget { get; set; }

        /// <summary>
        /// The total price of the gas used.
        /// </summary>
        [JsonProperty("price", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? Price { get; set; }

        /// <summary>
        /// The payment objects used for the transaction.
        /// </summary>
        [JsonProperty("payment")]
        public Sui.Types.SuiObjectRef[] Payment { get; set; }

        /// <summary>
        /// The owner of the gas coins.
        /// </summary>
        [JsonProperty("owner")]
        public AccountAddress Owner { get; set; }

        public GasData(
            string budget = null,
            string price = null,
            Sui.Types.SuiObjectRef[] payment = null,
            AccountAddress owner = null
        )
        {
            this.Budget = budget != null ? BigInteger.Parse(budget) : null;
            this.Price = price != null ? BigInteger.Parse(price) : null;
            this.Payment = payment;
            this.Owner = owner;
        }

        public void Serialize(Serialization serializer)
        {
            if (Payment != null)
            {
                Sequence paymentSeq = new Sequence(Payment);
                serializer.Serialize(paymentSeq);
            }

            if (Owner != null)
                serializer.Serialize(Owner);

            if (Price != null)
                serializer.SerializeU64((ulong)Price);

            if (Budget != null)
                serializer.SerializeU64((ulong)Budget);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            Sui.Types.SuiObjectRef[] payment = deserializer.DeserializeSequence(typeof(Sui.Types.SuiObjectRef)).Values.Cast<Sui.Types.SuiObjectRef>().ToArray();
            AccountAddress owner = (AccountAddress)AccountAddress.Deserialize(deserializer);
            U64 price = (U64)deserializer.DeserializeOptional(typeof(U64));
            U64 budget = (U64)deserializer.DeserializeOptional(typeof(U64));

            return new GasData
            (
                budget != null ? $"{budget.Value}" : null,
                price != null ? $"{price.Value}" : null,
                payment,
                owner
            );
        }
    }
}
