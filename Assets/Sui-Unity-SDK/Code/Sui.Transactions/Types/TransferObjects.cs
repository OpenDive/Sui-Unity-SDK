using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Builder;

namespace Sui.Transactions.Types
{
    public class TransferObjects : ITransactionType
    {
        /// <summary>
        /// 
        /// </summary>
        public ITransactionType[] Objects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //public AccountAddress Address { get; set; }
        public TransactionBlockInput Address { get; set; }

        /// <summary>
        /// Creates TransferObjects transaction.
        /// </summary>
        /// <param name="objects">Objects that we are transferring.</param>
        /// <param name="address">The recepient address nested withing the TransactionBlockInput.
        /// This will be what Sui refers to as "Pure". TODO: Check if we need to simply encode it as byte array.
        /// </param>
        public TransferObjects(ITransactionType[] objects, TransactionBlockInput address)
        {
            Objects = objects;
            Address = address;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Objects);
            serializer.Serialize(Address);
        }
    }
}