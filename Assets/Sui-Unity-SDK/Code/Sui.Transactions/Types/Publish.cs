using System;
using OpenDive.BCS;
using Sui.Accounts;
using System.Collections.Generic;
using System.Linq;

namespace Sui.Transactions.Types
{
    /// <summary>
    /// Publishes a Move contract.
    /// </summary>
    public class Publish : ITransaction
    {
        public byte[][] Modules;
        public AccountAddress[] Dependencies;

        public Publish(byte[][] modules, AccountAddress[] dependencies) // dependencies are ObjectId
        {
            this.Modules = modules;
            this.Dependencies = dependencies;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)Modules.Length);

            foreach(byte[] module in Modules)
                serializer.Serialize(module);

            serializer.Serialize(Dependencies);
        }

        public static Publish Deserialize(Deserialization deserializer)
        {
            List<byte[]> modules = new List<byte[]>();
            int length = deserializer.DeserializeUleb128();

            for(int i = 0; i < length; ++i)
                modules.Add(deserializer.ToBytes());

            return new Publish(
                modules.ToArray(),
                deserializer.DeserializeSequence(typeof(AccountAddress)).Values.Cast<AccountAddress>().ToArray()
            );
        }
    }
}