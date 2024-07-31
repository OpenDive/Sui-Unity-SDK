//
//  Methods.cs
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

namespace Sui.Clients.Api
{
    /// <summary>
    /// Sui RPC methods
    /// </summary>
    public enum Methods
    {
        #region Coin Query API

        suix_getAllBalances,
        suix_getAllCoins,
        suix_getBalance,
        suix_getCoinMetadata,
        suix_getCoins,
        suix_getTotalSupply,

        #endregion

        #region Extended API

        suix_getDynamicFieldObject,
        suix_getDynamicFields,
        suix_getOwnedObjects,
        suix_queryEvents,
        suix_queryTransactionBlocks,
        suix_resolveNameServiceAddress,
        suix_resolveNameServiceNames,
        suix_subscribeEvent,
        suix_subscribeTransaction,

        #endregion

        #region Governance Read API

        suix_getCommitteeInfo,
        suix_getLatestSuiSystemState,
        suix_getReferenceGasPrice,
        suix_getStakes,
        suix_getStakesByIds,
        suix_getValidatorsApy,

        #endregion

        #region Move Utils

        sui_getMoveFunctionArgTypes,
        sui_getNormalizedMoveFunction,
        sui_getNormalizedMoveModule,
        sui_getNormalizedMoveModulesByPackage,
        sui_getNormalizedMoveStruct,

        #endregion

        #region Read API

        sui_getChainIdentifier,
        sui_getCheckpoint,
        sui_getCheckpoints,
        sui_getEvents,
        sui_getLatestCheckpointSequenceNumber,
        sui_getObject,
        sui_getProtocolConfig,
        sui_getTotalTransactionBlocks,
        sui_getTransactionBlock,
        sui_multiGetObjects,
        sui_multiGetTransactionBlocks,
        sui_tryGetPastObject,
        sui_tryMultiGetPastObjects,

        #endregion

        #region Write API

        sui_devInspectTransactionBlock,
        sui_dryRunTransactionBlock,
        sui_executeTransactionBlock

        #endregion
    }
}