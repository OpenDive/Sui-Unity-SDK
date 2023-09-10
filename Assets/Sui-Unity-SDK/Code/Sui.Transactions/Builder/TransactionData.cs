using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Builder
{
    public class TransactionData : ISerializable
    {
        public AccountAddress Sender { get; set; }
        public TransactionExpiration Expiration { get; set; }
        public GasConfig GasData { get; set; }

        /// <summary>
        /// This can be a ProgrammableTransaction,
        /// or ChangeEpoch, Genesis, or ConsensusCommitPrologue
        /// </summary>
        public ITransactionKind Transaction { get; set; }

        public TransactionData(
            AccountAddress sender,
            TransactionExpiration transactionExpiration,
            GasConfig gasdata,
            ITransactionKind transaction)
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

            Serialization ser = new Serialization();
            ser.SerializeU8(0); // We add the version number V1 - 0 byte
            Transaction.Serialize(ser);
            Sender.Serialize(ser);
            GasData.Serialize(ser);
            Expiration.Serialize(ser);

            Debug.Log("===  TransactionData");
            Debug.Log(ser.GetBytes().ByteArrayToString());
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            throw new NotImplementedException();
        }
    }
}