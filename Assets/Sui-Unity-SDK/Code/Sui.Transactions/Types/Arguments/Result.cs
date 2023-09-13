using System;
using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Types.Arguments
{
    public class Result : ITransactionArgument
    {
        public ITransactionArgument.Type Kind
        {
            get => ITransactionArgument.Type.Result;
        }

        public int Index { get; private set; }
        public Result(int index)
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