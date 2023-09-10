using System;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Cryptography;
using UnityEngine;

namespace Sui.BCS
{
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
    public interface IObjectRef : ISerializable
    {
        public string ObjectId { get; set; }
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
    public class CallArg : ISerializable
    {
        public ISerializable[] args; // Could be a Pure (native BCS type) or an IObjectRef type.
        public CallArg(ISerializable[] args)
        {
            this.args = args;
        }
        public void Serialize(Serialization serializer)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A Sui object can be immutable or owned.
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
        public string objectId;
        public int version;
        public string digest;

        public string ObjectId { get => objectId; set => objectId = value; }

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
    public class SharedObjectRef : ISerializable
    {
        /// <summary>
        /// Hex code as string representing the object id.
        /// </summary>
        public string objectId;

        /// <summary>
        /// The version the object was shared at.
        /// </summary>
        public int initialSharedVersion;

        /// <summary>
        /// Whether reference is mutable.
        /// </summary>
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

    /// <summary>
    ///    GasData: {
	///	        payment: [VECTOR, 'SuiObjectRef'],
	///	        owner: BCS.ADDRESS,
	///	        price: BCS.U64,
	///	        budget: BCS.U64,
	///    },
    /// </summary>
    public class GasData : ISerializable
    {
        public List<SuiObjectRef> payment;
        public string owner;
        public int price;
        public int budget;

        public GasData(List<SuiObjectRef> payment, string owner, int price, int budget)
        {
            this.payment = payment;
            this.owner = owner;
            this.price = price;
            this.budget = budget;
        }
        public void Serialize(Serialization serializer)
        {
            Sequence paymentSeq = new Sequence(this.payment.ToArray());
            AccountAddress owner = AccountAddress.FromHex(this.owner);
            U64 price = new U64((ulong) this.price);
            U64 budget = new U64((ulong)this.budget);

            paymentSeq.Serialize(serializer);
            price.Serialize(serializer);
            budget.Serialize(serializer);
        }

        // TODO: Implement GasData serialization
        public static ISerializable Deserialize(Deserialization deserializer)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///
    /// Signed transaction data needed to generate transaction digest.
    /// <code>
    ///     SenderSignedData: {
	///         data: 'TransactionData',
	///		    txSignatures: [VECTOR, [VECTOR, BCS.U8]],
	///     },
    /// </code> 
    /// </summary>
    public class SenderSignedData : ISerializable
    {
        public TransactionDataV1 transactionData;
        public List<byte[]> transactionSignatureBytes;

        public SenderSignedData(TransactionDataV1 transactionData, List<byte[]> transactionSignatures)
        {
            this.transactionData = transactionData;
            this.transactionSignatureBytes = transactionSignatures;
        }

        /// <summary>
        /// TODO: Implement. Serialize signatures and put in list.
        /// </summary>
        /// <param name="transactionData"></param>
        /// <param name="transactionSignatures"></param>
        public SenderSignedData(TransactionDataV1 transactionData, List<SignatureBase> transactionSignatures)
        {
            throw new NotImplementedException();
        }

        public void Serialize(Serialization serializer)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    ///
    /// <code>
    ///     const TransactionDataV1 = {
    ///         kind: 'TransactionKind',
    ///         sender: BCS.ADDRESS,
    ///         gasData: 'GasData',
    ///         expiration: 'TransactionExpiration',
    ///     };
    /// </code>
    /// </summary>
    public class TransactionDataV1 : ISerializable
    {
        public string sender;
        public GasData gasData;
        public TransactionExpiration expiration;

        public TransactionDataV1(string sender, GasData gasData, TransactionExpiration expiration)
        {
            this.sender = sender;
            this.gasData = gasData;
            this.expiration = expiration;
        }

        public void Serialize(Serialization serializer)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Indicates the expiration time for a transaction.
    /// This can be a null values, an object with property `None` or
    /// an object with `Epoch` property.
    /// 
    /// TODO: Create a pull request in Sui GitHub to fix type in documentation
    /// https://github.com/MystenLabs/sui/blob/948be00ce391e300b17cca9b74c2fc3981762b87/sdk/typescript/src/bcs/index.ts#L102C4-L102C54
    /// <code>
    /// export type TransactionExpiration = { None: null } | { Epoch: number };
    /// </code>
    /// </summary>
    public class TransactionExpiration : ISerializable
    {
        private bool None { get; set; }
        private int Epoch { get; set; }

        public TransactionExpiration()
        {
            None = true;
        }

        /// <summary>
        /// TODO: Inquire on validation for epoch number
        /// </summary>
        /// <param name="epoch"></param>
        public TransactionExpiration(int epoch)
        {
            None = false;
            Epoch = epoch;
        }

        public void Serialize(Serialization serializer)
        {
            if (None == true)
                serializer.SerializeU8(0); // Nothing
            else
            {
                serializer.SerializeU8(1);
                serializer.SerializeU64((ulong)Epoch);
            }
        }

        public static ISerializable Deserialize(Deserialization deserialization)
        {
            throw new NotImplementedException();
        }
    }
}