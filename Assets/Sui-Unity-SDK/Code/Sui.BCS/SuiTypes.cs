using System;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Cryptography;
using Sui.Types;
using Sui.Utilities;
using UnityEngine;

namespace Sui.BCS
{
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
}