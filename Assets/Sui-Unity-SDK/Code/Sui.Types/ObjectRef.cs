using System;
using System.Linq;
using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Client;
using Sui.Utilities;

namespace Sui.Types
{
    public enum ObjectRefType
    {
        ImmOrOwned,
        Shared
    }

    public interface IObjectRef : ISerializable
    {
        public string ObjectIDString { get; set; }
        public AccountAddress ObjectID { get; set; }
    }

    public class ObjectArg : ISerializable
    {
        public ObjectRefType Type { get; set; }
        public IObjectRef ObjectRef { get; set; }

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

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8().Value;

            switch(type)
            {
                case 0:
                    SuiObjectRef objectRef = (SuiObjectRef)SuiObjectRef.Deserialize(deserializer);
                    return new ObjectArg(ObjectRefType.ImmOrOwned, objectRef);
                case 1:
                    SharedObjectRef sharedObjectRef = (SharedObjectRef)SharedObjectRef.Deserialize(deserializer);
                    return new ObjectArg(ObjectRefType.Shared, sharedObjectRef);
                default:
                    return new SuiError(0, "Unable to deserialize ObjectArg", null);
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
        public string ObjectIDString { get; set; }

        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("digest")]
        public string Digest { get; set; }

        public AccountAddress ObjectID
        {
            get => AccountAddress.FromHex(this.ObjectIDString);
            set => this.ObjectIDString = value.ToHex();
        }

        public SuiObjectRef(string objectId, int version, string digest)
        {
            this.ObjectIDString = objectId;
            this.Version = version;
            this.Digest = digest;
        }

        public SuiObjectRef(AccountAddress objectId, int version, string digest)
        {
            this.ObjectIDString = objectId.ToHex();
            this.Version = version;
            this.Digest = digest;
        }

        public SuiObjectRef(string object_id, string version, string digest)
        {
            this.ObjectIDString = object_id;
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
            Base58Encoder decoder = new Base58Encoder();
            return new SuiObjectRef
            (
                (AccountAddress)AccountAddress.Deserialize(deserializer),
                (int)deserializer.DeserializeU64().Value,
                decoder.EncodeData(deserializer.ToBytes())
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
    [JsonObject]
    public class SharedObjectRef : IObjectRef
    {
        /// <summary>
        /// Hex code as string representing the object id.
        /// </summary>
        [JsonProperty("objectId")]
        public string ObjectIDString { get; set; }

        /// <summary>
        /// The version the object was shared at.
        /// </summary>
        [JsonProperty("initialSharedVersion")]
        public int InitialSharedVersion { get; set; }

        /// <summary>
        /// Whether reference is mutable.
        /// </summary>
        [JsonProperty("mutable")]
        public bool Mutable { get; set; }

        public AccountAddress ObjectID
        {
            get => AccountAddress.FromHex(this.ObjectIDString);
            set => this.ObjectIDString = value.ToHex();
        }

        public SharedObjectRef(string objectId, int initialSharedVersion, bool mutable)
        {
            this.ObjectIDString = objectId;
            this.InitialSharedVersion = initialSharedVersion;
            this.Mutable = mutable;
        }

        public SharedObjectRef(AccountAddress objectId, int initialSharedVersion, bool mutable)
        {
            this.ObjectIDString = objectId.ToHex();
            this.InitialSharedVersion = initialSharedVersion;
            this.Mutable = mutable;
        }

        public SharedObjectRef(string objectId, string initialSharedVersion, bool mutable)
        {
            this.ObjectIDString = objectId;
            this.InitialSharedVersion = int.Parse(initialSharedVersion);
            this.Mutable = mutable;
        }

        public void Serialize(Serialization serializer)
        {
            U64 initialSharedVersion = new U64((ulong)this.InitialSharedVersion);
            Bool mutable = new Bool(this.Mutable);

            this.ObjectID.Serialize(serializer);
            initialSharedVersion.Serialize(serializer);
            mutable.Serialize(serializer);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SharedObjectRef
            (
                (AccountAddress)AccountAddress.Deserialize(deserializer),
                (int)deserializer.DeserializeU64().Value,
                Bool.Deserialize(deserializer).Value
            );
        }
    }
}