using System;
using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Types;
using Sui.Transactions.Kinds;
using Sui.Utilities;
using Sui.Transactions.Types.Arguments;
using Sui.Types;
using Sui.Rpc.Client;

namespace Sui.Transactions.Builder
{
    public class TransactionBlockDataBuilder : ISerializable
    {
        /// <summary>
        /// Represents the version of the serialized transaction data.
        /// </summary>
        public int Version { get; set; }

        /// <summary>
        /// The account address of the sender of the transaction. It is optional and can be nil.
        /// </summary>
        public AccountAddress Sender { get; set; }

        /// <summary>
        /// Represents the expiration of the transaction. It is optional and defaults to `TransactionExpiration.none`.
        /// </summary>
        public ITransactionExpiration Expiration;

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
        public List<SuiTransaction> Transactions { get; set; }

        public SuiError Error { get; set; }

        /// <summary>
        /// Initializes a new instance of `SerializedTransactionDataBuilder`.
        /// </summary>
        /// <param name="version">Represents the version of the transaction. Defaults to 1.</param>
        /// <param name="sender">The account address of the sender of the transaction. Defaults to null.</param>
        /// <param name="expiration">Represents the expiration of the transaction. Defaults to null.</param>
        /// <param name="gasConfig">Holds the configuration for gas in the transaction. Defaults to null.</param>
        /// <param name="inputs">An array of inputs for the transaction block. Defaults to null.</param>
        /// <param name="transactions">An array of transactions. Defaults to null.</param>
        public TransactionBlockDataBuilder
        (
            int version = 1,
            AccountAddress sender = null,
            ITransactionExpiration expiration = null,
            GasData gasConfig = null,
            List<TransactionBlockInput> inputs = null,
            List<SuiTransaction> transactions = null
        )
        {
            this.Version = version;
            this.Sender = sender;
            this.Expiration = expiration != null ? expiration : new TransactionExpirationNone();
            this.GasConfig = gasConfig != null ? gasConfig : new GasData();
            this.Inputs = inputs != null ? inputs : new List<TransactionBlockInput>();
            this.Transactions = transactions != null ? transactions : new List<SuiTransaction>();
        }

        public TransactionBlockDataBuilder(TransactionDataV1 v1_transaction)
        {
            if (v1_transaction.Transaction.Type != SuiTransactionKindType.ProgrammableTransaction)
            {
                this.Error = new SuiError(0, "Unable to Create Transaction Block Data Builder.", null);
                return;
            }

            ProgrammableTransaction program_tx = (ProgrammableTransaction)v1_transaction.Transaction.Transaction;

            this.Version = 1;
            this.Sender = v1_transaction.Sender;
            this.Expiration = v1_transaction.Expiration;
            this.GasConfig = v1_transaction.GasData;
            this.Transactions = program_tx.Transactions;

            this.Inputs = program_tx.Inputs
                .Select((input, index) => new { Input = input, Index = index })
                .Select((item) =>
                    new TransactionBlockInput(
                        (ushort)item.Index,
                        item.Input,
                        item.Input.Type
                    )
                )
                .ToList();
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8((byte)Version);

            if (Sender != null)
                Sender.Serialize(serializer);

            if (Expiration != null)
                Expiration.Serialize(serializer);

            GasConfig.Serialize(serializer);
            serializer.Serialize(new Sequence(Inputs.ToArray()));
            serializer.Serialize(new Sequence(Transactions.ToArray()));
        }

        public static TransactionBlockDataBuilder Deserialize(Deserialization deserializer)
        {
            return new TransactionBlockDataBuilder
            (
                deserializer.DeserializeU8(),
                AccountAddress.Deserialize(deserializer),
                (ITransactionExpiration)Deserialize(deserializer),
                (GasData)GasData.Deserialize(deserializer),
                deserializer.DeserializeSequence(typeof(TransactionBlockInput)).Cast<TransactionBlockInput>().ToList(),
                deserializer.DeserializeSequence(typeof(SuiTransaction)).Cast<SuiTransaction>().ToList()
            );
        }
    }
}