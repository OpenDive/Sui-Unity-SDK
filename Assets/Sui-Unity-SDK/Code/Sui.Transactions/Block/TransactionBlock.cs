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

                    return objectIdValue == _objectRef.ObjectId;
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
            AccountAddress newObjectId = objectCallArg.ObjectArg.ObjectId;

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
        public TransactionBlockInput AddObjectRefInput(AccountAddress objectId,
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
        public TransactionBlockInput AddSharedObjectRefInput(AccountAddress objectId,
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

        /// <summary>
        /// Resolves all required Move modules and objects.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task PrepareTransactions(Block.BuilidOptions options)
        {
            // The inputs in the `TransactionBlock`
            List<TransactionBlockInput> inputs = this.BlockDataBuilder.Inputs;
            // The transactions in the `TransactionBlock`
            ITransaction[] transactions = this.BlockDataBuilder.Transactions;

            // A list of move modules identified as needing to be resolved
            List<MoveCall> moveModulesToResolve = new List<MoveCall>();
            // A list of object identified as needing to be resolved
            List<ObjectToResolve> objectsToResolve = new List<ObjectToResolve>();

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
                    //continue; // TODO: Review this implementation
                }
                #endregion Process MoveCall Transaction

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
                            this.BlockDataBuilder.Inputs[addressInput.Index].Value = new PureCallArg(input.Value);
                        }
                    }
                }
                #endregion Process TransferObjects Transaction

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
                                this.BlockDataBuilder.Inputs[amountTxbInput.Index].Value = new PureCallArg(input.Value); ;
                            }
                        }
                    }
                }
                #endregion Process SplitCoins Transaction
            }
            #endregion Process all transactions 

            #region Resolve Move modules
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

                    // The list of parameters returned by the RPC call
                    List<ISuiMoveNormalizedType> paramsList = (List<ISuiMoveNormalizedType>)(hasTxContext
                        ? normalized.Parameters.Take(normalized.Parameters.Count - 1)
                        : normalized.Parameters);

                    if(paramsList.Count != moveCall.Arguments.Length)
                    {
                        // TODO: Irvin fix this -- we cannot throw an exception
                        throw new ArgumentException("Incorrect number of arguments.");
                    }

                    for(int i = 0; i < paramsList.Count; i++)
                    {
                        ISuiMoveNormalizedType param = paramsList[i];

                        ITransactionArgument arg = moveCall.Arguments[i];
                        if(arg.Kind != Types.Arguments.Kind.Input) continue;

                        TransactionBlockInput inputArg = (TransactionBlockInput)arg;

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

                        string serType = Serializer.GetPureNormalizedType(param, inputValue);
                        if (serType != null)
                        {
                            // TODO: IRVIN update this to use a clone of the input list
                            inputs[inputArg.Index].Value = new PureCallArg(inputValue);
                            continue;
                        }

                        //Type serType = Serializer.GetPureNormalizedTypeType(param, inputValue);

                        // TODO: NOTE IRVIN -- All this `GetPureNormalizedTypeType` function does is "verify"
                        // TODO:    that the input value matches the type that the MoveCall expects.
                        // TODO: HENCE we don't really need to return anything, all we have to do is just check that the type of the input value
                        // TODO: matches what is expected, if doesn't match then we return false, and break / end the program.
                        // TODO: We don't need a "serType" because we are already passing concrete types such as:
                        // TODO:    `AccountAddress` or `U8` or `Bytes` for byte arrays, of Sequence for vectors

                        // TODO: NOW NOTE THAT -- for structs it's trickier because the MoveCall is expecting an object, and in the C# side
                        // TODO:    We can only work with class / objects, hence we just have to do a comparison of the properties of the expected object


                        //bool iSPureNormalizedType = Serializer.MatchesPureNormalizedType(param, inputValue);
                        // if(iSPureNormalizedType) { 
                        //this.BlockDataBuilder.Inputs[inputArg.Index].Value = new PureCallArg(inputValue);
                        // }


                        ISuiMoveNormalizedType structVal = Serializer.ExtractStructType(param);

                        if (structVal != null || param as SuiMoveNormalziedTypeParameterType != null)
                        {
                            if (inputValue.GetType() != typeof(AccountAddress))
                                throw new Exception($"Expect the argument to be an object id string, got {inputValue.GetType()}");

                            //SuiMoveNormalizedTypeString inputString = (SuiMoveNormalizedTypeString)inputValue;
                            //ObjectToResolve objectToResolve = new ObjectToResolve(
                            //    inputString.Value,
                            //    input,
                            //    param
                            //);

                            ObjectToResolve objectToResolve = new ObjectToResolve(
                                (AccountAddress)inputValue,
                                input,
                                param
                            );

                            objectsToResolve.Add(objectToResolve);

                            continue;
                        }

                        throw new Exception($"Unknown call arg type {param} for value {inputValue.GetType()}");
                    }
                }
            }
            #endregion Resolve MoveModules

            if (objectsToResolve.Count != 0)
            {
                List<AccountAddress> mappedIds = (List<AccountAddress>)objectsToResolve.Select(x => x.Id);
                List<AccountAddress> dedupedIds = new HashSet<AccountAddress>(mappedIds).ToList(); // NOTE: Insertion order in HashSet will be maintain until removing or re-adding

                // TODO: In the TypeScript SDK they use `Set` which is a set that maintains insertion order
                // TODO: Find data structure that does this in C#
                // https://gist.github.com/gmamaladze/3d60c127025c991a087e

                List<List<AccountAddress>> objectChunks = Chunk(dedupedIds, 50);

                List<List<ObjectDataResponse>> objectsResponse = new List<List<ObjectDataResponse>>();
                foreach(List<AccountAddress> objectIds in objectChunks)
                {
                    ObjectDataOptions optionsObj = new ObjectDataOptions();
                    optionsObj.ShowOwner = true;

                    RpcResult<IEnumerable<ObjectDataResponse>> response = await options.Client.MultiGetObjects(objectIds.ToArray(), optionsObj);
                    List<ObjectDataResponse> objects = (List<ObjectDataResponse>)response.Result;
                    objectsResponse.Add(objects);
                }
                // Flatten responses
                List<ObjectDataResponse> objectsFlatten = objectsResponse.SelectMany(x => x).ToList();

                Dictionary<AccountAddress, ObjectDataResponse> objectsById = new Dictionary<AccountAddress, ObjectDataResponse>();
                for(int i = 0; i < dedupedIds.Count; i++)
                {
                    AccountAddress id = dedupedIds[i];
                    ObjectDataResponse obj = objectsFlatten[i];
                    objectsById.Add(id, obj);
                }

                // TODO Check for invalid objects / objects with errors
                // TODO: Identify how to get error from object response -- talk to Marcus
                //List<ObjectDataResponse> invalidObjects = objectsById.Values.ToList().Where(obj => obj.Error);// TODO: Identify how to get error from object response
                //if(invalidObjects.Count > 0)
                //{
                //    throw new Exception("The following input objects are invalid: {}");
                //}

                foreach (ObjectToResolve objectToResolve in objectsToResolve)
                {
                    ObjectDataResponse obj = objectsById[objectToResolve.Id];
                    //AccountAddress owner = obj.Data.Owner; // could be an object
                    Owner owner = obj.Data.Owner;

                    int? initialSharedVersion = owner.Shared.InitialSharedVersion;

                    if (initialSharedVersion != null)
                    {
                        bool mutable = InputsHandler.isMutableSharedObjectInput((ICallArg)objectToResolve.Input.Value);
                        inputs[objectToResolve.Input.Index].Value = new SharedObjectRef(objectToResolve.Id, (int)initialSharedVersion, mutable);
                    }
                    else if (objectToResolve.NormalizedType != null)
                    {
                        // TODO: Implement Receiving Type casting
                    }
                    else
                    {
                        //inputs[objectToResolve.Input.Index].Value = new SuiObjectRef(); TODO: Implement GetObjectReference function
                    }
                }
            }
        }

        /// <summary>
        /// Chuck the list of duduped ids so that it can be used in the RPC call for multi objects.
        /// </summary>
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