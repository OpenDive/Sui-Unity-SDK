//
//  TransactionBlock.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Threading.Tasks;
using Chaos.NaCl;
using System.Text.RegularExpressions;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc;
using Sui.Rpc.Models;
using Sui.Transactions.Builder;
using Sui.Types;
using Sui.Rpc.Client;
using Sui.Utilities;
using Sui.Cryptography;

namespace Sui.Transactions
{
    /// <summary>
    /// A class that represents the inputted transaction commands and other configuration values
    /// used for a transactio on the Sui network.
    /// </summary>
    public class TransactionBlock : ReturnBase
    {
        /// <summary>
        /// A dictionary containing default offline limits with string keys and integer values.
        /// </summary>
        public Dictionary<LimitKey, long> DefaultOfflineLimits = new Dictionary<LimitKey, long>
        {
            { LimitKey.MaxPureArgumentSize, 16 * 1024 },
            { LimitKey.MaxTxGas, 50_000_000_000 },
            { LimitKey.MaxGasObjects, 256 },
            { LimitKey.MaxTxSizeBytes, 128 * 1024 }
        };

        /// <summary>
        /// The transaction block builder that contains the configuration values and transactions.
        /// </summary>
        public TransactionBlockDataBuilderSerializer BlockDataBuilder { get; set; }

        /// <summary>
        /// A `TransactionArgument` representing the gas of the transaction.
        /// </summary>
        public TransactionArgument gas
        {
            get => new TransactionArgument(TransactionArgumentKind.GasCoin, null);
        }

        /// <summary>
        /// A boolean value indicating whether the block is prepared to be executed or not.
        /// </summary>
        private bool IsPrepared { get; set; }

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

        /// <summary>
        /// Set the sender for the programmable transaction block.
        /// </summary>
        /// <param name="sender">The `Account` that represents the sender.</param>
        public void SetSender(Account sender)
            => this.BlockDataBuilder.Builder.Sender = sender.SuiAddress();

        /// <summary>
        /// The the Sender, if it has not been set in the programmable transaction.
        /// </summary>
        /// <param name="sender">The `Account` that represents the sender.</param>
        public void SetSenderIfNotSet(Account sender)
        {
            if (this.BlockDataBuilder.Builder.Sender == null)
                this.SetSender(sender);
        }

        /// <summary>
        /// Set the sender for the programmable transaction block.
        /// </summary>
        /// <param name="sender">The `SuiPublicKeyBase` that represents the sender.</param>
        public void SetSender(SuiPublicKeyBase sender)
            => this.BlockDataBuilder.Builder.Sender = sender.ToSuiAddress();

        /// <summary>
        /// The the Sender, if it has not been set in the programmable transaction.
        /// </summary>
        /// <param name="sender">The `SuiPublicKeyBase` that represents the sender.</param>
        public void SetSenderIfNotSet(SuiPublicKeyBase sender)
        {
            if (this.BlockDataBuilder.Builder.Sender == null)
                this.SetSender(sender);
        }

        /// <summary>
        /// Set the sender for the programmable transaction block.
        /// </summary>
        /// <param name="sender">The `AccountAddress` that represents the sender.</param>
        public void SetSender(AccountAddress sender)
            => this.BlockDataBuilder.Builder.Sender = sender;

        /// <summary>
        /// The the Sender, if it has not been set in the programmable transaction.
        /// </summary>
        /// <param name="sender">The `AccountAddress` that represents the sender.</param>
        public void SetSenderIfNotSet(AccountAddress sender)
        {
            if (this.BlockDataBuilder.Builder.Sender == null)
                this.SetSender(sender);
        }

        /// <summary>
        /// Set expiration for the programmable transaction.
        /// </summary>
        /// <param name="expiration">The expiration object.</param>
        public void SetExpiration(TransactionExpiration expiration)
            => this.BlockDataBuilder.Builder.Expiration = expiration;

        /// <summary>
        /// Sets the gas price.
        /// </summary>
        /// <param name="price">The gas price as a `BigInteger`.</param>
        public void SetGasPrice(BigInteger price)
            => this.BlockDataBuilder.Builder.GasConfig.Price = price;

        /// <summary>
        /// Set the gas budget for the programmable transaction block.
        /// </summary>
        /// <param name="budget">The gas budget as a `BigInteger`.</param>
        public void SetGasBudget(BigInteger budget)
            => this.BlockDataBuilder.Builder.GasConfig.Budget = budget;

        /// <summary>
        /// Set the gas owner for the programmable transaction block.
        /// </summary>
        /// <param name="owner">The gas owner as an `AccountAddress`.</param>
        public void SetGasOwner(AccountAddress owner)
            => this.BlockDataBuilder.Builder.GasConfig.Owner = owner;

        /// <summary>
        /// Set the gas owner for the programmable transaction block.
        /// </summary>
        /// <param name="owner">The gas owner as a `SuiPublicKeyBase`.</param>
        public void SetGasOwner(SuiPublicKeyBase owner)
            => this.BlockDataBuilder.Builder.GasConfig.Owner = owner.ToSuiAddress();

        /// <summary>
        /// Set the gas owner for the programmable transaction block.
        /// </summary>
        /// <param name="owner">The gas owner as an `Account`.</param>
        public void SetGasOwner(Account owner)
            => this.BlockDataBuilder.Builder.GasConfig.Owner = owner.SuiAddress();

        /// <summary>
        /// Set the gas payment for the programmable transaction block.
        /// </summary>
        /// <param name="payments">The array of object references for the gas payment (usually are gas coins).</param>
        public void SetGasPayment(SuiObjectRef[] payments)
        {
            if (payments.Count() >= (int)TransactionConstants.MaxGasObjects)
            {
                this.SetError<SuiError>("Gas Payment is too high.");
                return;
            }

            this.BlockDataBuilder.Builder.GasConfig.Payment = payments;
        }

        /// <summary>
        /// Creates and appends a `TransactionBlockInput` object to the `BlockDataBuilder.Builder.Inputs`
        /// array and returns it. The type and value of the `TransactionBlockInput` are specified
        /// by the function parameters.
        /// </summary>
        /// <param name="type">A `CallArgumentType` representing the type of the input.</param>
        /// <param name="value">An `ISerializable` representing the value of the input.</param>
        /// <returns>A `TransactionBlockInput` object.</returns>
        public TransactionArgument AddInput
        (
            CallArgumentType type,
            ISerializable value
        )
        {
            int index = this.BlockDataBuilder.Builder.Inputs.Count();
            TransactionBlockInput input = new TransactionBlockInput(index, value, type);
            this.BlockDataBuilder.Builder.Inputs.Add(input);

            return new TransactionArgument(TransactionArgumentKind.Input, input);
        }

        /// <summary>
        /// Creates and appends a `TransactionArgument` object to the `BlockDataBuilder.Builder.Inputs`
        /// array and returns it. The type and value of the `TransactionArgument` are specified
        /// by the function parameters.
        /// </summary>
        /// <param name="value">An `ITransactionObjectInput` representing the value to add.</param>
        /// <returns>A `SuiResult` containing the added `TransactionArgument`.</returns>
        public TransactionArgument AddObjectInput(ITransactionObjectInput value)
        {
            if (value.Type == TransactionObjectInputType.TransactionObjectArgument)
                return ((TransactionArgumentTransactionObjectInput)value).Input;

            SuiResult<string> id = InputsHandler.GetIDFromCallArg(value);

            if (id.Error != null)
                return this.SetError<TransactionArgument, SuiError>(null, id.Error.Message);

            TransactionBlockInput[] inserted_arr = BlockDataBuilder.Builder.Inputs.Where
            (
                (input) =>
                {
                    if (input.Value == null || input.Value.GetType() != typeof(BString))
                        return false;

                    return id.Result == Utils.NormalizeSuiAddress(((BString)input.Value).Value);
                }
            ).ToArray();

            if (inserted_arr.Count() != 0)
                return new TransactionArgument(TransactionArgumentKind.Input, inserted_arr[0]);

            switch(value.Type)
            {
                case TransactionObjectInputType.StringArgument:
                    string string_argument = ((StringTransactionObjectInput)value).Input;
                    return this.AddInput(CallArgumentType.Object, new BString(string_argument));
                case TransactionObjectInputType.ObjectCallArgument:
                    ObjectArg object_call_argument = ((CallArgTransactionObjectInput)value).Input;
                    return this.AddInput(CallArgumentType.Object, object_call_argument);
                case TransactionObjectInputType.TransactionObjectArgument:
                    TransactionArgument transaction_argument = ((TransactionArgumentTransactionObjectInput)value).Input;
                    return this.AddInput(CallArgumentType.Object, transaction_argument);
                default:
                    return this.SetError<TransactionArgument, SuiError>(null, "Unable to add transaction object input.");
            }
        }

        /// <summary>
        /// Add the Object input using the Object's ID.
        /// </summary>
        /// <param name="id">The ID of the object.</param>
        /// <returns>A `TransactionArgument` object representing the result of the object input.</returns>
        public TransactionArgument AddObjectInput(string id)
            => this.AddObjectInput(new StringTransactionObjectInput(id));

        /// <summary>
        /// Add the Object argument to `BlockDataBuilder.Builder.Inputs`. An object input
        /// can either be a string argument or a transaction object argument.
        /// </summary>
        /// <param name="object_argument">The object argument to add.</param>
        /// <returns>A `TransactionArgument` object representing the result of the object input.</returns>
        public TransactionArgument AddObjectInput(IObjectArgument object_argument)
        {
            switch(object_argument.Type)
            {
                case ObjectArgumentType.StringArgument:
                    string argument_string = ((StringObjectArgument)object_argument).Argument;
                    return this.AddObjectInput(new StringTransactionObjectInput(argument_string));
                case ObjectArgumentType.TransactionObjectArgument:
                    TransactionArgument argument_transaction = ((TransactionObjectArgument)object_argument).Argument;
                    return this.AddObjectInput(new TransactionArgumentTransactionObjectInput(argument_transaction));
            }

            return this.SetError<TransactionArgument, SuiError>(null, "Unable to add object argument.");
        }

        /// <summary>
        /// Add an object argument to `BlockDataBuilder.Builder.Inputs`.
        /// </summary>
        /// <param name="object_arg">The ObjectArg input.</param>
        /// <returns>A `TransactionArgument` object representing the result of the object input.</returns>
        public TransactionArgument AddObjectRef(ObjectArg object_arg)
            => this.AddObjectInput(new CallArgTransactionObjectInput(object_arg));

        /// <summary>
        /// Add a shared object reference to `BlockDataBuilder.Builder.Inputs`.
        /// </summary>
        /// <param name="shared_object_ref">The shared object reference input.</param>
        /// <returns>A `TransactionArgument` object representing the result of the object input.</returns>
        public TransactionArgument AddSharedObjectRef(SharedObjectRef shared_object_ref)
            => this.AddObjectInput(new CallArgTransactionObjectInput(InputsHandler.SharedObjectRef(shared_object_ref)));

        /// <summary>
        /// Add a new non-object input to the transaction.
        /// </summary>
        /// <param name="value">
        ///     Can be a BString, Bytes, U8, U64, or an AccountAddress.
        ///     The pure value that will be used as the input value.
        ///     If this is a byte array, then the value is assumed
        ///     to be raw bytes, and will be used directly.
        /// </param>
        /// <returns>A `TransactionArgument` from the input builder representing the result of the transaction.</returns>
        public TransactionArgument AddPure(ISerializable value)
            => this.AddInput(CallArgumentType.Pure, new CallArg(CallArgumentType.Pure, new PureCallArg(value)));

        /// <summary>
        /// Add a new non-object input to the transaction.
        /// </summary>
        /// <param name="value">
        ///     A byte array representing the raw data of the
        ///     pure value.
        /// </param>
        /// <returns>A `TransactionArgument` from the input builder representing the result of the transaction.</returns>
        public TransactionArgument AddPure(byte[] value)
            => this.AddInput(CallArgumentType.Pure, new CallArg(CallArgumentType.Pure, new PureCallArg(value)));

        /// <summary>
        /// Appends a `Command` object to the `BlockDataBuilder.Transactions` array and
        /// returns a `TransactionArgument` object representing the result.
        /// </summary>
        /// <param name="transaction">A `Command` object to be added.</param>
        /// <param name="return_value_count">
        /// If using a `MoveCall` transaction, this is the amount
        /// of return values (if greater than 1) that will be returned by the move call.
        /// </param>
        /// <returns>A `TransactionArgument` object representing the result of the addition.</returns>
        public List<TransactionArgument> AddTransaction(Command transaction, int? return_value_count = null)
        {
            this.BlockDataBuilder.Builder.Transactions.Add(transaction);

            int index = this.BlockDataBuilder.Builder.Transactions.Count();

            TransactionResult transaction_result = new TransactionResult((ushort)(index - 1), (ushort?)return_value_count);

            List<TransactionArgument> results = new List<TransactionArgument>();

            if (return_value_count == null)
                results.Add(transaction_result.TransactionArgument);
            else
            {
                foreach(TransactionArgument nested_result in transaction_result)
                    results.Add(nested_result);

                results.Reverse();
            }

            return results;
        }

        /// <summary>
        /// Add a SplitCoins transaction to the list of transaction in
        /// the Programmable Transaction Block.
        /// </summary>
        /// <param name="coin">GasCoin is a type of `TransactionArgument`.</param>
        /// <param name="amounts">A list of respective amounts for each coin we are splitting.</param>
        /// <returns>A list of `TransactionResult`s.</returns>
        public List<TransactionArgument> AddSplitCoinsTx(TransactionArgument coin, params TransactionArgument[] amounts)
            => this.AddTransaction(new Command(CommandKind.SplitCoins, new SplitCoins(coin, amounts)));

        /// <summary>
        /// Merges multiple source coins into a single destination coin.
        /// </summary>
        /// <param name="destination">An `ITransactionArgument` representing the destination coin.</param>
        /// <param name="sources">An array of `ITransactionArgument` representing the source coins.</param>
        /// <returns>A `TransactionArgument` array representing the result of the merge coin operation.</returns>
        public List<TransactionArgument> AddMergeCoinsTx(TransactionArgument destination, TransactionArgument[] sources)
            => this.AddTransaction(new Command(CommandKind.MergeCoins, new MergeCoins(destination, sources)));

        /// <summary>
        /// Publishes modules with given dependencies.
        /// </summary>
        /// <param name="modules">An array of `byte[]` representing the modules to be published.</param>
        /// <param name="dependencies">An array of `AccountAddress` representing the dependencies.</param>
        /// <returns>A `TransactionArgument` array representing the result of the publish operation.</returns>
        public List<TransactionArgument> AddPublishTx(byte[][] modules, AccountAddress[] dependencies)
            => this.AddTransaction(new Command(CommandKind.Publish, new Publish(modules, dependencies)));

        /// <summary>
        /// Publishes modules with given dependencies.
        /// </summary>
        /// <param name="modules">An array of `string` representing the modules to be published.</param>
        /// <param name="dependencies">An array of `string` representing the dependencies.</param>
        /// <returns>A `TransactionArgument` array representing the result of the publish operation.</returns>
        public List<TransactionArgument> AddPublishTx(string[] modules, string[] dependencies)
        {
            Publish publish_tx = new Publish
            (
                modules.Select((module) => { return CryptoBytes.FromBase64String(module); }).ToArray(),
                dependencies.Select((dependency) => AccountAddress.FromHex(dependency)).ToArray()
            );

            return this.AddTransaction(new Command(CommandKind.Publish, publish_tx));
        }

        /// <summary>
        /// Upgrades modules with given dependencies, packageId, and ticket.
        /// </summary>
        /// <param name="modules">An array of `byte[]` representing the modules to be upgraded.</param>
        /// <param name="dependencies">An array of `AccountAddress` representing the dependencies.</param>
        /// <param name="packageId">A `string` representing the package ID.</param>
        /// <param name="ticket">An `ITransactionArgument` representing the ticket.</param>
        /// <returns>A `TransactionArgument` array representing the result of the upgrade operation.</returns>
        public List<TransactionArgument> AddUpgradeTx
        (
            byte[][] modules,
            AccountAddress[] dependencies,
            string packageId,
            TransactionArgument ticket
        )
            => this.AddTransaction(new Command(CommandKind.Upgrade, new Upgrade(modules, dependencies, packageId, ticket)));

        /// <summary>
        /// Makes a move call with target, optional arguments, and optional type arguments.
        /// </summary>
        /// <param name="target">A `SuiMoveNormalizedStructType` representing the target of the move call.</param>
        /// <param name="type_arguments">An optional array of `SerializableTypeTag` representing the arguments of the move call.</param>
        /// <param name="arguments">An optional array of `TransactionArgument` representing the type arguments of the move call.</param>
        /// <param name="return_value_count">The number of return values, greater than 1, that are returned by the move call.</param>
        /// <returns>A `TransactionArgument` array representing the result of the move call.</returns>
        public List<TransactionArgument> AddMoveCallTx
        (
            SuiMoveNormalizedStructType target,
            SerializableTypeTag[] type_arguments = null,
            TransactionArgument[] arguments = null,
            int? return_value_count = null
        )
            => this.AddTransaction(new Command(CommandKind.MoveCall, new MoveCall(target, type_arguments, arguments)), return_value_count);

        /// <summary>
        /// Transfers objects to a specified address.
        /// </summary>
        /// <param name="objects">An array of `ITransactionArgument` representing the objects to be transferred.</param>
        /// <param name="address">An `ITransactionArgument` representing the address to transfer objects to.</param>
        /// <returns>A `TransactionArgument` array representing the result of the transfer object operation.</returns>
        public List<TransactionArgument> AddTransferObjectsTx(TransactionArgument[] objects, TransactionArgument address)
            => this.AddTransaction(new Command(CommandKind.TransferObjects, new TransferObjects(objects, address)));

        /// <summary>
        /// Transfers objects to a specified address.
        /// </summary>
        /// <param name="objects">An array of `ITransactionArgument` representing the objects to be transferred.</param>
        /// <param name="address">A `string` representing the address to transfer objects to.</param>
        /// <returns>A `TransactionArgument` array representing the result of the transfer object operation.</returns>
        public List<TransactionArgument> AddTransferObjectsTx(TransactionArgument[] objects, AccountAddress address)
            => this.AddTransaction(new Command(CommandKind.TransferObjects, new TransferObjects(objects, this.AddPure(address))));

        /// <summary>
        /// Makes a Move Vector with the specified type and objects.
        /// </summary>
        /// <param name="objects">An array of `ITransactionArgument` representing the objects of the Move Vector.</param>
        /// <param name="type">An optional `SuiStructTag` representing the type of the Move Vector.</param>
        /// <returns>A `TransactionArgument` array representing the result of the make Move Vector operation.</returns>
        public List<TransactionArgument> AddMakeMoveVecTx(TransactionArgument[] objects, SuiStructTag type = null)
            => this.AddTransaction(new Command(CommandKind.MakeMoveVec, new MakeMoveVec(objects, type)));

        /// <summary>
        /// Builds a block with the specified provider and optional transaction kind.
        /// </summary>
        /// <param name="build_options">An instance of `BuildOptions` that contains the options passed for preparing the transaction block.</param>
        /// <returns>A `byte` array representing the built block.</returns>
        public async Task<byte[]> Build(BuildOptions build_options)
        {
            await this.Prepare(build_options);

            if (this.Error != null)
                return null;

            byte[] build_result = this.BlockDataBuilder.Build(null, build_options.OnlyTransactionKind);

            if (this.BlockDataBuilder.Error != null)
                return this.SetError<byte[], SuiError>(null, this.BlockDataBuilder.Error.Message);

            return build_result;
        }

        /// <summary>
        /// Computes the digest of the block with the specified provider.
        /// </summary>
        /// <param name="build_options">An instance of `BuildOptions` that contains the options passed for preparing the transaction block.</param>
        /// <returns>A `string` representing the digest of the block.</returns>
        public async Task<string> GetDigest(BuildOptions build_options)
        {
            await this.Prepare(build_options);

            if (this.Error != null)
                return null;

            string digest_result = this.BlockDataBuilder.GetDigest();

            if (this.BlockDataBuilder.Error != null)
                return this.SetError<string, SuiError>(null, this.BlockDataBuilder.Error.Message);

            return digest_result;
        }

        /// <summary>
        /// Retrieves a configuration value for a specified key.
        /// </summary>
        /// <param name="key">A `LimitKey` representing the key for which the configuration value needs to be retrieved.</param>
        /// <param name="build_options">A `BuildOptions` object containing the build options including limits and protocolConfig.</param>
        /// <returns>An `int` representing the configuration value for the specified key.</returns>
        private BigInteger? GetConfig(LimitKey key, BuildOptions build_options)
        {
            if (build_options.Limits != null && build_options.Limits[BuildOptions.TransactionLimits[key]].HasValue)
            {
                int? result = build_options.Limits[BuildOptions.TransactionLimits[key]];

                if (result == null)
                    return this.SetError<BigInteger?, SuiError>(null, $"Unable to find {key}, got {BuildOptions.TransactionLimits[key]} and {build_options.Limits[BuildOptions.TransactionLimits[key]]}.");

                return new BigInteger((int)result);
            }

            if (build_options.ProtocolConfig == null)
                return new BigInteger(DefaultOfflineLimits[key]);

            AttributeValue attribute = build_options.ProtocolConfig.Attributes[BuildOptions.TransactionLimits[key]];
            BigInteger? attribute_value = attribute.GetValue();

            if (attribute_value == null)
                return this.SetError<BigInteger?, SuiError>(null, attribute.Error.Message);

            return attribute_value;
        }

        /// <summary>
        /// Prepares gas payment for transactions.
        /// </summary>
        /// <param name="options">A `BuildOptions` object that contains the `Provider` and `OnlyCommandKind` members.</param>
        /// <returns>A `Task` object used for implementation with Coroutines.</returns>
        private async Task PrepareGasPaymentAsync(BuildOptions options)
        {
            if (this.IsMissingSender(options.OnlyTransactionKind))
            {
                this.SetError<SuiError>("Sender Is Missing");
                return;
            }

            if ((options.OnlyTransactionKind.HasValue && options.OnlyTransactionKind == true) || this.BlockDataBuilder.Builder.GasConfig.Payment != null)
                return;

            if (this.BlockDataBuilder.Builder.GasConfig.Owner == null && this.BlockDataBuilder.Builder.Sender == null)
            {
                this.SetError<SuiError>("Gas Owner Cannot Be Found");
                return;
            }

            AccountAddress gas_owner =
                this.BlockDataBuilder.Builder.GasConfig.Owner != null ?
                this.BlockDataBuilder.Builder.GasConfig.Owner :
                this.BlockDataBuilder.Builder.Sender;

            RpcResult<CoinPage> coins = await options.Provider.GetCoinsAsync(gas_owner, Utils.SuiCoinStruct);

            if (coins.Error != null)
            {
                this.SetError<RpcError>(coins.Error.Message);
                return;
            }

            List<CoinDetails> filtered_coins = coins.Result.Data.Where((coin) => {
                return this.BlockDataBuilder.Builder.Inputs.Any((input) => {
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
                                    ((SuiObjectRef)object_arg.ObjectArg.ObjectRef).ObjectID;
                            }
                        }
                    }
                    return false;
                }) == false;
            }).ToList();

            int coin_range_max = Math.Min(filtered_coins.Count(), (int)TransactionConstants.MaxGasObjects);
            List<int> range = Enumerable.Range(0, coin_range_max).ToList();

            IEnumerable<SuiObjectRef> payment_coins = range.Select((idx) => {
                CoinDetails coin = filtered_coins.ElementAt(idx);
                return new SuiObjectRef
                (
                    coin.CoinObjectID,
                    coin.Version,
                    coin.Digest
                );
            });

            if (payment_coins.Count() == 0)
            {
                this.SetError<SuiError>("Owner Does Not Have Payment Coins");
                return;
            }

            this.SetGasPayment(payment_coins.ToArray());
        }

        /// <summary>
        /// Prepares gas price for transactions.
        /// </summary>
        /// <param name="options">A `BuildOptions` object that contains the `Provider` and `OnlyCommandKind` members.</param>
        /// <returns>A `Task` object used for implementation with Coroutines.</returns>
        private async Task PrepareGasPriceAsync(BuildOptions options)
        {
            if (this.IsMissingSender(options.OnlyTransactionKind))
            {
                this.SetError<SuiError>("Sender Is Missing");
                return;
            }

            RpcResult<BigInteger> gas_price = await options.Provider.GetReferenceGasPriceAsync();

            if (gas_price.Error != null)
            {
                this.SetError<RpcError>(gas_price.Error.Message);
                return;
            }

            this.SetGasPrice(gas_price.Result);
        }

        /// <summary>
        /// Resolves all required Move modules and objects.
        /// </summary>
        /// <param name="options">A `BuildOptions` object that contains the `Provider` and `OnlyCommandKind` members.</param>
        /// <returns>A `Task` object used for implementation with Coroutines that contains an `ErrorBase` object if any of the steps fail.</returns>
        private async Task PrepareTransactions(BuildOptions options)
        {
            #region Initialize lists

            // The inputs in the `TransactionBlock`.
            List<TransactionBlockInput> inputs = this.BlockDataBuilder.Builder.Inputs;
            List<Command> transactions = this.BlockDataBuilder.Builder.Transactions;

            // A list of move modules identified as needing to be resolved.
            List<MoveCall> move_modules_to_resolve = new List<MoveCall>();
            List<ObjectToResolve> objects_to_resolve = new List<ObjectToResolve>();
            List<ObjectToResolve> resolved_objects = new List<ObjectToResolve>();

            #endregion

            #region Resolve current inputs

            // If the input is a string (and is a valid hexadecimal value, e.g., Object ID), add it to
            // the objects to resolve list.
            foreach (TransactionBlockInput input in inputs)
                if (input.Value.GetType() == typeof(BString))
                    if (Regex.IsMatch(((BString)input.Value).Value, @"^(0x)?[0-9a-fA-F]{32,64}$"))
                        objects_to_resolve.Add
                        (
                            new ObjectToResolve
                            (
                                ((BString)input.Value).Value,
                                input,
                                null
                            )
                        );

            #endregion

            #region Process all transactions

            foreach (Command transaction in transactions)
            {
                #region Process MoveCall Transactions

                if (transaction.Kind == CommandKind.MoveCall)
                {
                    ((MoveCall)transaction.Function).AddToResolve(move_modules_to_resolve, inputs);
                    continue;
                }

                #endregion

                #region Process TransferObjects Transactions

                if (transaction.Kind == CommandKind.TransferObjects)
                {
                    TransferObjects transfer_objects_tx = (TransferObjects)transaction.Function;
                    TransactionArgument address = transfer_objects_tx.Address;

                    if (address.Kind == TransactionArgumentKind.Input)
                    {
                        // Cast the address as a `TransactionBlockInput` to get index property.
                        TransactionBlockInput address_input = (TransactionBlockInput)address.Argument;

                        // Get the transaction input object at the index provided by the addressInput argument.
                        TransactionBlockInput input = inputs[address_input.Index];

                        // If the value of the input is not an object type then it must be a Pure.
                        if (input.Value.GetType() != typeof(CallArg) && input.Value.GetType() != typeof(AccountAddress))
                        {
                            Serialization ser = new Serialization();
                            input.Value.Serialize(ser);
                            this.BlockDataBuilder.Builder.Inputs[address_input.Index].Value = new CallArg(CallArgumentType.Pure, new PureCallArg(ser.GetBytes()));
                        }
                    }

                    continue;
                }

                #endregion

                #region Process SplitCoins Transaction

                if (transaction.Kind == CommandKind.SplitCoins)
                {
                    SplitCoins split_coins_tx = (SplitCoins)transaction.Function;
                    TransactionArgument[] amounts = split_coins_tx.Amounts;

                    foreach (TransactionArgument amount in amounts)
                    {
                        if (amount.Kind == TransactionArgumentKind.Input)
                        {   // Cast the amount as a `TransactionBlockInput` to get index property.
                            TransactionBlockInput amount_tx_input = (TransactionBlockInput)amount.Argument;

                            // Get the transaction input object at the index provided by the amount argument.
                            TransactionBlockInput input = inputs[amount_tx_input.Index];

                            // If the value of the input is not an object type then it must be a Pure.
                            if(input.Value.GetType() != typeof(CallArg) && input.Value.GetType() != typeof(AccountAddress))
                            {
                                Serialization ser = new Serialization();
                                input.Value.Serialize(ser);
                                this.BlockDataBuilder.Builder.Inputs[amount_tx_input.Index].Value = new CallArg(CallArgumentType.Pure, new PureCallArg(ser.GetBytes()));
                            }
                        }
                    }
                }

                #endregion
            }

            #endregion

            #region Resolve Move modules

            if (move_modules_to_resolve.Count > 0)
            {
                foreach (MoveCall move_call in move_modules_to_resolve)
                {
                    #region RPC Call GetNormalizedMoveFunction

                    RpcResult<NormalizedMoveFunctionResponse> move_function_result = await options.Provider.GetNormalizedMoveFunctionAsync(move_call.Target);

                    if (move_function_result.Error != null)
                    {
                        this.SetError<RpcError>(move_function_result.Error.Message);
                        return;
                    }

                    NormalizedMoveFunctionResponse normalized = move_function_result.Result;

                    #endregion

                    #region Get and verify arguments

                    // Entry functions can have a mutable reference to an instance of the TxContext
                    // struct defined in the TxContext module as the last parameter. The caller of
                    // the function does not need to pass it in as an argument.
                    List<SuiMoveNormalizedType> params_list = normalized.HasTxContext() ?
                        normalized.Parameters.Take(normalized.Parameters.Count() - 1).ToList() :
                        normalized.Parameters.ToList();

                    if (params_list.Count != move_call.Arguments.Length)
                    {
                        this.SetError<SuiError>("Incorrect number of arguments.");
                        return;
                    }

                    #endregion

                    #region Normalize parameters

                    foreach (Tuple<int, SuiMoveNormalizedType> param_enumerated in params_list.Select((param, i) => new Tuple<int, SuiMoveNormalizedType>(i, param)))
                    {
                        TransactionArgument arg = move_call.Arguments[param_enumerated.Item1];

                        if (arg.Kind != TransactionArgumentKind.Input)
                            continue;

                        TransactionBlockInput input_arg = (TransactionBlockInput)arg.Argument;
                        TransactionBlockInput input = inputs[input_arg.Index];

                        // Skip if the input is already resolved, aka if the input is a BuilderArg.
                        if (input.Value.GetType() == typeof(CallArg))
                            continue;

                        // When we reach here, this means that the value could be a BString, a U8, etc.
                        // We need to compare agains the RPC response params to know how to cast to a concrete type.
                        // Once we know how to cast, then we will be able to serialize it later on.
                        ISerializable input_value = input.Value;
                        SuiResult<string> ser_type = NormalizedUtilities.GetPureNormalizedType(param_enumerated.Item2, input_value);

                        // If the serialization type is a pure call argument, add it to the builder inputs list.
                        if (ser_type != null)
                        {
                            if (ser_type.Error != null)
                            {
                                this.SetError<SuiError>(ser_type.Error.Message);
                                return;
                            }

                            this.BlockDataBuilder.Builder.Inputs[input_arg.Index].Value = new PureCallArg(input_value);
                            continue;
                        }

                        // If the parameter isn't a reference nor a mutable reference (and if it isn't a Type Parameter), it's an unknown call argument.
                        if (NormalizedUtilities.ExtractStructType(param_enumerated.Item2) == null && param_enumerated.Item2.Type != SuiMoveNormalizedTypeSerializationType.TypeParameter)
                        {
                            this.SetError<SuiError>("Unknown Call Arg Type.", param_enumerated.Item2);
                            return;
                        }

                        // If the input value is a string, we need to resolve it (it's an object ID).
                        if (input_value.GetType() == typeof(BString))
                        {
                            objects_to_resolve.Add(new ObjectToResolve(((BString)input_value).Value, input, param_enumerated.Item2));
                            continue;
                        }

                        this.SetError<SuiError>("Input Value Is Not Object ID.", input_value);
                        return;
                    }

                    #endregion
                }
            }

            #endregion

            #region Resolve objects

            if (objects_to_resolve.Count > 0)
            {
                List<string> mapped_ids = objects_to_resolve.Select(x => x.ID).ToList();

                // NOTE: Insertion order in HashSet will be maintained until removing or re-adding
                List<string> deduped_ids = new HashSet<string>(mapped_ids).ToList();

                // Chunk list of IDs into smaller lists to use in RPC Call `MultiGetObjects`
                List<List<string>> object_chunks = deduped_ids.Chunked((int)TransactionConstants.MaxObjectsPerFetch);

                List<ObjectDataResponse> objects_response = new List<ObjectDataResponse>();

                #region RPC Call MultiGetObjects

                foreach (List<string> object_ids in object_chunks)
                {
                    RpcResult<IEnumerable<ObjectDataResponse>> get_object_response = await options.Provider.MultiGetObjectsAsync
                    (
                        object_ids.Select((id) => AccountAddress.FromHex(id)).ToList(),
                        new ObjectDataOptions(show_owner: true)
                    );

                    if (get_object_response.Error != null)
                    {
                        this.SetError<RpcError>(get_object_response.Error.Message);
                        return;
                    }

                    objects_response.AddRange(get_object_response.Result);
                }

                #endregion

                #region Populate objects By ID

                // Create a map of IDs to ObjectDataResponse
                Dictionary<string, ObjectDataResponse> objects_by_id = new Dictionary<string, ObjectDataResponse>();

                // Populate map(Dictionary) `objectsById`
                for (int i = 0; i < deduped_ids.Count; i++)
                    objects_by_id.Add(deduped_ids[i], objects_response[i]);

                // Filter objects that returned an error
                List<string> invalid_objects = objects_by_id.Where((obj) =>
                    obj.Value.Error != null
                ).Select(obj => obj.Key).ToList();

                if (invalid_objects.Count > 0)
                {
                    this.SetError<SuiError>("Invalid object found.", invalid_objects);
                    return;
                }

                #endregion

                #region Resolve objects

                foreach (ObjectToResolve object_to_resolve in objects_to_resolve)
                {
                    ObjectDataResponse object_data_response = objects_by_id[object_to_resolve.ID];

                    int? inital_shared_version = object_data_response.GetSharedObjectInitialVersion();

                    if (inital_shared_version == null)
                    {
                        SuiObjectRef object_reference = object_data_response.GetObjectReference();

                        if (object_reference == null)
                            continue;

                        object_to_resolve.Input.Value = new CallArg(CallArgumentType.Object, new ObjectCallArg(new ObjectArg(ObjectRefType.ImmOrOwned, object_reference)));

                        if (resolved_objects.Count > object_to_resolve.Input.Index)
                            resolved_objects[object_to_resolve.Input.Index] = object_to_resolve;
                        else
                            resolved_objects.Add(object_to_resolve);

                        continue;
                    }

                    bool is_by_value =
                        object_to_resolve.NormalizedType != null &&
                        NormalizedUtilities.ExtractStructType(object_to_resolve.NormalizedType) == null;

                    bool mutable = is_by_value ||
                    (
                        object_to_resolve.NormalizedType != null &&
                        NormalizedUtilities.ExtractMutableReference(object_to_resolve.NormalizedType) != null
                    );

                    object_to_resolve.Input.Value = new CallArg
                    (
                        CallArgumentType.Object,
                        new ObjectCallArg
                        (
                            new ObjectArg
                            (
                                ObjectRefType.Shared,
                                new SharedObjectRef
                                (
                                    AccountAddress.FromHex(object_to_resolve.ID),
                                    (int)inital_shared_version,
                                    mutable
                                )
                            )
                        )
                    );

                    if (resolved_objects.Count > object_to_resolve.Input.Index)
                    {
                        if (resolved_objects[object_to_resolve.Input.Index].Input.Value.GetType() == typeof(CallArg))
                        {
                            CallArg call_arg_resolved = (CallArg)resolved_objects[object_to_resolve.Input.Index].Input.Value;
                            if (object_to_resolve.Input.Value.GetType() == typeof(CallArg))
                            {
                                CallArg call_arg_to_resolve = (CallArg)object_to_resolve.Input.Value;
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
                                    object_to_resolve.Input.Value = new CallArg
                                    (
                                        CallArgumentType.Object,
                                        new ObjectCallArg(new ObjectArg(ObjectRefType.Shared, shared))
                                    );
                                }
                            }
                        }
                        resolved_objects[object_to_resolve.Input.Index] = object_to_resolve;
                    }
                    else
                        resolved_objects.Add(object_to_resolve);
                }

                #endregion
            }

            #endregion

            #region Replace unresolved objects with resolved objects

            foreach (ObjectToResolve resolved_object in resolved_objects)
                this.BlockDataBuilder.Builder.Inputs[resolved_object.Input.Index] = resolved_object.Input;

            this.BlockDataBuilder.Builder.Inputs.Sort((TransactionBlockInput t1, TransactionBlockInput t2) => t1.Index.CompareTo(t2.Index));

            #endregion
        }

        /// <summary>
        /// Determines whether the sender is missing considering the specified transaction kind.
        /// </summary>
        /// <param name="only_transaction_kind">An optional `bool` representing whether only transaction kind should be considered.</param>
        /// <returns>A `bool` indicating whether the sender is missing.</returns>
        private bool IsMissingSender(bool? only_transaction_kind = null)
            => only_transaction_kind.HasValue && only_transaction_kind == false && this.BlockDataBuilder.Builder.Sender == null;

        /// <summary>
        /// Prepares the transaction block with the provided build options.
        /// </summary>
        /// <param name="options">An instance of `BuildOptions` that contains the options passed for preparing the transaction block.</param>
        /// <returns>A `Task` object used for implementations with async calls.</returns>
        private async Task Prepare(BuildOptions options)
        {
            if (IsPrepared)
                return;

            if (options.ProtocolConfig == null)
            {
                RpcResult<ProtocolConfig> protocol_config = await options.Provider.GetProtocolConfigAsync();

                if (protocol_config.Error != null)
                {
                    this.SetError<RpcError>(protocol_config.Error.Message);
                    return;
                }

                options.ProtocolConfig = protocol_config.Result;
            }

            await PrepareGasPriceAsync(options);

            if (this.Error != null)
                return;

            await PrepareTransactions(options);

            if (this.Error != null)
                return;

            if (options.OnlyTransactionKind == null || (options.OnlyTransactionKind != null && options.OnlyTransactionKind == false))
            {
                await this.PrepareGasPaymentAsync(options);

                if (this.Error != null)
                    return;

                if (this.BlockDataBuilder.Builder.GasConfig.Budget == null)
                {
                    GasData gas_config = new GasData();

                    BigInteger? gas_budget_result = GetConfig(LimitKey.MaxTxGas, options);

                    if (this.Error != null)
                        return;

                    gas_config.Budget = gas_budget_result;
                    gas_config.Payment = new SuiObjectRef[] { };
                    gas_config.Price = BlockDataBuilder.Builder.GasConfig.Price;
                    gas_config.Owner = BlockDataBuilder.Builder.GasConfig.Owner;

                    TransactionBlockDataBuilderSerializer tx_block_data_builder = this.BlockDataBuilder;

                    byte[] build_result = tx_block_data_builder.Build
                    (
                        new TransactionBlockDataBuilderSerializer
                        (
                            new TransactionBlockDataBuilder(gasConfig: gas_config)
                        )
                    );

                    if (tx_block_data_builder.Error != null)
                    {
                        this.SetError<SuiError>(tx_block_data_builder.Error.Message);
                        return;
                    }

                    RpcResult<TransactionBlockResponse> dry_run_result = await options.Provider.DryRunTransactionBlockAsync
                    (
                        Convert.ToBase64String(build_result)
                    );

                    if (dry_run_result.Error != null)
                    {
                        this.SetError<RpcError>(dry_run_result.Error.Message);
                        return;
                    }

                    if (dry_run_result.Result.Effects.Status.Status == ExecutionStatus.Failure)
                    {
                        this.SetError<SuiError>($"Transaction failed with message - {dry_run_result.Result.Effects.Status.Error}");
                        return;
                    }

                    BigInteger safe_overhead =
                        (int)TransactionConstants.GasSafeOverhead *
                        BigInteger.Parse
                        (
                            BlockDataBuilder.Builder.GasConfig.Price != null ?
                                BlockDataBuilder.Builder.GasConfig.Price.ToString() :
                                "1"
                        );

                    BigInteger base_computation_cost_with_overhead =
                        BigInteger.Parse
                        (
                            dry_run_result.Result.Effects.GasUsed.ComputationCost != null ?
                                dry_run_result.Result.Effects.GasUsed.ComputationCost.ToString() :
                                "0"
                        ) +
                        safe_overhead;

                    BigInteger gas_budget =
                        base_computation_cost_with_overhead +
                        BigInteger.Parse
                        (
                            dry_run_result.Result.Effects.GasUsed.StorageCost != null ?
                                dry_run_result.Result.Effects.GasUsed.StorageCost.ToString() :
                                "0"
                        ) +
                        BigInteger.Parse
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
        }
    }
}