using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Cryptography;
using UnityEngine;

namespace Sui.Rpc.Models
{
    public class PublicKeyConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(PublicKeyBase);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            string value = reader.Value.ToString();
            return new Cryptography.Ed25519.PublicKey(value);
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            Cryptography.Ed25519.PublicKey address = (Cryptography.Ed25519.PublicKey)value;
            writer.WriteValue(address.KeyHex);
        }
    }
}