using System.Linq;
using OpenDive.BCS;
using Sui.Types;

namespace Sui.Transactions.Types
{
    public class MakeMoveVec : ITransaction
    {
        public Kind Kind => Kind.MakeMoveVec;

        public IObjectRef[] Objects;
        public SuiStructTag Type;

        /// <summary>
        /// Byt default "type" is not used.
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="type"></param>
        public MakeMoveVec(IObjectRef[] objects, SuiStructTag type = null)
        {
            this.Objects = objects;
            this.Type = type;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128(5);
            serializer.Serialize(Objects);
            if (Type != null) serializer.Serialize(Type);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeUleb128();
            return new MakeMoveVec(
                deserializer.DeserializeSequence(typeof(IObjectRef)).Cast<IObjectRef>().ToArray(),
                SuiStructTag.Deserialize(deserializer)
            );
        }
    }
}