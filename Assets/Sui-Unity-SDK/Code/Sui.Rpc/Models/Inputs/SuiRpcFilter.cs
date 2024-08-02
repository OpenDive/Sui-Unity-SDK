//
//  SuiRpcFilter.cs
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
    /// Used for filtering results with the RPC client.
    /// </summary>
    public class SuiRpcFilter
    {
        /// <summary>
        /// An optional paging cursor. If provided, the query will start from
        /// the next item after the specified cursor. Default to start from the
        /// first item if not specified.
        /// </summary>
        public string Cursor { get; set; }

        /// <summary>
        /// Maximum item returned per page, default to
        /// [QUERY_MAX_RESULT_LIMIT] if not specified.
        /// </summary>
        public int? Limit { get; set; }

        /// <summary>
        /// Query result ordering, default to false
        /// (ascending order, SortOrder.Ascending), oldest record first.
        /// </summary>
        public SortOrder? Order { get; set; }

        public SuiRpcFilter
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null
        )
        {
            this.Cursor = cursor;
            this.Limit = limit;
            this.Order = order;
        }
    }
}

