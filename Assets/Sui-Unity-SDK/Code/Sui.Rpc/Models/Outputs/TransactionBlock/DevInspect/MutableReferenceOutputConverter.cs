//
//  MutableReferenceOutputConverter.cs
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
    public class MutableReferenceOutputConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(MutableReferenceOutput);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                JArray mutable_reference_output = (JArray)JToken.ReadFrom(reader);

                return new MutableReferenceOutput
                (
                    mutable_reference_output[0].Value<string>(),
                    ((JArray)mutable_reference_output[1]).Select(val => val.Value<byte>()).ToArray(),
                    new SuiStructTag(mutable_reference_output[2].Value<string>())
                );
            }

            return new MutableReferenceOutput(new SuiError(0, "Unable to convert JSON to MutableReferenceOutput.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartArray();

                MutableReferenceOutput mutable_reference_output = (MutableReferenceOutput)value;

                writer.WriteValue(mutable_reference_output.Name);
                writer.WriteValue(mutable_reference_output.ObjectID.ToReadableString());
                writer.WriteValue(mutable_reference_output.Type.ToString());

                writer.WriteEndArray();
            }
        }
    }
}