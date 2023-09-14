using System;
using OpenDive.BCS;

namespace Sui.Transactions.Types.Arguments
{
    /// <summary>
    /// Like a `Result` (`TransactionResult` but it accesses a nested result.
    /// Currently, the only usage of this is to access a value from
    /// a Move call with multiple return values.
    /// </summary>
    public class NestedResult : ITransactionArgument
    {
        public int Index;
        public int ResultIndex;

        public NestedResult(int index, int resultIndex)
        {
            Index = index;
            ResultIndex = resultIndex;
        }
        public ITransactionArgument.Type Kind
        {
            get => ITransactionArgument.Type.NestedResult;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)ITransactionArgument.Type.NestedResult);
            serializer.SerializeU16(Convert.ToUInt16(Index));
            serializer.SerializeU16(Convert.ToUInt16(ResultIndex));
        }
    }
}