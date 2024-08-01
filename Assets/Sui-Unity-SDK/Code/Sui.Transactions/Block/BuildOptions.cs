//
//  BuildOptions.cs
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

using Sui.Rpc.Client;
using Sui.Rpc.Models;
using System.Collections.Generic;

namespace Sui.Transactions
{
    /// <summary>
    /// Represents the various configuration options to use while building a transaction block.
    /// </summary>
    public class BuildOptions
    {
        /// <summary>
        /// The client used.
        /// </summary>
        public SuiClient Provider;

        /// <summary>
        /// A boolean representing whether or not the build outputs only the Transaction Kind
        /// (e.g., build as a Programmable Transaction only).
        /// </summary>
        public bool? OnlyTransactionKind;

        /// <summary>
        /// A dictionary representing the limits provided with a transaction
        /// (e.g., gas limits, pure argument size limits, etc).
        /// </summary>
        public Dictionary<string, int?> Limits;

        /// <summary>
        /// The protocol configuration options.
        /// </summary>
        public ProtocolConfig ProtocolConfig;

        /// <summary>
        /// The default transaction limit parameters.
        /// </summary>
        public static Dictionary<LimitKey, string> TransactionLimits = new Dictionary<LimitKey, string>() {
            { LimitKey.MaxTxGas, "max_tx_gas" },
            { LimitKey.MaxGasObjects, "max_gas_payment_objects" },
            { LimitKey.MaxTxSizeBytes, "max_tx_size_bytes" },
            { LimitKey.MaxPureArgumentSize, "max_pure_argument_size" }
        };

        public BuildOptions
        (
            SuiClient Provider,
            Dictionary<string, int?> Limits = null,
            bool? OnlyTransactionKind = null,
            ProtocolConfig ProtocolConfig = null
        )
        {
            this.Provider = Provider;
            this.OnlyTransactionKind = OnlyTransactionKind;
            this.Limits = Limits;
            this.ProtocolConfig = ProtocolConfig;
        }
    }
}