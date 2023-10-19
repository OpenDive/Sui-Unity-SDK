using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using static Sui.Rpc.Models.ValidatorsApy;

namespace Sui.Rpc.Models
{
    /// <summary>
    ///
    /// <code>
    /// {
    ///     "jsonrpc": "2.0",
    ///     "result": {
    ///         "coinType": "0x168da5bf1f48dafc111b0a488fa454aca95e0b5e::usdc::USDC",
    ///         "coinObjectCount": 15,
    ///         "totalBalance": "15",
    ///         "lockedBalance": {}
    ///     }
    /// }
    /// </code>
    /// </summary>
    [JsonObject]
    public class CommitteeInfo
    {
        [JsonProperty("epoch")]
        public BigInteger Epoch;

        [JsonProperty("validators"), JsonConverter(typeof(VotingRightsConverter))]
        public VotingRights[] Validators;


        [JsonObject]
        public class VotingRights
        {
            [JsonProperty("authority_name")]
            // TODO: Ask about the type of the string returned
            //public AccountAddress AuthorityName;
            public string AuthorityName;

            // TODO: FROM SUI MYSTEN LABS DOCS: the stake and voting power of a validator can be different so
            // in some places when we are actually referring to the voting power, we
            // should use a different type alias, field name, etc.
            [JsonProperty("stake_unit")]
            public BigInteger StakeUnit;
        }


        private class VotingRightsConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                //return (objectType == typeof(ValidatorApy));
                // TODO: Implement
                throw new NotImplementedException();
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                List<VotingRights> votingRightsList = new List<VotingRights>();

                JToken token = JToken.Load(reader);
                JArray votingRightsArr = token as JArray;

                foreach(JArray item in votingRightsArr.Cast<JArray>())
                {
                    // NOTE: We expect it to follow the response example provided in the RPC docs
                    // "validators": [
                    //      [
                    //          "jc/20VUECmVvSBmxMRG1LFdGqGunLzlfuv4uw4R9HoFA5iSnUf32tfIFC8cgXPnTAATJCwx0Cv/TJs5nPMKyOi0k1T4q/rKG38Zo/UBgCJ1tKxe3md02+Q0zLlSnozjU",
                    //          "2500"
                    //      ],
                    //      ...
                    // ]
                    string authorityName = (string)item[0];
                    BigInteger stakeUnit = BigInteger.Parse((string)item[1]);

                    VotingRights votingRights = new VotingRights();
                    // TODO: Ask about the type of the string returned
                    //votingRights.AuthorityName = AccountAddress.FromBase58((authorityName));
                    votingRights.AuthorityName = authorityName;
                    votingRights.StakeUnit = stakeUnit;

                    votingRightsList.Add(votingRights);
                }

                return votingRightsList.ToArray();
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}