using System.Linq;
using OpenDive.BCS;
using Sui.Transactions.Types.Arguments;
using Sui.Types;

namespace Sui.Transactions.Types
{
    public class MergeCoins : ITransaction
    {
        public Kind Kind => Kind.MergeCoins;

        SuiTransactionArgument Destination;
        SuiTransactionArgument[] Sources;

        public MergeCoins(SuiTransactionArgument destination, SuiTransactionArgument[] sources)
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
                (SuiTransactionArgument)SuiTransactionArgument.Deserialize(deserializer),
                deserializer.DeserializeSequence(typeof(SuiTransactionArgument)).Cast<SuiTransactionArgument>().ToArray()
            );
        }
    }
}
