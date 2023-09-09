namespace Sui.Transactions
{
    public class TransactionBlock
    {
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