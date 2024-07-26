using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Models;
using Sui.Transactions.Builder;
using Sui.Transactions.Types;
using Sui.Transactions.Types.Arguments;
using Sui.Types;
using UnityEngine;
using Kind = Sui.Transactions.Types.TransactionKind;
using Org.BouncyCastle.Crypto.Digests;
using NBitcoin.DataEncoders;
using Chaos.NaCl;
using Sui.Transactions.Kinds;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using Sui.Rpc.Client;
using Sui.Utilities;

namespace Sui.Transactions
{
    public class TransactionBlockDataBuilderSerializer : ReturnBase, ISerializable
    {
        public static int TransactionDataMaxSize = 128 * 1_024;

        public TransactionBlockDataBuilder Builder;

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
                    this.Builder = new TransactionBlockDataBuilder((Builder.TransactionDataV1)tx_data.Transaction);
                    break;
                default:
                    throw new Exception("Unable to convert byte array to TransactionBlockDataBuilderSerializer");
            }
        }

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

            NBitcoin.DataEncoders.Base58Encoder base58Encoder = new NBitcoin.DataEncoders.Base58Encoder();
            return base58Encoder.EncodeData(hashed_data);
        }

        public static TransactionBlockDataBuilderSerializer restore(TransactionBlockDataBuilder data)
        {
            return new TransactionBlockDataBuilderSerializer(data);
        }

        public SuiResult<byte[]> Build
        (
            TransactionBlockDataBuilderSerializer overrides = null,
            bool? only_transaction_kind = null
        )
        {
            // Resolve inputs down to values:
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

            ProgrammableTransaction programmableTx
                = new ProgrammableTransaction(inputs.ToArray(), Builder.Transactions.ToArray());

            if (only_transaction_kind != null && only_transaction_kind == true)
            {
                Kinds.TransactionBlockKind tx_block_kind = new Kinds.TransactionBlockKind
                (
                    SuiTransactionKindType.ProgrammableTransaction,
                    programmableTx
                );
                Serialization ser = new Serialization();
                ser.Serialize(tx_block_kind);
                return new SuiResult<byte[]>(ser.GetBytes());
            }

            TransactionExpiration Expiration;
            AccountAddress Sender;
            Builder.GasData GasConfig;

            if (overrides == null)
            {
                Expiration = Builder.Expiration;
                Sender = Builder.Sender;
                GasConfig = Builder.GasConfig;
            }
            else
            {
                Expiration = overrides.Builder.Expiration != null ? overrides.Builder.Expiration : Builder.Expiration;
                Sender = overrides.Builder.Sender != null ? overrides.Builder.Sender : Builder.Sender;
                GasConfig = overrides.Builder.GasConfig != null ? overrides.Builder.GasConfig : Builder.GasConfig;
            }

            if (Sender == null)
                return SuiResult<byte[]>.GetSuiErrorResult("Missing transaction sender");

            if (GasConfig.Budget == null)
                return SuiResult<byte[]>.GetSuiErrorResult("Missing gas budget");

            if (GasConfig.Payment == null)
                return SuiResult<byte[]>.GetSuiErrorResult("Missing gas payment");

            if (GasConfig.Price == null)
                return SuiResult<byte[]>.GetSuiErrorResult("Missing gas price");

            if (Builder.GasConfig.Owner != null)
                GasConfig.Owner = Builder.GasConfig.Owner;
            else
                GasConfig.Owner = Sender;

            Sui.Transactions.Builder.TransactionDataV1 transactionDataV1 = new Sui.Transactions.Builder.TransactionDataV1(
                Sender,
                Expiration,
                GasConfig,
                new Kinds.TransactionBlockKind
                (
                    SuiTransactionKindType.ProgrammableTransaction,
                    programmableTx
                )
            );

            TransactionData transaction_data = new TransactionData(TransactionType.V1, transactionDataV1);

            Serialization serializer = new Serialization();
            serializer.Serialize(transaction_data);

            return new SuiResult<byte[]>(serializer.GetBytes());
        }

        public SuiResult<string> GetDigest()
        {
            SuiResult<byte[]> bytes = this.Build();

            if (bytes.Error != null)
                return new SuiResult<string>(null, bytes.Error);

            return new SuiResult<string>(TransactionBlockDataBuilderSerializer.GetDigestFromBytes(bytes.Result));
        }

        public TransactionBlockDataBuilder Snapshot()
        {
            return this.Builder;
        }

        public void Serialize(Serialization serializer)
        {
            Builder.Serialize(serializer);
        }

        public static TransactionBlockDataBuilderSerializer Deserialize(Deserialization deserializer)
        {
            return new TransactionBlockDataBuilderSerializer(
                TransactionBlockDataBuilder.Deserialize(deserializer)
            );
        }
    }

    public enum ObjectArgumentType
    {
        stringArgument,
        transactionObjectArgument
    }

    public interface IObjectArgument
    {
        public ObjectArgumentType Type { get; }
    }

    public class StringObjectArgument : IObjectArgument
    {
        public ObjectArgumentType Type => ObjectArgumentType.stringArgument;
        public string Argument;

        public StringObjectArgument(string argument)
        {
            this.Argument = argument;
        }
    }

    public class TransactionObjectArgument : IObjectArgument
    {
        public ObjectArgumentType Type => ObjectArgumentType.transactionObjectArgument;
        public SuiTransactionArgument Argument;

        public TransactionObjectArgument(SuiTransactionArgument argument)
        {
            this.Argument = argument;
        }
    }

    public enum TransactionObjectInputType
    {
        stringArgument,
        objectCallArgument,
        transactionObjectArgument
    }

    public interface ITransactionObjectInput
    {
        public TransactionObjectInputType Type { get; }
    }

    public class StringTransactionObjectInput : ITransactionObjectInput
    {
        public TransactionObjectInputType Type => TransactionObjectInputType.stringArgument;
        public string Input;

        public StringTransactionObjectInput(string id)
        {
            this.Input = id;
        }
    }

    public class CallArgTransactionObjectInput : ITransactionObjectInput
    {
        public TransactionObjectInputType Type => TransactionObjectInputType.objectCallArgument;
        public ObjectArg Input;

        public CallArgTransactionObjectInput(ObjectArg input)
        {
            this.Input = input;
        }
    }

    public class TransactionArgumentTransactionObjectInput : ITransactionObjectInput
    {
        public TransactionObjectInputType Type => TransactionObjectInputType.transactionObjectArgument;
        public SuiTransactionArgument Input;

        public TransactionArgumentTransactionObjectInput(SuiTransactionArgument input)
        {
            this.Input = input;
        }
    }

    public enum LimitKey
    {
        MaxTxGas,
        MaxGasObjects,
        MaxTxSizeBytes,
        MaxPureArgumentSize
    }

    public class BuildOptions
    {
        public SuiClient Provider;
        public bool? OnlyTransactionKind;
        public Dictionary<string, int?> Limits;
        public ProtocolConfig ProtocolConfig;

        public static Dictionary<LimitKey, string> TransactionLimits = new Dictionary<LimitKey, string>() {
            { LimitKey.MaxTxGas, "max_tx_gas" },
            { LimitKey.MaxGasObjects, "max_gas_payment_objects" },
            { LimitKey.MaxTxSizeBytes, "max_tx_size_bytes" },
            { LimitKey.MaxPureArgumentSize, "max_pure_argument_size" }
        };

        public BuildOptions
        (
            SuiClient Provider,
            Dictionary<string, int?> Limits = null,
            bool? OnlyTransactionKind = null,
            ProtocolConfig ProtocolConfig = null
        )
        {
            this.Provider = Provider;
            this.OnlyTransactionKind = OnlyTransactionKind;
            this.Limits = Limits;
            this.ProtocolConfig = ProtocolConfig;
        }
    }

    /// <summary>
    /// A transaction block.
    /// </summary>
    public class TransactionBlock
    {

        /// <summary>
        /// A dictionary containing default offline limits with string keys and integer values.
        /// </summary> ✅
        public Dictionary<LimitKey, long> DefaultOfflineLimits = new Dictionary<LimitKey, long> {
            { LimitKey.MaxPureArgumentSize, (16 * 1024) },
            { LimitKey.MaxTxGas, 50_000_000_000 },
            { LimitKey.MaxGasObjects, 256 },
            { LimitKey.MaxTxSizeBytes, (128 * 1024) }
        };

        /// ✅
        public enum TransactionConstants : long
        {
            maxGasObjects = 256,
            maxGas = 50_000_000_000,
            gasSafeOverhead = 1_000,
            maxObjectsPerFetch = 50
        };

        /// <summary>
        /// The transaction block builder.
        /// </summary> ✅
        public TransactionBlockDataBuilderSerializer BlockDataBuilder { get; set; }

        /// <summary>
        /// A boolean value indicating whether the block is prepared or not.
        /// </summary> ✅
        private bool IsPrepared { get; set; }

        /// <summary>
        /// The list of transaction the "transaction builder" will use to create
        /// the transaction block. This can be any transaction type defined
        /// in the `ITransaction` interface, e.g. `MoveCall`, `SplitCoins`.
        /// </summary> ✅
        public List<ITransaction> Transactions { get; set; }

        /// <summary>
        /// Creates a TransactionBlock object from an existing TransactionBlock.
        /// </summary> ✅
        /// <param name="transactionBlock"></param>
        public TransactionBlock(TransactionBlock transactionBlock = null)
        {
            if (transactionBlock != null)
                BlockDataBuilder = transactionBlock.BlockDataBuilder;
            else
                BlockDataBuilder = new TransactionBlockDataBuilderSerializer();
        }

        public TransactionBlock(TransactionBlockDataBuilderSerializer serialized_tx_builder)
        {
            this.BlockDataBuilder = serialized_tx_builder;
        }

        /// <summary> ✅
        /// Set the sender for the programmable transaction block.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public void SetSender(AccountAddress sender)
        {
            this.BlockDataBuilder.Builder.Sender = sender;
        }

        /// <summary> ✅
        /// Set the sender for the programmable transaction block.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public void SetSender(string sender)
        {
            this.BlockDataBuilder.Builder.Sender = AccountAddress.FromHex(sender);
        }

        /// <summary> ✅
        /// The the Sender, if it has not been set in the programmable transaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public void SetSenderIfNotSet(AccountAddress sender)
        {
            if (this.BlockDataBuilder.Builder.Sender == null)
                this.SetSender(sender);
        }

        /// <summary> ✅
        /// The the Sender, if it has not been set in the programmable transaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public void SetSenderIfNotSet(string sender)
        {
            if (this.BlockDataBuilder.Builder.Sender == null)
                this.SetSender(AccountAddress.FromHex(sender));
        }

        /// <summary> ✅
        /// Set expiration for the programmable transaction.
        /// </summary>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public void SetExpiration(TransactionExpiration expiration)
        {
            this.BlockDataBuilder.Builder.Expiration = expiration;
        }

        /// <summary> ✅
        /// Sets the gas price.
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public void SetGasPrice(BigInteger price)
        {
            this.BlockDataBuilder.Builder.GasConfig.Price = price;
        }

        /// <summary> ✅
        /// Set the gas budget for the programmable transaction block.
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public void SetGasBudget(int budget)
        {
            this.BlockDataBuilder.Builder.GasConfig.Budget = budget;
        }

        /// <summary> ✅
        /// Set the gas owner for the programmable transaction block.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public void SetGasOwner(AccountAddress owner)
        {
            this.BlockDataBuilder.Builder.GasConfig.Owner = owner;
        }

        /// <summary>
        /// A `TransactionArgument` representing the gas of the transaction.
        /// </summary> ✅
        public SuiTransactionArgument gas = new SuiTransactionArgument(TransactionArgumentKind.GasCoin, null);

        /// <summary> ✅
        /// Set the gas payment for the programmable transaction block.
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        public void SetGasPayment(Sui.Types.SuiObjectRef[] payments)
        {
            if (payments.Count() >= (int)TransactionConstants.maxGasObjects)
                throw new Exception("Gas Payment is too high.");

            this.BlockDataBuilder.Builder.GasConfig.Payment = payments;
        }
    
        /// <summary>
        /// Creates and appends a `TransactionBlockInput` object to the `blockData.builder.inputs`
        /// array and returns it. The type and value of the `TransactionBlockInput` are specified
        /// by the function parameters.
        /// </summary> ✅
        /// <param name="type">A `Sui.Types.Type` representing the type of the input.</param>
        /// <param name="value">An `ISerializable` representing the value of the input.</param>
        /// <returns>A `TransactionBlockInput` object.</returns>
        public SuiTransactionArgument AddInput
        (
            CallArgumentType type,
            ISerializable value
        )
        {
            int index = this.BlockDataBuilder.Builder.Inputs.Count();
            var input = new TransactionBlockInput(index, value, type);
            this.BlockDataBuilder.Builder.Inputs.Add(input);
            return new SuiTransactionArgument(TransactionArgumentKind.Input, input);
        }

        ///  ✅
        public SuiTransactionArgument AddObjectInput(ITransactionObjectInput value)
        {
            if (value.Type == TransactionObjectInputType.transactionObjectArgument)
                return ((TransactionArgumentTransactionObjectInput)value).Input;

            string id = InputsHandler.GetIDFromCallArg(value);
            TransactionBlockInput[] inserted_arr = BlockDataBuilder.Builder.Inputs.Where((input) => {
                if (input.Value == null || input.Value.GetType() != typeof(BString))
                    return false;
                return id == Utils.NormalizeSuiAddress(((BString)input.Value).Value);
            }).ToArray();

            if (inserted_arr.Count() != 0)
                return new SuiTransactionArgument(TransactionArgumentKind.Input, inserted_arr[0]);

            switch(value.Type)
            {
                case TransactionObjectInputType.stringArgument:
                    string string_argument = ((StringTransactionObjectInput)value).Input;
                    return AddInput(CallArgumentType.Object, new BString(string_argument));
                case TransactionObjectInputType.objectCallArgument:
                    ObjectArg object_call_argument = ((CallArgTransactionObjectInput)value).Input;
                    return AddInput(CallArgumentType.Object, object_call_argument);
                case TransactionObjectInputType.transactionObjectArgument:
                    SuiTransactionArgument transaction_argument = ((TransactionArgumentTransactionObjectInput)value).Input;
                    return AddInput(CallArgumentType.Object, transaction_argument);
            }

            throw new Exception("Not Implemented");
        }

        /// ✅
        public SuiTransactionArgument AddObjectInput(string id)
        {
            return AddObjectInput(new StringTransactionObjectInput(id));
        }

        /// ✅
        public SuiTransactionArgument AddObjectInput(IObjectArgument object_argument)
        {
            switch(object_argument.Type)
            {
                case ObjectArgumentType.stringArgument:
                    string argument_string = ((StringObjectArgument)object_argument).Argument;
                    return AddObjectInput(new StringTransactionObjectInput(argument_string));
                case ObjectArgumentType.transactionObjectArgument:
                    SuiTransactionArgument argument_transaction = ((TransactionObjectArgument)object_argument).Argument;
                    return AddObjectInput(new TransactionArgumentTransactionObjectInput(argument_transaction));
            }

            throw new Exception("Not Implemented");
        }

        /// ✅
        public SuiTransactionArgument AddObjectRef(ObjectArg object_arg)
        {
            return AddObjectInput(new CallArgTransactionObjectInput(object_arg));
        }

        /// ✅
        public SuiTransactionArgument AddSharedObjectRef(SharedObjectRef shared_object_ref)
        {
            return AddObjectInput(new CallArgTransactionObjectInput(InputsHandler.SharedObjectRef(shared_object_ref)));
        }

        /// <summary>
        /// Add a new non-object input to the transaction.
        /// </summary>
        /// <param name="value">
        ///     Can be a BString, Bytes, U8, U64, AccountAddress
        ///     The pure value that will be used as the input value.
        ///     If this is a Uint8Array, then the value is assumed
        ///     to be raw bytes, and will be used directly.
        /// </param> ✅
        /// <returns></returns>
        public SuiTransactionArgument AddPure(ISerializable value)
        {
            return AddInput(CallArgumentType.Pure, new CallArg(CallArgumentType.Pure, new PureCallArg(value)));
        }

        /// <summary>
        /// Add a new non-object input to the transaction.
        /// </summary>
        /// <param name="value">
        ///     A byte array representing the raw data of the
        ///     pure value.
        /// </param> ✅
        /// <returns></returns>
        public SuiTransactionArgument AddPure(byte[] value)
        {
            return AddInput(CallArgumentType.Pure, new CallArg(CallArgumentType.Pure, new PureCallArg(value)));
        }

        /// <summary>
        /// Appends a `SuiTransaction` object to the `BlockDataBuilder.Transactions` array and
        /// returns a `TransactionArgument` object representing the result.
        /// </summary>
        /// <param name="transaction">A `SuiTransaction` object to be added.</param>
        /// <param name="return_value_count">
        /// If using a `MoveCall` transaction, this is the amount
        /// of return values (if greater than 1) that will be returned by the move call.
        /// </param> ✅
        /// <returns>A `SuiTransactionArgument` object representing the result of the addition.</returns>
        public List<SuiTransactionArgument> AddTransaction(Types.SuiTransaction transaction, int? return_value_count = null)
        {
            BlockDataBuilder.Builder.Transactions.Add(transaction);

            int index = BlockDataBuilder.Builder.Transactions.Count();

            TransactionResult transaction_result = new TransactionResult((ushort)(index - 1), (ushort?)return_value_count);

            List<SuiTransactionArgument> results = new List<SuiTransactionArgument>();

            if (return_value_count == null)
                results.Add(transaction_result.TransactionArgument);
            else
            {
                foreach(SuiTransactionArgument nested_result in transaction_result)
                    results.Add(nested_result);

                results.Reverse();
            }

            return results;
        }

        /// <summary>
        /// Add a SplitCoins transaction to our list of transaction in
        /// the Programmable Transaction Block.
        ///
        /// <code>
        ///     const coins = await toolbox.getGasObjectsOwnedByAddress();
        ///     const tx = new TransactionBlock();
        ///     const coin_0 = coins[0].data as SuiObjectData;
        /// 
        ///     const coin = tx.splitCoins(tx.object(coin_0.objectId), [tx.pure(DEFAULT_GAS_BUDGET * 2)]);
        ///     tx.transferObjects([coin], tx.pure(toolbox.address()));
        ///
        ///     // Another example
        ///     const txb = new TransactionBlock();
        ///     const [coin] = txb.splitCoins(txb.gas, [txb.pure(1)]);
        ///     txb.transferObjects([coin], txb.pure(currentAccount!.address));
        ///
        ///     // Another example of transferring many objects to one address.
        ///     const [nft1, nft2] = txb.moveCall({ target: '0x2::nft::mint_many' });
        ///     txb.transferObjects([nft1, nft2], txb.pure(address));
        /// </code>
        /// </summary> ✅
        /// <param name="coin">GasCoin is a type of `TransactionArgument`.</param>
        /// <param name="amounts">A list of respective amounts for each coin we are splitting.</param>
        /// <returns>A list of `TransactionResult`s.</returns>
        public List<SuiTransactionArgument> AddSplitCoinsTx(SuiTransactionArgument coin, params SuiTransactionArgument[] amounts)
        {
            SplitCoins splitCoinsTx = new SplitCoins(coin, amounts);
            return this.AddTransaction(new Types.SuiTransaction(Kind.SplitCoins, splitCoinsTx));
        }

        /// <summary>
        /// Merges multiple source coins into a single destination coin.
        /// </summary> ✅
        /// <param name="destination">An `ITransactionArgument` representing the destination coin.</param>
        /// <param name="sources">An array of `ITransactionArgument` representing the source coins.</param>
        /// <returns>A `SuiTransactionArgument` array representing the result of the merge coin operation.</returns>
        public List<SuiTransactionArgument> AddMergeCoinsTx(SuiTransactionArgument destination, SuiTransactionArgument[] sources)
        {
            MergeCoins merge_coins_tx = new MergeCoins(destination, sources);
            return this.AddTransaction(new Types.SuiTransaction(Kind.MergeCoins, merge_coins_tx));
        }

        /// <summary>
        /// Publishes modules with given dependencies.
        /// </summary> ✅
        /// <param name="modules">An array of `byte[]` representing the modules to be published.</param>
        /// <param name="dependencies">An array of `AccountAddress` representing the dependencies.</param>
        /// <returns>A `SuiTransactionArgument` array representing the result of the publish operation.</returns>
        public List<SuiTransactionArgument> AddPublishTx(byte[][] modules, AccountAddress[] dependencies)
        {
            Publish publish_tx = new Publish(modules, dependencies);
            return this.AddTransaction(new Types.SuiTransaction(Kind.Publish, publish_tx));
        }

        /// <summary>
        /// Publishes modules with given dependencies.
        /// </summary> ✅
        /// <param name="modules">An array of `string` representing the modules to be published.</param>
        /// <param name="dependencies">An array of `string` representing the dependencies.</param>
        /// <returns>A `SuiTransactionArgument` array representing the result of the publish operation.</returns>
        public List<SuiTransactionArgument> AddPublishTx(string[] modules, string[] dependencies)
        {
            Publish publish_tx = new Publish
            (
                modules.Select((module) => {
                    return CryptoBytes.FromBase64String(module);
                }).ToArray(),
                dependencies.Select((dependency) => AccountAddress.FromHex(dependency)).ToArray()
            );
            return this.AddTransaction(new Types.SuiTransaction(Kind.Publish, publish_tx));
        }

        /// <summary>
        /// Upgrades modules with given dependencies, packageId, and ticket.
        /// </summary> ✅
        /// <param name="modules">An array of `byte[]` representing the modules to be upgraded.</param>
        /// <param name="dependencies">An array of `AccountAddress` representing the dependencies.</param>
        /// <param name="packageId">A `string` representing the package ID.</param>
        /// <param name="ticket">An `ITransactionArgument` representing the ticket.</param>
        /// <returns>A `SuiTransactionArgument` array representing the result of the upgrade operation.</returns>
        public List<SuiTransactionArgument> AddUpgradeTx
        (
            byte[][] modules,
            AccountAddress[] dependencies,
            string packageId,
            SuiTransactionArgument ticket
        )
        {
            Upgrade upgrade_tx = new Upgrade(modules, dependencies, packageId, ticket);
            return this.AddTransaction(new Types.SuiTransaction(Kind.Upgrade, upgrade_tx));
        }

        /// <summary>
        /// Makes a move call with target, optional arguments, and optional type arguments.
        /// </summary> ✅
        /// <param name="target">A `SuiMoveNormalizedStructType` representing the target of the move call.</param>
        /// <param name="typeArguments">An optional array of `SerializableTypeTag` representing the arguments of the move call.</param>
        /// <param name="arguments">An optional array of `SuiTransactionArgument` representing the type arguments of the move call.</param>
        /// <param name="return_value_count">The number of return values, greater than 1, that are returned by the move call.</param>
        /// <returns>A `SuiTransactionArgument` array representing the result of the move call.</returns>
        public List<SuiTransactionArgument> AddMoveCallTx
        (
            SuiMoveNormalizedStructType target,
            SerializableTypeTag[] typeArguments = null,
            SuiTransactionArgument[] arguments = null,
            int? return_value_count = null
        )
        {
            MoveCall moveCallTx = new MoveCall(target, typeArguments, arguments);
            return AddTransaction(new Types.SuiTransaction(Kind.MoveCall, moveCallTx), return_value_count);
        }

        /// <summary>
        /// Transfers objects to a specified address.
        /// </summary> ✅
        /// <param name="objects">An array of `ITransactionArgument` representing the objects to be transferred.</param>
        /// <param name="address">An `ITransactionArgument` representing the address to transfer objects to.</param>
        /// <returns>A `SuiTransactionArgument` array representing the result of the transfer object operation.</returns>
        public List<SuiTransactionArgument> AddTransferObjectsTx(SuiTransactionArgument[] objects, SuiTransactionArgument address)
        {
            TransferObjects transfer_tx = new TransferObjects(objects, address);
            return this.AddTransaction(new Types.SuiTransaction(Kind.TransferObjects, transfer_tx));
        }

        /// <summary>
        /// Transfers objects to a specified address.
        /// </summary> ✅
        /// <param name="objects">An array of `ITransactionArgument` representing the objects to be transferred.</param>
        /// <param name="address">A `string` representing the address to transfer objects to.</param>
        /// <returns>A `SuiTransactionArgument` array representing the result of the transfer object operation.</returns>
        public List<SuiTransactionArgument> AddTransferObjectsTx(SuiTransactionArgument[] objects, string address)
        {
            TransferObjects transfer_tx = new TransferObjects(objects, this.AddPure(AccountAddress.FromHex(address)));
            return this.AddTransaction(new Types.SuiTransaction(Kind.TransferObjects, transfer_tx));
        }

        /// <summary>
        /// Makes a Move Vector with the specified type and objects.
        /// </summary> ✅
        /// <param name="objects">An array of `ITransactionArgument` representing the objects of the Move Vector.</param>
        /// <param name="type">An optional `SuiStructTag` representing the type of the Move Vector.</param>
        /// <returns>A `SuiTransactionArgument` array representing the result of the make Move Vector operation.</returns>
        public List<SuiTransactionArgument> AddMakeMoveVecTx(SuiTransactionArgument[] objects, SuiStructTag type = null)
        {
            MakeMoveVec make_move_vec_tx = new MakeMoveVec(objects, type);
            return this.AddTransaction(new Types.SuiTransaction(Kind.MakeMoveVec, make_move_vec_tx));
        }

        /// <summary>
        /// Retrieves a configuration value for a specified key.
        /// </summary> ✅
        /// <param name="key">A `LimitKey` representing the key for which the configuration value needs to be retrieved.</param>
        /// <param name="build_options">A `BuildOptions` object containing the build options including limits and protocolConfig.</param>
        /// <returns>An `int` representing the configuration value for the specified key.</returns>
        private SuiResult<BigInteger?> GetConfig(LimitKey key, BuildOptions build_options)
        {
            if (build_options.Limits != null && build_options.Limits[BuildOptions.TransactionLimits[key]].HasValue)
            {
                int? result = build_options.Limits[BuildOptions.TransactionLimits[key]];

                if (result == null)
                    return new SuiResult<BigInteger?>
                    (
                        null,
                        new SuiError
                        (
                            0,
                            $"Unable to find {key}, got {BuildOptions.TransactionLimits[key]} and {build_options.Limits[BuildOptions.TransactionLimits[key]]}.",
                            null
                        )
                    );

                return new SuiResult<BigInteger?>(new BigInteger((int)result));
            }

            if (build_options.ProtocolConfig == null)
                return new SuiResult<BigInteger?>(new BigInteger(DefaultOfflineLimits[key]));

            AttributeValue attribute = build_options.ProtocolConfig.Attributes[BuildOptions.TransactionLimits[key]];
            BigInteger? attribute_value = attribute.GetValue();

            if (attribute_value == null)
                return new SuiResult<BigInteger?>(null, (SuiError)attribute.Error);

            return new SuiResult<BigInteger?>(attribute_value);
        }

        /// <summary>
        /// Builds a block with the specified provider and optional transaction kind.
        /// </summary> ✅
        /// <param name="build_options">An instance of `BuildOptions` that contains the options passed for preparing the transaction block.</param>
        /// <returns>A `byte[]` object representing the built block.</returns>
        public async Task<SuiResult<byte[]>> Build(BuildOptions build_options)
        {
            ErrorBase err = await Prepare(build_options);

            if (err != null)
                return new SuiResult<byte[]>(null, err);

            return BlockDataBuilder.Build(null, build_options.OnlyTransactionKind);
        }

        /// <summary>
        /// Computes the digest of the block with the specified provider.
        /// </summary> ✅
        /// <param name="build_options">An instance of `BuildOptions` that contains the options passed for preparing the transaction block.</param>
        /// <returns>A `string` representing the digest of the block.</returns>
        public async Task<SuiResult<string>> GetDigest(BuildOptions build_options)
        {
            ErrorBase err = await Prepare(build_options);

            if (err != null)
                return new SuiResult<string>(null, err);

            return BlockDataBuilder.GetDigest();
        }

        /// <summary>
        /// Determines whether the sender is missing considering the specified transaction kind.
        /// </summary> ✅
        /// <param name="only_transaction_kind">An optional `bool` representing whether only transaction kind should be considered.</param>
        /// <returns>A `bool` indicating whether the sender is missing.</returns>
        private bool IsMissingSender(bool? only_transaction_kind = null)
            => only_transaction_kind.HasValue && only_transaction_kind == false && BlockDataBuilder.Builder.Sender == null;

        /// <summary>
        /// Prepares gas payment for transactions.
        /// </summary> ✅
        /// <param name="options">A `BuildOptions` object that contains the `Provider` and `OnlyTransactionKind` members.</param>
        /// <returns>A `Task` object used for implementation with Coroutines.</returns>
        public async Task<ErrorBase> PrepareGasPaymentAsync(BuildOptions options)
        {
            if (IsMissingSender(options.OnlyTransactionKind))
                return new SuiError(0, "Sender Is Missing", null);

            if ((options.OnlyTransactionKind.HasValue && options.OnlyTransactionKind == true) || this.BlockDataBuilder.Builder.GasConfig.Payment != null)
                return null;

            if (BlockDataBuilder.Builder.GasConfig.Owner == null && BlockDataBuilder.Builder.Sender == null)
                return new SuiError(0, "Gas Owner Cannot Be Found", null);

            AccountAddress gas_owner =
                BlockDataBuilder.Builder.GasConfig.Owner != null ?
                BlockDataBuilder.Builder.GasConfig.Owner :
                BlockDataBuilder.Builder.Sender;

            RpcResult<CoinPage> coins = await options.Provider.GetCoinsAsync(gas_owner, new SuiStructTag("0x2::sui::SUI"));

            if (coins.Error != null)
                return coins.Error;

            List<CoinDetails> filtered_coins = coins.Result.Data.Where((coin) => {
                return BlockDataBuilder.Builder.Inputs.Any((input) => {
                    if (input.Value.GetType() == typeof(CallArg))
                    {
                        CallArg call_arg = (CallArg)input.Value;
                        if (call_arg.Type == CallArgumentType.Object)
                        {
                            ObjectCallArg object_arg = (ObjectCallArg)call_arg.CallArgument;
                            if (object_arg.ObjectArg.Type == ObjectRefType.ImmOrOwned)
                            {
                                return
                                    coin.CoinObjectID ==
                                    ((Sui.Types.SuiObjectRef)object_arg.ObjectArg.ObjectRef).ObjectID;
                            }
                        }
                    }
                    return false;
                }) == false;
            }).ToList();

            int coin_range_max = Math.Min(filtered_coins.Count(), (int)TransactionConstants.maxGasObjects);
            List<int> range = Enumerable.Range(0, coin_range_max).ToList();

            IEnumerable<Sui.Types.SuiObjectRef> payment_coins = range.Select((idx) => {
                CoinDetails coin = filtered_coins.ElementAt(idx);
                return new Sui.Types.SuiObjectRef
                (
                    coin.CoinObjectID,
                    coin.Version,
                    coin.Digest
                );
            });

            if (payment_coins.Count() == 0)
                return new SuiError(0, "Owner Does Not Have Payment Coins", null);

            SetGasPayment(payment_coins.ToArray());

            return null;
        }

        /// <summary>
        /// Prepares gas price for transactions.
        /// </summary> ✅
        /// <param name="options">A `BuildOptions` object that contains the `Provider` and `OnlyTransactionKind` members.</param>
        /// <returns>A `Task` object used for implementation with Coroutines.</returns>
        public async Task<ErrorBase> PrepareGasPriceAsync(BuildOptions options)
        {
            if (IsMissingSender(options.OnlyTransactionKind))
                return new SuiError(-1, "Sender Is Missing", null);

            RpcResult<BigInteger> gas_price = await options.Provider.GetReferenceGasPriceAsync();

            if (gas_price.Error != null)
                return gas_price.Error;

            SetGasPrice(gas_price.Result);

            return null;
        }

        /// <summary>
        /// Resolves all required Move modules and objects.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task<ErrorBase> PrepareTransactions(BuildOptions options)
        {
            // The inputs in the `TransactionBlock`
            List<TransactionBlockInput> inputs              = this.BlockDataBuilder.Builder.Inputs;
            Types.SuiTransaction[] transactions             = this.BlockDataBuilder.Builder.Transactions.ToArray();

            // A list of move modules identified as needing to be resolved
            List<MoveCall> moveModulesToResolve     = new List<MoveCall>();
            List<ObjectToResolve> objectsToResolve  = new List<ObjectToResolve>();
            List<ObjectToResolve> resolved_objects  = new List<ObjectToResolve>();

            foreach (TransactionBlockInput input in inputs)
            {
                // The value is an ObjectID (AccountAddress) add it to the objects to resolve
                if (input.Value.GetType() == typeof(BString))
                {
                    if (Regex.IsMatch(((BString)input.Value).Value, @"^(0x)?[0-9a-fA-F]{32,64}$"))
                    {
                        ObjectToResolve objectToResolve = new ObjectToResolve
                        (
                            ((BString)input.Value).Value,
                            input,
                            null
                        );
                        objectsToResolve.Add(objectToResolve);
                    }
                }
            }

            #region Process all transactions
            foreach (Sui.Transactions.Types.SuiTransaction transaction in transactions)
            {
                #region Process MoveCall Transaction
                // Special case move call:
                if (transaction.Kind == Kind.MoveCall)
                {
                    // Determine if any of the arguments require encoding.
                    // - If they don't, then this is good to go.
                    // - If they do, then we need to fetch the normalized move module.
                    MoveCall moveTx = (MoveCall)transaction.Transaction;
                    SuiTransactionArgument[] arguments = moveTx.Arguments;

                    bool needsResolution = arguments.Any(arg => {
                        if (arg.Kind == TransactionArgumentKind.Input)
                        {
                            TransactionBlockInput argInput = (TransactionBlockInput)arg.TransactionArgument;

                            // Is it a PureCallArg or ObjectCallArg?
                            // If the argument is a `TransactionBlockInput`
                            // and the value of the input at `index` is NOT a BuilderArg (`CallArg`)
                            // then we need to resolve it.
                            return inputs[argInput.Index].Value.GetType() != typeof(CallArg);
                        }
                        return false;
                    });

                    // If any of the arguments in the MoveCall need to be resolved
                    // This loop verifies that the move calls within the resolve list
                    // match up with that in the current transaction within the outer loop
                    // of blockDataBuilder's transactions
                    if(needsResolution)
                    {
                        foreach(MoveCall move_call in moveModulesToResolve)
                        {
                            foreach (var argument_outer in moveTx.Arguments.Select((value, i) => new { i, value }))
                            {
                                if (argument_outer.value.Kind == TransactionArgumentKind.Input)
                                {
                                    foreach (var argument_inner in move_call.Arguments.Select((value, i) => new { i, value }))
                                    {
                                        if
                                        (
                                            argument_inner.value.Kind == TransactionArgumentKind.Input
                                        )
                                        {
                                            TransactionBlockInput outer_input =
                                                (TransactionBlockInput)argument_outer.value.TransactionArgument;
                                            TransactionBlockInput inner_input =
                                                (TransactionBlockInput)argument_inner.value.TransactionArgument;

                                            if
                                            (
                                                outer_input.Value.Equals(inner_input.Value) &&
                                                outer_input.Index != inner_input.Index
                                            )
                                            {
                                                moveTx.Arguments[argument_outer.i].Equals(move_call.Arguments[argument_inner.i]);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                        moveModulesToResolve.Add(moveTx);
                    }
                }
                #endregion END - Process MoveCall Transaction

                #region Process TransferObjects Transaction
                else if (transaction.Kind == Kind.TransferObjects)
                {
                    TransferObjects transferObjectsTx = (TransferObjects)transaction.Transaction;
                    SuiTransactionArgument address = transferObjectsTx.Address;

                    if (address.Kind == TransactionArgumentKind.Input)
                    {
                        // Cast the address as a `TransactionBlockInput` to get index property
                        TransactionBlockInput addressInput = (TransactionBlockInput)address.TransactionArgument;
                        // Get the TXBInput object at the index provided by the addressInput argument
                        TransactionBlockInput input = inputs[addressInput.Index];

                        // If the value of the input is not an object type then it must be a Pure
                        if (input.Value.GetType() != typeof(CallArg) && input.Value.GetType() != typeof(AccountAddress))
                        {
                            Serialization ser = new Serialization();
                            input.Value.Serialize(ser);
                            this.BlockDataBuilder.Builder.Inputs[addressInput.Index].Value = new CallArg(CallArgumentType.Pure, new PureCallArg(ser.GetBytes()));
                        }
                    }
                }
                #endregion END - Process TransferObjects Transaction

                #region Process SplitCoins Transaction
                // Special handling for values that where previously encoded using the wellKnownEncoding pattern.
                // This should only happen when transaction block data was hydrated from an old version of the SDK
                else if (transaction.Kind == Kind.SplitCoins)
                {
                    SplitCoins splitCoinsTx = (SplitCoins)transaction.Transaction;
                    SuiTransactionArgument[] amounts = splitCoinsTx.Amounts;
                    foreach(SuiTransactionArgument amount in amounts)
                    {
                        if(amount.Kind == TransactionArgumentKind.Input)
                        {   // Cast the amount as a `TransactionBlockInput` to get index property
                            TransactionBlockInput amountTxbInput = (TransactionBlockInput)amount.TransactionArgument;
                            // Get the TXBInput object at the index provided by the amount argument
                            TransactionBlockInput input = inputs[amountTxbInput.Index];

                            // If the value of the input is not an object type then it must be a Pure
                            if(input.Value.GetType() != typeof(CallArg) && input.Value.GetType() != typeof(AccountAddress))
                            {
                                Serialization ser = new Serialization();
                                input.Value.Serialize(ser);
                                this.BlockDataBuilder.Builder.Inputs[amountTxbInput.Index].Value = new CallArg(CallArgumentType.Pure, new PureCallArg(ser.GetBytes()));
                            }
                        }
                    }
                }
                #endregion END - Process SplitCoins Transaction
            }
            #endregion END - Process all transactions 

            #region Resolve Move modules
            if (moveModulesToResolve.Count > 0)
            {
                foreach(MoveCall moveCall in moveModulesToResolve)
                {
                    #region RPC Call GetNormalizedMoveFunction
                    RpcResult<NormalizedMoveFunctionResponse> result
                        = await options.Provider.GetNormalizedMoveFunctionAsync(moveCall.Target);

                    NormalizedMoveFunctionResponse normalized = result.Result;
                    #endregion END - RPC Call

                    //// Entry functions can have a mutable reference to an instance of the TxContext
                    //// struct defined in the TxContext module as the last parameter. The caller of
                    //// the function does not need to pass it in as an argument.
                    bool hasTxContext = normalized.HasTxContext();

                    //// The list of parameters returned by the RPC call
                    List<SuiMoveNormalizedType> paramsList
                        = hasTxContext
                        ? normalized.Parameters.Take(normalized.Parameters.Count() - 1).ToList()
                        : normalized.Parameters.ToList();

                    if (paramsList.Count != moveCall.Arguments.Length)
                        return new RpcError(-1, "Incorrect number of arguments.", null);

                    foreach (Tuple<int, SuiMoveNormalizedType> param_enumerated in paramsList.Select((param, i) => new Tuple<int, SuiMoveNormalizedType>(i, param)))
                    {
                        SuiTransactionArgument arg = moveCall.Arguments[param_enumerated.Item1];

                        if (arg.Kind != TransactionArgumentKind.Input)
                            continue;

                        TransactionBlockInput inputArg = (TransactionBlockInput)arg.TransactionArgument;
                        TransactionBlockInput input = inputs[inputArg.Index];

                        //// Skip if the input is already resolved, aka if the input is a BuilderArg
                        if (input.Value.GetType() == typeof(CallArg)) continue;

                        //// When we reach here, this means that the value could be a BString, a U8, etc.
                        //// We need to compare agains the RPC response params to know how to cast to a concrete type
                        //// Once we know how to cast, then we will be able to serialize it later on
                        ISerializable inputValue = input.Value;
                        string ser_type = Serializer.GetPureNormalizedType(param_enumerated.Item2, inputValue);

                        if (ser_type != null)
                        {
                            this.BlockDataBuilder.Builder.Inputs[inputArg.Index].Value = new PureCallArg(inputValue);
                            continue;
                        }

                        if (Serializer.ExtractStructType(param_enumerated.Item2) == null && param_enumerated.Item2.Type != SuiMoveNormalizedTypeSerializationType.TypeParameter)
                            return new RpcError(-1, "Unknown Call Arg Type", param_enumerated.Item2);

                        if (inputValue.GetType() == typeof(BString))
                        {
                            objectsToResolve.Add(new ObjectToResolve(((BString)inputValue).Value, input, param_enumerated.Item2));
                            continue;
                        }

                        return new RpcError(-1, "Input Value Is Not Object ID", inputValue);
                    }
                }
            }
            #endregion END - Resolve MoveModules

            #region Resolve objects
            if (objectsToResolve.Count > 0)
            {
                List<string> mappedIds = objectsToResolve.Select(x => x.Id).ToList();
                // NOTE: Insertion order in HashSet will be maintained until removing or re-adding
                List<string> dedupedIds = new HashSet<string>(mappedIds).ToList();

                // TODO: In the TypeScript SDK they use `Set` which is a set that maintains insertion order
                // TODO: Find data structure that does this in C#
                // https://gist.github.com/gmamaladze/3d60c127025c991a087e

                // Chunk list of IDs into smaller lists to use in RPC Call `MultiGetObjects`
                List<List<string>> objectChunks = dedupedIds.Chunked((int)TransactionConstants.maxObjectsPerFetch);

                List<ObjectDataResponse> objectsResponse = new List<ObjectDataResponse>();
                foreach(List<string> objectIds in objectChunks)
                {
                    #region RPC Call MultiGetObjects
                    RpcResult<IEnumerable<ObjectDataResponse>> response
                        = await options.Provider.MultiGetObjectsAsync(
                            objectIds.Select((id) => AccountAddress.FromHex(id)).ToList(),
                            new ObjectDataOptions(show_owner: true)
                    );
                    #endregion END - Call MultiGetObjects

                    List<ObjectDataResponse> objects = response.Result.ToList();
                    objectsResponse.AddRange(objects);
                }

                // Create a map of IDs to ObjectDataResponse
                Dictionary<string, ObjectDataResponse> objectsById
                    = new Dictionary<string, ObjectDataResponse>();

                // Populate map(Dictionary) `objectsById`
                for (int i = 0; i < dedupedIds.Count; i++)
                {
                    string id = dedupedIds[i];
                    ObjectDataResponse obj = objectsResponse[i];
                    objectsById.Add(id, obj);
                }

                // Filter objects that returned an error
                List<string> invalidObjects
                    = objectsById.Where(
                        obj => obj.Value.Error != null
                    ).Select(obj => obj.Key).ToList();

                if (invalidObjects.Count > 0)
                    return new RpcError(-1, "Invalid object found.", invalidObjects);

                foreach (ObjectToResolve objectToResolve in objectsToResolve)
                {
                    ObjectDataResponse obj = objectsById[objectToResolve.Id];

                    int? initialSharedVersion = obj.GetSharedObjectInitialVersion();

                    if (initialSharedVersion == null)
                    {
                        Sui.Types.SuiObjectRef obj_ref = obj.GetObjectReference();

                        if (obj_ref == null)
                            continue;

                        objectToResolve.Input.Value = new CallArg(CallArgumentType.Object, new ObjectCallArg(new ObjectArg(ObjectRefType.ImmOrOwned, obj_ref)));
                        if (resolved_objects.Count > objectToResolve.Input.Index)
                            resolved_objects[objectToResolve.Input.Index] = objectToResolve;
                        else
                            resolved_objects.Add(objectToResolve);

                        continue;
                    }

                    bool is_by_value =
                        objectToResolve.NormalizedType != null &&
                        Serializer.ExtractStructType(objectToResolve.NormalizedType) == null;

                    bool mutable = is_by_value || (
                        objectToResolve.NormalizedType != null &&
                        Serializer.ExtractMutableReference(objectToResolve.NormalizedType) != null
                    );

                    objectToResolve.Input.Value = new CallArg
                    (
                        CallArgumentType.Object,
                        new ObjectCallArg
                        (
                            new ObjectArg
                            (
                                ObjectRefType.Shared,
                                new SharedObjectRef
                                (
                                    AccountAddress.FromHex(objectToResolve.Id),
                                    (int)initialSharedVersion,
                                    mutable
                                )
                            )
                        )
                    );

                    if (resolved_objects.Count > objectToResolve.Input.Index)
                    {
                        if (resolved_objects[objectToResolve.Input.Index].Input.Value.GetType() == typeof(CallArg))
                        {
                            CallArg call_arg_resolved = (CallArg)resolved_objects[objectToResolve.Input.Index].Input.Value;
                            if (objectToResolve.Input.Value.GetType() == typeof(CallArg))
                            {
                                CallArg call_arg_to_resolve = (CallArg)objectToResolve.Input.Value;
                                if
                                (
                                    call_arg_to_resolve.Type == CallArgumentType.Object &&
                                    ((ObjectCallArg)call_arg_to_resolve.CallArgument).ObjectArg.Type == ObjectRefType.Shared
                                )
                                {
                                    SharedObjectRef shared = (SharedObjectRef)((ObjectCallArg)call_arg_to_resolve.CallArgument).ObjectArg.ObjectRef;
                                    shared.Mutable =
                                        (call_arg_resolved.Type == CallArgumentType.Object && ((ObjectCallArg)call_arg_resolved.CallArgument).IsMutableSharedObjectInput()) ||
                                        shared.Mutable;
                                    objectToResolve.Input.Value = new CallArg
                                    (
                                        CallArgumentType.Object,
                                        new ObjectCallArg(new ObjectArg(ObjectRefType.Shared, shared))
                                    );
                                }
                            }
                        }
                        resolved_objects[objectToResolve.Input.Index] = objectToResolve;
                    }
                    else
                        resolved_objects.Add(objectToResolve);
                }

                foreach (ObjectToResolve resolved_object in resolved_objects)
                    this.BlockDataBuilder.Builder.Inputs[resolved_object.Input.Index] = resolved_object.Input;

                this.BlockDataBuilder.Builder.Inputs.Sort((TransactionBlockInput t1, TransactionBlockInput t2) => t1.Index.CompareTo(t2.Index));
            }
            #endregion END - Resolve objects

            return null;
        }

        public class ObjectToResolve
        {
            public string Id { get; set; }
            public TransactionBlockInput Input { get; set; }
            public SuiMoveNormalizedType NormalizedType;

            public ObjectToResolve(string id, TransactionBlockInput input, SuiMoveNormalizedType normalizedType) 
            {
                this.Id = id;
                this.Input = input;
                this.NormalizedType = normalizedType;
            }
        }

        /// <summary>
        /// Prepares the transaction block with the provided build options.
        /// </summary>
        /// <param name="options_passed">An instance of `BuildOptions` that contains the options passed for preparing the transaction block.</param>
        /// <returns>A `Task` object used for implementations with async calls.</returns>
        private async Task<ErrorBase> Prepare(BuildOptions options_passed)
        {
            if (IsPrepared)
                return null;

            BuildOptions options = options_passed;

            if (options.ProtocolConfig == null)
            {
                RpcResult<ProtocolConfig> protocol_config = await options.Provider.GetProtocolConfigAsync();

                if (protocol_config.Error != null)
                    return protocol_config.Error;

                options.ProtocolConfig = protocol_config.Result;
            }

            ErrorBase gas_price_error = await PrepareGasPriceAsync(options);

            if (gas_price_error != null)
                return gas_price_error;

            ErrorBase prepare_transactions_error = await PrepareTransactions(options);

            if (prepare_transactions_error != null)
                return prepare_transactions_error;

            if (options.OnlyTransactionKind == null || (options.OnlyTransactionKind != null && options.OnlyTransactionKind == false))
            {
                ErrorBase gas_payment_error = await this.PrepareGasPaymentAsync(options);

                if (gas_payment_error != null)
                    return gas_payment_error;

                if (this.BlockDataBuilder.Builder.GasConfig.Budget == null)
                {
                    GasData gas_config = new GasData();

                    SuiResult<BigInteger?> gas_budget_result = GetConfig(LimitKey.MaxTxGas, options);

                    if (gas_budget_result.Error != null)
                        return gas_budget_result.Error;

                    gas_config.Budget = gas_budget_result.Result;
                    gas_config.Payment = new Sui.Types.SuiObjectRef[] { };
                    gas_config.Price = BlockDataBuilder.Builder.GasConfig.Price;
                    gas_config.Owner = BlockDataBuilder.Builder.GasConfig.Owner;

                    TransactionBlockDataBuilderSerializer tx_block_data_builder = this.BlockDataBuilder;

                    SuiResult<byte[]> build_result = tx_block_data_builder.Build(new TransactionBlockDataBuilderSerializer(new TransactionBlockDataBuilder(gasConfig: gas_config)));

                    if (build_result.Error != null)
                        return build_result.Error;

                    RpcResult<TransactionBlockResponse> dry_run_result = await options.Provider.DryRunTransactionBlockAsync
                    (
                        Convert.ToBase64String(build_result.Result)
                    );

                    if (dry_run_result.Error != null || dry_run_result.Result.Effects.Status.Status == ExecutionStatus.Failure)
                        return dry_run_result.Error ?? new SuiError(-1, $"Transaction failed with message - {dry_run_result.Result.Effects.Status.Error}", null);

                    int safe_overhead =
                        (int)TransactionConstants.gasSafeOverhead *
                        int.Parse
                        (
                            BlockDataBuilder.Builder.GasConfig.Price != null ?
                                BlockDataBuilder.Builder.GasConfig.Price.ToString() :
                                "1"
                        );

                    int base_computation_cost_with_overhead =
                        int.Parse
                        (
                            dry_run_result.Result.Effects.GasUsed.ComputationCost != null ?
                                dry_run_result.Result.Effects.GasUsed.ComputationCost.ToString() :
                                "0"
                        ) +
                        safe_overhead;

                    int gas_budget =
                        base_computation_cost_with_overhead +
                        int.Parse
                        (
                            dry_run_result.Result.Effects.GasUsed.StorageCost != null ?
                                dry_run_result.Result.Effects.GasUsed.StorageCost.ToString() :
                                "0"
                        ) +
                        int.Parse
                        (
                            dry_run_result.Result.Effects.GasUsed.StorageRebate != null ?
                                dry_run_result.Result.Effects.GasUsed.StorageRebate.ToString() :
                                "0"
                        );

                    SetGasBudget
                    (
                        gas_budget > base_computation_cost_with_overhead ?
                            gas_budget :
                            base_computation_cost_with_overhead
                    );
                }
            }

            IsPrepared = true;
            return null;
        }
    }

    public static class ListExtensions
    {
        public static List<List<T>> Chunked<T>(this List<T> source, int size)
        {
            return Enumerable.Range(0, (source.Count + size - 1) / size)
                             .Select(i => source.Skip(i * size).Take(size).ToList())
                             .ToList();
        }
    }
}