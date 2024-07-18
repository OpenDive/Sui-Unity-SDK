using System;
using OpenDive.BCS;
using Sui.Rpc.Client;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Types
{
    public enum ExpirationType
    {
        None,
        Epoch
    }

    public class TransactionExpiration: ISerializable
    {
        public ExpirationType Type { get; private set; }

        private int? epoch;

        public int? Epoch
        {
            get => this.epoch;
            set
            {
                if (value == null)
                    this.Type = ExpirationType.None;
                else
                    this.Type = ExpirationType.Epoch;

                this.epoch = value;
            }
        }

        public TransactionExpiration(int? epoch_value = null)
        {
            this.Epoch = epoch_value;
        }

        public void Serialize(Serialization serializer)
        {
            switch (this.Type)
            {
                case ExpirationType.None:
                    serializer.SerializeU8(0);
                    break;
                case ExpirationType.Epoch:
                    serializer.SerializeU8(1);
                    serializer.SerializeU64((ulong)this.Epoch);
                    break;
            }
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8().Value;

            switch (type)
            {
                case 0:
                    return new TransactionExpiration();
                case 1:
                    return new TransactionExpiration((int)deserializer.DeserializeU64().Value);
                default:
                    return new SuiError(0, "Unable to deserialize TransactionExpiration", null);
            }
        }
    }
}