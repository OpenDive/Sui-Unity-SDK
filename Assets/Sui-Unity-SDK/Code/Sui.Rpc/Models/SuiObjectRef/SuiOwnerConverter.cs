using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace Sui.Rpc.Models
{
    public class SuiOwnerConverter : JsonConverter<Owner>
    {
        public override Owner ReadJson(JsonReader reader, Type objectType, Owner existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string initial_shared_version_parameter = "initial_shared_version";

            if (reader.TokenType == JsonToken.StartObject)
            {
                Newtonsoft.Json.Linq.JObject owner_object = Newtonsoft.Json.Linq.JObject.Load(reader);
                if (owner_object.ContainsKey(SuiOwnerType.Shared.ToString()))
                {
                    return new Owner(owner_object[SuiOwnerType.Shared.ToString()][initial_shared_version_parameter].Value<int>());
                }
                else if (owner_object.ContainsKey(SuiOwnerType.AddressOwner.ToString()))
                {
                    return new Owner(SuiOwnerType.AddressOwner, owner_object[SuiOwnerType.AddressOwner.ToString()].Value<string>());
                }
                else if (owner_object.ContainsKey(SuiOwnerType.ObjectOwner.ToString()))
                {
                    return new Owner(SuiOwnerType.ObjectOwner, owner_object[SuiOwnerType.ObjectOwner.ToString()].Value<string>());
                }
            }

            return new Owner();
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, Owner value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}