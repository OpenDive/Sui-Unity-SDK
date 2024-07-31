//
//  ObjectQuery.cs
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
    /// The query object used for object endpoints.
    /// </summary>
    public class ObjectQuery : SuiRpcFilter
    {
        /// <summary>
        /// The options for what the return object will send.
        /// </summary>
        public ObjectDataOptions ObjectDataOptions { get; set; }

        /// <summary>
        /// The filter options for querying object.
        /// </summary>
        public IObjectDataFilter ObjectDataFilter { get; set; }

        public ObjectQuery
        (
            string cursor = null,
            int? limit = null,
            SortOrder? order = null,
            ObjectDataOptions object_data_options = null,
            IObjectDataFilter object_data_filter = null
        ) : base(cursor, limit, order)
        {
            this.ObjectDataOptions = object_data_options;
            this.ObjectDataFilter = object_data_filter;
        }
    }
}