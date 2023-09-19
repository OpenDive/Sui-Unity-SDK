using OpenDive.BCS;
using Sui.Types;

namespace Sui.Transactions.Types
{
    public class MergeCoins : ITransaction
    {
        public Kind Kind => Kind.MergeCoins;

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
