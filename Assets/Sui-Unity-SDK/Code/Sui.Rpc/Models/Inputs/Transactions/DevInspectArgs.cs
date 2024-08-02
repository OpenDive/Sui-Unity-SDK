//
//  DevInspectArgs.cs
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

using Sui.Accounts;
using System.Numerics;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Additional arguments supplied to dev inspect beyond what is allowed in Sui's API.
    /// </summary>
    [JsonObject]
    public class DevInspectArgs
    {
        /// <summary>
        /// The sponsor of the gas for the transaction, might be different from the sender.
        /// </summary>
        [JsonProperty("gasSponsor")]
        public AccountAddress GasSponsor { get; set; }

        /// <summary>
        /// The gas budget for the transaction.
        /// </summary>
        [JsonProperty("gasBudget")]
        public BigInteger? GasBudget { get; set; }

        /// <summary>
        /// The gas objects used to pay for the transaction.
        /// </summary>
        [JsonProperty("gasObjects")]
        public List<Sui.Types.SuiObjectRef> GasObjects { get; set; }

        /// <summary>
        /// Whether to skip transaction checks for the transaction.
        /// </summary>
        [JsonProperty("skipChecks")]
        public bool? SkipChecks { get; set; }

        /// <summary>
        /// Whether to return the raw transaction data and effects.
        /// </summary>
        [JsonProperty("showRawTxnDataAndEffects")]
        public bool? ShowRawTxnDataAndEffects { get; set; }

        public DevInspectArgs
        (
            AccountAddress gas_sponsor = null,
            BigInteger? gas_budget = null,
            List<Sui.Types.SuiObjectRef> gas_objects = null,
            bool? skip_checks = null,
            bool? show_raw_txn_data_and_effects = null
        )
        {
            this.GasSponsor = gas_sponsor;
            this.GasBudget = gas_budget;
            this.GasObjects = gas_objects;
            this.SkipChecks = skip_checks;
            this.ShowRawTxnDataAndEffects = show_raw_txn_data_and_effects;
        }
    }
}