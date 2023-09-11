using OpenDive.BCS;

namespace Sui.Transactions.Builder.TransactionObjects
{
    public interface ITransactionType : ISerializable
    {
        public string EncodeTransaction();
    }
}