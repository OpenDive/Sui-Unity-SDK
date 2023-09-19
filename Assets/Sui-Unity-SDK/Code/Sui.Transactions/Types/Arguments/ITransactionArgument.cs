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

}