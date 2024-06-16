using System.Linq;
using OpenDive.BCS;
using Sui.Transactions.Types.Arguments;
using Sui.Types;

namespace Sui.Transactions.Types
{
    public class MergeCoins : ITransaction
    {
        public Kind Kind => Kind.MergeCoins;

        ITransactionArgument Destination;
        ITransactionArgument[] Sources;

        public MergeCoins(ITransactionArgument destination, ITransactionArgument[] sources)
        {
            this.Destination = destination;
            this.Sources = sources;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Destination);
            serializer.Serialize(Sources);
        }

        public static MergeCoins Deserialize(Deserialization deserializer)
        {
            return new MergeCoins(
                (ITransactionArgument)ITransactionArgument.Deserialize(deserializer),
                deserializer.DeserializeSequence(typeof(ITransactionArgument)).Cast<ITransactionArgument>().ToArray()
            );
        }
    }
}
