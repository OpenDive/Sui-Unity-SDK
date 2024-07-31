//
//  ParsedMoveObject.cs
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

using Newtonsoft.Json.Linq;
using Sui.Types;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a Move Object in Move programming language, detailing its fields, type, and whether it has public transfer enabled.
    /// </summary>
    public class ParsedMoveObject : IParsedData
    {
        /// <summary>
        /// Represents the fields of the Move object.
        /// </summary>
        public JToken Fields { get; internal set; }

        /// <summary>
        /// Indicates whether public transfer is enabled for the object.
        /// </summary>
        public bool HasPublicTransfer { get; internal set; }

        /// <summary>
        /// Represents the type of the Move object.
        /// </summary>
        public SuiStructTag Type { get; internal set; }

        public ParsedMoveObject
        (
            JToken fields,
            bool has_public_transfer,
            SuiStructTag type
        )
        {
            this.Fields = fields;
            this.HasPublicTransfer = has_public_transfer;
            this.Type = type;
        }
    }
}