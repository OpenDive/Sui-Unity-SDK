//
//  MoveFunctionArgTypesConverter.cs
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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    public class MoveFunctionArgTypesConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(MoveFunctionArgType);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject object_arg_type = JObject.Load(reader);

                switch (object_arg_type[ArgumentType.Object.ToString()].Value<string>())
                {
                    case "ByImmutableReference":
                        return new MoveFunctionArgType
                        (
                            ArgumentType.Object,
                            ObjectValueType.ByImmutableReference
                        );
                    case "ByMutableReference":
                        return new MoveFunctionArgType
                        (
                            ArgumentType.Object,
                            ObjectValueType.ByMutableReference
                        );
                    case "ByValue":
                        return new MoveFunctionArgType
                        (
                            ArgumentType.Object,
                            ObjectValueType.ByValue
                        );
                    default:
                        return new MoveFunctionArgType
                        (
                            new SuiError
                            (
                                0,
                                $"Unable to convert {object_arg_type[ArgumentType.Object.ToString()].Value<string>()} to MoveFunctionArgType.",
                                reader
                            )
                        );
                }
            }
            else if (reader.TokenType == JsonToken.String)
                return new MoveFunctionArgType(ArgumentType.Pure);

            return new MoveFunctionArgType(new SuiError(0, "Unable to convert JSON to MoveFunctionArgType.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                MoveFunctionArgType move_function_arg_type = (MoveFunctionArgType)value;

                switch (move_function_arg_type.ArgumentType)
                {
                    case ArgumentType.Pure:
                        writer.WriteValue(ArgumentType.Pure.ToString());
                        break;
                    case ArgumentType.Object:
                        writer.WriteStartObject();

                        writer.WritePropertyName(ArgumentType.Object.ToString());
                        writer.WriteValue(move_function_arg_type.ArgumentReference.ToString());

                        writer.WriteEndObject();
                        break;
                }
            }
        }
    }
}