using System;
using OpenDive.BCS;
using Sui.Transactions.Builder;

namespace Sui.Types
{
    /// <summary>
    ///
    /// <code>
    ///     const TransactionDataV1 = {
    ///         kind: 'TransactionKind',
    ///         sender: BCS.ADDRESS,
    ///         gasData: 'GasData',
    ///         expiration: 'TransactionExpiration',
    ///     };
    /// </code>
    /// </summary>
    public class TransactionDataV1 : ISerializable
    {
        public string sender;
        public GasData gasData;
        public TransactionExpiration expiration;

        public TransactionDataV1(string sender, GasData gasData, TransactionExpiration expiration)
        {
            this.sender = sender;
            this.gasData = gasData;
            this.expiration = expiration;
        }

        public void Serialize(Serialization serializer)
        {
            throw new NotImplementedException();
        }
    }
}