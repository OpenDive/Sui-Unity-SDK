//
//  ObjectArgumentType.cs
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

using Sui.Rpc.Models;

namespace Sui.Transactions
{
    /// <summary>
    /// When building a transaction, this is used to
    /// track objects that are needing to be resolved
    /// (e.g., given an object type).
    /// </summary>
    public class ObjectToResolve
    {
        /// <summary>
        /// The object's ID.
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// The input corresponding to the object.
        /// </summary>
        public TransactionBlockInput Input { get; set; }

        /// <summary>
        /// The object's type.
        /// </summary>
        public SuiMoveNormalizedType NormalizedType { get; set; }

        public ObjectToResolve
        (
            string id,
            TransactionBlockInput input,
            SuiMoveNormalizedType normalized_type
        )
        {
            this.ID = id;
            this.Input = input;
            this.NormalizedType = normalized_type;
        }
    }
}