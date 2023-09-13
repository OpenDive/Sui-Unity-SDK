using OpenDive.BCS;

namespace Sui.Transactions.Types.Arguments
{
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
            throw new System.NotImplementedException();
        }
    }
}