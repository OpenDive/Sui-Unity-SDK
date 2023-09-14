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
        public ITransactionArgument.Type Kind
        {
            get => ITransactionArgument.Type.Result;
        }

        public int Index { get; private set; }
        public TransactionResult(int index)
        {
            Index = index;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)ITransactionArgument.Type.Result);
            serializer.SerializeU16(Convert.ToUInt16(Index));

            Serialization ser = new Serialization();
            ser.SerializeU32AsUleb128((uint)ITransactionArgument.Type.Result);
            ser.SerializeU16(Convert.ToUInt16(Index));
            Debug.Log("==== Result "  + ser.GetBytes().ByteArrayToString());
        }
    }
}