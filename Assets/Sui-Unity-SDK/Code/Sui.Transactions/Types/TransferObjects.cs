using OpenDive.BCS;
using Sui.Accounts;

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
        public AccountAddress Address { get; set; }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="address"></param>
        public TransferObjects(ITransactionType[] objects, AccountAddress address)
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