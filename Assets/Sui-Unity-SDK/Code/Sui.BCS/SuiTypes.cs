using System;
using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Accounts;
using UnityEngine;

namespace Sui.BCS
{
    /// <summary>
    /// SuiObjectRef is a Sui type that has the following format:
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
    public class SuiObjectRef : ISerializable
    {
        public string objectId;
        public int version;
        public string digest;

        public SuiObjectRef(string objectId, int version, string digest)
        {
            this.objectId = objectId;
            this.version = version;
            this.digest = digest;
        }

        public void Serialize(Serialization serializer)
        {
            AccountAddress objectId = AccountAddress.FromHex(this.objectId);
            U64 version = new U64((ulong)this.version);
            BString digest = new BString(this.digest);

            objectId.Serialize(serializer);
            version.Serialize(serializer);
            digest.Serialize(serializer);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            AccountAddress objectId = new AccountAddress(deserializer.FixedBytes(AccountAddress.Length));
            U64 version = U64.Deserialize(deserializer);
            // TODO: Check is this breaks. We are deserializing a base58 string.
            // TODO: Check Sui BCS Base58 deserialization
            BString digest = BString.Deserialize(deserializer);

            return new SuiObjectRef(
                (string)objectId.GetValue(),
                (int)version.GetValue(),
                (string)digest.GetValue()
            );
        }
    }

    /// <summary>
    /// SharedObjectRef is a Sui type of the following format:
    /// <code>
    ///    SharedObjectRef: {
	///		    objectId: BCS.ADDRESS,
	///		    initialSharedVersion: BCS.U64,
	///		    mutable: BCS.BOOL,
	///    },
    /// </code>
    /// </summary>
    public class SharedObjectRef : ISerializable
    {
        public string objectId;
        public int initialSharedVersion;
        public bool mutable;

        public SharedObjectRef(string objectId, int initialSharedVersion, bool mutable)
        {
            this.objectId = objectId;
            this.initialSharedVersion = initialSharedVersion;
            this.mutable = mutable;
        }

        public void Serialize(Serialization serializer)
        {
            AccountAddress objectId = AccountAddress.FromHex(this.objectId);
            U64 initialSharedVersion = new U64((ulong)this.initialSharedVersion);
            Bool mutable = new Bool(this.mutable);

            objectId.Serialize(serializer);
            initialSharedVersion.Serialize(serializer);
            mutable.Serialize(serializer);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            AccountAddress objectId = new AccountAddress(deserializer.FixedBytes(AccountAddress.Length));
            U64 initialSharedVersion = U64.Deserialize(deserializer);
            Bool mutable = Bool.Deserialize(deserializer);

            return new SharedObjectRef(
                (string)objectId.GetValue(),
                (int)initialSharedVersion.GetValue(),
                (bool)mutable.GetValue()
            );
        }
    }
}