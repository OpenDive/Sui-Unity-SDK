using OpenDive.BCS;
using Sui.Transactions.Types.Arguments;

namespace Sui.Transactions.Types
{
    /// <summary>
    /// SplitCoins transaction.
    /// Note that a ITransactionType is also an ITransactionArgument,
    /// which means that it can be used as an input into other transactions
    /// </summary>
    public class SplitCoins : ITransaction
    {
        public SplitCoins(ITransactionArgument coin, U64[] amounts)
        {
            // Check that it's an actual in or U16, U32, U64 wrapped
        }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}
