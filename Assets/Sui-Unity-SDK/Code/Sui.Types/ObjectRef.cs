using OpenDive.BCS;
using Sui.Accounts;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Types
{
    public interface IObjectRef : ISerializable
    {
        public AccountAddress ObjectId { get; set; }
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
    /// TODO: Look in BCS using base58
    /// </summary>
    public class SuiObjectRef : IObjectRef
    {
        private AccountAddress objectId;
        public int version;
        public string digest;

        public AccountAddress ObjectId { get => objectId; set => objectId = value; }

        public SuiObjectRef(AccountAddress objectId, int version, string digest)
        {
            this.objectId = objectId;
            this.version = version;
            this.digest = digest;
        }

        public void Serialize(Serialization serializer)
        {
            U64 version = new U64((ulong)this.version);

            Base58Encoder decoder = new Base58Encoder();
            byte[] decode = decoder.DecodeData(this.digest);

            // TODO: Ask Marcus if SuiObjectRef also encodes an enum
            // For objects the enum is 1, for pure is 0 // TODO: Add a list of enum,
            //serializer.SerializeU8(0);

            this.objectId.Serialize(serializer);
            version.Serialize(serializer);
            serializer.Serialize(decode);

            Serialization ser = new Serialization();
            objectId.Serialize(ser);
            version.Serialize(ser);
            ser.Serialize(decode);
            Debug.Log("===== SuiObjectRef ::: " + ser.GetBytes().ByteArrayToString());
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            AccountAddress objectId = new AccountAddress(deserializer.FixedBytes(AccountAddress.Length));
            U64 version = U64.Deserialize(deserializer);
            // TODO: Check is this breaks. We are deserializing a base58 string.
            // TODO: Check Sui BCS Base58 deserialization
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

        public AccountAddress ObjectId { get => objectId; set => objectId = value; }

        public SharedObjectRef(AccountAddress objectId, int initialSharedVersion, bool mutable)
        {
            this.ObjectId = objectId;
            this.InitialSharedVersion = initialSharedVersion;
            this.mutable = mutable;
        }

        public void Serialize(Serialization serializer)
        {
            U64 initialSharedVersion = new U64((ulong)this.InitialSharedVersion);
            Bool mutable = new Bool(this.mutable);

            this.ObjectId.Serialize(serializer);
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