﻿//
//  ATransactionBlockQuery.cs
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

namespace Sui.Rpc.Models
{
    /// <summary>
    /// The query object used for any transaction block endpoints.
    /// </summary>
    public abstract class ATransactionBlockQuery : SuiRpcFilter
    {
        /// <summary>
        /// The options for what the return transaction block responses will send. 
        /// </summary>
        public TransactionBlockResponseOptions TransactionBlockResponseOptions { get; set; }

        public ATransactionBlockQuery
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null,
            TransactionBlockResponseOptions transaction_block_response_options = null
        ) : base(cursor, limit, order)
        {
            this.TransactionBlockResponseOptions = transaction_block_response_options;
        }
    }
}