using OpenDive.BCS;

namespace Sui.Types
{
    public enum Type
    {
        Pure,
        Object,
        ObjectVec
    }

    /// <summary>
    /// A Sui type that represents a transaction call arguments.
    /// A call arg can be a (1) vector / list of byte (BCS U8),
    /// (2) an ObjectRef (also known as object arg, (3) or a vector / list of ObjectRef.
    ///
    /// The following is the TypeScript SDK schema
    /// <code>
    ///     CallArg: {
    ///		    Pure: [VECTOR, BCS.U8],
    ///		    Object: 'ObjectArg',
    ///		    ObjVec: [VECTOR, 'ObjectArg'],
    ///	    },
    /// </code>
    ///
    /// In our implementation CallArg by default takes in a list of args / ISerializable object.
    /// </summary>
    public interface ICallArg: ISerializable
    {
        public Type Type { get; }
    }

    /// <summary>
    /// A pure value takes in U8, U256, BString, AccountAddress, etc
    /// </summary>
    public class PureCallArg : ICallArg
    {
        public ISerializable Value { get; set; }
        public Type Type
        {
            get => Type.Pure;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public PureCallArg(ISerializable value)
        {
            Value = value;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)Type.Pure);
            serializer.Serialize(Value);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeUleb128();
            return new PureCallArg(
                ISerializable.Deserialize(deserializer)
            );
        }
    }

    /// <summary>
    /// Base interfaces that both a SuiObjectRef and a SharedObjectRef must implement.
    /// This is used to create an abstraction that will allow it to become an argument.
    /// In the Sui TypeScript SDK this is referred to as an `ObjectArg`, it's schema
    /// is below:
    /// 
    /// <code>
    ///	    ObjectArg: {
    ///		    ImmOrOwned: 'SuiObjectRef',
    ///		    Shared: 'SharedObjectRef',
    ///	    },
    /// </code>
    ///
    /// This interface extends ISeriliazable which allows these objects that
    /// implement the interface to be passed as arguments (`CallArgs` in Sui TypeScript)
    /// </summary>
    ///
    public class ObjectCallArg : ICallArg
    {
        public ObjectArg ObjectArg { get; set; }

        public Type Type
        {
            get => Type.Object;
        }

        public ObjectCallArg(ObjectArg objectArg)
        {
            ObjectArg = objectArg;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)Type.Object);
            serializer.Serialize(this.ObjectArg);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            deserializer.DeserializeUleb128();
            return new PureCallArg(
                ISerializable.Deserialize(deserializer)
            );
        }
    }
}