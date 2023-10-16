using System.Collections.Generic;
using Newtonsoft.Json;
using System.Numerics;
using OpenDive.BCS;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class Checkpoint
    {
        [JsonProperty("checkpointCommitments")]
        public ISerializable[] CheckpointCommitments { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        [JsonProperty("endOfEpochData", Required = Required.Default)]
        public EndOfEpochData EndOfEpochData { get; set; }

        [JsonProperty("epoch")]
        public BigInteger Epoch { get; set; }

        [JsonProperty("epochRollingGasCostSummary")]
        public GasCostSummary EpochRollingGasCostSummary { get; set; }

        [JsonProperty("networkTotalTransactions")]
        public BigInteger NetworkTotalTransactions { get; set; }

        [JsonProperty("previousDigest", Required = Required.AllowNull)]
        public string PreviousDigest { get; set; }

        [JsonProperty("sequenceNumber")]
        public BigInteger SequenceNumber { get; set; }

        [JsonProperty("timestampMs")]
        public BigInteger TimestampMs { get; set; }

        [JsonProperty("transactions")]
        public string[] Transactions { get; set; }

        [JsonProperty("validatorSignature")]
        public string ValidatorSignature { get; set; }
    }

    [JsonObject]
    public class EndOfEpochData
    {
        [JsonProperty("nextEpochCommittee")]
        public string[][] NextEpochCommittee { get; set; }

        [JsonProperty("nextEpochProtocolVersion")]
        public string NextEpochProtocolVersion { get; set; }

        [JsonProperty("epochCommitments")]
        public ISerializable EpochCommitments { get; set; }
    }
}