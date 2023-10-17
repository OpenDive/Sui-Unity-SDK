using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;
using OpenDive.BCS;
using System;

namespace Sui.Rpc.Models
{
    public class NormalizedMoveModuleConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}

