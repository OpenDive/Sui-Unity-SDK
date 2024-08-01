//
//  TransactionDataV1.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using OpenDive.BCS;
using Sui.Accounts;
using Sui.Types;

namespace Sui.Transactions.Data
{
    /// <summary>
    /// The representation of version 1 of a Sui Transaction Data.
    /// </summary>
    public class TransactionDataV1 : ITransactionData
    {
        /// <summary>
        /// The sender of the transaction.
        /// </summary>
        public AccountAddress Sender { get; set; }

        /// <summary>
        /// The transaction's expiration.
        /// </summary>
        public TransactionExpiration Expiration { get; set; }

        /// <summary>
        /// The gas configuration details.
        /// </summary>
        public GasData GasData { get; set; }

        /// <summary>
        /// The transaction's details.
        /// </summary>
        public TransactionKind Transaction { get; set; }

        public TransactionDataV1
        (
            AccountAddress sender,
            TransactionExpiration transaction_expiration,
            GasData gas_data,
            TransactionKind transaction
        )
        {
            this.Sender = sender;
            this.Expiration = transaction_expiration;
            this.GasData = gas_data;
            this.Transaction = transaction;
        }

        public void Serialize(Serialization serializer)
        {
            this.Transaction.Serialize(serializer);
            this.Sender.Serialize(serializer);
            this.GasData.Serialize(serializer);
            this.Expiration.Serialize(serializer);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            TransactionKind tx_block_kind = (TransactionKind)TransactionKind.Deserialize(deserializer);
            AccountAddress sender = (AccountAddress)AccountAddress.Deserialize(deserializer);
            GasData gas_data = (GasData)GasData.Deserialize(deserializer);
            TransactionExpiration expiration = (TransactionExpiration)TransactionExpiration.Deserialize(deserializer);
            
            return new TransactionDataV1
            (
                sender,
                expiration,
                gas_data,
                tx_block_kind
            );
        }
    }
}