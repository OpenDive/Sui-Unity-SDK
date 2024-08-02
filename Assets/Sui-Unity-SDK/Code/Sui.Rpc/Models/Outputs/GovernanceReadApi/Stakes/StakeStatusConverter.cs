//
//  StakeStatusConverter.cs
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

using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    public class StakeStatusConverter : JsonConverter
    {
        public readonly string StakeStatusProperty = "status";

        public readonly string StakeObjectPrincipalProperty = "principal";
        public readonly string StakeObjectStakeActiveEpochProperty = "stakeActiveEpoch";
        public readonly string StakeObjectStakeRequestEpochProperty = "stakeRequestEpoch";
        public readonly string StakeObjectStakedSuiIDProperty = "stakedSuiId";
        public readonly string StakeObjectEstimatedRewardProperty = "estimatedReward";

        public override bool CanConvert(Type objectType) => objectType == typeof(StakeStatus);

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartObject)
            {
                JObject stake_status = (JObject)JToken.ReadFrom(reader);

                switch (stake_status[this.StakeStatusProperty].Value<string>())
                {
                    case "Active":
                        return new StakeStatus
                        (
                            StakeStatusType.Active,
                            new ActiveStakeObject
                            (
                                BigInteger.Parse(stake_status[this.StakeObjectPrincipalProperty].Value<string>()),
                                BigInteger.Parse(stake_status[this.StakeObjectStakeActiveEpochProperty].Value<string>()),
                                BigInteger.Parse(stake_status[this.StakeObjectStakeRequestEpochProperty].Value<string>()),
                                (AccountAddress)stake_status[this.StakeObjectStakedSuiIDProperty].ToObject(typeof(AccountAddress)),
                                stake_status[this.StakeObjectEstimatedRewardProperty] != null ?
                                    BigInteger.Parse(stake_status[this.StakeObjectEstimatedRewardProperty].Value<string>()) :
                                    null
                            )
                        );
                    case "Pending":
                        return new StakeStatus
                        (
                            StakeStatusType.Pending,
                            new PendingStakeObject
                            (
                                BigInteger.Parse(stake_status[this.StakeObjectPrincipalProperty].Value<string>()),
                                BigInteger.Parse(stake_status[this.StakeObjectStakeActiveEpochProperty].Value<string>()),
                                BigInteger.Parse(stake_status[this.StakeObjectStakeRequestEpochProperty].Value<string>()),
                                (AccountAddress)stake_status[this.StakeObjectStakedSuiIDProperty].ToObject(typeof(AccountAddress)),
                                stake_status[this.StakeObjectEstimatedRewardProperty] != null ?
                                    BigInteger.Parse(stake_status[this.StakeObjectEstimatedRewardProperty].Value<string>()) :
                                    null
                            )
                        );
                    case "Unstaked":
                        return new StakeStatus
                        (
                            StakeStatusType.Unstaked,
                            new UnstakedStakeObject
                            (
                                BigInteger.Parse(stake_status[this.StakeObjectPrincipalProperty].Value<string>()),
                                BigInteger.Parse(stake_status[this.StakeObjectStakeActiveEpochProperty].Value<string>()),
                                BigInteger.Parse(stake_status[this.StakeObjectStakeRequestEpochProperty].Value<string>()),
                                (AccountAddress)stake_status[this.StakeObjectStakedSuiIDProperty].ToObject(typeof(AccountAddress)),
                                stake_status[this.StakeObjectEstimatedRewardProperty] != null ?
                                    BigInteger.Parse(stake_status[this.StakeObjectEstimatedRewardProperty].Value<string>()) :
                                    null
                            )
                        );
                    default:
                        return new StakeStatus
                        (
                            new SuiError
                            (
                                0,
                                $"Cannot convert {stake_status[this.StakeStatusProperty].Value<string>()} to StakeStatusType.",
                                stake_status
                            )
                        );
                }
            }

            return new StakeStatus(new SuiError(0, "Unable to convert JSON to StakeStatus.", reader));
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
                writer.WriteNull();
            else
            {
                StakeStatus stake_status = (StakeStatus)value;

                writer.WriteStartObject();

                writer.WritePropertyName(this.StakeStatusProperty);
                writer.WriteValue(stake_status.Type.ToString());

                writer.WritePropertyName(this.StakeObjectPrincipalProperty);
                writer.WriteValue(stake_status.Stake.Principal.ToString());

                writer.WritePropertyName(this.StakeObjectStakeActiveEpochProperty);
                writer.WriteValue(stake_status.Stake.StakeActiveEpoch.ToString());

                writer.WritePropertyName(this.StakeObjectStakeRequestEpochProperty);
                writer.WriteValue(stake_status.Stake.StakeRequestEpoch.ToString());

                writer.WritePropertyName(this.StakeObjectStakedSuiIDProperty);
                writer.WriteValue(stake_status.Stake.StakedSuiID.KeyHex);

                writer.WritePropertyName(this.StakeObjectEstimatedRewardProperty);
                writer.WriteValue(stake_status.Stake.EstimatedReward.ToString());

                writer.WriteEndObject();
            }
        }
    }
}