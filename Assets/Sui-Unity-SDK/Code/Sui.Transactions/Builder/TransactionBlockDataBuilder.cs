using System;
using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;
using Sui.Transactions.Builder.TransactionObjects;
using Sui.Transactions.Kinds;
using Sui.Utilities;

namespace Sui.Transactions.Builder
{
    /// <summary>
    ///
    /// <code>
    ///     version = 1 as const;
    ///     sender?: string;
    ///     expiration?: TransactionExpiration;
    ///     gasConfig: GasConfig;
    ///     inputs: TransactionBlockInput[];
    ///     transactions: TransactionType[];
    /// </code>
    /// </summary>
    public class TransactionBlockDataBuilder : ISerializable
    {
        public int Version { get; set; }
        public TransactionExpiration Expiration;
        public AccountAddress Sender { get; set; }
        public GasConfig GasConfig { get; set; }
        // TODO: Consider whether we actually need a TransactionBlockInput abstraction, otherwise just use Serializable
        // public ISerializable[] transactionBlockInput; //TransactionBlockInput
        public TransactionBlockInput[] Inputs { get; set; }

        /// <summary>
        /// A list of transaction, e.g. MoveCallTransaction, TransferObjectTransaction, etc
        /// </summary>
        public ITransactionType[] Transactions { get; set; }

        /// <summary>
        /// TODO: Implement
        /// https://github.com/MystenLabs/sui/blob/3253d9a3c629fb142dbf492c22afca14114d1df8/sdk/typescript/src/builder/TransactionBlockData.ts#L156
        /// https://github.com/MystenLabs/sui/blob/948be00ce391e300b17cca9b74c2fc3981762b87/sdk/typescript/src/builder/Transactions.ts#L29C14-L29C35
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="gasConfig"></param>
        public TransactionBlockDataBuilder(
            AccountAddress sender = null,
            TransactionExpiration expiration = null,
            GasConfig gasConfig = null,
            TransactionBlockInput[] inputs = null,
            ITransactionType[] transactions = null
            )
        {
            this.Sender = sender;
            this.Expiration = expiration;
            this.GasConfig = gasConfig;
            this.Inputs = inputs;
            this.Transactions = transactions;
        }

        public TransactionBlockDataBuilder()
            => new TransactionBlockDataBuilder(null, null, null, null, null);

        public byte[] Build(
            int? maxSizeBytes = null,
            bool onlyTransactionKind = false,
            AccountAddress sender = null,
            GasConfig gasConfig = null,
            TransactionExpiration expiration = null)
        {
            // Resolve inputs down to values:
            List<ISerializable> inputs = Inputs.Select(
                x =>  x.Value
            ).ToList();

            ProgrammableTransaction programmableTx
                = new ProgrammableTransaction(Inputs, Transactions);

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

            if (GasConfig.budget == null)
            {
                throw new Exception("Missing gas budget");
            }

            if (GasConfig.payment == null)
            {
                throw new Exception("Missing gas payment");
            }

            if (GasConfig.price  == null)
            {
                throw new Exception("Missing gas price");
            }

            //TransactionData transactionData = new()
            //{
            //    Sender = this.Sender,
            //    Expiration = this.Expiration,
            //    GasData = this.GasConfig,
            //    Transaction = new ProgrammableTransaction(Inputs, Transactions)
            //};

            TransactionData transactionData = new TransactionData(
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
        /// Check if all TransactionBlockInput inputs are only of transaction
        /// </summary>
        /// <returns></returns>
        private bool IsOnlyTransactionKind()
        {
            // Check value, etc
            throw new NotImplementedException();
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
            TransactionData txData = (TransactionData)TransactionData.Deserialize(deserializer);
            throw new NotImplementedException();
        }

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

        //export const SerializedTransactionDataBuilder = object({
        //      version: literal(1),
        //      sender: optional(string()),
        //      expiration: TransactionExpiration,
        //      gasConfig: GasConfig,
        //      inputs: array(TransactionBlockInput),
        //      transactions: array(TransactionType),
        //});

        // const serialized = create(
        //	{
        //		version: 1,
        //      gasConfig: {},
        //		inputs: programmableTx.inputs.map((value: unknown, index: number) =>
        //			create(

        //                      {
        //                          kind: 'Input',
        //					value,
        //					index,
        //					type: is (value, PureCallArg) ? 'pure' : 'object',
        //				},
        //				TransactionBlockInput,
        //			),
        //		),
        //		transactions: programmableTx.transactions,
        //	},
        //	SerializedTransactionDataBuilder,
        //);

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

        public void Serialize(Serialization serializer)
        {
            throw new NotImplementedException();
        }
    }
}