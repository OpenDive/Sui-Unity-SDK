//
//  Base64ByteArrayResultConverter.cs
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
using Chaos.NaCl;
using Newtonsoft.Json;

namespace Sui.Utilities
{
    /// <summary>
    /// Used for converting JSON (A Base 64 string) to a Result object containing a byte array.
    /// </summary>
    public class Base64ByteArrayResultConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(SuiResult<byte[]>);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.String)
                return new SuiResult<byte[]>(CryptoBytes.FromBase64String(reader.Value.ToString()));

            return new SuiResult<byte[]>(null, new SuiError(0, "Unable to convert JSON to a byte array.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                SuiResult<byte[]> byte_array_result = (SuiResult<byte[]>)value;

                if (byte_array_result.Error != null)
                    writer.WriteValue(JsonConvert.SerializeObject(byte_array_result.Error));
                else
                    writer.WriteValue(CryptoBytes.ToBase64String(byte_array_result.Result));
            }
        }
    }
}