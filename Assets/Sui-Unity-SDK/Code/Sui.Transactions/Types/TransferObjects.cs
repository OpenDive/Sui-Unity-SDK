using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;
using Sui.Transactions.Builder;

namespace Sui.Transactions.Types
{
    public class TransferObjects : ITransaction
    {
        /// <summary>
        /// 
        /// </summary>
        public ITransaction[] Objects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        //public AccountAddress Address { get; set; }
        public TransactionBlockInput Address { get; set; }

        /// <summary>
        /// Creates TransferObjects transaction.
        /// </summary>
        /// <param name="objects">Objects that we are transferring.</param>
        /// <param name="address">The recepient address (AccountAddress) nested within the TransactionBlockInput.
        /// This will be what Sui refers to as "Pure". TODO: Check if we need to simply encode it as byte array.
        /// </param>
        public TransferObjects(ITransaction[] objects, TransactionBlockInput address)
        {
            Objects = objects;

            Type callArgType = address.Value.GetType();
            if (callArgType == typeof(PureCallArg)
                && ((PureCallArg)address.Value).Value.GetType() == typeof(AccountAddress)) {
                Address = address;
            }
            else
            {
                throw new ArgumentException("TransactionBlockInput must be of Pure type and and AccountAddress");
            }
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Objects);
            serializer.Serialize(Address);
        }
    }
}