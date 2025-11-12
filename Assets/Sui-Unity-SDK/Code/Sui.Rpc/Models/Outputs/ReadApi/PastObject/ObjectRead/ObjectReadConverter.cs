//
//  ObjectReadConverter.cs
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
using Sui.Utilities;
using System;

namespace Sui.Rpc.Models
{
    public class ObjectReadConverter : JsonConverter
    {
        public readonly string ObjectReadStatusProperty = "status";
        public readonly string ObjectReadDetailsProperty = "details";

        public override bool CanConvert(Type objectType) => objectType == typeof(ObjectRead);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject object_read = JObject.Load(reader);

                switch (object_read[this.ObjectReadStatusProperty].Value<string>())
                {
                    case "VersionFound":
                        return new ObjectRead
                        (
                            new VersionFound(JsonConvert.DeserializeObject<ObjectData>(JsonConvert.SerializeObject(object_read[this.ObjectReadDetailsProperty]))),
                            ObjectReadType.VersionFound
                        );
                    case "ObjectNotExists":
                        return new ObjectRead
                        (
                            new ObjectNotExists(object_read[this.ObjectReadDetailsProperty].Value<string>()),
                            ObjectReadType.ObjectNotExists
                        );
                    case "ObjectDeleted":
                        return new ObjectRead
                        (
                            new ObjectDeleted(JsonConvert.DeserializeObject<Sui.Types.SuiObjectRef>(JsonConvert.SerializeObject(object_read[this.ObjectReadDetailsProperty]))),
                            ObjectReadType.ObjectDeleted
                        );
                    case "VersionNotFound":
                        JArray not_found_details = (JArray)object_read[this.ObjectReadDetailsProperty];
                        return new ObjectRead
                        (
                            new VersionNotFound
                            (
                                new Tuple<string, string>
                                (
                                    not_found_details[0].ToString(),
                                    not_found_details[1].ToString()
                                )
                            ),
                            ObjectReadType.VersionNotFound
                        );
                    case "VersionTooHigh":
                        return new ObjectRead
                        (
                            new VersionTooHigh(JsonConvert.DeserializeObject<VersionTooHighDetails>(JsonConvert.SerializeObject(object_read[this.ObjectReadDetailsProperty]))),
                            ObjectReadType.VersionTooHigh
                        );
                    default:
                        return new ObjectRead
                        (
                            new SuiError
                            (
                                0,
                                $"Unable to convert {object_read[this.ObjectReadStatusProperty].Value<string>()} to ObjectReadType.",
                                null
                            )
                        );
                }
            }

            return new ObjectRead(new SuiError(0, "Unable to convert JSON to ObjectRead.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                ObjectRead object_read = (ObjectRead)value;

                writer.WriteStartObject();

                writer.WritePropertyName(this.ObjectReadStatusProperty);
                writer.WriteValue(object_read.Type.ToString());

                writer.WritePropertyName(this.ObjectReadDetailsProperty);

                switch (object_read.Type)
                {
                    case ObjectReadType.VersionFound:
                        VersionFound version_found = (VersionFound)object_read.Object;
                        writer.WriteRawValue(JsonConvert.SerializeObject(version_found.Value));
                        break;
                    case ObjectReadType.ObjectNotExists:
                        ObjectNotExists object_not_exists = (ObjectNotExists)object_read.Object;
                        writer.WriteValue(JsonConvert.SerializeObject(object_not_exists.Value));
                        break;
                    case ObjectReadType.ObjectDeleted:
                        ObjectDeleted object_deleted = (ObjectDeleted)object_read.Object;
                        writer.WriteRawValue(JsonConvert.SerializeObject(object_deleted.Value));
                        break;
                    case ObjectReadType.VersionNotFound:
                        VersionNotFound version_not_found = (VersionNotFound)object_read.Object;

                        writer.WriteStartArray();

                        writer.WriteValue(version_not_found.Value.Item1);
                        writer.WriteValue(version_not_found.Value.Item2);

                        writer.WriteEndArray();
                        break;
                    case ObjectReadType.VersionTooHigh:
                        VersionTooHigh version_too_high = (VersionTooHigh)object_read.Object;
                        writer.WriteRawValue(JsonConvert.SerializeObject(version_too_high.Value));
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}