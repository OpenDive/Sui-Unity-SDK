using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using UnityEngine;

namespace Sui.Transactions.Builder
{
    public class Result : ITransactionArgument
    {
        public int Index { get; private set; }
        public Result(int index)
        {
            Index = index;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU16((ushort)Index);
        }
    }
}