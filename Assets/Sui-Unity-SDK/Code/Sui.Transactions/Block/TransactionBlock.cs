using System;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;
using Sui.Transactions.Builder;
using Sui.Transactions.Types;
using Sui.Transactions.Types.Arguments;

namespace Sui.Transactions
{
    /// <summary>
    /// A transaction block builder
    /// </summary>
    public class TransactionBlock : ISerializable
    {
        public TransactionBlockDataBuilder TxBlockDataBuilder { get; set; }

        /// <summary>
        /// The list of transaction the "transaction builder" will use to create
        /// the transaction block. This can be any transaction type defined
        /// in the `ITransaction` interface, e.g. `MoveCall`, `SplitCoins`.
        /// </summary>
        public List<ITransaction> Transactions { get; set; }

        public TransactionBlock(TransactionBlock transactionBlock = null)
        {
            if (transactionBlock != null)
                TxBlockDataBuilder = transactionBlock.TxBlockDataBuilder;
            else
                TxBlockDataBuilder = new TransactionBlockDataBuilder();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public TransactionBlock SetSender(AccountAddress sender)
        {
            this.TxBlockDataBuilder.Sender = sender;
            return this;
        }

        public TransactionBlock SetSenderIfNotSet(AccountAddress sender)
        {
            if (this.TxBlockDataBuilder.Sender == null)
                return this.SetSender(sender);
            return this;
        }

        public TransactionBlock SetExpiration(TransactionExpiration expiration)
        {
            this.TxBlockDataBuilder.Expiration = expiration;
            return this;
        }

        /// <summary>
        /// Sets the gas price.
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public TransactionBlock SetGasPrice(int price)
        {
            this.TxBlockDataBuilder.GasConfig.Price = price;
            return this;
        }

        public TransactionBlock SetGasBudget(int budget)
        {
            this.TxBlockDataBuilder.GasConfig.Budget = budget;
            return this;
        }

        public TransactionBlock SetGasOwner(AccountAddress owner)
        {
            this.TxBlockDataBuilder.GasConfig.Owner = owner;
            return this;
        }

        public TransactionBlock SetGasPayment(SuiObjectRef[] payments)
        {
            this.TxBlockDataBuilder.GasConfig.Payment = payments;
            return this;
        }

        public TransactionBlockData GetBlockData()
        {
            return this.TxBlockDataBuilder.Snapshot();
        }

        // Should either be an ITransactionArgument, or simply a GasCoin object
        // which is an ITransactionArgument
        //public TransactionArgument GetGas()
        //{
        //    //get gas(): TransactionArgument {
        //    //    return { kind: 'GasCoin' };
        //    //}
        //    throw new NotImplementedException();
        //}



        /// <summary>
        /// Add a new object input to the transaction.
        /// In the TypeScript SDK this is: `object(value: string | ObjectCallArg)`
        /// <code>
        ///     const coins = await toolbox.getGasObjectsOwnedByAddress();
        ///     const tx = new TransactionBlock();
        ///     const coin_0 = coins[0].data as SuiObjectData;
        /// 
        ///     const coin = tx.splitCoins(tx.object(coin_0.objectId), [tx.pure(DEFAULT_GAS_BUDGET * 2)]);
        ///     tx.transferObjects([coin], tx.pure(toolbox.address()));
        /// </code>
        /// </summary>
        /// <returns></returns>
        public TransactionBlockInput AddObjectInput(string objectId)
        {
            // https://github.com/MystenLabs/sui/blob/main/sdk/typescript/src/builder/Inputs.ts#L65
            PureCallArg pureCallArg = new PureCallArg(new BString(objectId));
            // return normalizeSuiAddress(objectId);
            throw new NotImplementedException();
        }

        public TransactionBlockInput AddObjectInput(IObjectRef objectRef)
        {
            System.Type objectType = objectRef.GetType();
            string newObjectId = "";
            if (objectType == typeof(SuiObjectRef))
            {
                SuiObjectRef ImmObjectRef =  (SuiObjectRef)objectRef;
                newObjectId = ImmObjectRef.ObjectId;
            }
            else
            {
                SharedObjectRef sharedObjectRef = (SharedObjectRef)objectRef;
                newObjectId = sharedObjectRef.ObjectId;
            }

            // Create ObjectCallArg which will add the appropriate byte when serializing
            ObjectCallArg newObjCallArg = new ObjectCallArg(objectRef);
            TransactionBlockInput newTxBlockInput = new TransactionBlockInput(0, null);

            List<TransactionBlockInput> inputs = TxBlockDataBuilder.Inputs;

            TransactionBlockInput inserted = inputs.Find((blockInput) => {
                Type blockInputValueType = blockInput.Value.GetType();
                if (blockInputValueType == typeof(ObjectCallArg))
                {
                    ObjectCallArg _objCallArg = (ObjectCallArg)blockInput.Value;
                    IObjectRef _objectRef = _objCallArg.ObjectArg;

                    return newObjectId == _objectRef.ObjectId;
                }
                return false;
            });

            if (inserted != null)
                return inserted;

            return AddCreateInput(newObjCallArg);

            throw new NotImplementedException();
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
        private TransactionBlockInput AddCreateInput(ICallArg value)
        {
            int index = this.TxBlockDataBuilder.Inputs.Count;

            TransactionBlockInput input = new TransactionBlockInput(index, value);
            this.TxBlockDataBuilder.Inputs.Add(input);
            return input;
        }

        public TransactionBlock AddObjectRef()
        {
            return this;
        }

        public TransactionBlock AddSharedObjectRef()
        {
            return this;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">Can be a BString, Bytes, U8, U64, AccountAddress
        /// The pure value that will be used as the input value. If this is a Uint8Array, then the value
        /// is assumed to be raw bytes, and will be used directly.
        /// </param>
        /// <returns></returns>
        public TransactionBlockInput AddPureInput(ISerializable value)
        {
            PureCallArg pureCallArg = new PureCallArg(value);
            return this.AddCreateInput(pureCallArg);
        }

        public List<TransactionResult> AddTx(ITransaction transaction, int resultsLength = 0)
        {
            Transactions.Add(transaction);
            List<TransactionResult> txResults = new List<TransactionResult>();
            int index = txResults.Count;
            return txResults;
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
        /// </code>
        /// </summary>
        /// <param name="coin">GasCoin is a type of `TransactionArgument`.</param>
        /// <param name="amounts">A list of respective amounts for each coin we are splitting.</param>
        /// <returns>A list of `TransactionResult`s.</returns>
        public List<TransactionResult> AddSplitCoinsTx(GasCoin coin, params ulong[] amounts)
        {
            SplitCoins splitCoinsTx = new SplitCoins(coin, amounts);
            return this.AddTx(splitCoinsTx);

        }

        public TransactionBlock AddMergeCoins()
        {
            return this;
        }

        public TransactionBlock AddPublish()
        {
            return this;
        }

        public TransactionBlock AddUpgrade()
        {
            return this;
        }

        public TransactionBlock AddMoveCall()
        {
            return this;
        }

        public TransactionBlock AddTransferObjects()
        {
            return this;
        }

        public TransactionBlock AddMoveVect()
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