using System;
using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Utilities;
using UnityEngine;

/// <summary>
/// TODO: Consider renaming to TransactionBlockInput
/// TODO: Missing implementation
/// </summary>
namespace Sui.Transactions.Types.Arguments
{
    public class Input : ITransactionArgument
    {
        public ITransactionArgument.Type Kind
        {
            get => ITransactionArgument.Type.Input;
        }

        public int Index;
        public ISerializable Value; // Only used to prepare the transaction

        public Input(int index, ISerializable value = null)
        {
            Index = index;
            Value = null;
        }
        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)ITransactionArgument.Type.Input);
            serializer.SerializeU16(Convert.ToUInt16(Index));

            Serialization ser = new Serialization();
            ser.SerializeU32AsUleb128((uint)ITransactionArgument.Type.Input);
            ser.SerializeU16(Convert.ToUInt16(Index));

            Debug.Log("==== Input");
            Debug.Log(ser.GetBytes().ByteArrayToString());
        }
    }

}