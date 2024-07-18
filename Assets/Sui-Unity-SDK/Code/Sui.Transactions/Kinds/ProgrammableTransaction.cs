using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Transactions.Types;
using Sui.Types;

namespace Sui.Transactions.Kinds
{
    public class ProgrammableTransaction : ITransactionKind
    {
        /// <summary>
        /// Can be a pure type (native BCS), or a Sui object (shared, or ImmutableOwned)
        /// Both type extend ISerialzable interface.
        /// </summary>
        [JsonProperty("inputs")]
        public CallArg[] Inputs { get; private set; }

        /// <summary>
        /// Holds a set of transactions, e.g. MoveCallTransaction, TransferObjectsTransaction, etc.
        /// </summary>
        [JsonProperty("transactions")]
        public List<SuiTransaction> Transactions { get; private set; }

        public ProgrammableTransaction(CallArg[] inputs, List<SuiTransaction> transactions)
        {
            Inputs = inputs;
            Transactions = transactions;
        }

        public void Serialize(Serialization serializer)
        {
            Sequence inputSeq = new Sequence(Inputs);
            Sequence transactionSeq = new Sequence(Transactions.ToArray());

            serializer.Serialize(inputSeq);
            serializer.Serialize(transactionSeq);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new ProgrammableTransaction(
                deserializer.DeserializeSequence(typeof(CallArg)).Values.Cast<CallArg>().ToArray(),
                deserializer.DeserializeSequence(typeof(SuiTransaction)).Values.Cast<SuiTransaction>().ToList()
            );
        }
    }

    [JsonObject]
    public class SuiConsensusCommitPrologue : ITransactionKind
    {
        [JsonProperty("epoch")]
        public string Epoch { get; set; }

        [JsonProperty("round")]
        public string Round { get; set; }

        [JsonProperty("commit_timestamp_ms")]
        public string CommitTimestampMs { get; set; }

        public SuiConsensusCommitPrologue
        (
            string epoch,
            string round,
            string commit_timestamp_ms
        )
        {
            this.Epoch = epoch;
            this.Round = round;
            this.CommitTimestampMs = commit_timestamp_ms;
        }

        public SuiConsensusCommitPrologue() { }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeString(this.Epoch);
            serializer.SerializeString(this.Round);
            serializer.SerializeString(this.CommitTimestampMs);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            SuiConsensusCommitPrologue commit = new SuiConsensusCommitPrologue();

            commit.Epoch = deserializer.DeserializeString().Value;
            commit.Round = deserializer.DeserializeString().Value;
            commit.CommitTimestampMs = deserializer.DeserializeString().Value;

            return commit;
        }
    }

    [JsonObject]
    public class Genesis : ITransactionKind
    {
        [JsonProperty("objects")]
        public List<string> Objects { get; set; }

        public Genesis(List<string> objects)
        {
            this.Objects = objects;
        }

        public void Serialize(Serialization serializer)
        {
            Sequence objects_sequence = new Sequence(this.Objects.Select((obj) => new BString(obj)).ToArray());
            serializer.Serialize(objects_sequence);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new Genesis(
                deserializer.DeserializeSequence(typeof(BString)).Values.Cast<BString>().Select((obj) => obj.Value).ToList()
            );
        }
    }

    [JsonObject]
    public class SuiChangeEpoch : ITransactionKind
    {
        [JsonProperty("epoch")]
        public string Epoch { get; set; }

        [JsonProperty("storage_charge")]
        public string StorageCharge { get; set; }

        [JsonProperty("computation_charge")]
        public string ComputationCharge { get; set; }

        [JsonProperty("storage_rebate")]
        public string StorageRebate { get; set; }

        [JsonProperty("epoch_start_timestamp_ms")]
        public string EpochStartTimestampMs { get; set; }

        public SuiChangeEpoch
        (
            string epoch,
            string storage_charge,
            string computation_charge,
            string storage_rebate,
            string epoch_start_tinmestamp_ms = null
        )
        {
            this.Epoch = epoch;
            this.StorageCharge = storage_charge;
            this.ComputationCharge = computation_charge;
            this.StorageRebate = storage_rebate;
            this.EpochStartTimestampMs = epoch_start_tinmestamp_ms;
        }

        public SuiChangeEpoch() { }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeString(this.Epoch);
            serializer.SerializeString(this.StorageCharge);
            serializer.SerializeString(this.ComputationCharge);
            serializer.SerializeString(this.StorageRebate);

            if (this.EpochStartTimestampMs != null) serializer.SerializeString(this.EpochStartTimestampMs);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            SuiChangeEpoch result = new SuiChangeEpoch();

            result.Epoch = deserializer.DeserializeString().Value;
            result.StorageCharge = deserializer.DeserializeString().Value;
            result.ComputationCharge = deserializer.DeserializeString().Value;
            result.StorageRebate = deserializer.DeserializeString().Value;
            result.EpochStartTimestampMs = ((BString)deserializer.DeserializeOptional(typeof(BString))).Value;

            return result;
        }
    }
}