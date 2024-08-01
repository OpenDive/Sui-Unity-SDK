//
//  NestedResult.cs
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

namespace Sui.Transactions
{
    /// <summary>
    /// Similar to a `Result` (`TransactionResult` but it accesses a nested result).
    /// Currently, the only usage of this is to access a value from
    /// a Move call with multiple return values.
    /// </summary>
    public class NestedResult : ITransactionArgument
    {
        /// <summary>
        /// The index of the outer result.
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// The index of the inner result.
        /// </summary>
        public int ResultIndex { get; set; }

        public NestedResult(int index, int result_index)
        {
            this.Index = index;
            this.ResultIndex = result_index;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU16((ushort)this.Index);
            serializer.SerializeU16((ushort)this.ResultIndex);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
            => new NestedResult
               (
                   deserializer.DeserializeU16().Value,
                   deserializer.DeserializeU16().Value
               );
    }
}