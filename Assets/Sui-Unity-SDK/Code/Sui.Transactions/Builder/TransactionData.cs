using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;

namespace Sui.Transactions.Builder
{
    public class TransactionData : ISerializable
    {
        public AccountAddress Sender { get; set; }
        public TransactionExpiration Expiration { get; set; }
        public GasConfig GasData { get; set; }

        /// <summary>
        /// This can be a ProgrammaleTransaction,
        /// or ChangeEpoch, Genesis, or ConsensusCommitPrologue
        /// </summary>
        public ITransactionKind Transaction { get; set; }


        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8(0); // We add the version number V1 - 0 byte
            Transaction.Serialize(serializer);
            Sender.Serialize(serializer);
            GasData.Serialize(serializer);
            Expiration.Serialize(serializer);
            throw new System.NotImplementedException();
        }
    }
}