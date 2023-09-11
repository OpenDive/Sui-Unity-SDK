using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.BCS;
using Sui.Transactions.Builder;

namespace Sui.Transactions
{
    /// <summary>
    /// A transaction block builder
    /// </summary>
    public class TransactionBlock : ISerializable
    {
        public TransactionBlockDataBuilder txBlockDataBuilder { get; set; }

        public TransactionBlock(TransactionBlock transactionBlock = null)
        {
            if (transactionBlock != null)
                txBlockDataBuilder = transactionBlock.txBlockDataBuilder;
            else
                txBlockDataBuilder = new TransactionBlockDataBuilder();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <returns></returns>
        public TransactionBlock SetSender(AccountAddress sender)
        {
            this.txBlockDataBuilder.Sender = sender;
            return this;
        }

        public TransactionBlock SetSenderIfNotSet(AccountAddress sender)
        {
            if (this.txBlockDataBuilder.Sender == null)
                return this.SetSender(sender);
            return this;
        }

        public TransactionBlock SetExpiration(TransactionExpiration expiration)
        {
            this.txBlockDataBuilder.Expiration = expiration;
            return this;
        }

        /// <summary>
        /// Sets the gas price.
        /// </summary>
        /// <param name="price"></param>
        /// <returns></returns>
        public TransactionBlock SetGasPrice(int price)
        {
            this.txBlockDataBuilder.GasConfig.Price = price;
            return this;
        }

        public TransactionBlock SetGasBudget(int budget)
        {
            this.txBlockDataBuilder.GasConfig.Budget = budget;
            return this;
        }

        public TransactionBlock SetGasOwner(AccountAddress owner)
        {
            this.txBlockDataBuilder.GasConfig.Owner = owner;
            return this;
        }

        public TransactionBlock SetGasPayment(SuiObjectRef[] payments)
        {
            this.txBlockDataBuilder.GasConfig.Payment = payments;
            return this;
        }

        public TransactionBlockData GetBlockData()
        {
            return this.txBlockDataBuilder.Snapshot();
        }

        public TransactionArgument GetGas()
        {
            //get gas(): TransactionArgument {
            //    return { kind: 'GasCoin' };
            //}
            throw new NotImplementedException();
        }

        ///**
        // * Dynamically create a new input, which is separate from the `input`. This is important
        // * for generated clients to be able to define unique inputs that are non-overlapping with the
        // * defined inputs.
        // *
        // * For `Uint8Array` type automatically convert the input into a `Pure` CallArg, since this
        // * is the format required for custom serialization.
        // *
        // */
        //# input(type: 'object' | 'pure', value?: unknown) {
        //        const index = this.#blockData.inputs.length;
		      //  const input = create(
			     //   {
				    //    kind: 'Input',
        //                // bigints can't be serialized to JSON, so just string-convert them here:
        //                value: typeof value === 'bigint' ? String(value) : value,
        //                index,
        //                type,

        //            },
			     //   TransactionBlockInput,
		      //  );

        //        this.#blockData.inputs.push(input);
		      //  return input;

        //    }

        public TransactionBlock AddObject()
        {
            return this;
        }

        public TransactionBlock AddObjectRef()
        {
            return this;
        }

        public TransactionBlock AddSharedObjectRef()
        {
            return this;
        }

        public TransactionBlock AddPure()
        {
            return this;
        }

        public TransactionBlock Add()
        {
            return this;
        }

        /// <summary>
        /// Add a SplitCoins transaction.
        /// </summary>
        /// <returns></returns>
        public TransactionBlock AddSplitCoins()
        {
            return this;
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