//
//  TransactionBlockDataBuilderSerializer.cs
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

using Org.BouncyCastle.Crypto.Digests;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Builder;
using Sui.Transactions.Data;
using Sui.Types;
using Sui.Utilities;

namespace Sui.Transactions
{
    /// <summary>
    /// The serializer wrapper used to serialize and deserialize transaction block data values.
    /// </summary>
    public class TransactionBlockDataBuilderSerializer : ReturnBase, ISerializable
    {
        /// <summary>
        /// The maximum size, in bytes, that a transaction data object can be
        /// in order to be executed on chain.
        /// </summary>
        public static int TransactionDataMaxSize { get => 128 * 1_024; }

        /// <summary>
        /// The builder object containing the various transaction values.
        /// </summary>
        public TransactionBlockDataBuilder Builder { get; set; }

        public TransactionBlockDataBuilderSerializer(TransactionBlockDataBuilder builder)
        {
            this.Builder = builder;
        }

        public TransactionBlockDataBuilderSerializer()
        {
            this.Builder = new TransactionBlockDataBuilder();
        }

        public TransactionBlockDataBuilderSerializer(byte[] bytes)
        {
            Deserialization der = new Deserialization(bytes);

            TransactionData tx_data =
                (TransactionData)TransactionData.Deserialize(der);

            switch (tx_data.Type)
            {
                case TransactionType.V1:
                    this.Builder = new TransactionBlockDataBuilder((TransactionDataV1)tx_data.Transaction);
                    break;
                default:
                    this.Error = new SuiError(0, "Unable to convert byte array to TransactionBlockDataBuilderSerializer", null);
                    break;
            }
        }

        /// <summary>
        /// Computes and returns the digest from the provided raw bytes.
        /// </summary>
        /// <param name="bytes">A byte array input.</param>
        /// <returns>A `string` representing the Base 58 encoded string of the hashed data.</returns>
        public static string GetDigestFromBytes(byte[] bytes)
        {
            string type_tag = "TransactionData";
            byte[] type_tag_bytes = Encoding.UTF8.GetBytes((type_tag + "::"));

            List<byte> data_with_tag = new List<byte>();

            data_with_tag.AddRange(type_tag_bytes);
            data_with_tag.AddRange(bytes);

            byte[] hashed_data = new byte[32];
            Blake2bDigest blake2b = new Blake2bDigest(256);
            blake2b.BlockUpdate(data_with_tag.ToArray(), 0, data_with_tag.Count());
            blake2b.DoFinal(hashed_data, 0);

            NBitcoin.DataEncoders.Base58Encoder base_58_encoder = new NBitcoin.DataEncoders.Base58Encoder();
            return base_58_encoder.EncodeData(hashed_data);
        }

        /// <summary>
        /// Restores and returns an instance of `TransactionBlockDataBuilderSerializer`
        /// from the provided `TransactionBlockDataBuilder`.
        /// </summary>
        /// <param name="data">The `TransactionBlockDataBuilder` object to restore from.</param>
        /// <returns>A `TransactionBlockDataBuilderSerializer` representing the restored serializer from the data builder.</returns>
        public static TransactionBlockDataBuilderSerializer restore(TransactionBlockDataBuilder data)
            => new TransactionBlockDataBuilderSerializer(data);

        /// <summary>
        /// Builds and returns the serialized transaction data with the optional provided overrides and transaction kind.
        /// </summary>
        /// <param name="overrides">An optional instance of `TransactionBlockDataBuilderSerializer` containing the overrides.</param>
        /// <param name="only_transaction_kind">A `Bool` indicating whether to only use the transaction kind when building. Defaults to `null`.</param>
        /// <returns>A byte array representing the serialized transaction data.</returns>
        public byte[] Build
        (
            TransactionBlockDataBuilderSerializer overrides = null,
            bool? only_transaction_kind = null
        )
        {
            // Resolve inputs down to values.
            List<CallArg> inputs = this.Builder.Inputs
                .Select((input) => {
                    return input.Value.GetType() == typeof(PureCallArg) ?
                        new CallArg(CallArgumentType.Pure, (PureCallArg)input.Value) :
                        input.Value.GetType() == typeof(ObjectCallArg) ?
                            new CallArg(CallArgumentType.Object, (ObjectCallArg)input.Value) :
                            input.Value.GetType() == typeof(CallArg) ? (CallArg)input.Value : null;
                })
                .Where(value => value != null)
                .ToList();

            ProgrammableTransaction programmable_tx
                = new ProgrammableTransaction(inputs.ToArray(), Builder.Transactions.ToArray());

            if (only_transaction_kind != null && only_transaction_kind == true)
            {
                TransactionKind tx_block_kind = new TransactionKind
                (
                    TransactionKindType.ProgrammableTransaction,
                    programmable_tx
                );
                Serialization ser = new Serialization();
                ser.Serialize(tx_block_kind);
                return ser.GetBytes();
            }

            TransactionExpiration expiration;
            AccountAddress sender;
            GasData gas_config;

            if (overrides == null)
            {
                expiration = Builder.Expiration;
                sender = Builder.Sender;
                gas_config = Builder.GasConfig;
            }
            else
            {
                expiration = overrides.Builder.Expiration != null ? overrides.Builder.Expiration : Builder.Expiration;
                sender = overrides.Builder.Sender != null ? overrides.Builder.Sender : Builder.Sender;
                gas_config = overrides.Builder.GasConfig != null ? overrides.Builder.GasConfig : Builder.GasConfig;
            }

            if (sender == null)
                return this.SetError<byte[], SuiError>(null, "Missing transaction sender.");

            if (gas_config.Budget == null)
                return this.SetError<byte[], SuiError>(null, "Missing gas budget.");

            if (gas_config.Payment == null)
                return this.SetError<byte[], SuiError>(null, "Missing gas payment.");

            if (gas_config.Price == null)
                return this.SetError<byte[], SuiError>(null, "Missing gas price.");

            if (Builder.GasConfig.Owner != null)
                gas_config.Owner = Builder.GasConfig.Owner;
            else
                gas_config.Owner = sender;

            TransactionDataV1 transaction_data_v1 = new TransactionDataV1
            (
                sender,
                expiration,
                gas_config,
                new TransactionKind
                (
                    TransactionKindType.ProgrammableTransaction,
                    programmable_tx
                )
            );

            TransactionData transaction_data = new TransactionData(TransactionType.V1, transaction_data_v1);

            Serialization serializer = new Serialization();
            serializer.Serialize(transaction_data);

            return serializer.GetBytes();
        }

        /// <summary>
        /// Computes and returns the digest of the built transaction data.
        /// </summary>
        /// <returns>A `string` representing the Base 58 encoded string of the hashed data.</returns>
        public string GetDigest()
        {
            byte[] bytes = this.Build();

            if (this.Error != null)
                return null;

            return TransactionBlockDataBuilderSerializer.GetDigestFromBytes(bytes);
        }

        /// <summary>
        /// Return this serializer's builder as a call by value.
        /// </summary>
        /// <returns>A `TransactionBlockDataBuilder` object.</returns>
        public TransactionBlockDataBuilder Snapshot() => this.Builder;

        public void Serialize(Serialization serializer) => Builder.Serialize(serializer);

        public static TransactionBlockDataBuilderSerializer Deserialize(Deserialization deserializer)
            => new TransactionBlockDataBuilderSerializer
               (
                   TransactionBlockDataBuilder.Deserialize(deserializer)
               );
    }
}