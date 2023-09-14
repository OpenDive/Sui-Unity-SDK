using OpenDive.BCS;
using Sui.BCS;

namespace Sui.Transactions.Types
{
    public class MergeCoins : ITransaction
    {
        IObjectRef Destination;
        IObjectRef[] Sources;
        public MergeCoins(IObjectRef destination, IObjectRef[] sources)
        {

        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Destination);
            serializer.Serialize(Sources);
            throw new System.NotImplementedException();
        }
    }
}
