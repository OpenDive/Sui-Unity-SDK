using Sui.Transactions.Types.Arguments;

namespace Sui.Transactions.Types
{
    /// <summary>
    /// A TransactionObject can be:
    /// MakeMove, MergeCoin, MoveCall, Publish, SplitCOins, TransferObject, Upgrade
    /// </summary>
    public interface ITransaction : ITransactionArgument {
        //public enum Kind
        //{
        //    MoveCallTransaction,
        //    TransferObjectsTransaction,
        //    SplitCoinsTransaction,
        //    MergeCoinsTransaction,
        //    PublishTransaction,
        //    UpgradeTransaction,
        //    MakeMoveVecTransaction,
        //}

        public enum Kind
        {
            MoveCall,
            TransferObjects,
            SplitCoins,
            MergeCoins,
            Publish,
            Upgrade,
            MakeMoveVec,
        }
    }
}