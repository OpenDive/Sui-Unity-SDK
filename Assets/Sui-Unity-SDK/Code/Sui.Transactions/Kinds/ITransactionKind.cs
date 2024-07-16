using System;
using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Rpc.Client;
using Sui.Types;
using UnityEngine;
using Newtonsoft.Json;
using Sui.Rpc.Models;
using Newtonsoft.Json.Linq;

namespace Sui.Transactions.Kinds
{
    public enum SuiTransactionKindType
    {
        ProgrammableTransaction,
        ChangeEpoch,
        Genesis,
        ConsensusCommitPrologue
    }

    public class TransactionKindConverter : JsonConverter
    {
        public override bool CanConvert(System.Type objectType)
        {
            return objectType == typeof(TransactionBlockKind);
        }

        public override object ReadJson(JsonReader reader, System.Type objectType, object existingValue, JsonSerializer serializer)
        {
            JObject jsonObject = JObject.Load(reader);
            string kind = jsonObject["kind"].Value<string>();

            switch (kind)
            {
                case "ProgrammableTransaction":
                    return new TransactionBlockKind
                    (
                        SuiTransactionKindType.ProgrammableTransaction,
                        jsonObject.ToObject<ProgrammableTransaction>(serializer)
                    );
                case "ChangeEpoch":
                    return new TransactionBlockKind
                    (
                        SuiTransactionKindType.ChangeEpoch,
                        jsonObject.ToObject<SuiChangeEpoch>(serializer)
                    );
                case "Genesis":
                    return new TransactionBlockKind
                    (
                        SuiTransactionKindType.Genesis,
                        jsonObject.ToObject<Genesis>(serializer)
                    );
                case "ConsensusCommitPrologue":
                    return new TransactionBlockKind
                    (
                        SuiTransactionKindType.ConsensusCommitPrologue,
                        jsonObject.ToObject<SuiConsensusCommitPrologue>(serializer)
                    );
                default:
                    return new SuiError(0, "Unable to convert JSON to TransactionBlockKind", jsonObject);
            }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }

    public interface ITransactionKind : ISerializable { }

    [JsonConverter(typeof(TransactionKindConverter))]
    public class TransactionBlockKind: ISerializable
    {
        public SuiTransactionKindType Type { get; private set; }

        private ITransactionKind transaction;

        public ITransactionKind Transaction
        {
            get => this.transaction;
            set
            {
                if (value.GetType() == typeof(ProgrammableTransaction))
                    this.Type = SuiTransactionKindType.ProgrammableTransaction;
                else if (value.GetType() == typeof(SuiChangeEpoch))
                    this.Type = SuiTransactionKindType.ChangeEpoch;
                else if (value.GetType() == typeof(Genesis))
                    this.Type = SuiTransactionKindType.Genesis;
                else if (value.GetType() == typeof(SuiConsensusCommitPrologue))
                    this.Type = SuiTransactionKindType.ConsensusCommitPrologue;
                else
                    throw new Exception("Unable to set Transaction Kind");

                this.transaction = value;
            }
        }

        public TransactionBlockKind
        (
            SuiTransactionKindType type,
            ITransactionKind transaction
        )
        {
            this.Type = type;
            this.Transaction = transaction;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8((byte)this.Type);
            serializer.Serialize(this.Transaction);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8();
            switch (type)
            {
                case 0:
                    return new TransactionBlockKind
                    (
                        SuiTransactionKindType.ProgrammableTransaction,
                        (ProgrammableTransaction)ISerializable.Deserialize(deserializer)
                    );
                case 1:
                    return new TransactionBlockKind
                    (
                        SuiTransactionKindType.ChangeEpoch,
                        (SuiChangeEpoch)ISerializable.Deserialize(deserializer)
                    );
                case 2:
                    return new TransactionBlockKind
                    (
                        SuiTransactionKindType.Genesis,
                        (Genesis)ISerializable.Deserialize(deserializer)
                    );
                case 3:
                    return new TransactionBlockKind
                    (
                        SuiTransactionKindType.ConsensusCommitPrologue,
                        (SuiConsensusCommitPrologue)ISerializable.Deserialize(deserializer)
                    );
                default:
                    return new SuiError(0, "Unable to deserialize TransactionBlockKind", null);
            }
        }
    }
}
