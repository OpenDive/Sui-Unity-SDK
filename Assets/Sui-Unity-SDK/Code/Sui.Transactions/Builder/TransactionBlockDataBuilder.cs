using System;
using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;
using Sui.Transactions.Builder.TransactionObjects;
using Sui.Transactions.Kinds;

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
        public int Version { get => 1;  }
        public TransactionExpiration Expiration;
        public AccountAddress Sender { get; set; }
        public GasConfig GasConfig;
        // TODO: Consider whether we actually need a TransactionBlockInput abstraction, otherwise just use Serializable
        // public ISerializable[] transactionBlockInput; //TransactionBlockInput
        public TransactionBlockInput[] Inputs;

        /// <summary>
        /// A list of transaction, e.g. MoveCallTransaction, TransferObjectTransaction, etc
        /// </summary>
        public ITransaction[] Transactions;

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
            ITransaction[] transactions = null
            )
        {
            this.Sender = sender;
            this.Expiration = expiration;
            this.GasConfig = gasConfig;
            this.Inputs = inputs;
            this.Transactions = transactions;
        }

        public TransactionBlockDataBuilder()
        {
            new TransactionBlockDataBuilder(null, null, null, null, null);
        }

        public byte[] Build(
            int maxSizeBytes,
            AccountAddress sender = null,
            GasConfig gasConfig = null,
            TransactionExpiration expiration = null
            )
        {
            // Resolve inputs down to values:
            List<ISerializable> inputs = Inputs.Select(
                x =>  x.Value
            ).ToList();

            if (IsOnlyTransactionKind())
            {
                // return builder.ser('TransactionKind', kind, { maxSize: maxSizeBytes }).toBytes();
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

            TransactionData transactionData = new()
            {
                Sender = this.Sender,
                Expiration = this.Expiration,
                GasData = this.GasConfig,
                Transaction = new ProgrammableTransaction(Inputs, Transactions)
            };

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

        public static TransactionBlockDataBuilder FromKindBytes(byte[] bytes)
        {
            Deserialization deserializer = new Deserialization(bytes);
            ProgrammableTransaction programmableTx = (ProgrammableTransaction)ProgrammableTransaction.Deserialize(deserializer);

            throw new NotImplementedException();
        }

        public static TransactionBlockDataBuilder FromBytes(byte[] bytes)
        {
            //return new TransactionBlockDataBuilder();
            throw new NotImplementedException();
        }

        //public static TransactionBlockDataBuilder Restore(SerializedTransactionDataBuilder data)
        //{
        //    throw new NotImplementedException();
        //}


            public static string GetDigestFromBytes(byte[] bytes)
        {
            //return new TransactionBlockDataBuilder();
            throw new NotImplementedException();
        }

        public void Serialize(Serialization serializer)
        {
            throw new NotImplementedException();
        }
    }
}