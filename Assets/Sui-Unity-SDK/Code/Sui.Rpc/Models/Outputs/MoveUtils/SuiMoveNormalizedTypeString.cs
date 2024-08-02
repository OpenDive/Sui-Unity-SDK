//
//  SuiMoveNormalizedTypeString.cs
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

using OpenDive.BCS;
using Sui.Utilities;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents an unsigned integer, boolean, or address.
    /// </summary>
    public class SuiMoveNormalizedTypeString : ISuiMoveNormalizedType
    {
        public string Value { get; internal set; }

        public SuiMoveNormalizedTypeString(string Value)
        {
            this.Value = Value;
        }

        public override void Serialize(Serialization serializer)
        {
            switch (this.Value)
            {
                case "Bool":
                    serializer.SerializeU8(0);
                    break;
                case "U8":
                    serializer.SerializeU8(1);
                    break;
                case "U16":
                    serializer.SerializeU8(2);
                    break;
                case "U32":
                    serializer.SerializeU8(3);
                    break;
                case "U64":
                    serializer.SerializeU8(4);
                    break;
                case "U128":
                    serializer.SerializeU8(5);
                    break;
                case "U256":
                    serializer.SerializeU8(6);
                    break;
                case "Address":
                    serializer.SerializeU8(7);
                    break;
                case "Signer":
                    serializer.SerializeU8(8);
                    break;
                default:
                    this.SetError<SuiError>("Invalid value", this.Value);
                    break;
            }
        }
    }
}