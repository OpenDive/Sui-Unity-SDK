//
//  ExecutionResultType.cs
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

using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents the type of execution result, which includes mutable reference outputs
    /// and return values after the execution of a function or operation.
    /// </summary>
    [JsonObject]
    public class ExecutionResultType
    {
        /// <summary>
        /// An optional list of `MutableReferenceOutput` representing the mutable
        /// reference outputs obtained as a result of the execution. If `null`, it may indicate
        /// that there were no mutable reference outputs produced during the execution.
        /// </summary>
        [JsonProperty("mutableReferenceOutputs", NullValueHandling = NullValueHandling.Include)]
        public MutableReferenceOutput[] MutableReferenceOutputs { get; internal set; }

        /// <summary>
        /// An optional array of `ReturnValue` representing the return values obtained
        /// as a result of the execution. If `null`, it may indicate that there were no return
        /// values produced during the execution.
        /// </summary>
        [JsonProperty("returnValues", NullValueHandling = NullValueHandling.Include)]
        public ReturnValue[] ReturnValues { get; internal set; }
    }
}