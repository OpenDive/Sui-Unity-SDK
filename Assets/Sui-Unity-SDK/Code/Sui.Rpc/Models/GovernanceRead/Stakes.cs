using System;
using System.Numerics;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Sui.Accounts;
using Sui.Rpc.Client;
using UnityEngine;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A class representing the information of delegated stakes.
    /// </summary>
    [JsonObject]
    public class DelegatedStake
    {
        /// <summary>
        /// An `AccountAddress` representing the address of the validator managing the delegated stakes.
        /// </summary>
        [JsonProperty("validatorAddress")]
        public AccountAddress ValidatorAddress { get; internal set; }

        /// <summary>
        /// An `AccountAddress` representing the staking pool to which the stakes are delegated.
        /// </summary>
        [JsonProperty("stakingPool")]
        public AccountAddress StakingPool { get; internal set; }

        /// <summary>
        /// A list of `StakeStatus` representing the status of each stake delegated.
        /// </summary>
        [JsonProperty("stakes")]
        public StakeStatus[] Stakes { get; internal set; }
    }

    /// <summary>
    /// Represents a stake object, detailing information regarding a staking process or state,
    /// and conforming to the `Equatable` protocol to allow comparisons between instances.
    /// </summary>
    public abstract class StakeObject : ReturnBase
    {
        /// <summary>
        /// A `BigInteger` representing the principal amount within the stake
        /// itself, represented by SUI.
        /// </summary>
        [JsonProperty("principal")]
        public BigInteger Principal { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the epoch in which the stake became active.
        /// </summary>
        [JsonProperty("stakeActiveEpoch")]
        public BigInteger StakeActiveEpoch { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the epoch in which the stake request was made.
        /// </summary>
        [JsonProperty("stakeRequestEpoch")]
        public BigInteger StakeRequestEpoch { get; internal set; }

        /// <summary>
        /// An `AccountAddress` representing the unique identifier associated with this stake in the Sui
        /// Blockchain, enabling reference and lookup of this specific stake instance.
        /// </summary>
        [JsonProperty("stakedSuiId")]
        public AccountAddress StakedSuiID { get; internal set; }

        /// <summary>
        /// An optional `BigInteger` representing the estimated reward amount for the given stake.
        /// </summary>
        [JsonProperty("estimatedReward", NullValueHandling = NullValueHandling.Include)]
        public BigInteger? EstimatedReward { get; internal set; }

        public StakeObject
        (
            BigInteger principal,
            BigInteger stake_active_epoch,
            BigInteger stake_request_epoch,
            AccountAddress staked_sui_id,
            BigInteger? estimated_reward
        )
        {
            this.Principal = principal;
            this.StakeActiveEpoch = stake_active_epoch;
            this.StakeRequestEpoch = stake_request_epoch;
            this.StakedSuiID = staked_sui_id;
            this.EstimatedReward = estimated_reward;
        }
    }

    /// <summary>
    /// The stake is in a pending state and is not yet active.
    /// </summary>
    public class PendingStakeObject : StakeObject
    {
        public PendingStakeObject
        (
            BigInteger principal,
            BigInteger stake_active_epoch,
            BigInteger stake_request_epoch,
            AccountAddress staked_sui_id,
            BigInteger? estimated_reward
        ) :
        base
        (
            principal,
            stake_active_epoch,
            stake_request_epoch,
            staked_sui_id,
            estimated_reward
        ) { }

        public override bool Equals(object obj)
        {
            if (obj is not PendingStakeObject)
                return this.SetError<bool, SuiError>(false, "Compared object is not an PendingStakeObject.", obj);

            PendingStakeObject other_stake_object_pending = (PendingStakeObject)obj;

            return
                this.Principal.Equals(other_stake_object_pending.Principal) &&
                this.StakeActiveEpoch.Equals(other_stake_object_pending.StakeActiveEpoch) &&
                this.StakeRequestEpoch.Equals(other_stake_object_pending.StakeRequestEpoch) &&
                this.StakedSuiID.Equals(other_stake_object_pending.StakedSuiID) &&
                this.EstimatedReward.Equals(other_stake_object_pending.EstimatedReward);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// The stake is active and participating.
    /// </summary>
    public class ActiveStakeObject : StakeObject
    {
        public ActiveStakeObject
        (
            BigInteger principal,
            BigInteger stake_active_epoch,
            BigInteger stake_request_epoch,
            AccountAddress staked_sui_id,
            BigInteger? estimated_reward
        ) :
        base
        (
            principal,
            stake_active_epoch,
            stake_request_epoch,
            staked_sui_id,
            estimated_reward
        )
        { }

        public override bool Equals(object obj)
        {
            if (obj is not ActiveStakeObject)
                return this.SetError<bool, SuiError>(false, "Compared object is not an ActiveStakeObject.", obj);

            ActiveStakeObject other_stake_object_active = (ActiveStakeObject)obj;

            return
                this.Principal.Equals(other_stake_object_active.Principal) &&
                this.StakeActiveEpoch.Equals(other_stake_object_active.StakeActiveEpoch) &&
                this.StakeRequestEpoch.Equals(other_stake_object_active.StakeRequestEpoch) &&
                this.StakedSuiID.Equals(other_stake_object_active.StakedSuiID) &&
                this.EstimatedReward.Equals(other_stake_object_active.EstimatedReward);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// The stake has been unstaked and is no longer participating.
    /// </summary>
    public class UnstakedStakeObject : StakeObject
    {
        public UnstakedStakeObject
        (
            BigInteger principal,
            BigInteger stake_active_epoch,
            BigInteger stake_request_epoch,
            AccountAddress staked_sui_id,
            BigInteger? estimated_reward
        ) :
        base
        (
            principal,
            stake_active_epoch,
            stake_request_epoch,
            staked_sui_id,
            estimated_reward
        )
        { }

        public override bool Equals(object obj)
        {
            if (obj is not UnstakedStakeObject)
                return this.SetError<bool, SuiError>(false, "Compared object is not an UnstakedStakeObject.", obj);

            UnstakedStakeObject other_stake_object_unstaked = (UnstakedStakeObject)obj;

            return
                this.Principal.Equals(other_stake_object_unstaked.Principal) &&
                this.StakeActiveEpoch.Equals(other_stake_object_unstaked.StakeActiveEpoch) &&
                this.StakeRequestEpoch.Equals(other_stake_object_unstaked.StakeRequestEpoch) &&
                this.StakedSuiID.Equals(other_stake_object_unstaked.StakedSuiID) &&
                this.EstimatedReward.Equals(other_stake_object_unstaked.EstimatedReward);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    /// <summary>
    /// `StakeStatus` represents the status of a stake in a staking system.
    ///
    /// - `pending`: The stake is in a pending state and is not yet active.
    /// - `active`: The stake is active and participating in the staking system.
    /// - `unstaked`: The stake has been unstaked and is no longer participating.
    /// </summary>
    [JsonConverter(typeof(StakeStatusConverter))]
    public class StakeStatus : ReturnBase
    {
        public StakeStatusType Type { get; internal set; }

        public StakeObject Stake { get; internal set; }

        public StakeStatus
        (
            StakeStatusType type,
            StakeObject stake
        )
        {
            this.Type = type;
            this.Stake = stake;
        }

        public StakeStatus(SuiError error)
        {
            this.Error = error;
        }

        public override bool Equals(object obj)
        {
            if (obj is not StakeStatus)
                return this.SetError<bool, SuiError>(false, "Compared object is not an StakeStatus.", obj);

            StakeStatus other_stake_status = (StakeStatus)obj;

            return
                this.Type == other_stake_status.Type &&
                this.Stake.Equals(other_stake_status.Stake);
        }

        public override int GetHashCode() => base.GetHashCode();
    }

    public enum StakeStatusType
    {
        Pending,
        Active,
        Unstaked
    }

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