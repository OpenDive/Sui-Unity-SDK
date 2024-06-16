using System;
using OpenDive.BCS;

namespace Sui.Transactions.Types.Arguments
{
    public enum Kind
    {
        GasCoin,
        Input, // TransactionBlockInput
        Result,
        NestedResult
    }

    /// <summary>
    /// There are 4 types of transaction arguments.
    /// 1) GasCoin, 2) TransactionBlockInput, 3) Result, 4) NestedResult
    /// A TransactionArgument is an abstraction over these 4 types, and does
    /// not have a concrete implementation or serialization strategy.
    /// Each concrete type has it's own serialization strategy, particularly
    /// add a byte that represents it's type.
    /// </summary>
    public interface ITransactionArgument : ISerializable
    {
        public Kind Kind { get; }
    }

    public class SuiTransactionArgument: ISerializable
    {
        public ITransactionArgument TransactionArgument;

        public SuiTransactionArgument(ITransactionArgument transactionArgument)
        {
            this.TransactionArgument = transactionArgument;
        }

        public void Serialize(Serialization serializer)
        {
            switch (TransactionArgument.Kind)
            {
                case Kind.GasCoin:
                    break;
                case Kind.Input:
                    serializer.SerializeU8(1);
                    break;
                case Kind.Result:
                    serializer.SerializeU8(2);
                    break;
                case Kind.NestedResult:
                    serializer.SerializeU8(3);
                    break;
            }
            serializer.Serialize(TransactionArgument);
        }

        public static SuiTransactionArgument Deserialize(Deserialization deserializer)
        {
            var value = deserializer.DeserializeU8();
            switch (value)
            {
                case 0:
                    return new SuiTransactionArgument(new GasCoin());
                case 1:
                    return new SuiTransactionArgument(
                        TransactionBlockInput.Deserialize(deserializer)
                    );
                case 2:
                    return new SuiTransactionArgument(
                        Result.Deserialize(deserializer)
                    );
                case 3:
                    return new SuiTransactionArgument(
                        NestedResult.Deserialize(deserializer)
                    );
                default:
                    throw new NotImplementedException();
            }
        }
    }
}