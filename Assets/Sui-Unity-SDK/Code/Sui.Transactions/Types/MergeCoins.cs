using System.Linq;
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
            this.Destination = destination;
            this.Sources = sources;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128(3);
            serializer.Serialize(Destination);
            serializer.Serialize(Sources);
        }

        public static MergeCoins Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeUleb128();
            return new MergeCoins(
                (IObjectRef)IObjectRef.Deserialize(deserializer),
                deserializer.DeserializeSequence(typeof(IObjectRef)).Cast<IObjectRef>().ToArray()
            );
        }
    }
}
