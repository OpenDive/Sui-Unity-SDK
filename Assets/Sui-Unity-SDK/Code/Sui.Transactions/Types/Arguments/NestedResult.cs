using System;
using OpenDive.BCS;

namespace Sui.Transactions.Types.Arguments
{
    /// <summary>
    /// Like a `Result` (`TransactionResult` but it accesses a nested result.
    /// Currently, the only usage of this is to access a value from
    /// a Move call with multiple return values.
    ///
    /// <code>
    ///     // Schema
    ///     NestedResult: {
    ///         index: BCS.U16,
    ///         resultIndex: BCS.U16
    ///     },
    ///
    ///     // Object definition
    /// 	object({
    /// 	    kind: literal('NestedResult'),
    /// 	    index: integer(),
    /// 	    resultIndex: integer(),
    /// 	}),
    /// </code>
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

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU16((ushort)Index);
            serializer.SerializeU16((ushort)ResultIndex);
        }

        public static NestedResult Deserialize(Deserialization deserializer)
        {
            return new NestedResult(
                deserializer.DeserializeU16(),
                deserializer.DeserializeU16()
            );
        }
    }
}