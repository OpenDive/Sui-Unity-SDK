using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Client;
using Sui.Transactions.Kinds;
using Sui.Types;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Builder
{
    public enum TransactionType
    {
        V1
    }

    public interface ITransactionData : ISerializable { }

    public class TransactionData: ISerializable
    {
        public TransactionType Type { get; set; }
        public ITransactionData Transaction;

        public TransactionData
        (
            TransactionType type,
            ITransactionData transaction
        )
        {
            this.Type = type;
            this.Transaction = transaction;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8((byte)this.Type);
            serializer.Serialize(Transaction);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8();
            switch (type)
            {
                case 0:
                    return new TransactionData
                    (
                        TransactionType.V1,
                        (TransactionDataV1)ISerializable.Deserialize(deserializer)
                    );
                default:
                    return new SuiError(0, "Unable to deserialize Transaction Data", null);
            }
        }
    }

    public class TransactionDataV1 : ITransactionData
    {
        public AccountAddress Sender { get; set; }
        public ITransactionExpiration Expiration { get; set; }
        public GasData GasData { get; set; }
        public TransactionBlockKind Transaction { get; set; }

        public TransactionDataV1
        (
            AccountAddress sender,
            ITransactionExpiration transactionExpiration,
            GasData gasdata,
            TransactionBlockKind transaction
        )
        {
            this.Sender = sender;
            this.Expiration = transactionExpiration;
            this.GasData = gasdata;
            this.Transaction = transaction;
        }

        public void Serialize(Serialization serializer)
        {
            Transaction.Serialize(serializer);
            Sender.Serialize(serializer);
            GasData.Serialize(serializer);
            Expiration.Serialize(serializer);
        }

        public static TransactionDataV1 Deserialize(Deserialization deserializer)
        {
            return new TransactionDataV1(
                AccountAddress.Deserialize(deserializer),
                (ITransactionExpiration)ISerializable.Deserialize(deserializer),
                (GasData)GasData.Deserialize(deserializer),
                (TransactionBlockKind)ISerializable.Deserialize(deserializer)
            );
        }
    }
}