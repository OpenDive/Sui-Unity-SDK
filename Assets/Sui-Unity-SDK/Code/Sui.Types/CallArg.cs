using OpenDive.BCS;

namespace Sui.Types
{
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
    public interface ICallArg : ISerializable
    {
        public enum Type
        {
            Pure,
            Object,
            ObjectVec
        }
    }

    /// <summary>
    /// A pure value takes in U8, U256, BString, AccountAddress, etc
    /// </summary>
    public class PureCallArg : ICallArg
    {
        public ISerializable Value { get; set; }
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
            // TODO: Add enum byte of Pure => 0, ObjectRef => 1
            serializer.SerializeU32AsUleb128((uint)ICallArg.Type.Pure);
            serializer.Serialize(Value);
        }
    }

    /// <summary>
    /// Abstraction for an object argument (ObjectArg).
    /// An object argument is a type of call argument (CallArg)
    /// </summary>
    //public abstract class ObjectArg : ICallArg
    //{
    //    public abstract string ObjectId { get; set; }

    //    public void Serialize(Serialization serializer)
    //    {
    //        serializer.SerializeU32AsUleb128(1);
    //    }
    //}

    /// <summary>
    /// Base interfaces that both a SuiObjectRef and a SharedObjectRef must implement.
    /// This is used to create an abstraction that will allow it to become an argument.
    /// In the Sui TypeScript SDK this is referred to as an `ObjectArg`, it's schema
    /// is below:
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
        public IObjectRef ObjectArg { get; set; }

        public ObjectCallArg(IObjectRef objectArg)
        {
            ObjectArg = objectArg;
        }

        public enum Type
        {
            ImmOrOwned,     // SuiObjectRef
            Shared          // SharedObjectRef
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)ICallArg.Type.Object);

            System.Type objectType = ObjectArg.GetType();

            if (objectType == typeof(SuiObjectRef))
                serializer.SerializeU32AsUleb128((uint)ObjectCallArg.Type.ImmOrOwned);
            else
                serializer.SerializeU32AsUleb128((uint)ObjectCallArg.Type.Shared);

            serializer.Serialize(ObjectArg);
        }
    }

}