using OpenDive.BCS;
using Sui.Accounts;

namespace Sui.Transactions.Types
{
    /// <summary>
    /// TransferObjects transaction.
    /// <code>
    ///     const tx = new TransactionBlock();
    ///     const coin = tx.add(Transactions.SplitCoins(tx.gas, [tx.pure(100)]));
    ///     tx.add(Transactions.TransferObjects([coin], tx.object('0x2')));
    ///
    ///     const txb = new TransactionBlock();
    ///     const [coin] = txb.splitCoins(txb.gas, [txb.pure(1)]);
    ///     txb.transferObjects([coin], txb.pure(currentAccount!.address));
    /// 
    ///     export const TransferObjectsTransaction = object({
	///         kind: literal('TransferObjects'),
    ///         objects: array(ObjectTransactionArgument),
    ///         address: PureTransactionArgument(BCS.ADDRESS),
    ///     });
    /// </code>
    /// </summary>
public class TransferObjects : ITransaction
    {
        public Kind Kind => Kind.TransferObjects;

        /// <summary>
        /// 
        /// </summary>
        public ITransaction[] Objects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public AccountAddress Address { get; set; }

        //public TransactionBlockInput Address { get; set; }

        /// <summary>
        /// Creates TransferObjects transaction.
        /// </summary>
        /// <param name="objects">Objects that we are transferring.</param>
        /// <param name="address">The recepient address (AccountAddress) nested within the TransactionBlockInput.
        /// This will be what Sui refers to as "Pure". TODO: Check if we need to simply encode it as byte array.
        /// </param>
        //public TransferObjects(ITransaction[] objects, TransactionBlockInput address)
        public TransferObjects(ITransaction[] objects, AccountAddress address)
        {
            Objects = objects;
            Address = address;

            //SuiObjectRef objectRef = new SuiObjectRef();
            //ObjectCallArg addressObjectCallArg = new ObjectCallArg(new SuiObjectRef(address));

            //Type callArgType = address.Value.GetType();
            //if (callArgType == typeof(PureCallArg)
            //    && ((PureCallArg)address.Value).Value.GetType() == typeof(AccountAddress)) {
            //    Address = address;
            //}
            //else
            //{
            //    throw new ArgumentException("TransactionBlockInput must be of Pure type and and AccountAddress");
            //}
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Objects);
            serializer.Serialize(Address);
        }
    }
}