//
//  TransactionResult.cs
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

using System;
using System.Collections;
using System.Collections.Generic;

namespace Sui.Transactions
{
    /// <summary>
    /// Used for generating a nested result or a regular result for a given index.
    /// Main case is for adding inputs to the transaction block builder's input.
    /// </summary>
    public class TransactionResult : IEnumerable<TransactionArgument>, IEnumerator<TransactionArgument>
    {
        /// <summary>
        /// The argument result to either nest or not.
        /// </summary>
        public TransactionArgument TransactionArgument { get; set; }

        /// <summary>
        /// The current position within the nest of results.
        /// Set to -1 to count for non-nested results.
        /// </summary>
        private int _position = -1;

        /// <summary>
        /// The amount of results that are nested.
        /// </summary>
        private ushort _count;

        /// <summary>
        /// The list of nested results.
        /// </summary>
        private List<TransactionArgument> _nested_results;

        /// <summary>
        /// The current result this enumerable is currently on.
        /// </summary>
        public TransactionArgument Current
        {
            get => this.NestedResultFor((ushort)(this._count - this._position - 1));
        }

        object IEnumerator.Current => this.Current;

        // TODO: Look into whether we need to implement this in the future or not.
        public void Dispose()
            => throw new NotImplementedException();

        public IEnumerator<TransactionArgument> GetEnumerator() => this;

        public bool MoveNext()
        {
            this._position++;
            return this._position < this._count;
        }

        public void Reset() => this._position = -1;

        public TransactionResult(ushort index, ushort? amount = null)
        {
            this.TransactionArgument = new TransactionArgument(TransactionArgumentKind.Result, new Result(index));
            this._count = amount ?? 1;
            this._nested_results = new List<TransactionArgument>();
        }

        /// <summary>
        /// Retrieve the nested result for a given set of results.
        /// </summary>
        /// <param name="result_index">The index of the result to iterate through.</param>
        /// <returns>A `TransactionArgument` representing either a `Result` or `NestedResult`.</returns>
        public TransactionArgument NestedResultFor(ushort result_index)
        {
            int index = result_index + 1;

            if (index < this._nested_results.Count)
            {
                return this._nested_results[index];
            }

            if (this.TransactionArgument.Kind == TransactionArgumentKind.Result)
            {
                int result = ((Result)this.TransactionArgument.Argument).Index;

                NestedResult nested_result = new NestedResult(result, result_index);
                TransactionArgument argument = new TransactionArgument(TransactionArgumentKind.NestedResult, nested_result);

                this._nested_results.Add(argument);

                return argument;
            }

            return null;
        }

        IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();
    }
}