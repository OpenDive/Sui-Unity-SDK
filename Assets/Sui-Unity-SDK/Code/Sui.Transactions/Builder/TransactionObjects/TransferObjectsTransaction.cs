
using OpenDive.BCS;

namespace Sui.Transactions.Builder.TransactionObjects
{
    public class TransferObjectsTransaction : TransactionBase, ISerializable
    {
        public override string EncodeTransaction()
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}