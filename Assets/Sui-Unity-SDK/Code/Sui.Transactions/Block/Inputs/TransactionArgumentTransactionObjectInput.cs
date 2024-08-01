﻿//
//  TransactionArgumentTransactionObjectInput.cs
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

namespace Sui.Transactions
{
    /// <summary>
    /// An object input that is represented as a transaction argument
    /// (e.g., gas coin, block input, result, nested result).
    /// </summary>
    public class TransactionArgumentTransactionObjectInput : ITransactionObjectInput
    {
        public TransactionObjectInputType Type => TransactionObjectInputType.TransactionObjectArgument;

        /// <summary>
        /// The object input represented as a transaction argument.
        /// </summary>
        public TransactionArgument Input;

        public TransactionArgumentTransactionObjectInput(TransactionArgument input)
        {
            this.Input = input;
        }
    }
}