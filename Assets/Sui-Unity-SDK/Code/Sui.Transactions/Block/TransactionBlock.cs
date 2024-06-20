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
using Kind = Sui.Transactions.Types.Kind;
using Org.BouncyCastle.Crypto.Digests;
using NBitcoin.DataEncoders;
using Sui.Transactions.Kinds;

namespace Sui.Transactions
{
    public class TransactionBlockDataBuilderSerializer: ISerializable
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
            Sui.Transactions.Builder.TransactionDataV1 tx_data =
                Sui.Transactions.Builder.TransactionDataV1.Deserialize(der);
            this.Builder = new TransactionBlockDataBuilder(tx_data);
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

            Base58Encoder base58Encoder = new Base58Encoder();
            return base58Encoder.EncodeData(hashed_data);
        }

        public static TransactionBlockDataBuilderSerializer restore(TransactionBlockDataBuilder data)
        {
            return new TransactionBlockDataBuilderSerializer(data);
        }

        public byte[] Build
        (
            TransactionBlockDataBuilderSerializer overrides = null,
            bool? only_transaction_kind = null
        )
        {
            // Resolve inputs down to values:
            List<ICallArg> inputs = this.Builder.Inputs
                .Select((input) => {
                    return input.Value.GetType() == typeof(ICallArg) ? (ICallArg)input.Value : null;
                })
                .Where(value => value != null)
                .Select(value => value)
                .ToList();

            ProgrammableTransaction programmableTx
                = new ProgrammableTransaction(inputs.ToArray(), Builder.Transactions);

            if (only_transaction_kind.HasValue && only_transaction_kind == true)
            {
                Serialization ser = new Serialization();
                ser.Serialize(programmableTx);
                return ser.GetBytes();
            }

            ITransactionExpiration Expiration = overrides.Builder.Expiration != null ? overrides.Builder.Expiration : Builder.Expiration;
            AccountAddress Sender = overrides.Builder.Sender != null ? overrides.Builder.Sender : Builder.Sender;
            GasConfig GasConfig = overrides.Builder.GasConfig != null ? overrides.Builder.GasConfig : Builder.GasConfig;

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

            if (GasConfig.Price == null)
            {
                throw new Exception("Missing gas price");
            }

            Sui.Transactions.Builder.TransactionDataV1 transactionData = new Sui.Transactions.Builder.TransactionDataV1(
                Sender,
                Expiration,
                GasConfig,
                programmableTx
            );

            Serialization serializer = new Serialization();
            serializer.Serialize(transactionData);
            return serializer.GetBytes();
        }

        public string GetDigest()
        {
            byte[] bytes = Build();
            return TransactionBlockDataBuilderSerializer.GetDigestFromBytes(bytes);
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
        public ITransactionArgument Argument;

        public TransactionObjectArgument(ITransactionArgument argument)
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
        public ITransactionArgument Input;

        public TransactionArgumentTransactionObjectInput(ITransactionArgument input)
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
            Dictionary<string, int?> Limits,
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

    public class ProtocolConfig
    {
        public Dictionary<string, ProtocolConfigValue> Attributes;
        public Dictionary<string, bool> FeatureFlags;
        public string MaxSupportedProtocolVersion;
        public string MinSupportedProtocolVersion;
        public string ProtocolVersion;

        public ProtocolConfig
        (
            Dictionary<string, ProtocolConfigValue> Attributes,
            Dictionary<string, bool> FeatureFlags,
            string MaxSupportedProtocolVersion,
            string MinSupportedProtocolVersion,
            string ProtocolVersion
        )
        {
            this.Attributes = Attributes;
            this.FeatureFlags = FeatureFlags;
            this.MaxSupportedProtocolVersion = MaxSupportedProtocolVersion;
            this.MinSupportedProtocolVersion = MinSupportedProtocolVersion;
            this.ProtocolVersion = ProtocolVersion;
        }

        public ProtocolConfig(Rpc.Models.ProtocolConfig config)
        {
            Dictionary<string, ProtocolConfigValue> attribute_dict = new Dictionary<string, ProtocolConfigValue>();
            foreach (var (attribute, details) in config.Attributes)
            {
                ProtocolConfigValue detail_type;

                if (details.F64 != null)
                    detail_type = new ProtocolConfigValue.F64(double.Parse(details.U64));
                else if (details.U64 != null)
                    detail_type = new ProtocolConfigValue.U64(ulong.Parse(details.U64));
                else if (details.U32 != null)
                    detail_type = new ProtocolConfigValue.U32(uint.Parse(details.U32));
                else if (details.U16 != null)
                    detail_type = new ProtocolConfigValue.U16(ushort.Parse(details.U16));
                else
                    throw new Exception("Unable to Unwrap Attribute");

                attribute_dict.Add(attribute, detail_type);
            }

            this.Attributes = attribute_dict;
            this.FeatureFlags = config.FeatureFlags;
            this.MaxSupportedProtocolVersion = config.MaxSupportedProtocolVersion;
            this.MinSupportedProtocolVersion = config.MinSupportedProtocolVersion;
            this.ProtocolVersion = config.ProtocolVersion;
        }
    }

    public abstract class ProtocolConfigValue
    {
        private ProtocolConfigValue() { }

        public sealed class U64 : ProtocolConfigValue
        {
            public ulong Value { get; }
            public U64(ulong value) => Value = value;
        }

        public sealed class U32 : ProtocolConfigValue
        {
            public uint Value { get; }
            public U32(uint value) => Value = value;
        }

        public sealed class F64 : ProtocolConfigValue
        {
            public double Value { get; }
            public F64(double value) => Value = value;
        }

        public sealed class U16 : ProtocolConfigValue
        {
            public ushort Value { get; }
            public U16(ushort value) => Value = value;
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
        /// A list of object that we need to resolve by first querying the RPC API.
        /// </summary> ✅
        private List<ObjectToResolve> objectsToResolve = new List<ObjectToResolve>();

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

        /// <summary> ✅
        /// Set the sender for the programmable transaction block.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public TransactionBlock SetSender(AccountAddress sender)
        {
            this.BlockDataBuilder.Builder.Sender = sender;
            return this;
        }

        /// <summary> ✅
        /// The the Sender, if it has not been set in the programmable transaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public TransactionBlock SetSenderIfNotSet(AccountAddress sender)
        {
            if (this.BlockDataBuilder.Builder.Sender == null)
                return this.SetSender(sender);
            return this;
        }

        /// <summary> ✅
        /// Set expiration for the programmable transaction.
        /// </summary>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public TransactionBlock SetExpiration(ITransactionExpiration expiration)
        {
            this.BlockDataBuilder.Builder.Expiration = expiration;
            return this;
        }

        /// <summary> ✅
        /// Sets the gas price.
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public TransactionBlock SetGasPrice(BigInteger price)
        {
            this.BlockDataBuilder.Builder.GasConfig.Price = price;
            return this;
        }

        /// <summary> ✅
        /// Set the gas budget for the programmable transaction block.
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public TransactionBlock SetGasBudget(int budget)
        {
            this.BlockDataBuilder.Builder.GasConfig.Budget = budget;
            return this;
        }

        /// <summary> ✅
        /// Set the gas owner for the programmable transaction block.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public TransactionBlock SetGasOwner(AccountAddress owner)
        {
            this.BlockDataBuilder.Builder.GasConfig.Owner = owner;
            return this;
        }

        /// <summary>
        /// A `TransactionArgument` representing the gas of the transaction.
        /// </summary> ✅
        public ITransactionArgument gas = new GasCoin();

        /// <summary> ✅
        /// Set the gas payment for the programmable transaction block.
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        public TransactionBlock SetGasPayment(Sui.Types.SuiObjectRef[] payments)
        {
            if (payments.Count() >= (int)TransactionConstants.maxGasObjects)
                throw new Exception("Gas Payment is too high.");

            this.BlockDataBuilder.Builder.GasConfig.Payment = payments;
            return this;
        }
    
        /// <summary>
        /// Creates and appends a `TransactionBlockInput` object to the `blockData.builder.inputs`
        /// array and returns it. The type and value of the `TransactionBlockInput` are specified
        /// by the function parameters.
        /// </summary> ✅
        /// <param name="type">A `Sui.Types.Type` representing the type of the input.</param>
        /// <param name="value">An `ISerializable` representing the value of the input.</param>
        /// <returns>A `TransactionBlockInput` object.</returns>
        public TransactionBlockInput AddInput
        (
            Sui.Types.Type type,
            ISerializable value
        )
        {
            int index = this.BlockDataBuilder.Builder.Inputs.Count();
            var input = new TransactionBlockInput(index, value, type);
            this.BlockDataBuilder.Builder.Inputs.Add(input);
            return input;
        }

        ///  ✅
        public ITransactionArgument AddObjectInput(ITransactionObjectInput value)
        {
            if (value.Type == TransactionObjectInputType.transactionObjectArgument)
                return ((TransactionArgumentTransactionObjectInput)value).Input;

            string id = InputsHandler.GetIDFromCallArg(value);
            TransactionBlockInput[] inserted_arr = BlockDataBuilder.Builder.Inputs.Where((input) => {
                if (input.Value == null || input.Value.GetType() != typeof(string))
                    return false;
                return id == NormalizedTypeConverter.NormalizeSuiAddress(((BString)input.Value).value);
            }).ToArray();

            if (inserted_arr.Count() != 0)
                return inserted_arr[0];

            switch(value.Type)
            {
                case TransactionObjectInputType.stringArgument:
                    string string_argument = ((StringTransactionObjectInput)value).Input;
                    return AddInput(Sui.Types.Type.Object, new BString(string_argument));
                case TransactionObjectInputType.objectCallArgument:
                    ObjectArg object_call_argument = ((CallArgTransactionObjectInput)value).Input;
                    return AddInput(Sui.Types.Type.Object, object_call_argument);
                case TransactionObjectInputType.transactionObjectArgument:
                    ITransactionArgument transaction_argument = ((TransactionArgumentTransactionObjectInput)value).Input;
                    return AddInput(Sui.Types.Type.Object, transaction_argument);
            }

            throw new Exception("Not Implemented");
        }

        /// ✅
        public ITransactionArgument AddObjectInput(string id)
        {
            return AddObjectInput(new StringTransactionObjectInput(id));
        }

        /// ✅
        public ITransactionArgument AddObjectInput(IObjectArgument object_argument)
        {
            switch(object_argument.Type)
            {
                case ObjectArgumentType.stringArgument:
                    string argument_string = ((StringObjectArgument)object_argument).Argument;
                    return AddObjectInput(new StringTransactionObjectInput(argument_string));
                case ObjectArgumentType.transactionObjectArgument:
                    ITransactionArgument argument_transaction = ((TransactionObjectArgument)object_argument).Argument;
                    return AddObjectInput(new TransactionArgumentTransactionObjectInput(argument_transaction));
            }

            throw new Exception("Not Implemented");
        }

        /// ✅
        public ITransactionArgument AddObjectRef(ObjectArg object_arg)
        {
            return AddObjectInput(new CallArgTransactionObjectInput(object_arg));
        }

        /// ✅
        public ITransactionArgument AddSharedObjectRef(SharedObjectRef shared_object_ref)
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
        public TransactionBlockInput AddPure(ISerializable value)
        {
            return AddInput(Sui.Types.Type.Pure, value);
        }

        /// <summary>
        /// Add a new non-object input to the transaction.
        /// </summary>
        /// <param name="value">
        ///     A byte array representing the raw data of the
        ///     pure value.
        /// </param> ✅
        /// <returns></returns>
        public TransactionBlockInput AddPure(byte[] value)
        {
            return AddInput(Sui.Types.Type.Pure, new Bytes(value));
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
        /// <returns>An `ITransactionArgument` object representing the result of the addition.</returns>
        public List<ITransactionArgument> AddTransaction(Types.SuiTransaction transaction, int? return_value_count = null)
        {
            BlockDataBuilder.Builder.Transactions.Add(transaction);

            int index = BlockDataBuilder.Builder.Transactions.Count();

            TransactionResult transaction_result = new TransactionResult((ushort)(index - 1), (ushort?)return_value_count);

            List<ITransactionArgument> results = new List<ITransactionArgument>();

            if (return_value_count == null)
                results.Add(transaction_result.TransactionArgument);
            else
            {
                foreach(ITransactionArgument nested_result in transaction_result)
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
        public List<ITransactionArgument> AddSplitCoinsTx(ITransactionArgument coin, params TransactionBlockInput[] amounts)
        {
            SplitCoins splitCoinsTx = new SplitCoins(coin, amounts);
            return this.AddTransaction(new Types.SuiTransaction(splitCoinsTx));
        }

        /// <summary>
        /// Merges multiple source coins into a single destination coin.
        /// </summary> ✅
        /// <param name="destination">An `ITransactionArgument` representing the destination coin.</param>
        /// <param name="sources">An array of `ITransactionArgument` representing the source coins.</param>
        /// <returns>An `ITransactionArgument` array representing the result of the merge coin operation.</returns>
        public List<ITransactionArgument> AddMergeCoinsTx(ITransactionArgument destination, ITransactionArgument[] sources)
        {
            MergeCoins merge_coins_tx = new MergeCoins(destination, sources);
            return this.AddTransaction(new Types.SuiTransaction(merge_coins_tx));
        }

        /// <summary>
        /// Publishes modules with given dependencies.
        /// </summary> ✅
        /// <param name="modules">An array of `byte[]` representing the modules to be published.</param>
        /// <param name="dependencies">An array of `AccountAddress` representing the dependencies.</param>
        /// <returns>An `ITransactionArgument` array representing the result of the publish operation.</returns>
        public List<ITransactionArgument> AddPublishTx(byte[][] modules, AccountAddress[] dependencies)
        {
            Publish publish_tx = new Publish(modules, dependencies);
            return this.AddTransaction(new Types.SuiTransaction(publish_tx));
        }

        /// <summary>
        /// Upgrades modules with given dependencies, packageId, and ticket.
        /// </summary> ✅
        /// <param name="modules">An array of `byte[]` representing the modules to be upgraded.</param>
        /// <param name="dependencies">An array of `AccountAddress` representing the dependencies.</param>
        /// <param name="packageId">A `string` representing the package ID.</param>
        /// <param name="ticket">An `ITransactionArgument` representing the ticket.</param>
        /// <returns>An `ITransactionArgument` representing the result of the upgrade operation.</returns>
        public List<ITransactionArgument> AddUpgradeTx
        (
            byte[][] modules,
            AccountAddress[] dependencies,
            string packageId,
            ITransactionArgument ticket
        )
        {
            Upgrade upgrade_tx = new Upgrade(modules, dependencies, packageId, ticket);
            return this.AddTransaction(new Types.SuiTransaction(upgrade_tx));
        }

        /// <summary>
        /// Makes a move call with target, optional arguments, and optional type arguments.
        /// </summary> ✅
        /// <param name="target">A `SuiMoveNormalizedStructType` representing the target of the move call.</param>
        /// <param name="typeArguments">An optional array of `ISerializableTag` representing the arguments of the move call.</param>
        /// <param name="arguments">An optional array of `SuiTransactionArgument` representing the type arguments of the move call.</param>
        /// <param name="return_value_count">The number of return values, greater than 1, that are returned by the move call.</param>
        /// <returns>An array of `ITransactionArgument` representing the result of the move call.</returns>
        public List<ITransactionArgument> AddMoveCallTx
        (
            SuiMoveNormalizedStructType target,
            ISerializableTag[] typeArguments = null,
            SuiTransactionArgument[] arguments = null,
            int? return_value_count = null
        )
        {
            MoveCall moveCallTx = new MoveCall(target, typeArguments, arguments);
            return AddTransaction(new Types.SuiTransaction(moveCallTx), return_value_count);
        }

        /// <summary>
        /// Transfers objects to a specified address.
        /// </summary> ✅
        /// <param name="objects">An array of `ITransactionArgument` representing the objects to be transferred.</param>
        /// <param name="address">An `ITransactionArgument` representing the address to transfer objects to.</param>
        /// <returns>An array of `ITransactionArgument` representing the result of the transfer object operation.</returns>
        public List<ITransactionArgument> AddTransferObjectsTx(ITransactionArgument[] objects, ITransactionArgument address)
        {
            TransferObjects transfer_tx = new TransferObjects(objects, address);
            return this.AddTransaction(new Types.SuiTransaction(transfer_tx));
        }

        /// <summary>
        /// Makes a Move Vector with the specified type and objects.
        /// </summary> ✅
        /// <param name="objects">An array of `ITransactionArgument` representing the objects of the Move Vector.</param>
        /// <param name="type">An optional `SuiStructTag` representing the type of the Move Vector.</param>
        /// <returns>An array of `ITransactionArgument` representing the result of the make Move Vector operation.</returns>
        public List<ITransactionArgument> AddMakeMoveVecTx(ITransactionArgument[] objects, SuiStructTag type = null)
        {
            MakeMoveVec make_move_vec_tx = new MakeMoveVec(objects, type);
            return this.AddTransaction(new Types.SuiTransaction(make_move_vec_tx));
        }

        /// <summary>
        /// Retrieves a configuration value for a specified key.
        /// </summary> ✅
        /// <param name="key">A `LimitKey` representing the key for which the configuration value needs to be retrieved.</param>
        /// <param name="build_options">A `BuildOptions` object containing the build options including limits and protocolConfig.</param>
        /// <returns>An `int` representing the configuration value for the specified key.</returns>
        private int GetConfig(LimitKey key, BuildOptions build_options)
        {
            if (build_options.Limits[BuildOptions.TransactionLimits[key]].HasValue)
                return (int)build_options.Limits[BuildOptions.TransactionLimits[key]];

            if (build_options.ProtocolConfig == null)
                return (int)DefaultOfflineLimits[key];

            return build_options.ProtocolConfig.Attributes[BuildOptions.TransactionLimits[key]] switch
            {
                ProtocolConfigValue.F64 f64 => (int)f64.Value,
                ProtocolConfigValue.U32 u32 => (int)u32.Value,
                ProtocolConfigValue.U64 u64 => (int)u64.Value,
                _ => throw new Exception("Cannot Find Attribute"),
            };
        }

        /// <summary>
        /// Builds a block with the specified provider and optional transaction kind.
        /// </summary> ✅
        /// <param name="build_options">An instance of `BuildOptions` that contains the options passed for preparing the transaction block.</param>
        /// <returns>A `byte[]` object representing the built block.</returns>
        public async Task<byte[]> Build(BuildOptions build_options)
        {
            await Prepare(build_options);
            return BlockDataBuilder.Build(null, build_options.OnlyTransactionKind);
        }

        /// <summary>
        /// Computes the digest of the block with the specified provider.
        /// </summary> ✅
        /// <param name="build_options">An instance of `BuildOptions` that contains the options passed for preparing the transaction block.</param>
        /// <returns>A `string` representing the digest of the block.</returns>
        public async Task<string> GetDigest(BuildOptions build_options)
        {
            await Prepare(build_options);
            return BlockDataBuilder.GetDigest();
        }

        /// <summary>
        /// Determines whether the sender is missing considering the specified transaction kind.
        /// </summary> ✅
        /// <param name="only_transaction_kind">An optional `bool` representing whether only transaction kind should be considered.</param>
        /// <returns>A `bool` indicating whether the sender is missing.</returns>
        private bool IsMissingSender(bool? only_transaction_kind = null)
        {
            return only_transaction_kind.HasValue && only_transaction_kind == false && BlockDataBuilder.Builder.Sender == null;
        }

        /// <summary>
        /// Prepares gas payment for transactions.
        /// </summary> ✅
        /// <param name="options">A `BuildOptions` object that contains the `Provider` and `OnlyTransactionKind` members.</param>
        /// <returns>A `Task` object used for implementation with Coroutines.</returns>
        public async Task PrepareGasPaymentAsync(BuildOptions options)
        {
            if (IsMissingSender(options.OnlyTransactionKind))
                throw new Exception("Sender Is Missing");

            if ((options.OnlyTransactionKind.HasValue && options.OnlyTransactionKind == true) || this.BlockDataBuilder.Builder.GasConfig.Price != null) {
                return;
            }

            if (BlockDataBuilder.Builder.GasConfig.Owner == null && BlockDataBuilder.Builder.Sender == null)
                throw new Exception("Gas Owner Cannot Be Found");

            string gas_owner =
                BlockDataBuilder.Builder.GasConfig.Owner != null ?
                BlockDataBuilder.Builder.GasConfig.Owner.ToHex() :
                BlockDataBuilder.Builder.Sender.ToHex();

            RpcResult<CoinPage> coins = await options.Provider.GetCoins(gas_owner, "0x2::sui::SUI", 10);

            IEnumerable<CoinDetails> filtered_coins = coins.Result.Data.Where((coin) => {
                return BlockDataBuilder.Builder.Inputs.Any((input) => {
                    if (input.Value.GetType() == typeof(ICallArg))
                    {
                        ICallArg call_arg = (ICallArg)input.Value;
                        if (call_arg.Type == Sui.Types.Type.Object)
                        {
                            ObjectArg object_arg = ((ObjectCallArg)call_arg).ObjectArg;
                            if (object_arg.Type == ObjectRefType.ImmOrOwned)
                            {
                                return
                                    coin.CoinObjectId ==
                                    ((Sui.Types.SuiObjectRef)object_arg.ObjectRef).ObjectId.ToHex();
                            }
                        }
                    }
                    return false;
                });
            });

            int coin_range_max = Math.Min(filtered_coins.Count(), (int)TransactionConstants.maxGasObjects);
            var range = Enumerable.Range(0, coin_range_max);

            IEnumerable<Sui.Types.SuiObjectRef> payment_coins = range.Select((idx) => {
                CoinDetails coin = filtered_coins.ElementAt(idx);
                return new Sui.Types.SuiObjectRef(
                    AccountAddress.FromHex(coin.CoinObjectId),
                    int.Parse(coin.Version),
                    coin.Digest
                );
            });

            if (payment_coins.Count() == 0)
                throw new Exception("Owner Does Not Have Payment Coins");

            SetGasPayment(payment_coins.ToArray());
        }

        /// <summary>
        /// Prepares gas price for transactions.
        /// </summary> ✅
        /// <param name="options">A `BuildOptions` object that contains the `Provider` and `OnlyTransactionKind` members.</param>
        /// <returns>A `Task` object used for implementation with Coroutines.</returns>
        public async Task PrepareGasPriceAsync(BuildOptions options)
        {
            if (IsMissingSender(options.OnlyTransactionKind))
                throw new Exception("Sender Is Missing");

            RpcResult<BigInteger> gas_price = await options.Provider.GetReferenceGasPriceAsync();

            SetGasPrice(gas_price.Result);
        }

        // TODO: Check implementation to verify that it works as intended.
        /// <summary>
        /// Resolves all required Move modules and objects.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task PrepareTransactions(BuildOptions options)
        {
            // The inputs in the `TransactionBlock`
            List<TransactionBlockInput> inputs              = this.BlockDataBuilder.Builder.Inputs;
            Types.SuiTransaction[] transactions             = this.BlockDataBuilder.Builder.Transactions.ToArray();

            // A list of move modules identified as needing to be resolved
            List<MoveCall> moveModulesToResolve     = new List<MoveCall>();
            List<ObjectToResolve> objectsToResolve  = new List<ObjectToResolve>();

            foreach (TransactionBlockInput input in inputs)
            {
                // The value is an ObjectID (AccountAddress) add it to the objects to resolve
                if (input.Value.GetType() == typeof(AccountAddress))
                {
                    ObjectToResolve objectToResolve = new ObjectToResolve(
                        (AccountAddress)input.Value,
                        input,
                        null
                    );
                    objectsToResolve.Add(objectToResolve);
                }
            }

            #region Process all transactions
            foreach (ITransaction transaction in transactions)
            {
                #region Process MoveCall Transaction
                // Special case move call:
                if (transaction.Kind == Kind.MoveCall)
                {
                    // Determine if any of the arguments require encoding.
                    // - If they don't, then this is good to go.
                    // - If they do, then we need to fetch the normalized move module.
                    MoveCall moveTx = (MoveCall)transaction;
                    SuiTransactionArgument[] arguments = moveTx.Arguments;

                    bool needsResolution = arguments.Any(arg => {
                        if (arg.TransactionArgument.Kind == Types.Arguments.Kind.Input)
                        {
                            TransactionBlockInput argInput = (TransactionBlockInput)arg.TransactionArgument;
                            int index = argInput.Index;

                            // Is it a PureCallArg or ObjectCallArg?
                            // If the argument is a `TransactionBlockInput`
                            // and the value of the input at `index` is NOT a BuilderArg (`ICallArg`)
                            // then we need to resolve it.
                            bool isBuilderCallArg = inputs[index].Value.GetType() != typeof(ICallArg);
                            return isBuilderCallArg;
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
                                if (argument_outer.value.TransactionArgument.Kind == Types.Arguments.Kind.Input)
                                {
                                    foreach (var argument_inner in move_call.Arguments.Select((value, i) => new { i, value }))
                                    {
                                        if
                                        (
                                            argument_inner.value.TransactionArgument.Kind == Types.Arguments.Kind.Input
                                        )
                                        {
                                            TransactionBlockInput outer_input =
                                                (TransactionBlockInput)argument_outer.value.TransactionArgument;
                                            TransactionBlockInput inner_input =
                                                (TransactionBlockInput)argument_inner.value.TransactionArgument;

                                            if
                                            (
                                                outer_input.Value == inner_input.Value &&
                                                outer_input.Index != inner_input.Index
                                            )
                                            {
                                                moveTx.Arguments[argument_outer.i] = move_call.Arguments[argument_inner.i];
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
                    TransferObjects transferObjectsTx = (TransferObjects)transaction;
                    ITransactionArgument address = transferObjectsTx.Address;

                    if (address.GetType() == typeof(TransactionBlockInput))
                    {
                        // Cast the address as a `TransactionBlockInput` to get index property
                        TransactionBlockInput addressInput = (TransactionBlockInput)address;
                        // Get the TXBInput object at the index provided by the addressInput argument
                        TransactionBlockInput input = inputs[addressInput.Index];

                        // If the value of the input is not an object type then it must be a Pure
                        if (input.Value.GetType() != typeof(IObjectRef))
                        {
                            // TODO: IRVIN update this to use a clone of the input list
                            this.BlockDataBuilder.Builder.Inputs[addressInput.Index].Value = new PureCallArg(input.Value);
                        }
                    }
                }
                #endregion END - Process TransferObjects Transaction

                #region Process SplitCoins Transaction
                // Special handling for values that where previously encoded using the wellKnownEncoding pattern.
                // This should only happen when transaction block data was hydrated from an old version of the SDK
                else if (transaction.Kind == Kind.SplitCoins)
                {
                    SplitCoins splitCoinsTx = (SplitCoins)transaction;
                    ITransactionArgument[] amounts = splitCoinsTx.Amounts;
                    foreach(ITransactionArgument amount in amounts)
                    {
                        if(amount.GetType() == typeof(TransactionBlockInput))
                        {   // Cast the amount as a `TransactionBlockInput` to get index property
                            TransactionBlockInput amountTxbInput = (TransactionBlockInput)amount;
                            // Get the TXBInput object at the index provided by the amount argument
                            TransactionBlockInput input = inputs[amountTxbInput.Index];

                            // If the value of the input is not an object type then it must be a Pure
                            if(input.Value.GetType() != typeof(IObjectRef))
                            {
                                // TODO: IRVIN update this to use a clone of the input list
                                this.BlockDataBuilder.Builder.Inputs[amountTxbInput.Index].Value
                                    = new PureCallArg(input.Value); ;
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
                    string packageId = moveCall.Target.StructTag.address.ToHex();
                    string moduleName = moveCall.Target.StructTag.module;
                    string functionName = moveCall.Target.StructTag.name;

                    #region RPC Call GetNormalizedMoveFunction
                    RpcResult<NormalizedMoveFunctionResponse> result
                        = await options.Provider.GetNormalizedMoveFunction(
                            packageId,
                            moduleName,
                            functionName
                    );
                    NormalizedMoveFunctionResponse normalized = result.Result;
                    #endregion END - RPC Call

                    // Entry functions can have a mutable reference to an instance of the TxContext
                    // struct defined in the TxContext module as the last parameter. The caller of
                    // the function does not need to pass it in as an argument.
                    bool hasTxContext
                        = normalized.Parameters.Count > 0
                        && normalized.Parameters.Last() as SuiMoveNormalizedTypeString != null
                        && IsTxContext(new SuiStructTag(((SuiMoveNormalizedTypeString)normalized.Parameters.Last()).Value));

                    // The list of parameters returned by the RPC call
                    List<ISuiMoveNormalizedType> paramsList
                        = (List<ISuiMoveNormalizedType>)(hasTxContext
                        ? normalized.Parameters.Take(normalized.Parameters.Count - 1)
                        : normalized.Parameters);

                    if(paramsList.Count != moveCall.Arguments.Length)
                    {
                        // TODO: Irvin fix this -- we cannot throw an exception
                        // MARCUS: Maybe we can look into error enums for handling
                        // different exception issues.
                        throw new ArgumentException("Incorrect number of arguments.");
                    }

                    foreach(var param_enumerated in paramsList.Select((param, i) => new { i, param }))
                    {
                        SuiTransactionArgument arg = moveCall.Arguments[param_enumerated.i];

                        if (arg.TransactionArgument.Kind != Types.Arguments.Kind.Input)
                            continue;

                        TransactionBlockInput inputArg = (TransactionBlockInput)arg.TransactionArgument;
                        TransactionBlockInput input = inputs[inputArg.Index];
                        // Skip if the input is already resolved, aka if the input is a BuilderArg
                        if (input.Value.GetType() == typeof(ICallArg)) continue;

                        // When we reach here, this means that the value could be a BString, a U8, etc.
                        // We need to compare agains the RPC response params to know how to cast to a concrete type
                        // Once we know how to cast, then we will be able to serialize it later on
                        ISerializable inputValue = input.Value;

                        //Type t = Type.GetType(param); // for reference
                        //Convert.ChangeType(value1, intType); // for reference

                        //ICallArg inputValue = input.Value;
                        //// Check if param received from RPC is Pure serializable
                        //Serialization ser = new Serialization();
                        //input.Value.Serialize(ser);
                        ////input.Value = new Bytes(ser.GetBytes());
                        ///

                        string serType = Serializer.GetPureNormalizedType(param_enumerated.param, inputValue);
                        if (serType != null)
                        {
                            // TODO: IRVIN update this to use a clone of the input list
                            inputs[inputArg.Index].Value = new PureCallArg(inputValue);
                            continue;
                        }

                        //Type serType = Serializer.GetPureNormalizedTypeType(param, inputValue);

                        // TODO: NOTE IRVIN -- All this `GetPureNormalizedTypeType` function does is "verify"
                        // TODO:    that the input value matches the type that the MoveCall expects.
                        // TODO: NOTE: HENCE we don't really need to return anything, all we have to do is just check that the type of the input value
                        // TODO:    matches what is expected, if doesn't match then we return false, and break / end the program.
                        // TODO: NOTE: We don't need a "serType" because we are already passing concrete types such as:
                        // TODO:    `AccountAddress` or `U8` or `Bytes` for byte arrays, of Sequence for vectors

                        // TODO: NOW NOTE THAT -- for structs it's trickier because the MoveCall is expecting an object, and in the C# side
                        // TODO:    We can only work with class / objects, hence we just have to do a comparison of the properties of the expected object


                        //bool iSPureNormalizedType = Serializer.MatchesPureNormalizedType(param, inputValue);
                        // if(iSPureNormalizedType) { 
                        //this.BlockDataBuilder.Inputs[inputArg.Index].Value = new PureCallArg(inputValue);
                        // }

                        ISuiMoveNormalizedType structVal = Serializer.ExtractStructType(param_enumerated.param);

                        if (structVal != null || param_enumerated.param as SuiMoveNormalziedTypeParameterType != null)
                        {
                            if (inputValue.GetType() != typeof(AccountAddress))
                                throw new Exception($"Expect the argument to be an object id string, got {inputValue.GetType()}");

                            ObjectToResolve objectToResolve = new ObjectToResolve(
                                (AccountAddress)inputValue,
                                input,
                                param_enumerated.param
                            );
                            objectsToResolve.Add(objectToResolve);
                            continue;
                        }

                        throw new Exception($"Unknown call arg type {param_enumerated.param} for value {inputValue.GetType()}");
                    }
                }
            }
            #endregion END - Resolve MoveModules
            #region Resolve objects
            Debug.Log($"MARCUS: OBJECTS TO RESOLVE COUNT - {objectsToResolve.Count}");
            throw new Exception($"Not Implemented");
            if (objectsToResolve.Count != 0)
            {
                List<AccountAddress> mappedIds = (List<AccountAddress>)objectsToResolve.Select(x => x.Id);
                // NOTE: Insertion order in HashSet will be maintained until removing or re-adding
                List<AccountAddress> dedupedIds = new HashSet<AccountAddress>(mappedIds).ToList();

                // TODO: In the TypeScript SDK they use `Set` which is a set that maintains insertion order
                // TODO: Find data structure that does this in C#
                // https://gist.github.com/gmamaladze/3d60c127025c991a087e

                // Chunk list of IDs into smaller lists to use in RPC Call `MultiGetObjects`
                List<List<AccountAddress>> objectChunks = Chunk(dedupedIds, 50);

                List<List<ObjectDataResponse>> objectsResponse = new List<List<ObjectDataResponse>>();
                foreach(List<AccountAddress> objectIds in objectChunks)
                {
                    ObjectDataOptions optionsObj = new ObjectDataOptions();
                    optionsObj.ShowOwner = true;

                    #region RPC Call MultiGetObjects
                    RpcResult<IEnumerable<ObjectDataResponse>> response
                        = await options.Provider.MultiGetObjects(
                            objectIds.ToArray(),
                            optionsObj
                    );
                    #endregion END - Call MultiGetObjects

                    List<ObjectDataResponse> objects
                        = (List<ObjectDataResponse>)response.Result;
                    objectsResponse.Add(objects);
                }
                // Flatten responses from MultiGetObjects
                List<ObjectDataResponse> objectsFlatten
                    = objectsResponse.SelectMany(x => x).ToList();

                // Create a map of IDs to ObjectDataResponse
                Dictionary<AccountAddress, ObjectDataResponse> objectsById
                    = new Dictionary<AccountAddress, ObjectDataResponse>();

                // Populate map(Dictionary) `objectsById`
                for(int i = 0; i < dedupedIds.Count; i++)
                {
                    AccountAddress id = dedupedIds[i];
                    ObjectDataResponse obj = objectsFlatten[i];
                    objectsById.Add(id, obj);
                }

                // Filter objects that returned an error
                List<ObjectDataResponse> invalidObjects
                    = (List<ObjectDataResponse>)objectsById.Values.ToList().Where(
                        obj => obj.Error != null
                    );

                if (invalidObjects.Count > 0)
                    throw new Exception("The following input objects are invalid: {}");


                foreach (ObjectToResolve objectToResolve in objectsToResolve)
                {
                    ObjectDataResponse obj = objectsById[objectToResolve.Id];
                    //AccountAddress owner = obj.Data.Owner; // could be an object
                    Owner owner = obj.Data.Owner;

                    int? initialSharedVersion = owner.Shared.InitialSharedVersion;

                    if (initialSharedVersion != null)
                    {
                        bool mutable = InputsHandler.isMutableSharedObjectInput(
                            (ICallArg)objectToResolve.Input.Value
                        );

                        inputs[objectToResolve.Input.Index].Value
                            = new SharedObjectRef(
                                objectToResolve.Id,
                                (int)initialSharedVersion,
                                mutable
                        );
                    }
                    else if (objectToResolve.NormalizedType != null)
                    {
                        // TODO: Implement Receiving Type casting
                    }
                    else
                    {
                        ObjectData data = obj.Data;
                        if (data != null)
                        {
                            inputs[objectToResolve.Input.Index].Value
                                = new Sui.Types.SuiObjectRef(
                                    AccountAddress.FromHex(data.ObjectId),
                                    (int)data.Version,
                                    data.Digest
                            );
                        }
                    }
                }
            }
            #endregion END - Resolve objects
        }

        /// <summary>
        /// Chuck the list of duduped ids so that it can be used in the RPC call for multi objects.
        /// </summary> ✅
        /// <param name="dedupedIds"></param>
        /// <param name="size"></param>
        /// <returns></returns>
        private List<List<AccountAddress>> Chunk(List<AccountAddress> dedupedIds, int size)
        {
            int length = (int)Math.Ceiling((double)(dedupedIds.Count / size));
            List<List<AccountAddress>> ret = new List<List<AccountAddress>>();
            int i = 0;
            while(i < length)
            {
                // TODO: Check if this might break because of index out of bounds
                List<AccountAddress> chunk = dedupedIds.GetRange(i * size, i * size + size);
                ret.Add(chunk);
                i++;
            }
            return ret;
        }

        /// <summary>
        /// Encodes a `TransactionBlockInput` input.
        /// Meaning that if the `value` inside the `TransactionBlockInput`
        /// is simply an objectID (`AccountAddress`) then we need to resolve it.
        /// Otherwise, if it's a "pure", then we create a `TransactionBlockInput`
        /// with a "pure" value.
        /// </summary>
        /// <param name="index"></param>
        //private void EncodeInput(int index)
        //{
        //    TransactionBlockInput input = BlockDataBuilder.Inputs[index];

        //    Type type = input.Value.GetType();

        //    // If the value of the `TransactionBlockInput` input is already a `BuilderCallArg`
        //    if (type == typeof(ICallArg)) return;

        //    if(type == typeof(AccountAddress))
        //    {
        //        // TODO: Figure out porting logic from TypeScript where they pass `input` by reference
        //        // <code> input.value = Inputs.Pure(input.value, wellKnownEncoding.type); </code>
        //    } else if(type == typeof(Bytes))// else if (wellKnownEncoding.kind === 'pure') 
        //    {
        //        //input.Value = input.Value.Serialize
        //    }
        //    else
        //    {

        //    }
        //}

        public class ObjectToResolve
        {
            public AccountAddress Id { get; set; }
            public TransactionBlockInput Input { get; set; }
            public ISuiMoveNormalizedType NormalizedType;

            public ObjectToResolve(AccountAddress id, TransactionBlockInput input, ISuiMoveNormalizedType normalizedType) 
            {
                this.Id = id;
                this.Input = input;
                this.NormalizedType = normalizedType;
            }
        }

        private string GetPureSeralizationType(string normalizedType, ISerializable argVal)
        {
            bool isPure = Enum.IsDefined(typeof(AllowedTypes), normalizedType);

            if (isPure)
            {
                //string[] uTypes = new string[] { "U8", "U16", "U32", "U64", "U128", "U256" };
                //if (uTypes.Contains(normalizedType))
                //{
                //    Type argValType = argVal.GetType();
                //}
                //else if (normalizedType.Equals("Bool"))
                //{
                //    //bool booleanValue;
                //    //if (bool.TryParse(pureArgVal.Value, out booleanValue))
                //    //{
                //    //    Console.WriteLine($"Conversion successful: '{value}' to {booleanValue}.\n");
                //    //}
                //    //else
                //    //{
                //    //    Console.WriteLine($"Conversion Failed: '{value}' to {booleanValue}.\n");
                //    //}
                //}
                //else if (normalizedType.Equals("Address"))
                //{

                //}
                return normalizedType.ToLower();

            }
            else if(normalizedType.Equals("string"))
            {
                throw new Exception("Unknown pure normalized type: " + normalizedType);
            }

            if (normalizedType.Equals("Vector"))
            {

            }

            throw new NotImplementedException();
        }

        private bool IsTxContext(SuiStructTag @struct)
        {
            return @struct.address.ToHex().Equals("0x2") && @struct.module.Equals("tx_context") && @struct.name.Equals("TxContext");
        }

        /// <summary>
        /// Prepares the transaction block with the provided build options.
        /// </summary>
        /// <param name="options_passed">An instance of `BuildOptions` that contains the options passed for preparing the transaction block.</param>
        /// <returns>A `Task` object used for implementations with async calls.</returns>
        private async Task Prepare(BuildOptions options_passed)
        {
            if (IsPrepared)
                return;

            BuildOptions options = options_passed;

            if (options.ProtocolConfig == null)
            {
                RpcResult<Rpc.Models.ProtocolConfig> protocol_config = await options.Provider.GetProtocolConfigAsync();
                options.ProtocolConfig = new ProtocolConfig(protocol_config.Result);
            }

            await PrepareGasPriceAsync(options);
            await PrepareTransactions(options);

            if (options.OnlyTransactionKind == null || (options.OnlyTransactionKind.HasValue && options.OnlyTransactionKind == false))
            {
                GasConfig gas_config = BlockDataBuilder.Builder.GasConfig;

                gas_config.Budget = new BigInteger(GetConfig(LimitKey.MaxTxGas, options));
                gas_config.Payment = new Sui.Types.SuiObjectRef[] { };

                TransactionBlockDataBuilderSerializer tx_block_data_builder = new TransactionBlockDataBuilderSerializer(
                    new TransactionBlockDataBuilder()
                );
                tx_block_data_builder.Builder.GasConfig = gas_config;

                RpcResult<TransactionBlockResponse> dry_run_result = await options.Provider.DryRunTransactionBlock(
                    Convert.ToBase64String(tx_block_data_builder.Build())
                );

                if (dry_run_result.Result.Effects.Status.Status == ExecutionStatus.Failure)
                    throw new Exception("Dry Run Failed");

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

                var gas_budget =
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

            IsPrepared = true;
        }
    }
}