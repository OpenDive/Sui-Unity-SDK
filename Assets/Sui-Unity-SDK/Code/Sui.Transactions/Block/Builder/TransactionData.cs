using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Types;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Builder
{
    public interface ITransactionData : ISerializable
    {
        public enum Type
        {
            V1
        }
    }

    public class TransactionDataV1 : ITransactionData
    {
        public AccountAddress Sender { get; set; }
        public ITransactionExpiration Expiration { get; set; }
        public GasConfig GasData { get; set; }

        /// <summary>
        /// This can be a ProgrammableTransaction,
        /// or ChangeEpoch, Genesis, or ConsensusCommitPrologue
        /// </summary>
        public Sui.Transactions.Kinds.ITransactionKind Transaction { get; set; }

        public TransactionDataV1
        (
            AccountAddress sender,
            ITransactionExpiration transactionExpiration,
            GasConfig gasdata,
            Sui.Transactions.Kinds.ITransactionKind transaction
        )
        {
            this.Sender = sender;
            this.Expiration = transactionExpiration;
            this.GasData = gasdata;
            this.Transaction = transaction;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8(0); // We add the version number V1 - 0 byte
            Transaction.Serialize(serializer);
            Sender.Serialize(serializer);
            GasData.Serialize(serializer);
            Expiration.Serialize(serializer);
        }

        public static TransactionDataV1 Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeUleb128();
            return new TransactionDataV1(
                AccountAddress.Deserialize(deserializer),
                (ITransactionExpiration)ISerializable.Deserialize(deserializer),
                (GasConfig)GasConfig.Deserialize(deserializer),
                (Sui.Transactions.Kinds.ITransactionKind)ISerializable.Deserialize(deserializer)
            );
        }
    }
}