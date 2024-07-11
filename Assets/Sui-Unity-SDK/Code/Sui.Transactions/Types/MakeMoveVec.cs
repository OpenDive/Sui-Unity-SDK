using System.Linq;
using OpenDive.BCS;
using Sui.Transactions.Types.Arguments;

namespace Sui.Transactions.Types
{
    public class MakeMoveVec : ITransaction
    {
        public Kind Kind => Kind.MakeMoveVec;

        public SuiTransactionArgument[] Objects;
        public SuiStructTag Type;

        /// <summary>
        /// Byt default "type" is not used.
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="type"></param>
        public MakeMoveVec(SuiTransactionArgument[] objects, SuiStructTag type = null)
        {
            this.Objects = objects;
            this.Type = type;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8(0);
            serializer.Serialize(Objects);
            if (Type != null) serializer.Serialize(Type);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeU8();
            return new MakeMoveVec(
                deserializer.DeserializeSequence(typeof(SuiTransactionArgument)).Cast<SuiTransactionArgument>().ToArray(),
                SuiStructTag.Deserialize(deserializer)
            );
        }
    }
}