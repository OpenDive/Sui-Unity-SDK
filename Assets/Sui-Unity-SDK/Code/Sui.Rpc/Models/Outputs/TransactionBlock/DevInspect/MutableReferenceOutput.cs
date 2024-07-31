//
//  MutableReferenceOutput.cs
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
using Sui.Types;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// The mutable reference outputs as a result of a transaction.
    /// </summary>
    [JsonConverter(typeof(MutableReferenceOutputConverter))]
    public class MutableReferenceOutput : ReturnBase
    {
        /// <summary>
        /// The name of the mutable reference.
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The object ID of the mutable reference.
        /// </summary>
        public byte[] ObjectID { get; internal set; }

        /// <summary>
        /// The struct type representation of the mutable reference.
        /// </summary>
        public SuiStructTag Type { get; internal set; }

        public MutableReferenceOutput
        (
            string name,
            byte[] object_id,
            SuiStructTag type
        )
        {
            this.Name = name;
            this.ObjectID = object_id;
            this.Type = type;
        }

        public MutableReferenceOutput(SuiError error)
        {
            this.Error = error;
        }
    }
}