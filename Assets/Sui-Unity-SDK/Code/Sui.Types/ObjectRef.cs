using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Utilities;
using Newtonsoft.Json;

namespace Sui.Types
{
    public enum ObjectRefType
    {
        ImmOrOwned = 0,     // SuiObjectRef
        Shared = 1          // SharedObjectRef
    }

    public interface IObjectRef : ISerializable
    {
        public AccountAddress ObjectID { get; set; }
    }

    public class ObjectArg : ISerializable
    {
        public ObjectRefType Type;
        public IObjectRef ObjectRef;

        public ObjectArg(ObjectRefType type, IObjectRef object_ref)
        {
            this.Type = type;
            this.ObjectRef = object_ref;
        }
        
        public void Serialize(Serialization serializer)
        {
            serializer.Serialize((byte)Type);
            serializer.Serialize(ObjectRef);
        }

        public static ObjectArg Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8();

            switch(type)
            {
                case 0:
                    SuiObjectRef objectRef = (SuiObjectRef)SuiObjectRef.Deserialize(deserializer);
                    return new ObjectArg(ObjectRefType.ImmOrOwned, objectRef);
                case 1:
                    SharedObjectRef sharedObjectRef = (SharedObjectRef)SharedObjectRef.Deserialize(deserializer);
                    return new ObjectArg(ObjectRefType.Shared, sharedObjectRef);
                default:
                    throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// A Sui object can be immutable or owned (`ImmOrOwned`).
    /// 
    /// A reference to this type of object has the following format:
	/// <code>
    ///     SuiObjectRef: {
	///		    objectId: BCS.ADDRESS,
	///		    version: BCS.U64,
	///		    digest: 'ObjectDigest',
	///     },
    /// </code>
    /// Where `ObjectDigest` is a base58 BCS serialized string
    /// </summary>
    [JsonObject]
    public class SuiObjectRef : IObjectRef
    {
        [JsonProperty("objectId")]
        public AccountAddress ObjectID { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        public SuiObjectRef(AccountAddress objectId, int version, string digest)
        {
            this.ObjectID = objectId;
            this.Version = version;
            this.Digest = digest;
        }

        public SuiObjectRef(string object_id, string version, string digest)
        {
            this.ObjectID = AccountAddress.FromHex(object_id);
            this.Version = int.Parse(version);
            this.Digest = digest;
        }

        public void Serialize(Serialization serializer)
        {
            U64 version = new U64((ulong)this.Version);

            Base58Encoder decoder = new Base58Encoder();
            byte[] decode = decoder.DecodeData(this.Digest);

            this.ObjectID.Serialize(serializer);
            version.Serialize(serializer);
            serializer.Serialize(decode);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            AccountAddress objectId = new AccountAddress(deserializer.FixedBytes(AccountAddress.Length));
            U64 version = U64.Deserialize(deserializer);
            BString digest = BString.Deserialize(deserializer);

            return new SuiObjectRef(
                (AccountAddress)objectId.GetValue(),
                (int)version.GetValue(),
                (string)digest.GetValue()
            );
        }
    }

    /// <summary>
    /// A shared object is a Sui object that can be accessed by others.
    /// This type of object can also be mutated is allowed.
    /// A reference to this type of object has the following format:
    /// <code>
    ///    SharedObjectRef: {
	///		    objectId: BCS.ADDRESS,
	///		    initialSharedVersion: BCS.U64,
	///		    mutable: BCS.BOOL,
	///    },
    /// </code>
    /// </summary>
    public class SharedObjectRef : IObjectRef
    {
        /// <summary>
        /// Hex code as string representing the object id.
        /// </summary>
        private AccountAddress objectId;

        /// <summary>
        /// The version the object was shared at.
        /// </summary>
        public int InitialSharedVersion;

        /// <summary>
        /// Whether reference is mutable.
        /// </summary>
        public bool mutable;

        public AccountAddress ObjectID { get => objectId; set => objectId = value; }

        public SharedObjectRef(AccountAddress objectId, int initialSharedVersion, bool mutable)
        {
            this.ObjectID = objectId;
            this.InitialSharedVersion = initialSharedVersion;
            this.mutable = mutable;
        }

        public void Serialize(Serialization serializer)
        {
            U64 initialSharedVersion = new U64((ulong)this.InitialSharedVersion);
            Bool mutable = new Bool(this.mutable);

            this.ObjectID.Serialize(serializer);
            initialSharedVersion.Serialize(serializer);
            mutable.Serialize(serializer);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            AccountAddress objectId = new AccountAddress(deserializer.FixedBytes(AccountAddress.Length));
            U64 initialSharedVersion = U64.Deserialize(deserializer);
            Bool mutable = Bool.Deserialize(deserializer);

            return new SharedObjectRef(
                (AccountAddress)objectId.GetValue(),
                (int)initialSharedVersion.GetValue(),
                (bool)mutable.GetValue()
            );
        }
    }
}