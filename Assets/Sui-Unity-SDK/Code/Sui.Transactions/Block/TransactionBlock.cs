using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
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

namespace Sui.Transactions
{
    /// <summary>
    /// A transaction block builder.
    /// </summary>
    public class TransactionBlock : ISerializable
    {
        /// <summary>
        /// The transaction block builder.
        /// </summary>
        public TransactionBlockDataBuilder BlockDataBuilder { get; set; }

        /// <summary>
        /// The list of transaction the "transaction builder" will use to create
        /// the transaction block. This can be any transaction type defined
        /// in the `ITransaction` interface, e.g. `MoveCall`, `SplitCoins`.
        /// </summary>
        public List<ITransaction> Transactions { get; set; }

        /// <summary>
        /// A list of object that we need to resolve by first querying the RPC API.
        /// </summary>
        private List<ObjectToResolve> objectsToResolve = new List<ObjectToResolve>();

        /// <summary>
        /// Creates a TransactionBlock object from an existing TransactionBlock.
        /// </summary>
        /// <param name="transactionBlock"></param>
        public TransactionBlock(TransactionBlock transactionBlock = null)
        {
            if (transactionBlock != null)
                BlockDataBuilder = transactionBlock.BlockDataBuilder;
            else
                BlockDataBuilder = new TransactionBlockDataBuilder();
        }

        /// <summary>
        /// Set the sender for the programmable transaction block.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public TransactionBlock SetSender(AccountAddress sender)
        {
            this.BlockDataBuilder.Sender = sender;
            return this;
        }

        /// <summary>
        /// The the Sender, if it has not been set in the programmable transaction.
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public TransactionBlock SetSenderIfNotSet(AccountAddress sender)
        {
            if (this.BlockDataBuilder.Sender == null)
                return this.SetSender(sender);
            return this;
        }

        /// <summary>
        /// Set expiration for the programmable transaction.
        /// </summary>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public TransactionBlock SetExpiration(TransactionExpiration expiration)
        {
            this.BlockDataBuilder.Expiration = expiration;
            return this;
        }

        /// <summary>
        /// Sets the gas price.
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public TransactionBlock SetGasPrice(BigInteger price)
        {
            this.BlockDataBuilder.GasConfig.Price = price;
            return this;
        }

        /// <summary>
        /// Set the gas budget for the programmable transaction block.
        /// </summary>
        /// <param name="budget"></param>
        /// <returns></returns>
        public TransactionBlock SetGasBudget(int budget)
        {
            this.BlockDataBuilder.GasConfig.Budget = budget;
            return this;
        }

        /// <summary>
        /// Set the gas owner for the programmable transaction block.
        /// </summary>
        /// <param name="owner"></param>
        /// <returns></returns>
        public TransactionBlock SetGasOwner(AccountAddress owner)
        {
            this.BlockDataBuilder.GasConfig.Owner = owner;
            return this;
        }

        /// <summary>
        /// Set the gas payment for the programmable transaction block.
        /// </summary>
        /// <param name="payments"></param>
        /// <returns></returns>
        public TransactionBlock SetGasPayment(Sui.Types.SuiObjectRef[] payments)
        {
            this.BlockDataBuilder.GasConfig.Payment = payments;
            return this;
        }

        /// <summary>
        /// Gets the programmable transaction block data.
        /// </summary>
        /// <returns></returns>
        public Builder.TransactionBlockData GetBlockData()
        {
            return this.BlockDataBuilder.Snapshot();
        }


        //public TransactionBlockInput AddObjectInput(IObjectRef objectRef)
        //{
        //    Type objectType = objectRef.GetType();
        //    string newObjectId = "";
        //    //if (objectType == typeof(SuiObjectRef))
        //    //{
        //    //    SuiObjectRef ImmObjectRef =  (SuiObjectRef)objectRef;
        //    //    newObjectId = ImmObjectRef.ObjectId;
        //    //}
        //    //else
        //    //{
        //    //    SharedObjectRef sharedObjectRef = (SharedObjectRef)objectRef;
        //    //    newObjectId = sharedObjectRef.ObjectId;
        //    //}

        //    newObjectId = objectRef.ObjectId;

        //    List<TransactionBlockInput> inputs = this.BlockDataBuilder.Inputs;

        //    // Search through the list of inputs in the transaction block
        //    // for a block input that has the name id as a `newObjectId`
        //    TransactionBlockInput inserted = inputs.Find((blockInput) =>
        //    {
        //        Type blockInputValueType = blockInput.Value.GetType();
        //        if (blockInputValueType == typeof(ObjectCallArg))
        //        {
        //            ObjectCallArg _objCallArg = (ObjectCallArg)blockInput.Value;
        //            IObjectRef _objectRef = _objCallArg.ObjectArg;

        //            return newObjectId == _objectRef.ObjectId;
        //        }
        //        return false;
        //    });

        //    // If it it's already in the list of inputs, then don't insert it
        //    if (inserted != null)
        //        return inserted;

        //    // Otherwise,
        //    // create ObjectCallArg which will add the appropriate byte when serializing
        //    // then add it to the list of inputs
        //    ObjectCallArg newObjCallArg = new ObjectCallArg(objectRef);
        //    return this.CreateAddInput(newObjCallArg);
        //}

        /// <summary>
        /// Utility function to create and add an object input to the TransactionBlock.
        /// <code>
        ///     // In TypeScript SDK
        ///     txb.object(object_id)
        ///
        ///     object(value: string | ObjectCallArg)
        /// </code>
        /// </summary>
        /// <param name="objectRef"></param>
        /// <returns></returns>
        public TransactionBlockInput AddObjectInput(string objectId)
        {
            AccountAddress address = AccountAddress.FromHex(objectId);
            return AddObjectInput(address);
        }

        /// <summary>
        /// Utility function that creates a `TransactionBlockInput` from a
        /// given objectID address.
        ///
        /// <code>
        ///     // In TypeScript SDK
        ///     txb.object(object_id)
        ///
        ///     object(value: string | ObjectCallArg)
        /// </code>
        /// </summary>
        /// <param name="objectRef"></param>
        /// <returns></returns>
        public TransactionBlockInput AddObjectInput(AccountAddress objectIdValue)
        {
            string newObjectId = objectIdValue.ToHex();

            List<TransactionBlockInput> inputs = this.BlockDataBuilder.Inputs;

            // Search through the list of inputs in the transaction block
            // for a block input that has the name id as a `newObjectId`
            TransactionBlockInput inserted = inputs.Find((blockInput) =>
            {
                Type blockInputValueType = blockInput.Value.GetType();
                if (blockInputValueType == typeof(ObjectCallArg))
                {
                    ObjectCallArg _objCallArg = (ObjectCallArg)blockInput.Value;
                    IObjectRef _objectRef = _objCallArg.ObjectArg;

                    return newObjectId == _objectRef.ObjectId;
                }
                return false;
            });

            // If it it's already in the list of inputs, then don't insert it
            if (inserted != null)
                return inserted;

            // Otherwise,
            // create ObjectCallArg which will add the appropriate byte when serializing
            // then add it to the list of inputs
            return this.CreateAddInput(objectIdValue);

            throw new NotSupportedException();
        }

        public TransactionBlockInput AddObjectInput(ObjectCallArg objectCallArg)
        {
            string newObjectId = objectCallArg.ObjectArg.ObjectId;

            List<TransactionBlockInput> inputs = this.BlockDataBuilder.Inputs;

            // Search through the list of inputs in the transaction block
            // for a block input that has the name id as a `newObjectId`
            TransactionBlockInput inserted = inputs.Find((blockInput) =>
            {
                Type blockInputValueType = blockInput.Value.GetType();
                if (blockInputValueType == typeof(ObjectCallArg))
                {
                    ObjectCallArg _objCallArg = (ObjectCallArg)blockInput.Value;
                    IObjectRef _objectRef = _objCallArg.ObjectArg;

                    return newObjectId == _objectRef.ObjectId;
                }
                return false;
            });

            // If it it's already in the list of inputs, then don't insert it
            if (inserted != null)
                return inserted;

            // Otherwise,
            // create ObjectCallArg which will add the appropriate byte when serializing
            // then add it to the list of inputs
            return this.CreateAddInput(objectCallArg);
        }

        /// <summary>
        /// Dynamically create a new input, which is separate from the `input`. This is important
        /// for generated clients to be able to define unique inputs that are non-overlapping with the
        /// defined inputs.
        /// 
        /// For `Uint8Array` type automatically convert the input into a `Pure` CallArg, since this
        /// is the format required for custom serialization.
        /// <code>
        ///     #input(type: 'object' | 'pure', value?: unknown) {
        /// </code>
        /// </summary>
        /// <param name="value">Can be a `PureCallArg` or an `ObjectCallArg`</param>
        /// <returns></returns>
        private TransactionBlockInput CreateAddInput(ICallArg value)
        {
            // Get the index of of the legth of inputs, and use it as an index
            int index = this.BlockDataBuilder.Inputs.Count;
            TransactionBlockInput input = new TransactionBlockInput(
                index,
                value
            );
            this.BlockDataBuilder.Inputs.Add(input);
            return input;
        }

        /// <summary>
        /// Utility function with method signature for
        /// creating a `TransactionBlockInput` (input) with an objectID address.
        /// We explicitly define this method signature for readability,
        /// otherwise we can use a `TransactionBlockInput` method signature that
        /// takes in an `ISerializable` object
        /// NOTE: `AccountAddress` and `ICallArg` are both `ISerializable`.
        /// NOTE: The input created will later need to be resolved to an actual object definition.
        /// </summary>
        /// <param name="value">An objectId</param>
        /// <returns></returns>
        private TransactionBlockInput CreateAddInput(AccountAddress objectIdValue)
        {
            // Get the index of of the legth of inputs, and use it as an index
            int index = this.BlockDataBuilder.Inputs.Count;
            TransactionBlockInput input = new TransactionBlockInput(
                index,
                objectIdValue
            );
            this.BlockDataBuilder.Inputs.Add(input);
            return input;
        }

        /// <summary>
        /// Add a new object ref (`ImmOrOwned`) input to the transaction
        /// using the fully-resolved object reference.
        /// If you only have an object ID, use `builder.object(id)` instead.
        ///
        /// In the TypeScript SDK, this is:
        /// <code>
        ///     objectRef(...args: Parameters<(typeof Inputs)['ObjectRef']>) {
        /// </code>
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="version"></param>
        /// <param name="digest"></param>
        /// <returns></returns>
        public TransactionBlockInput AddObjectRefInput(string objectId,
            int version, string digest)
        {
            Sui.Types.SuiObjectRef objectRef = new Sui.Types.SuiObjectRef(
                objectId,
                version,
                digest
            );
            return this.AddObjectRefInput(objectRef);
        }

        /// <summary>
        /// Adds a Sui Object Ref (`ImmOrOwned`) to the inputs of
        /// a programmable transaction.
        /// </summary>
        /// <param name="objectRef"></param>
        /// <returns></returns>
        public TransactionBlockInput AddObjectRefInput(Sui.Types.SuiObjectRef objectRef)
        {
            ObjectCallArg newObjCallArg = new ObjectCallArg(objectRef);
            return this.AddObjectInput(newObjCallArg);
        }

        /// <summary>
        /// Add a new shared object input to the transaction using
        /// the fully-resolved shared object reference.
        ///
        /// If you only have an object ID, use `builder.object(id)` instead.
        /// </summary>
        /// <param name="objectId"></param>
        /// <param name="initialSharedVersion"></param>
        /// <param name="mutable"></param>
        /// <returns></returns>
        public TransactionBlockInput AddSharedObjectRefInput(string objectId,
            int initialSharedVersion, bool mutable)
        {
            SharedObjectRef sharedObjectRef = new SharedObjectRef(
                objectId,
                initialSharedVersion,
                mutable
            );
            return this.AddSharedObjectRefInput(sharedObjectRef);
        }

        /// <summary>
        /// Add a new shared object input to the programmable transaction.
        /// </summary>
        /// <param name="sharedObjectRef"></param>
        /// <returns></returns>
        public TransactionBlockInput AddSharedObjectRefInput(
            SharedObjectRef sharedObjectRef)
        {
            ObjectCallArg objectCallArg = new ObjectCallArg(sharedObjectRef);
            return this.AddObjectInput(objectCallArg);
        }

        /// <summary>
        /// Add a new non-object input to the transaction.
        /// </summary>
        /// <param name="value">
        ///     Can be a BString, Bytes, U8, U64, AccountAddress
        ///     The pure value that will be used as the input value.
        ///     If this is a Uint8Array, then the value is assumed
        ///     to be raw bytes, and will be used directly.
        /// </param>
        /// <returns></returns>
        public TransactionBlockInput AddPureInput(ISerializable value)
        {
            PureCallArg pureCallArg = new PureCallArg(value);
            return this.CreateAddInput(pureCallArg);
        }

        /// <summary>
        /// Adds a transaction to the programmable transaction block.
        /// </summary>
        /// <param name="transaction"></param>
        /// <param name="resultsLength"></param>
        /// <returns></returns>
        public List<TransactionResult> AddTx(ITransaction transaction, int resultsLength = 0)
        {
            this.Transactions.Add(transaction);
            int index = this.Transactions.Count;

            TransactionResult txResult =  this.CreateTransactionResult(index - 1);
            List<TransactionResult> txResults = new List<TransactionResult>();
            return txResults;
        }

        /// <summary>
        /// Creates a TransactionResult object for the given transaction index.
        /// </summary>
        /// <param name="index"></param>
        /// <returns></returns>
        public TransactionResult CreateTransactionResult(int index)
        {
            throw new NotImplementedException();
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
        /// </summary>
        /// <param name="coin">GasCoin is a type of `TransactionArgument`.</param>
        /// <param name="amounts">A list of respective amounts for each coin we are splitting.</param>
        /// <returns>A list of `TransactionResult`s.</returns>
        public List<TransactionResult> AddSplitCoinsTx(GasCoin coin, params ITransactionArgument[] amounts)
        {
            SplitCoins splitCoinsTx = new SplitCoins(coin, amounts);
            return this.AddTx(splitCoinsTx);

        }

        public TransactionBlock AddMergeCoinsTx()
        {
            return this;
        }

        public TransactionBlock AddPublishTx()
        {
            return this;
        }

        public TransactionBlock AddUpgradeTx()
        {
            return this;
        }

        /// <summary>
        /// <code>
        ///     const [nft1, nft2] = txb.moveCall({ target: "0x2::nft::mint_many" });
        ///     txb.transferObjects([nft1, nft2], txb.pure(address));
        ///
        /// const coin = tx.splitCoins(tx.gas, [tx.pure(amount)]);
        /// tx.moveCall({
        ///     target: `${SUI_SYSTEM_ADDRESS}::${SUI_SYSTEM_MODULE_NAME}::${ ADD_STAKE_FUN_NAME}`,
        ///     arguments:[tx.object(SUI_SYSTEM_STATE_OBJECT_ID), coin, tx.pure(validatorAddress)],
        /// })
        /// </code>
        /// </summary>
        /// <param name="target"></param>
        /// <param name="typeArguments"></param>
        /// <param name="arguments"></param>
        /// <returns></returns>
        public TransactionBlock AddMoveCallTx(SuiStructTag target,
            ISerializableTag[] typeArguments = null, ITransactionArgument[] arguments = null)
        {
            MoveCall moveCallTx = new MoveCall(target, typeArguments, arguments);
            return this;
        }

        public TransactionBlock AddTransferObjectsTx()
        {
            return this;
        }

        public TransactionBlock AddMoveVectTx()
        {
            return this;
        }

        private void GetConfig()
        {
            throw new NotImplementedException();
        }

        private void Validate()
        {
            throw new NotImplementedException();
        }

        public byte[] Build(Block.BuilidOptions options)
        {
            // Coroutine _prepare = new Coroutine(PrepareCor(options));
            // await _prepare;

            // int maxSizeBytes = this.getConfig("maxTxSizeBytes, options);
            // bool onlyTransactionKind = options.OnlyTransactionKin

            //return this.BlockDataBuilder.Build(maxSizeBytes, onlyTransactionKind);
            throw new NotImplementedException();
        }

        /// <summary>
        /// Queries RPC for reference gas price, then sets the price to the
        /// transaction builder.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public async Task PrepareGasPriceAsync(Block.BuilidOptions options)
        {
            if(options.OnlyTransactionKind || this.BlockDataBuilder.GasConfig.Price != null) {
                return;
            }

            RpcResult<BigInteger> referenceGasPrice = await options.Client.GetReferenceGasPriceAsync();
            BigInteger gasPrice = referenceGasPrice.Result;
            this.SetGasPrice(gasPrice);
        }

        public async Task PrepareTransactions(Block.BuilidOptions options)
        {
            List<TransactionBlockInput> inputs = this.BlockDataBuilder.Inputs;
            ITransaction[] transactions = this.BlockDataBuilder.Transactions;

            List<MoveCall> moveModulesToResolve = new List<MoveCall>();
            //List<ObjectToResolve> objectsToResolve = new List<ObjectToResolve>();

            foreach (TransactionBlockInput input in inputs)
            {
                if (input.Value.GetType() == typeof(string))
                {
                    ObjectToResolve objectToResolve = new ObjectToResolve(
                        ((BString)input.Value).ToString(),
                        input,
                        null
                    );
                    objectsToResolve.Add(objectToResolve);
                }
            }

            foreach(ITransaction transaction in transactions)
            {
                #region Process MoveCall Transaction
                // Special case move call:
                if (transaction.Kind == Kind.MoveCall)
                {
                    // Determine if any of the arguments require encoding.
                    // - If they don't, then this is good to go.
                    // - If they do, then we need to fetch the normalized move module.
                    MoveCall moveTx = (MoveCall)transaction;
                    ITransactionArgument[] arguments = moveTx.Arguments;

                    bool needsResolution = arguments.Any(arg => {
                        bool isInput = (arg.Kind == Types.Arguments.Kind.Input);
                        if(isInput)
                        {
                            TransactionBlockInput argInput = (TransactionBlockInput)arg;
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
                    if(needsResolution)
                        moveModulesToResolve.Add(moveTx);
                    continue;
                }
                #endregion
                else if(transaction.Kind == Kind.TransferObjects)
                {
                    TransferObjects transferObjectsTx = (TransferObjects)transaction;
                    ITransactionArgument address = transferObjectsTx.Address;

                    if (address.GetType() == typeof(TransactionBlockInput))
                    {
                        TransactionBlockInput addressInput = (TransactionBlockInput)address;
                        TransactionBlockInput input = inputs[addressInput.Index];

                        if (input.Value.GetType() != typeof(IObjectRef))
                        {
                            this.BlockDataBuilder.Inputs[addressInput.Index].Value = new PureCallArg(input.Value); ;
                        }
                    }
                }
                else if(transaction.Kind == Kind.SplitCoins)
                {
                    SplitCoins splitCoinsTx = (SplitCoins)transaction;
                    ITransactionArgument[] amounts = splitCoinsTx.Amounts;
                    foreach(ITransactionArgument amount in amounts)
                    {
                        if(amount.GetType() == typeof(TransactionBlockInput))
                        {
                            TransactionBlockInput amountInput = (TransactionBlockInput)amount;
                            TransactionBlockInput input = inputs[amountInput.Index];

                            if(input.Value.GetType() != typeof(IObjectRef))
                            {
                                this.BlockDataBuilder.Inputs[amountInput.Index].Value = new PureCallArg(input.Value); ;
                            }
                        }
                    }
                }
                // TODO: IRVIN, continue implementation
            }

            if (moveModulesToResolve.Count > 0)
            {
                foreach(MoveCall moveCall in moveModulesToResolve)
                {
                    string packageId = moveCall.Target.address.ToHex();
                    string moduleName = moveCall.Target.module;
                    string functionName = moveCall.Target.name;

                    // RPC Call
                    RpcResult<NormalizedMoveFunctionResponse> result
                        = await options.Client.GetNormalizedMoveFunction(packageId, moduleName, functionName);
                    NormalizedMoveFunctionResponse normalized = result.Result;

                    // Entry functions can have a mutable reference to an instance of the TxContext
                    // struct defined in the TxContext module as the last parameter. The caller of
                    // the function does not need to pass it in as an argument.

                    bool hasTxContext = normalized.Parameters.Count > 0
                        && normalized.Parameters.Last() as SuiMoveNormalizedTypeString != null
                        && IsTxContext(new SuiStructTag(((SuiMoveNormalizedTypeString)normalized.Parameters.Last()).Value));

                    List<ISuiMoveNormalizedType> moduleParams = (List<ISuiMoveNormalizedType>)(hasTxContext ? normalized.Parameters.Take(normalized.Parameters.Count - 1) : normalized.Parameters);

                    if(moduleParams.Count != moveCall.Arguments.Length)
                    {
                        throw new Exception("Incorrect number of arguments.");
                    }

                    for(int i = 0; i < moduleParams.Count; i++)
                    {
                        ISuiMoveNormalizedType param = moduleParams[i];

                        ITransactionArgument arg = moveCall.Arguments[i];
                        if(arg.Kind != Types.Arguments.Kind.Input) continue;

                        TransactionBlockInput inputArg = (TransactionBlockInput)arg;

                        TransactionBlockInput input = inputs[inputArg.Index];
                        // Skip if the input is already resolved
                        if (input.Value.GetType() == typeof(ICallArg)) continue;

                        ISerializable inputValue = input.Value;

                        //Type t = Type.GetType(param); // for reference
                        //Convert.ChangeType(value1, intType); // for reference

                        //ICallArg inputValue = input.Value;
                        //// Check if param received from RPC is Pure serializable
                        //Serialization ser = new Serialization();
                        //input.Value.Serialize(ser);
                        ////input.Value = new Bytes(ser.GetBytes());
                        ///

                        string serType = Serializer.GetPureNormalizedType(param, inputValue);

                        if (serType != null)
                        {
                            this.BlockDataBuilder.Inputs[inputArg.Index].Value = new PureCallArg(inputValue);
                            continue;
                        }

                        ISuiMoveNormalizedType structVal = Serializer.ExtractStructType(param);

                        if (structVal != null || param as SuiMoveNormalziedTypeParameterType != null)
                        {
                            if (inputValue.GetType() != typeof(string))
                                throw new Exception($"Expect the argument to be an object id string, got {inputValue}");

                            SuiMoveNormalizedTypeString inputString = (SuiMoveNormalizedTypeString)inputValue;

                            ObjectToResolve objectToResolve = new ObjectToResolve(
                                inputString.Value,
                                input,
                                param
                            );

                            objectsToResolve.Add(objectToResolve);

                            continue;
                        }

                        throw new Exception($"Unknown call arg type {param} for value {inputValue}");
                    }
                }
            }

            if (objectsToResolve.Count != 0)
            {

            }
        }

        /// <summary>
        /// Encodes a `TransactionBlockInput` input.
        /// Meaning that if the `value` inside the `TransactionBlockInput`
        /// is simply an objectID (`AccountAddress`) then we need to resolve it.
        /// Otherwise, if it's a "pure", then we create a `TransactionBlockInput`
        /// with a "pure" value.
        /// </summary>
        /// <param name="index"></param>
        private void EncodeInput(int index)
        {
            TransactionBlockInput input = BlockDataBuilder.Inputs[index];

            Type type = input.Value.GetType();

            // If the value of the `TransactionBlockInput` input is already a `BuilderCallArg`
            if (type == typeof(ICallArg)) return;

            if(type == typeof(AccountAddress))
            {
                // TODO: Figure out porting logic from TypeScript where they pass `input` by reference
                // <code> input.value = Inputs.Pure(input.value, wellKnownEncoding.type); </code>
            } else if(type == typeof(Bytes))// else if (wellKnownEncoding.kind === 'pure') 
            {
                //input.Value = input.Value.Serialize
            }
            else
            {

            }
        }

        private class ObjectToResolve
        {
            string id { get; set; }
            TransactionBlockInput input { get; set; }
            ISuiMoveNormalizedType normalizedType;

            public ObjectToResolve(string id, TransactionBlockInput input, ISuiMoveNormalizedType normalizedType) 
            {
                this.id = id;
                this.input = input;
                this.normalizedType = normalizedType;
            }
        }

        private string GetPureSeralizationTYpe(string normalizedType, ISerializable argVal)
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

        public IEnumerator PrepareCor(Block.BuilidOptions options)
        {
            if(!options.OnlyTransactionKind && this.BlockDataBuilder.Sender != null)
            {
                throw new ArgumentException("Missing transaction sender.");
            }

            //const client = options.client || options.provider;
            bool client = true;

            // TODO: Fix the limits arg
            if(options.ProtocolConfigArg == null && options.LimitsArg == null && client)
            {
                // TODO: RPC Call to get protocol config
                //options.ProtocolConfigArg = await client.getProtocolConfig()
            }

            yield return true;
        }

        public void Serialize(Serialization serializer)
        {
            throw new System.NotImplementedException();
        }

        /// VARIABLES
        // blockData: TransactionBlockDataBuilder
        // defaultOfflineLimits: [String: Int]

        // INITIALIZER
        // init(blockData: TransactionBlockDataBuilder?)

        /// SENDER FUNCTIONS
        // setSender(sender: String)
        // setSenderIfNotSet(sender: String) throws
        // setExpiration(expiration: TransactionExpiration)

        /// GAS FUNCTIONS
        // setGasPrice(price: Int)
        // setGasBudget(budget: Int)
        // setGasOwner(owner: String)
        // setGasPayment(payments: [SuiObjectRef])
        // gas: TransactionArgument

        /// INPUT FUNCTIONS
        // input(type: ValueType, value: SuiJsonValue?) throws -> TransactionBlockInput
        // object(value: String) throws -> TransactionBlockInput
        // object(value: ObjectArg) throws -> TransactionBlockInput
        // objectRef(objectArg: ObjectArg) throws -> [TransactionBlockInput]
        // shredObjectRef(sharedObjectRef: SharedObjectRef) throws -> [TransactionBlockInput]
        // pure(value: SuiJsonValue) throws -> TransactionArgument
        // add(transaction: SuiTransaction) throws -> TransactionArgument

        /// TRANSACTION FUNCTIONS
        // splitCoin(coin: TransactionArgument, amounts: [TransactionBlockInput]) throws -> TransactionArgument
        // mergeCoin(destination: TransactionBlockInput, sources: [TransactionBlockInput]) throws -> TransactionArgument
        // publish(modules: [Data], dependecies: [String]) throws -> TransactionArgument
        // publish(modules: [String], dependencies: [String]) throws -> TransactionArgument
        // upgrade(modules: [Data], dependencies: [String], packageId: String, ticket: TransactionArgument) throws -> TransactionArgument
        // moveCall(target: String, arguments: [TransactionArgument]? = nil, typeArguments: [String]? = nil) throws -> TransactionArgument
        // transferObject(objects: [TransactionArgument], address: String) throws -> TransactionArgument
        // makeMoveVec(type: String? = nil, objects: [TransactionBlockInput]) throws -> TransactionArgument

        /// TRANSACTION BUILDER TOOL FUNCTIONS
        // getConfig(key: LimitKey, buildOptions: BuildOptions) throws -> Int
        // build(_ provider: SuiProvider, _ onlyTransactionKind: Bool? = nil) async throws -> Data
        // getDigest(_ provider: SuiProvider) async throws -> String
        // isMissingSender(_ onlyTransactionKind: Bool? = nil) -> Bool

        /// TRANSACTION PREPARATION FUNCTIONS
        // prepareGasPayment(provider: SuiProvider, onlyTransactionKind: Bool? = nil) async throws
        // prepareGasPrice(provider: SuiProvider, onlyTransactionKind: Bool? = nil) async throws
        // prepareTransactions(provider: SuiProvider) async throws

        /// CORE PREPARE FUNCTION
        // prepare(_ optionsPassed: BuildOptions) async throws
    }
}