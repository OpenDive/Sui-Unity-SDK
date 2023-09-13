using Sui.Transactions.Types.Arguments;

namespace Sui.Transactions.Types
{
    /// <summary>
    /// A TransactionObject can be:
    /// MakeMove, MergeCoin, MoveCall, Publish, SplitCOins, TransferObject, Upgrade
    /// </summary>
    public interface ITransaction : ITransactionArgument {
        //public enum Type
        //{
        //    MoveCallTransaction,
        //    TransferObjectsTransaction,
        //    SplitCoinsTransaction,
        //    MergeCoinsTransaction,
        //    PublishTransaction,
        //    UpgradeTransaction,
        //    MakeMoveVecTransaction,
        //}
    }
}