using OpenDive.BCS;
using Sui.Transactions.Builder.TransactionObjects;

namespace Sui.Transactions.Kinds
{
    public class ProgrammableTransaction : ITransactionKind, ISerializable
    {
        /// <summary>
        /// Can be a pure type (native BCS), or a Sui object (shared, or ImmutableOwned)
        /// Both type extend ISerialzable interface.
        /// </summary>
        public ISerializable[] Inputs { get; private set; }

        public ITransaction[] Transactions { get; private set; }

        public ProgrammableTransaction(ISerializable[] inputs, ITransaction[] transactions)
        {
            Inputs = inputs;
            Transactions = transactions;
        }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}