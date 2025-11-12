//
//  SuiMoveNormalizedModule.cs
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

using System.Collections.Generic;
using System.Numerics;
using Newtonsoft.Json;
using Sui.Accounts;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a normalized Sui Move Module, containing various details about the module including its address,
    /// name, friend modules, structs, and exposed functions.
    /// </summary>
    [JsonObject]
    public class SuiMoveNormalizedModule
	{
        /// <summary>
        /// The address where the module is located.
        /// </summary>
        [JsonProperty("address")]
        public AccountAddress Address { get; internal set; }

        /// <summary>
        /// The name of the module.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; internal set; }

        /// <summary>
        /// Specifies the file format version of the module.
        /// </summary>
        [JsonProperty("fileFormatVersion")]
        public BigInteger FileFormatVersion { get; internal set; }

        /// <summary>
        /// A list of `Friend` representing the friend modules of this module.
        /// </summary>
        [JsonProperty("friends")]
        public Friend[] Friends { get; internal set; }

        /// <summary>
        /// A dictionary containing the structs present in the module, where the
        /// key is the name of the struct and the value is an instance of `SuiMoveNormalizedStruct`.
        /// </summary>
        [JsonProperty("structs")]
        public Dictionary<string, SuiMoveNormalizedStruct> Structs { get; internal set; }

        /// <summary>
        /// A dictionary containing the exposed functions of the module,
        /// where the key is the name of the function and the value is an instance of `SuiMoveNormalizedFunction`.
        /// </summary>
        [JsonProperty("exposedFunctions")]
        public Dictionary<string, NormalizedMoveFunctionResponse> ExposedFunctions { get; internal set; }
    }
}