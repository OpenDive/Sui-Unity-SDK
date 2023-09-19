using System;
using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;
using Sui.Transactions.Types.Arguments;

namespace Sui.Transactions.Types
{
    /// <summary>
    /// Models a SplitCoins transaction and implements it's serialization.
    /// This class is leveraged by the `TransactionBuilder` to create
    /// a SplitCoins transaction that returns `n` of results based on
    /// the amounts being passed the constructors. For example:
    /// if `uint[] amounts` is of length 1, then only one `TransactionResult
    /// will be created, is it's o length 10 then 10 results will be created.
    /// 
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
        public Kind Kind => Kind.SplitCoins;

        /// <summary>
        /// Coin transaction argument.
        /// </summary>
        public GasCoin Coin;

        /// <summary>
        /// Amount of coins to use to split into.
        /// </summary>
        public ulong[] Amounts;

        /// <summary>
        /// Create a SplitCoins transaction.
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="amounts"></param>
        public SplitCoins(GasCoin coin, ulong[] amounts)
        {
            // Check that it's an actual in or U16, U32, U64 wrapped
            Coin = coin;
            Amounts = amounts;
        }

        public static List<SplitCoins> Create(GasCoin coin, ulong[] amounts)
        {
            throw new NotImplementedException();
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
