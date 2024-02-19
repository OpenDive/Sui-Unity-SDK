using OpenDive.BCS;

namespace Sui.Transactions.Types.Arguments
{
    /// <summary>
    /// One of the input objects or primitive values (from `ProgrammableTransactionBlock` inputs) 
    /// <code>
    ///     { kind: 'GasCoin' }
    /// </code>
    /// </summary>
    public class GasCoin : ITransactionArgument
    {
        public Kind Kind => Kind.GasCoin;

        public GasCoin() { }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)Kind.GasCoin); ;
        }

        public static GasCoin Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeUleb128();
            return new GasCoin();
        }
    }
}