using System.Linq;
using OpenDive.BCS;
using Sui.Types;

namespace Sui.Transactions.Types
{
    public class MakeMoveVec : ITransaction
    {
        public IObjectRef[] Objects;
        public string Type;

        /// <summary>
        /// Byt default "type" is not used.
        /// </summary>
        /// <param name="objects"></param>
        /// <param name="type">TODO: Check if this is a normalized struct tag, (SuiStructTag)</param>
        public MakeMoveVec(IObjectRef[] objects, string type = null)
        {
            this.Objects = objects;
            this.Type = type;
        }

        public MakeMoveVec(string[] objects, string type = null)
        {
            // Grab that list of strings, create a list of Tx Objects
            //this.Objects = objects;

            //this.Type = type;
            IObjectRef[] objectRefs = objects.Select(o => new SuiObjectRef(null, 0, null)).ToArray();
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128(0);
            serializer.Serialize(Objects);
            if (Type != null) serializer.SerializeString(Type);
        }
    }
}