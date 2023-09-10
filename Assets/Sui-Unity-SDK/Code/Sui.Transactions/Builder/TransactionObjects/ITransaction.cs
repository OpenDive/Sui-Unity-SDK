using OpenDive.BCS;

namespace Sui.Transactions.Builder.TransactionObjects
{
    public interface ITransaction : ISerializable
    {
        public string EncodeTransaction();
    }
}