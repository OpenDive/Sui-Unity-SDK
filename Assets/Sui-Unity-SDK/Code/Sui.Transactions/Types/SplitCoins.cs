using System;
using System.Linq;
using OpenDive.BCS;

namespace Sui.Transactions.Types
{
    /// <summary>
    /// SplitCoins transaction.
    /// Note that a ITransactionType is also an ITransactionArgument,
    /// which means that it can be used as an input into other transactions
    /// <code>
	///     const tx = new TransactionBlock();
    ///     const coin = tx.add(Transactions.SplitCoins(tx.gas, [tx.pure(100)]));
    ///     tx.add(Transactions.TransferObjects([coin], tx.object ('0x2')));
    /// </code>
    /// </summary>
    public class SplitCoins : ITransaction
    {
        /// <summary>
        /// SplitCoins transaction kind.
        /// </summary>
        public ITransaction.Kind Kind { get => ITransaction.Kind.SplitCoins; }

        /// <summary>
        /// Coin transaction argument.
        /// </summary>
        public ITransaction Coin;

        /// <summary>
        /// Amount of coins to use to split into.
        /// </summary>
        public ulong[] Amounts;

        /// <summary>
        /// Create a SplitCoins transaction.
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="amounts"></param>
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
