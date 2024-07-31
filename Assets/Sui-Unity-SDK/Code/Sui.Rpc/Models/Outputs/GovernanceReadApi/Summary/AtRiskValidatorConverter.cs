//
//  AtRiskValidatorConverter.cs
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
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    public class AtRiskValidatorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(AtRiskValidator);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray at_risk_validator = JArray.Load(reader);

                return new AtRiskValidator
                (
                    (AccountAddress)at_risk_validator[0].ToObject(typeof(AccountAddress)),
                    BigInteger.Parse(at_risk_validator[1].Value<string>())
                );
            }

            return new AtRiskValidator(new SuiError(0, "Unable to convert JSON to AtRiskValidator.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                AtRiskValidator at_risk_validator = (AtRiskValidator)value;

                writer.WriteStartArray();

                writer.WriteValue(at_risk_validator.Validator.KeyHex);
                writer.WriteValue(at_risk_validator.NumberOfEpochs);

                writer.WriteEndArray();
            }
        }
    }
}