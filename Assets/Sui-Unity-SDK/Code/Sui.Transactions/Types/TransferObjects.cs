using System.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Types.Arguments;

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
        /// <summary>
        /// 
        /// </summary>
        public SuiTransactionArgument[] Objects { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public SuiTransactionArgument Address { get; set; }

        /// <summary>
        /// Creates TransferObjects transaction.
        /// </summary>
        /// <param name="objects">Objects that we are transferring.</param>
        /// <param name="address">The recepient address (AccountAddress) nested within the TransactionBlockInput.
        /// This will be what Sui refers to as "Pure".
        /// </param>
        //public TransferObjects(ITransaction[] objects, TransactionBlockInput address)
        public TransferObjects(SuiTransactionArgument[] objects, SuiTransactionArgument address)
        {
            this.Objects = objects;
            this.Address = address;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Objects);
            serializer.Serialize(Address);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new TransferObjects(
                deserializer.DeserializeSequence(typeof(SuiTransactionArgument)).Values.Cast<SuiTransactionArgument>().ToArray(),
                (SuiTransactionArgument)ISerializable.Deserialize(deserializer)
            );
        }
    }
}