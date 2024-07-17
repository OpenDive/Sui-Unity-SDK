using System;
using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;
using Sui.Rpc.Client;
using Sui.Rpc.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Rpc;
using Sui.Accounts;
using System.Text;
using UnityEngine;
using System.Numerics;

namespace Sui.Types
{
    public enum CallArgumentType
    {
        Pure,
        Object,
        ObjectVec
    }

    public class CallArgumentConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(CallArg);
        }

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
                    switch(object_type)
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
                                            input["objectId"].Value<string>(),
                                            input["initialSharedVersion"].Value<string>(),
                                            input["mutable"].Value<bool>()
                                        )
                                    )
                                )
                            );
                        default:
                            return new SuiError(0, "Unable to convert from JSON to ObjectCallArg", input);
                    }
                default:
                    return new SuiError(0, "Unable to convert from JSON to CallArg", input);
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

                switch(call_arg.Type)
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

    /// <summary>
    /// A Sui type that represents a transaction call arguments.
    /// A call arg can be a (1) vector / list of byte (BCS U8),
    /// (2) an ObjectRef (also known as object arg, (3) or a vector / list of ObjectRef.
    ///
    /// The following is the TypeScript SDK schema
    /// <code>
    ///     CallArg: {
    ///		    Pure: [VECTOR, BCS.U8],
    ///		    Object: 'ObjectArg',
    ///		    ObjVec: [VECTOR, 'ObjectArg'],
    ///	    },
    /// </code>
    ///
    /// In our implementation CallArg by default takes in a list of args / ISerializable object.
    /// </summary>
    public interface ICallArg: ISerializable { }

    [JsonConverter(typeof(CallArgumentConverter))]
    public class CallArg : ISerializable
    {
        public CallArgumentType Type { get; set; }

        public ICallArg CallArgument { get; set; }

        public CallArg(CallArgumentType type, ICallArg call_argument)
        {
            this.Type = type;
            this.CallArgument = call_argument;
        }

        public SharedObjectRef GetSharedObectInput()
        {
            return this.Type switch
            {
                CallArgumentType.Object => ((ObjectCallArg)this.CallArgument).ObjectArg.Type switch
                {
                    ObjectRefType.Shared => (SharedObjectRef)((ObjectCallArg)this.CallArgument).ObjectArg.ObjectRef,
                    _ => null,
                },
                _ => null,
            };
        }

        public bool IsMutableSharedObjectInput()
        {
            if (this.GetSharedObectInput() != null)
                return this.GetSharedObectInput().Mutable;
            else
                return false;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8((byte)this.Type);
            serializer.Serialize(this.CallArgument);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8();
            switch (type)
            {
                case 0:
                    return new CallArg(CallArgumentType.Pure, (PureCallArg)PureCallArg.Deserialize(deserializer));
                case 1:
                    return new CallArg(CallArgumentType.Object, (ObjectCallArg)ObjectCallArg.Deserialize(deserializer));
                default:
                    return new SuiError(0, "Unable to deserialize Call Argument", null);
            }
        }
    }

    /// <summary>
    /// A pure value takes in U8, U256, BString, AccountAddress, etc
    /// </summary>
    public class PureCallArg : ICallArg
    {
        public byte[] Value { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PureCallArg(byte[] value)
        {
            this.Value = value;
        }

        public PureCallArg(JToken value, string value_type)
        {
            Serialization ser = new Serialization();
            SuiStructTag struct_tag = SuiStructTag.FromStr(value.Value<string>());

            if (struct_tag != null)
                struct_tag.Serialize(ser);
            else
            {
                if (value_type == "bool")
                    ser.SerializeBool(value.Value<bool>());
                else if (value_type == "u8")
                    ser.SerializeU8(value.Value<byte>());
                else if (value_type == "u16")
                    ser.SerializeU16(value.Value<ushort>());
                else if (value_type == "u32")
                    ser.SerializeU32(value.Value<uint>());
                else if (value_type == "u64")
                    ser.SerializeU64(value.Value<ulong>());
                else if (value_type == "u128")
                    ser.SerializeU128(BigInteger.Parse(value.Value<string>()));
                else if (value_type == "u256")
                    ser.SerializeU256(BigInteger.Parse(value.Value<string>()));
                else
                {
                    AccountAddress account_address = AccountAddress.FromHex(value.Value<string>());
                    if (account_address != null)
                        account_address.Serialize(ser);
                    else
                        ser.SerializeString(value.Value<string>());
                }
            }
            this.Value = ser.GetBytes();
        }

        public PureCallArg(ISerializable value)
        {
            Serialization ser = new Serialization();
            ser.Serialize(value);
            this.Value = ser.GetBytes();
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Value);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new PureCallArg(
                deserializer.ToBytes()
            );
        }
    }

    /// <summary>
    /// Base interfaces that both a SuiObjectRef and a SharedObjectRef must implement.
    /// This is used to create an abstraction that will allow it to become an argument.
    /// In the Sui TypeScript SDK this is referred to as an `ObjectArg`, it's schema
    /// is below:
    /// 
    /// <code>
    ///	    ObjectArg: {
    ///		    ImmOrOwned: 'SuiObjectRef',
    ///		    Shared: 'SharedObjectRef',
    ///	    },
    /// </code>
    ///
    /// This interface extends ISeriliazable which allows these objects that
    /// implement the interface to be passed as arguments (`CallArgs` in Sui TypeScript)
    /// </summary>
    ///
    public class ObjectCallArg : ICallArg
    {
        public ObjectArg ObjectArg { get; set; }

        public ObjectCallArg(ObjectArg objectArg)
        {
            ObjectArg = objectArg;
        }

        public bool IsMutableSharedObjectInput()
        {
            if (this.ObjectArg.Type == ObjectRefType.ImmOrOwned)
                return false;

            return
                ((SharedObjectRef)this.ObjectArg.ObjectRef).Mutable;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.ObjectArg);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new ObjectCallArg(
                (ObjectArg)ObjectArg.Deserialize(deserializer)
            );
        }
    }
}