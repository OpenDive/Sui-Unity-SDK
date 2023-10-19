using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using UnityEngine;
using static Sui.Rpc.Models.CommitteeInfo;

namespace Sui.Rpc.Models
{
    /// <summary>
    ///
    /// <code>
    /// {
    ///     "jsonrpc": "2.0",
    ///     "result": {
    ///         "apys": [
    ///             {
    ///                 "address": "0xb7d1cb695b9491893f88a5ae1b9d4f235b3c7e00acf5386662fa062483ba507b",
    ///                 "apy": 0.06
    ///             },
    ///             {
    ///                 "address": "0x1e9e3039750f0a270f2e12441ad7f611a5f7fd0b2c4326c56b1fec231d73038d",
    ///                 "apy": 0.02
    ///             },
    ///             {
    ///                 "address": "0xba0f0885b97982f5fcac3ec6f5c8cae16743671832358f25bfacde706e528df4",
    ///                 "apy": 0.05
    ///             }
    ///         ],
    ///         "epoch": "420"
    ///     }
    /// }
    /// </code>
    /// </summary>
    [JsonObject]
    public class ValidatorsApy
    {
        [JsonProperty("apys")]
        public ValidatorApy[] Apys;

        [JsonProperty("epoch")]
        public BigInteger Epoch;

        [JsonObject, JsonConverter(typeof(ValidatorApyConverter))]
        public class ValidatorApy
        {
            [JsonProperty("address")]
            public AccountAddress Address;

            [JsonProperty("apy")]
            public double Apy; // f64
        }

        private class ValidatorApyConverter : JsonConverter
        {
            public override bool CanConvert(Type objectType)
            {
                return (objectType == typeof(ValidatorApy));
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                JObject jo = JObject.Load(reader);
                AccountAddress address = AccountAddress.FromHex((string)jo["address"]);
                Debug.Log("APY VALUE: " + (string)jo["apy"]);
                double apy = double.Parse((string)jo["apy"]);

                ValidatorApy validatorApy = new ValidatorApy();
                validatorApy.Address = address;
                validatorApy.Apy = apy;
                return validatorApy;
            }

            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                throw new NotImplementedException();
            }
        }
    }
}