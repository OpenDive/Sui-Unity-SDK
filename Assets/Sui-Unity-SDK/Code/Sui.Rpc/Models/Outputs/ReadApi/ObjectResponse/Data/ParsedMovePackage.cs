//
//  ParsedMovePackage.cs
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

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents a Move Package in the Move contract language.
    /// A Move Package is a container for Move Modules, where each module contains Move Scripts and Move Structs.
    /// </summary>
    public class ParsedMovePackage : IParsedData
    {
        /// <summary>
        /// A dictionary where the key is a `string` representing the name of a module, script, or struct,
        /// and the value is a `string` representing the disassembled bytecode of that entity.
        /// </summary>
        public Dictionary<string, string> Disassembled { get; internal set; }

        public ParsedMovePackage
        (
            Dictionary<string, string> disassembled
        )
        {
            this.Disassembled = disassembled;
        }
    }
}