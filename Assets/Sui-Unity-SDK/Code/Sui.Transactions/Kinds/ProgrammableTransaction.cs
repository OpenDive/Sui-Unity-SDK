using System;
using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;
using Sui.Transactions.Types;
using Sui.Types;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Kinds
{
    public class ProgrammableTransaction : ITransactionKind, ISerializable
    {
        /// <summary>
        /// Can be a pure type (native BCS), or a Sui object (shared, or ImmutableOwned)
        /// Both type extend ISerialzable interface.
        /// </summary>
        public ICallArg[] Inputs { get; private set; }

        /// <summary>
        /// Holds a set of transactions, e.g. MoveCallTransaction, TransferObjectsTransaction, etc.
        /// </summary>
        public List<SuiTransaction> Transactions { get; private set; }

        public ProgrammableTransaction(ICallArg[] inputs, List<SuiTransaction> transactions)
        {
            Inputs = inputs;
            Transactions = transactions;
        }

        public void Serialize(Serialization serializer)
        {
            Sequence inputSeq = new Sequence(Inputs);
            Sequence transactionSeq = new Sequence(Transactions.ToArray());

            serializer.SerializeU8(0);
            serializer.Serialize(inputSeq);
            serializer.Serialize(transactionSeq);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeUleb128();
            return new ProgrammableTransaction(
                deserializer.DeserializeSequence(typeof(ICallArg)).Cast<ICallArg>().ToArray(),
                deserializer.DeserializeSequence(typeof(SuiTransaction)).Cast<SuiTransaction>().ToList()
            );
        }
    }
}