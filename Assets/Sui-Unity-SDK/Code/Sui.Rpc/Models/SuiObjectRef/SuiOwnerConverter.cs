using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Unity.Plastic.Newtonsoft.Json.Linq;

namespace Sui.Rpc.Models
{
    public class SuiOwnerConverter : JsonConverter<Owner>
    {
        public readonly string InitialSharedVersion = "initial_shared_version";

        public override Owner ReadJson(JsonReader reader, Type objectType, Owner existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                Newtonsoft.Json.Linq.JObject owner_object = Newtonsoft.Json.Linq.JObject.Load(reader);
                if (owner_object.ContainsKey(SuiOwnerType.Shared.ToString()))
                {
                    return new Owner(owner_object[SuiOwnerType.Shared.ToString()][this.InitialSharedVersion].Value<int>());
                }
                else if (owner_object.ContainsKey(SuiOwnerType.AddressOwner.ToString()))
                {
                    return new Owner(SuiOwnerType.AddressOwner, (AccountAddress)owner_object[SuiOwnerType.AddressOwner.ToString()].ToObject(typeof(AccountAddress)));
                }
                else if (owner_object.ContainsKey(SuiOwnerType.ObjectOwner.ToString()))
                {
                    return new Owner(SuiOwnerType.ObjectOwner, (AccountAddress)owner_object[SuiOwnerType.ObjectOwner.ToString()].ToObject(typeof(AccountAddress)));
                }
            }

            return new Owner();
        }

        public override void WriteJson(JsonWriter writer, Owner value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                writer.WriteStartObject();

                switch (value.Type)
                {
                    case SuiOwnerType.Shared:
                        writer.WritePropertyName(SuiOwnerType.Shared.ToString());
                        writer.WriteStartObject();

                        writer.WritePropertyName(this.InitialSharedVersion);
                        writer.WriteValue(value.Shared.InitialSharedVersion);

                        writer.WriteEndObject();
                        break;
                    case SuiOwnerType.AddressOwner:
                        writer.WritePropertyName(SuiOwnerType.AddressOwner.ToString());
                        writer.WriteValue(value.Address.KeyHex);
                        break;
                    case SuiOwnerType.ObjectOwner:
                        writer.WritePropertyName(SuiOwnerType.ObjectOwner.ToString());
                        writer.WriteValue(value.Address.KeyHex);
                        break;
                    case SuiOwnerType.Immutable:
                        writer.WriteValue(SuiOwnerType.Immutable.ToString());
                        break;
                }

                writer.WriteEndObject();
            }
        }
    }
}