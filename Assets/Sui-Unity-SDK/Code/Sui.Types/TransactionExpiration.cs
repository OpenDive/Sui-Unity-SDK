using System;
using OpenDive.BCS;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Types
{
    public interface ITransactionExpiration: ISerializable
    {
        public enum Type
        {
            None,
            Epoch
        }
    }

    public class TransactionExpirationNone: ITransactionExpiration
    {
        public ITransactionExpiration.Type ExpirationType = ITransactionExpiration.Type.None;

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8(0);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new TransactionExpirationNone();
        }
    }

    public class TransactionExpirationEpoch: ITransactionExpiration
    {
        public ITransactionExpiration.Type ExpirationType = ITransactionExpiration.Type.Epoch;
        public int Epoch { get; set; }

        public TransactionExpirationEpoch(int epoch)
        {
            this.Epoch = epoch;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8(1);
            serializer.SerializeU64((ulong)Epoch);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeUleb128();
            return new TransactionExpirationEpoch(
                (int)deserializer.DeserializeU64()
            );
        }
    }
}