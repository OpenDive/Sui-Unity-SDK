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
        public GasConfig GasConfig { get; set; }

        /// <summary>
        /// An array of inputs for the transaction block.
        /// </summary>
        public List<TransactionBlockInput> Inputs { get; set; }

        /// <summary>
        /// A list of transaction, e.g. MoveCallTransaction, TransferObjectTransaction, etc.
        /// </summary>
        public List<SuiTransaction> Transactions { get; set; }

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
            GasConfig gasConfig = null,
            List<TransactionBlockInput> inputs = null,
            List<SuiTransaction> transactions = null
        )
        {
            this.Version = version;
            this.Sender = sender;
            this.Expiration = expiration;
            this.GasConfig = gasConfig;
            this.Inputs = inputs;
            this.Transactions = transactions;
        }

        public TransactionBlockDataBuilder()
            => new TransactionBlockDataBuilder(1, null, null, null, null, null);

        public byte[] Build(
            int? maxSizeBytes = null,
            bool onlyTransactionKind = false,
            AccountAddress sender = null,
            GasConfig gasConfig = null,
            ITransactionExpiration expiration = null)
        {
            // Resolve inputs down to values:
            ICallArg[] inputs = (ICallArg[])Inputs.Select(
                x => x.Value
            );

            ProgrammableTransaction programmableTx
                = new ProgrammableTransaction(inputs, Transactions);

            //if (IsOnlyTransactionKind())
            if(onlyTransactionKind)
            {
                // return builder.ser('TransactionKind', kind, { maxSize: maxSizeBytes }).toBytes();
                Serialization ser = new Serialization();
                //programmableTx.Serialize(ser);
                ser.Serialize(programmableTx);
                return ser.GetBytes();
            }

            Expiration = expiration != null ? expiration : Expiration;
            Sender = sender != null ? sender : Sender;
            GasConfig = gasConfig != null ? gasConfig : GasConfig;

            if (Sender == null)
            {
                throw new Exception("Missing transaction sender");
            }

            if (GasConfig.Budget == null)
            {
                throw new Exception("Missing gas budget");
            }

            if (GasConfig.Payment == null)
            {
                throw new Exception("Missing gas payment");
            }

            if (GasConfig.Price  == null)
            {
                throw new Exception("Missing gas price");
            }

            TransactionDataV1 transactionData = new TransactionDataV1(
                Sender,
                Expiration,
                GasConfig,
                programmableTx
            );

            Serialization serializer = new Serialization();
            serializer.Serialize(transactionData);
            return serializer.GetBytes();
        }

        /// <summary>
        /// Deserializes a ITransactionKind, e.g. ProgrammableTransaction.
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static TransactionBlockDataBuilder FromKindBytes(byte[] bytes)
        {
            Deserialization deserializer = new Deserialization(bytes);
            ProgrammableTransaction programmableTx = (ProgrammableTransaction)ProgrammableTransaction.Deserialize(deserializer);

            // TODO: Implement checking whether the deserialization was succesfull, or I guess we don't care because "Deserialize" will break otherwise.
            //const kind = builder.de('TransactionKind', bytes);
            //const programmableTx = kind?.ProgrammableTransaction;
            //if (!programmableTx)
            //{
            //    throw new Error('Unable to deserialize from bytes.');
            //}

            throw new NotImplementedException();
        }

        /// <summary>
        /// Deserializes a TransactionData
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static TransactionBlockDataBuilder FromBytes(byte[] bytes)
        {
            Deserialization deserializer = new Deserialization(bytes);
            TransactionDataV1 data = (TransactionDataV1)TransactionDataV1.Deserialize(deserializer);
            TransactionBlockDataBuilder txBlockDataBuilder = new TransactionBlockDataBuilder();
            txBlockDataBuilder.Version = 1;
            txBlockDataBuilder.Sender = data.Sender;
            txBlockDataBuilder.Expiration = data.Expiration;
            txBlockDataBuilder.GasConfig = data.GasData;

            ProgrammableTransaction programmableTx = (ProgrammableTransaction)data.Transaction;
            ICallArg[] callArgs = programmableTx.Inputs;
            List<TransactionBlockInput> txBlockInputs = new List<TransactionBlockInput>();

            for (int i = 0; i < callArgs.Length; i++)
            {
                TransactionBlockInput input = new TransactionBlockInput(i, callArgs[i], null);
            }

            txBlockDataBuilder.Inputs = txBlockInputs;
            txBlockDataBuilder.Transactions = programmableTx.Transactions;

            return txBlockDataBuilder;
        }

        /// <summary>
        /// Create a TransactionBlockdataBuilder object from a given TransactionBlockData
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public static TransactionBlockDataBuilder Restore(TransactionBlockData data)
        {
            TransactionBlockDataBuilder transactionBlockDataBuilder = new TransactionBlockDataBuilder();
            transactionBlockDataBuilder.Version = data.Version;
            transactionBlockDataBuilder.Sender = data.Sender;
            transactionBlockDataBuilder.Expiration = data.Expiration;
            transactionBlockDataBuilder.GasConfig = data.GasConfig;
            transactionBlockDataBuilder.Inputs = data.Inputs;
            transactionBlockDataBuilder.Transactions = data.Transactions;
            return transactionBlockDataBuilder;
        }

        /// <summary>
        /// Get base58 digest of a full transaction kind, e.g. ProgrammableTransaction.
        /// </summary>
        /// <returns>Base58 hash</returns>
        public string GetDigest()
        {
            byte[] bytes = this.Build(null, false, null, null, null);
            return GetDigestFromBytes(bytes);
        }

        /// <summary>
        /// Utility function to get a base58 digest from the TransactionData
        /// </summary>
        /// <param name="bytes"></param>
        /// <returns></returns>
        public static string GetDigestFromBytes(byte[] bytes)
        {
            byte[] hash = Utilities.Utils.HashTypedData("TransactionData", bytes);
            Base58Encoder encoder = new Base58Encoder();
            string base58 = encoder.EncodeData(hash, 0, hash.Length);
            return base58;
        }

        public TransactionBlockData Snapshot()
        {
            return new TransactionBlockData(
                Version, Sender, Expiration, GasConfig, Inputs, Transactions
            );
        }

        public void Serialize(Serialization serializer)
        {
            throw new NotImplementedException();
        }
    }
}