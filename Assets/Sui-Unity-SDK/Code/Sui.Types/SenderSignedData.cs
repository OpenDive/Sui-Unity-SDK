using System;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Cryptography;

namespace Sui.Types
{
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