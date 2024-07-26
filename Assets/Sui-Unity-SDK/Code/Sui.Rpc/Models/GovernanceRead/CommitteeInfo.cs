using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A class representing the committee information.
    /// </summary>
    [JsonObject]
    public class CommitteeInfo
    {
        /// <summary>
        /// A `BigInteger` representing the epoch related to the committee information.
        /// </summary>
        [JsonProperty("epoch")]
        public BigInteger Epoch { get; internal set; }

        /// <summary>
        /// A `VotingRights` list containing the validators and their corresponding voting rights.
        /// </summary>
        [JsonProperty("validators")]
        public VotingRights[] Validators { get; internal set; }

        // TODO: FROM SUI MYSTEN LABS DOCS: the stake and voting power of a validator can be different so
        // in some places when we are actually referring to the voting power, we
        // should use a different type alias, field name, etc.
        /// <summary>
        /// The class that represents the voting rights a validator has within the committee.
        /// </summary>
        [JsonConverter(typeof(VotingRightsConverter))]
        public class VotingRights : ReturnBase
        {
            // TODO: Ask about the type of the string returned
            /// <summary>
            /// The name of the authority represented as an encoded base 64
            /// Public Key.
            /// </summary>
            [JsonProperty("authority_name")]
            public string AuthorityName { get; internal set; }

            /// <summary>
            /// The amount of stake the authority is holding.
            /// This represents their power within the voting system.
            /// </summary>
            [JsonProperty("stake_unit")]
            public BigInteger StakeUnit { get; internal set; }

            public VotingRights
            (
                string authority_name,
                BigInteger stake_unit
            )
            {
                this.AuthorityName = authority_name;
                this.StakeUnit = stake_unit;
            }

            public VotingRights(SuiError error)
            {
                this.Error = error;
            }
        }

        private class VotingRightsConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType) => objectType == typeof(VotingRights);

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                if (reader.TokenType == JsonToken.StartArray)
                {
                    JArray voting_rights = JArray.Load(reader);

                    // NOTE: We expect it to follow the response example provided in the RPC docs
                    // "validators": [
                    //      [
                    //          "jc/20VUECmVvSBmxMRG1LFdGqGunLzlfuv4uw4R9HoFA5iSnUf32tfIFC8cgXPnTAATJCwx0Cv/TJs5nPMKyOi0k1T4q/rKG38Zo/UBgCJ1tKxe3md02+Q0zLlSnozjU",
                    //          "2500"
                    //      ],
                    //      ...
                    //      [
                    //          "rd7vlNiYyI5A297/kcXxBfnPLHR/tvK8N+wD1ske2y4aV4z1RL6LCTHiXyQ9WbDDDZihbOO6HWzx1/UEJpkusK2zE0sFW+gUDS218l+wDYP45CIr8B/WrJOh/0152ljy",
                    //          "2500"
                    //      ]
                    // ]
                    return new VotingRights
                    (
                        (string)voting_rights[0],
                        BigInteger.Parse((string)voting_rights[1])
                    );
                }

                return new VotingRights(new SuiError(0, "Unable to convert JSON to VotingRights.", reader));
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                if (value == null)
                    writer.WriteNull();
                else
                {
                    VotingRights voting_rights = (VotingRights)value;

                    writer.WriteStartArray();

                    writer.WriteValue(voting_rights.AuthorityName);
                    writer.WriteValue(voting_rights.StakeUnit.ToString());

                    writer.WriteEndArray();
                }
            }
        }
    }
}