//
//  ObjectResponseErrorConverter.cs
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
    public class ObjectResponseErrorConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(ObjectResponseError);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject object_response_error = (JObject)JToken.ReadFrom(reader);

                switch (object_response_error["code"].Value<string>())
                {
                    case "notExist":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorNotExist
                            (
                                (AccountAddress)object_response_error["objectId"].ToObject(typeof(AccountAddress))
                            ),
                            ObjectResponseErrorType.NotExist
                        );
                    case "dynamicFieldNotFound":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorDynamicFieldNotFound
                            (
                                (AccountAddress)object_response_error["parentObjectId"].ToObject(typeof(AccountAddress))
                            ),
                            ObjectResponseErrorType.DynamicFieldNotFound
                        );
                    case "deleted":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorDeleted
                            (
                                object_response_error["digest"].Value<string>(),
                                (AccountAddress)object_response_error["objectId"].ToObject(typeof(AccountAddress)),
                                BigInteger.Parse(object_response_error["version"].Value<string>())
                            ),
                            ObjectResponseErrorType.Deleted
                        );
                    case "unknown":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorUnknown(),
                            ObjectResponseErrorType.Unknown
                        );
                    case "displayError":
                        return new ObjectResponseError
                        (
                            new ObjectResponseErrorDisplayError
                            (
                                object_response_error["error"].Value<string>()
                            ),
                            ObjectResponseErrorType.DisplayError
                        );
                    default:
                        return new ObjectResponseError(new SuiError(0, "An unknown error has occured.", object_response_error));
                }
            }

            return new ObjectResponseError(new SuiError(0, "Unable to convert JSON to ObjectResponseError.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartObject();
                ObjectResponseError object_error = (ObjectResponseError)value;

                writer.WritePropertyName("code");
                writer.WriteValue(object_error.Type.ToString());

                switch (object_error.Type)
                {
                    case ObjectResponseErrorType.NotExist:
                        writer.WritePropertyName("objectId");
                        writer.WriteValue(((ObjectResponseErrorNotExist)object_error.ObjectError).ObjectID);
                        break;
                    case ObjectResponseErrorType.DynamicFieldNotFound:
                        writer.WritePropertyName("parentObjectId");
                        writer.WriteValue(((ObjectResponseErrorDynamicFieldNotFound)object_error.ObjectError).ParentObjectID);
                        break;
                    case ObjectResponseErrorType.Deleted:
                        ObjectResponseErrorDeleted object_deleted = (ObjectResponseErrorDeleted)object_error.ObjectError;

                        writer.WritePropertyName("digest");
                        writer.WriteValue(object_deleted.Digest);

                        writer.WritePropertyName("objectId");
                        writer.WriteValue(object_deleted.ObjectID);

                        writer.WritePropertyName("version");
                        writer.WriteValue(object_deleted.Version);
                        break;
                    case ObjectResponseErrorType.DisplayError:
                        writer.WritePropertyName("error");
                        writer.WriteValue(((ObjectResponseErrorDisplayError)object_error.ObjectError).ObjectError);
                        break;
                    default:
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}