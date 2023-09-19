using System;
using OpenDive.BCS;

namespace Sui.Transactions.Types
{
    /// <summary>
    /// Publishes a Move contract.
    /// </summary>
    public class Publish : ITransaction
    {
        public Kind Kind => Kind.Publish;

        public Publish(byte[][] modules, string[] dependencies) // dependencies are ObjectId
        {
            throw new NotImplementedException();
        }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}