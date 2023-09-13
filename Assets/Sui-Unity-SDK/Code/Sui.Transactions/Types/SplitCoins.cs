using System;
using System.Linq;
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
        //public ITransactionArgument Coin;
        public ITransaction Coin;
        //public U64[] Amounts;
        public ulong[] Amounts;

        public SplitCoins(ITransaction coin, ulong[] amounts)
        {
            // Check that it's an actual in or U16, U32, U64 wrapped
            Coin = coin;
            Amounts = amounts;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Coin);
            U64[] u64Amounts = Amounts.Select(coin => new U64(coin)).ToArray();
            serializer.Serialize(u64Amounts);
        }

        public static SplitCoins Deserialize(Deserialization deserializer)
        {
            throw new NotImplementedException();
        }
    }
}
