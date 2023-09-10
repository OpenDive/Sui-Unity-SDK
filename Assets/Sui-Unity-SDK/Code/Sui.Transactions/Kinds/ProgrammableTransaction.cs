using System;
using OpenDive.BCS;
using Sui.Transactions.Builder.TransactionObjects;

namespace Sui.Transactions.Kinds
{
    public class ProgrammableTransaction : ITransactionKind, ISerializable
    {
        /// <summary>
        /// Can be a pure type (native BCS), or a Sui object (shared, or ImmutableOwned)
        /// Both type extend ISerialzable interface.
        /// </summary>
        public ISerializable[] Inputs { get; private set; }

        /// <summary>
        /// Holds a set of transactions, e.g. MoveCallTransaction, TransferObjectsTransaction, etc.
        /// </summary>
        public ITransaction[] Transactions { get; private set; }

        public ProgrammableTransaction(ISerializable[] inputs, ITransaction[] transactions)
        {
            Inputs = inputs;
            Transactions = transactions;
        }

        public void Serialize(Serialization serializer)
        {
            Sequence inputSeq = new Sequence(Inputs);
            Sequence transactionSeq = new Sequence(Transactions);

            // Serialize the kind enum -- for programmable transaction  it's 0
            serializer.SerializeU8(0);
            serializer.Serialize(inputSeq);
            serializer.Serialize(transactionSeq);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            throw new NotImplementedException();
        }
    }
}