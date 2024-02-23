using System;
using OpenDive.BCS;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Types.Arguments
{
    /// <summary>
    /// The result of another transaction (from `ProgrammableTransactionBlock` transactions)
    /// </summary>
    public class TransactionResult : ITransactionArgument
    {
        Kind ITransactionArgument.Kind => Kind.Result;

        public int Index { get; private set; }

        /// <summary>
        /// Represents a result from a given transaction.
        /// The index is the location of the result within the
        /// </summary>
        /// <param name="index"></param>
        public TransactionResult(int index)
        {
            Index = index;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU16((ushort)Index);
        }

        public static TransactionResult Deserialize(Deserialization deserializer)
        {
            return new TransactionResult(deserializer.DeserializeU16());
        }
    }
}