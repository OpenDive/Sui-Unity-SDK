using System;
using OpenDive.BCS;

namespace Sui.Transactions.Types
{
    public enum Kind
    {
        MoveCall,
        TransferObjects,
        SplitCoins,
        MergeCoins,
        Publish,
        Upgrade,
        MakeMoveVec,
    }

    /// <summary>
    /// A TransactionObject can be:
    /// MakeMove, MergeCoin, MoveCall, Publish, SplitCOins, TransferObject, Upgrade
    /// </summary>
    public interface ITransaction : ISerializable {
        public Kind Kind { get; }
    }

    public class SuiTransaction: ISerializable
    {
        public ITransaction Transaction;

        public SuiTransaction(ITransaction transaction)
        {
            this.Transaction = transaction;
        }

        public void Serialize(Serialization serializer)
        {
            switch(Transaction.Kind)
            {
                case Kind.MoveCall:
                    serializer.SerializeU8(0);
                    break;
                case Kind.TransferObjects:
                    serializer.SerializeU8(1);
                    break;
                case Kind.SplitCoins:
                    serializer.SerializeU8(2);
                    break;
                case Kind.MergeCoins:
                    serializer.SerializeU8(3);
                    break;
                case Kind.Publish:
                    serializer.SerializeU8(4);
                    break;
                case Kind.MakeMoveVec:
                    serializer.SerializeU8(5);
                    break;
                case Kind.Upgrade:
                    serializer.SerializeU8(6);
                    break;
            }
            serializer.Serialize(Transaction);
        }

        public static SuiTransaction Deserialize(Deserialization deserializer)
        {
            var value = deserializer.DeserializeU8();
            switch (value)
            {
                case 0:
                    return new SuiTransaction(
                        (MoveCall)MoveCall.Deserialize(deserializer)
                    );
                case 1:
                    return new SuiTransaction(
                        (TransferObjects)TransferObjects.Deserialize(deserializer)
                    );
                case 2:
                    return new SuiTransaction(
                        SplitCoins.Deserialize(deserializer)
                    );
                case 3:
                    return new SuiTransaction(
                        MergeCoins.Deserialize(deserializer)
                    );
                case 4:
                    return new SuiTransaction(
                        Publish.Deserialize(deserializer)
                    );
                case 5:
                    return new SuiTransaction(
                        (MakeMoveVec)MakeMoveVec.Deserialize(deserializer)
                    );
                case 6:
                    return new SuiTransaction(
                        (Upgrade)Upgrade.Deserialize(deserializer)
                    );
                default:
                    throw new NotImplementedException();
            }
        }
    }
}