//
//  CallArgumentConverter.cs
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
using Sui.Utilities;
using System;
using System.Numerics;

namespace Sui.Types
{
    public class CallArgumentConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(CallArg);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject input = JObject.Load(reader);
            string input_type = input["type"].Value<string>();

            switch (input_type)
            {
                case "pure":
                    return new CallArg
                    (
                        CallArgumentType.Pure,
                        new PureCallArg
                        (
                            input["value"],
                            input["valueType"].Value<string>()
                        )
                    );
                case "object":
                    string object_type = input["objectType"].Value<string>();
                    switch (object_type)
                    {
                        case "immOrOwnedObject":
                            return new CallArg
                            (
                                CallArgumentType.Object,
                                new ObjectCallArg
                                (
                                    new ObjectArg
                                    (
                                        ObjectRefType.ImmOrOwned,
                                        new SuiObjectRef
                                        (
                                            input["objectId"].Value<string>(),
                                            input["version"].Value<string>(),
                                            input["digest"].Value<string>()
                                        )
                                    )
                                )
                            );
                        case "sharedObject":
                            return new CallArg
                            (
                                CallArgumentType.Object,
                                new ObjectCallArg
                                (
                                    new ObjectArg
                                    (
                                        ObjectRefType.Shared,
                                        new SharedObjectRef
                                        (
                                            input["objectId"].ToObject<AccountAddress>(),
                                            BigInteger.Parse(input["initialSharedVersion"].Value<string>()),
                                            input["mutable"].Value<bool>()
                                        )
                                    )
                                )
                            );
                        default:
                            return new CallArg(new SuiError(0, "Unable to convert from JSON to ObjectCallArg", input));
                    }
                default:
                    return new CallArg(new SuiError(0, "Unable to convert from JSON to CallArg", input));
            }
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
                CallArg call_arg = (CallArg)value;

                writer.WritePropertyName("type");
                writer.WriteValue(call_arg.Type.ToString().ToLower());

                switch (call_arg.Type)
                {
                    case CallArgumentType.Pure:
                        writer.WritePropertyName("value");
                        writer.WriteValue(String.Join(", ", ((PureCallArg)call_arg.CallArgument).Value));
                        break;
                    case CallArgumentType.Object:
                        ObjectArg object_arg = ((ObjectCallArg)call_arg.CallArgument).ObjectArg;
                        switch (object_arg.Type)
                        {
                            case ObjectRefType.ImmOrOwned:
                                writer.WritePropertyName("objectType");
                                writer.WriteValue("immOrOwnedObject");

                                writer.WritePropertyName("objectId");
                                writer.WriteValue(((SuiObjectRef)object_arg.ObjectRef).ObjectIDString);

                                writer.WritePropertyName("version");
                                writer.WriteValue(((SuiObjectRef)object_arg.ObjectRef).Version);

                                writer.WritePropertyName("digest");
                                writer.WriteValue(((SuiObjectRef)object_arg.ObjectRef).Digest);
                                break;
                            case ObjectRefType.Shared:
                                writer.WritePropertyName("objectType");
                                writer.WriteValue("sharedObject");

                                writer.WritePropertyName("objectId");
                                writer.WriteValue(((SharedObjectRef)object_arg.ObjectRef).ObjectIDString);

                                writer.WritePropertyName("initialSharedVersion");
                                writer.WriteValue(((SharedObjectRef)object_arg.ObjectRef).InitialSharedVersion);

                                writer.WritePropertyName("mutable");
                                writer.WriteValue(((SharedObjectRef)object_arg.ObjectRef).Mutable);
                                break;
                        }
                        break;
                    default:
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}