using OpenDive.BCS;

namespace Sui.Transactions.Objects
{
    /// <summary>
    /// A TransactionObject can be:
    /// MakeMove, MergeCoin, MoveCall, Publish, SplitCOins, TransferObject, Upgrade
    /// </summary>
    public interface ITransactionObject : ISerializable
    {
    }
}