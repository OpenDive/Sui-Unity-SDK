using System;
using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Types.Arguments;
using Sui.Types;

namespace Sui.Transactions.Types
{
    public class Upgrade : ITransaction
    {
        public Kind Kind => Kind.Upgrade;

        public byte[][] Modules;
        public AccountAddress[] Dependencies;
        public string PackagID;
        public ITransactionArgument Ticket;

        public Upgrade(
            byte[][] modules,
            AccountAddress[] dependencies,
            string packageId,
            ITransactionArgument ticket
        )
        {
            this.Modules = modules;
            this.Dependencies = dependencies;
            this.PackagID = packageId;
            this.Ticket = ticket;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU64((ulong)Modules.Length);

            foreach (byte[] module in Modules)
                serializer.Serialize(module);

            serializer.Serialize(Dependencies);
            serializer.Serialize(PackagID);
            serializer.Serialize(Ticket);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            List<byte[]> modules = new List<byte[]>();
            ulong length = deserializer.DeserializeU64();
            for (ulong i = 0; i < length; ++i)
            {
                modules.Add(deserializer.ToBytes());
            }
            return new Upgrade(
                modules.ToArray(),
                deserializer.DeserializeSequence(typeof(AccountAddress)).Cast<AccountAddress>().ToArray(),
                deserializer.DeserializeString(),
                (ITransactionArgument)ITransactionArgument.Deserialize(deserializer)
            );
        }
    }
}