using OpenDive.BCS;

namespace Sui.Transactions.Types
{
    /// <summary>
    /// A TransactionObject can be:
    /// MakeMove, MergeCoin, MoveCall, Publish, SplitCOins, TransferObject, Upgrade
    /// </summary>
    public interface ITransactionType : ISerializable
    {
    }
}