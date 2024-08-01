//
//  TransactionBlockDataBuilder.cs
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

using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Utilities;
using Sui.Types;
using Sui.Transactions.Data;

namespace Sui.Transactions.Builder
{
    /// <summary>
    /// Represents a transaction block's content for serialization.
    /// </summary>
    public class TransactionBlockDataBuilder : ReturnBase, ISerializable
    {
        /// <summary>
        /// Represents the version of the serialized transaction data.
        /// </summary>
        public byte Version { get; set; }

        /// <summary>
        /// The account address of the sender of the transaction. It is optional and can be nil.
        /// </summary>
        public AccountAddress Sender { get; set; }

        /// <summary>
        /// Represents the expiration of the transaction. It is optional and defaults to `TransactionExpiration.none`.
        /// </summary>
        public TransactionExpiration Expiration { get; set; }

        /// <summary>
        /// Holds the configuration for gas in the transaction.
        /// </summary>
        public GasData GasConfig { get; set; }

        /// <summary>
        /// An array of inputs for the transaction block.
        /// </summary>
        public List<TransactionBlockInput> Inputs { get; set; }

        /// <summary>
        /// A list of transaction, e.g. MoveCallTransaction, TransferObjectTransaction, etc.
        /// </summary>
        public List<Command> Transactions { get; set; }

        public TransactionBlockDataBuilder
        (
            byte version = 1,
            AccountAddress sender = null,
            TransactionExpiration expiration = null,
            GasData gasConfig = null,
            List<TransactionBlockInput> inputs = null,
            List<Command> transactions = null
        )
        {
            this.Version = version;
            this.Sender = sender;
            this.Expiration = expiration != null ? expiration : new TransactionExpiration();
            this.GasConfig = gasConfig != null ? gasConfig : new GasData();
            this.Inputs = inputs != null ? inputs : new List<TransactionBlockInput>();
            this.Transactions = transactions != null ? transactions : new List<Command>();
        }

        public TransactionBlockDataBuilder(TransactionDataV1 v1_transaction)
        {
            if (v1_transaction.Transaction.Type != TransactionKindType.ProgrammableTransaction)
            {
                this.Error = new SuiError(0, "Unable to Create Transaction Block Data Builder.", null);
                return;
            }

            ProgrammableTransaction program_tx = (ProgrammableTransaction)v1_transaction.Transaction.Transaction;

            this.Version = 1;
            this.Sender = v1_transaction.Sender;
            this.Expiration = v1_transaction.Expiration;
            this.GasConfig = v1_transaction.GasData;
            this.Transactions = program_tx.Transactions.ToList();

            this.Inputs = program_tx.Inputs
                .Select((input, index) => new { Input = input, Index = index })
                .Select((item) =>
                    new TransactionBlockInput
                    (
                        (ushort)item.Index,
                        item.Input,
                        item.Input.Type
                    )
                )
                .ToList();
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8(this.Version);

            if (this.Sender != null)
                this.Sender.Serialize(serializer);

            if (this.Expiration != null)
                this.Expiration.Serialize(serializer);

            this.GasConfig.Serialize(serializer);

            serializer.Serialize(new Sequence(this.Inputs.ToArray()));
            serializer.Serialize(new Sequence(this.Transactions.ToArray()));
        }

        public static TransactionBlockDataBuilder Deserialize(Deserialization deserializer)
        {
            return new TransactionBlockDataBuilder
            (
                deserializer.DeserializeU8().Value,
                (AccountAddress)AccountAddress.Deserialize(deserializer),
                (TransactionExpiration)TransactionExpiration.Deserialize(deserializer),
                (GasData)GasData.Deserialize(deserializer),
                deserializer.DeserializeSequence(typeof(TransactionBlockInput)).Values.Cast<TransactionBlockInput>().ToList(),
                deserializer.DeserializeSequence(typeof(Command)).Values.Cast<Command>().ToList()
            );
        }
    }
}