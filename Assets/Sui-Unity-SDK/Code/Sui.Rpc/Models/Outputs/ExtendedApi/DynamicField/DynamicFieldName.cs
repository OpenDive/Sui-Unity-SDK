//
//  DynamicFieldName.cs
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
using Newtonsoft.Json.Linq;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A structure representing the name of a dynamic field.
    /// </summary>
    [JsonObject]
    public class DynamicFieldName
    {
        /// <summary>
        /// A `string` representing the type of dynamic field.
        /// This can help in identifying the nature or category of the field.
        /// </summary>
        [JsonProperty("type")]
        public string Type { get; internal set; }

        /// <summary>
        /// A `JValue` representing the actual name or value of the dynamic field.
        /// This is the specific identifier for the field.
        /// </summary>
        [JsonProperty("value")]
        public JValue Value { get; internal set; }

        /// <summary>
        /// Converts an output Dynamic Field Name to an input for the RPC client.
        /// </summary>
        /// <returns>A `DynamicFieldInput` value.</returns>
        public DynamicFieldNameInput ToInput()
            => new DynamicFieldNameInput(this.Type, this.Value.Value.ToString());
    }
}