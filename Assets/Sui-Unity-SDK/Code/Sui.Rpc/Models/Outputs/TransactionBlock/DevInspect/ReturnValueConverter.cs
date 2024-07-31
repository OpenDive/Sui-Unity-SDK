//
//  ReturnValueConverter.cs
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
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Types;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    public class ReturnValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ReturnValue);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray return_value = (JArray)JToken.ReadFrom(reader);

                return new ReturnValue
                (
                    ((JArray)return_value[0]).Select(val => val.Value<byte>()).ToArray(),
                    new SuiStructTag(return_value[1].Value<string>())
                );
            }

            return new ReturnValue(new SuiError(0, "Unable to convert JSON to ReturnValue.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartArray();

                ReturnValue return_value = (ReturnValue)value;

                writer.WriteValue(return_value.ObjectID.ToReadableString());
                writer.WriteValue(return_value.Type.ToString());

                writer.WriteEndArray();
            }
        }
    }
}