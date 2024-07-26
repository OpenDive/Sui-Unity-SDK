using Newtonsoft.Json;
using System.Numerics;
using OpenDive.BCS;

namespace Sui.Rpc.Models
{
    [JsonObject]
    public class Checkpoint
    {
        [JsonProperty("checkpointCommitments")]
        public ISerializable[] CheckpointCommitments { get; internal set; }

        [JsonProperty("digest")]
        public string Digest { get; internal set; }

        [JsonProperty("endOfEpochData", Required = Required.Default)]
        public EndOfEpochData EndOfEpochData { get; internal set; }

        [JsonProperty("epoch")]
        public BigInteger Epoch { get; internal set; }

        [JsonProperty("epochRollingGasCostSummary")]
        public GasCostSummary EpochRollingGasCostSummary { get; internal set; }

        [JsonProperty("networkTotalTransactions")]
        public BigInteger NetworkTotalTransactions { get; internal set; }

        [JsonProperty("previousDigest", Required = Required.Default)]
        public string PreviousDigest { get; internal set; }

        [JsonProperty("sequenceNumber")]
        public BigInteger SequenceNumber { get; internal set; }

        [JsonProperty("timestampMs")]
        public BigInteger TimestampMs { get; internal set; }

        [JsonProperty("transactions")]
        public string[] Transactions { get; internal set; }

        [JsonProperty("validatorSignature")]
        public string ValidatorSignature { get; internal set; }
    }

    [JsonObject]
    public class EndOfEpochData
    {
        [JsonProperty("nextEpochCommittee")]
        public string[][] NextEpochCommittee { get; internal set; }

        [JsonProperty("nextEpochProtocolVersion")]
        public string NextEpochProtocolVersion { get; internal set; }

        [JsonProperty("epochCommitments")]
        public ISerializable EpochCommitments { get; internal set; }
    }

    [JsonObject]
    public class Checkpoints
    {
        [JsonProperty("data")]
        public Checkpoint[] Data { get; internal set; }

        [JsonProperty("nextCursor")]
        public string NextCursor { get; internal set; }

        [JsonProperty("hasNextPage")]
        public bool HasNextPage { get; internal set; }
    }
}