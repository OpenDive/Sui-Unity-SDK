//
//  AtRiskValidator.cs
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

using System.Numerics;
using Newtonsoft.Json;
using Sui.Accounts;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents the validator that is at risk of being removed from the network.
    /// </summary>
    [JsonConverter(typeof(AtRiskValidatorConverter))]
    public class AtRiskValidator : ReturnBase
    {
        /// <summary>
        /// The at risk validator.
        /// </summary>
        public AccountAddress Validator { get; internal set; }

        /// <summary>
        /// The number of epochs it has been since the validator was last valid.
        /// </summary>
        public BigInteger NumberOfEpochs { get; internal set; }

        public AtRiskValidator
        (
            AccountAddress validator,
            BigInteger number_of_epochs
        )
        {
            this.Validator = validator;
            this.NumberOfEpochs = number_of_epochs;
        }

        public AtRiskValidator(SuiError error)
        {
            this.Error = error;
        }
    }
}