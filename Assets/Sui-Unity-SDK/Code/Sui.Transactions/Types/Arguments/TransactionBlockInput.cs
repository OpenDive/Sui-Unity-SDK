
using OpenDive.BCS;
using Sui.Types;

namespace Sui.Transactions.Types.Arguments
{
    /// <summary>
    /// An abstraction of a Transaction block input.
    /// A transaction block input is a:
    /// a TransactionInput,
    ///
    /// 
    /// In the TypeScript SDK the explicity define whether it's a pure or object,
    /// but in this SDK it automatically infers its own type, and also knows its "kind".
    /// 
    /// <code>
    ///     export const TransactionBlockInput = object({
	///         kind: literal('Input'),
    ///         index: integer(),
    ///         value: optional(any()),
    ///         type: optional(union([literal('pure'), literal('object')])),
    ///     });
    /// </code>
    /// </summary>
    public class TransactionBlockInput : ITransactionArgument
    {
        /// <summary>
        /// Transaction block input kind.
        /// </summary>
        public Kind Kind => Kind.Input;

        /// <summary>
        /// The index within the programmable block transaction input list.
        /// </summary>
        public int Index { get; private set; }

        /// <summary>
        /// The value of this transaction input.
        /// </summary>
        /// TODO: Look into this
        //public ICallArg Value { get; set; } // An object ref, or a core type like address or u8
        public ISerializable Value { get; set; } // An object ref, or a core type like address or u8

        /// <summary>
        /// Create a TransactionBlockInput object
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value"></param>
        //public TransactionBlockInput(int index, ICallArg value) TODO: Look into this
        public TransactionBlockInput(int index, ISerializable value)
        {
            this.Index = index;
            this.Value = value;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)Kind.Input);
            //serializer.SerializeU16(Convert.ToUInt16(Index));

        }
    }
}