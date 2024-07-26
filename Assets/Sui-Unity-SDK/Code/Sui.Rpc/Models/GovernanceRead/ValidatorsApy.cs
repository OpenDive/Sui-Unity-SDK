using System.Numerics;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Rpc.Client;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a collection of `ValidatorApy` instances associated with a specific epoch.
    /// </summary>
    [JsonObject]
    public class ValidatorsApy
    {
        /// <summary>
        /// An list containing `ValidatorApy` instances, each of which represents
        /// the Annual Percentage Yield (APY) of a specific validator.
        /// </summary>
        [JsonProperty("apys")]
        public ValidatorApy[] APYs { get; internal set; }

        /// <summary>
        /// A `BigInteger` representing the epoch associated with the APYs.
        /// </summary>
        [JsonProperty("epoch")]
        public BigInteger Epoch { get; internal set; }
    }

    /// <summary>
    /// Represents the Annual Percentage Yield (APY) related information for a validator.
    /// </summary>
    [JsonObject]
    public class ValidatorApy : ReturnBase
    {
        /// <summary>
        /// An `AccountAddress` representing the address of the validator.
        /// </summary>
        [JsonProperty("address")]
        public AccountAddress Address { get; internal set; }

        /// <summary>
        /// A `ulong` value representing the Annual Percentage Yield (APY) of the validator.
        /// The APY is a percentage that indicates the profitability of the validator on an annual basis,
        /// allowing delegators to assess the potential return on their staked assets.
        /// </summary>
        [JsonProperty("apy")]
        public double APY { get; internal set; }
    }
}