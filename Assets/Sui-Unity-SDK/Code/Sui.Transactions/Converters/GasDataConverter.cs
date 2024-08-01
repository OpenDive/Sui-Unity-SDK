//
//  GasDataConverter.cs
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
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Types;
using System;
using System.Collections.Generic;
using System.Numerics;

namespace Sui.Transactions
{
    public class GasDataConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(GasData);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject gas_data = JObject.Load(reader);

            string owner = gas_data["owner"].Value<string>();
            string price = gas_data["price"].Value<string>();
            string budget = gas_data["budget"].Value<string>();

            List<SuiObjectRef> objects = new List<SuiObjectRef>();
            foreach (JObject payment_object in (JArray)gas_data["payment"])
                objects.Add
                (
                    new SuiObjectRef
                    (
                        payment_object["objectId"].ToObject<AccountAddress>(),
                        new BigInteger(payment_object["version"].Value<int>()),
                        payment_object["digest"].Value<string>()
                    )
                );

            return new GasData
            (
                budget,
                price,
                objects.ToArray(),
                AccountAddress.FromHex(owner)
            );
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                writer.WriteStartObject();
                GasData gas_data = (GasData)value;

                writer.WritePropertyName("owner");
                writer.WriteValue(gas_data.Owner.ToHex());

                writer.WritePropertyName("price");
                writer.WriteValue(gas_data.Price);

                writer.WritePropertyName("budget");
                writer.WriteValue(gas_data.Budget);

                writer.WritePropertyName("payment");
                writer.WriteStartArray();

                foreach (SuiObjectRef payment in gas_data.Payment)
                {
                    writer.WriteStartObject();
                    writer.WritePropertyName("objectId");
                    writer.WriteValue(payment.ObjectIDString);

                    writer.WritePropertyName("version");
                    writer.WriteValue(payment.Version);

                    writer.WritePropertyName("digest");
                    writer.WriteValue(payment.Digest);
                    writer.WriteEndObject();
                }

                writer.WriteEndArray();
                writer.WriteEndObject();
            }
        }
    }
}