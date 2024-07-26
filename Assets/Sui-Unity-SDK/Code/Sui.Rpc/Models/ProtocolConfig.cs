using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents protocol config table for the given version number.
    /// If the version number is not specified, If none is specified,
    /// the node uses the version of the latest epoch it has processed.
    /// </summary>
    public class ProtocolConfig
    {
        /// <summary>
        /// A `BigInteger` representing the minimum supported protocol version.
        /// </summary>
        [JsonProperty("minSupportedProtocolVersion")]
        public BigInteger MinSupportedProtocolVersion { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the maximum supported protocol version.
        /// </summary>
        [JsonProperty("maxSupportedProtocolVersion")]
        public BigInteger MaxSupportedProtocolVersion { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the current protocol version.
        /// </summary>
        [JsonProperty("protocolVersion")]
        public BigInteger ProtocolVersion { get; internal set; }

        /// <summary>
        /// A dictionary containing feature flags related to the protocol configuration.
        /// The keys are strings representing the name of the feature flag,
        /// and the values are Booleans representing whether the feature is enabled (`true`) or disabled (`false`).
        /// </summary>
        [JsonProperty("featureFlags")]
        public Dictionary<string, bool> FeatureFlags { get; internal set; }

        /// <summary>
        /// A dictionary containing attributes related to the protocol configuration.
        /// The keys are strings representing the name of the attribute,
        /// and the values are optional `AttributeValue` instances representing the value of the attribute.
        /// </summary>
        [JsonProperty("attributes")]
        public Dictionary<string, AttributeValue> Attributes { get; internal set; }
    }

    public interface IAttributeValue { }

    public enum AttributeValueType
    {
        U64,
        U32,
        F64,
        U16
    }

    [JsonConverter(typeof(AttributeValueConverter))]
    public class AttributeValue : ReturnBase
    {
        public AttributeValueType Type { get; internal set; }

        public IAttributeValue Attribute { get; internal set; }

        internal AttributeValue
        (
            AttributeValueType type,
            IAttributeValue attribute
        )
        {
            this.Type = type;
            this.Attribute = attribute;
        }

        internal AttributeValue(SuiError error)
        {
            this.Error = error;
        }

        public BigInteger? GetValue()
        {
            return this.Type switch
            {
                AttributeValueType.F64 => new BigInteger(((U64AttributeValue)this.Attribute).Value),
                AttributeValueType.U32 => new BigInteger(((U64AttributeValue)this.Attribute).Value),
                AttributeValueType.U64 => new BigInteger(((U64AttributeValue)this.Attribute).Value),
                AttributeValueType.U16 => new BigInteger(((U64AttributeValue)this.Attribute).Value),
                _ => this.SetError<BigInteger?, SuiError>(null, "Unable to convert to a BigInteger")
            };
        }
    }

    public class U64AttributeValue : IAttributeValue
    {
        public ulong Value { get; internal set; }

        internal U64AttributeValue(ulong value)
        {
            this.Value = value;
        }
    }

    public class U32AttributeValue : IAttributeValue
    {
        public uint Value { get; internal set; }

        internal U32AttributeValue(uint value)
        {
            this.Value = value;
        }
    }

    public class F64AttributeValue : IAttributeValue
    {
        public double Value { get; internal set; }

        internal F64AttributeValue(double value)
        {
            this.Value = value;
        }
    }

    public class U16AttributeValue : IAttributeValue
    {
        public ushort Value { get; internal set; }

        internal U16AttributeValue(ushort value)
        {
            this.Value = value;
        }
    }

    public class AttributeValueConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType) => objectType == typeof(AttributeValue);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject attribute_value = JObject.Load(reader);

                string attribute_type = attribute_value.Properties().ToArray()[0].Name;

                return attribute_type switch
                {
                    "u64" => new AttributeValue
                    (
                        AttributeValueType.U64,
                        new U64AttributeValue(attribute_value[AttributeValueType.U64.ToString().ToLower()].Value<ulong>())
                    ),
                    "u32" => new AttributeValue
                     (
                        AttributeValueType.U32,
                        new U32AttributeValue(attribute_value[AttributeValueType.U32.ToString().ToLower()].Value<uint>())
                    ),
                    "f64" => new AttributeValue
                    (
                        AttributeValueType.F64,
                        new F64AttributeValue(attribute_value[AttributeValueType.F64.ToString().ToLower()].Value<double>())
                    ),
                    "u16" => new AttributeValue
                    (
                        AttributeValueType.U16,
                        new U16AttributeValue(attribute_value[AttributeValueType.U16.ToString().ToLower()].Value<ushort>())
                    ),
                    _ => new AttributeValue
                    (
                        new SuiError
                        (
                            0,
                            $"Unable to convert {attribute_type} to AttributeValueType.",
                            null
                        )
                    ),
                };
            }

            return new AttributeValue(new SuiError(0, "Unable to convert JSON to AttributeValue.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                AttributeValue attribute_value = (AttributeValue)value;

                writer.WriteStartObject();

                switch (attribute_value.Type)
                {
                    case AttributeValueType.U64:
                        U64AttributeValue u64 = (U64AttributeValue)attribute_value.Attribute;
                        writer.WritePropertyName(AttributeValueType.U64.ToString().ToLower());
                        writer.WriteValue(u64.Value);
                        break;
                    case AttributeValueType.U32:
                        U32AttributeValue u32 = (U32AttributeValue)attribute_value.Attribute;
                        writer.WritePropertyName(AttributeValueType.U32.ToString().ToLower());
                        writer.WriteValue(u32.Value);
                        break;
                    case AttributeValueType.F64:
                        F64AttributeValue f64 = (F64AttributeValue)attribute_value.Attribute;
                        writer.WritePropertyName(AttributeValueType.F64.ToString().ToLower());
                        writer.WriteValue(f64.Value);
                        break;
                    case AttributeValueType.U16:
                        U16AttributeValue u16 = (U16AttributeValue)attribute_value.Attribute;
                        writer.WritePropertyName(AttributeValueType.U16.ToString().ToLower());
                        writer.WriteValue(u16.Value);
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}