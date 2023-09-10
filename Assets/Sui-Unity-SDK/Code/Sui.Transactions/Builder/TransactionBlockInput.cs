
using OpenDive.BCS;

namespace Sui.Transactions.Builder
{
    /// <summary>
    /// An abstraction of a Transaction block input.
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
    public class TransactionBlockInput : ISerializable
    {
        public static string Kind { get => "Input";  }
        public int Index { get; private set; }
        public ISerializable Value { get; private set; } // An object ref, or a core type like address or u8

        public TransactionBlockInput(int index, ISerializable value)
        {
            this.Index = index;
            this.Value = value;
        }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }
    }
}