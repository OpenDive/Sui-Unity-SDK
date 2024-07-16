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
        /// Coin transaction argument.
        /// </summary>
        public SuiTransactionArgument Coin;

        /// <summary>
        /// Amount of coins to use to split into.
        /// </summary>
        public SuiTransactionArgument[] Amounts;

        /// <summary>
        /// Create a SplitCoins transaction.
        /// </summary>
        /// <param name="coin"></param>
        /// <param name="amounts"></param>
        public SplitCoins(SuiTransactionArgument coin, SuiTransactionArgument[] amounts)
        {
            // Check that it's an actual in or U16, U32, U64 wrapped
            Coin = coin;
            Amounts = amounts;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Coin);
            serializer.Serialize(Amounts);
        }

        public static SplitCoins Deserialize(Deserialization deserializer)
        {
            return new SplitCoins(
                (SuiTransactionArgument)SuiTransactionArgument.Deserialize(deserializer),
                deserializer.DeserializeSequence(typeof(SuiTransactionArgument)).Cast<SuiTransactionArgument>().ToArray()
            );
        }
    }
}
