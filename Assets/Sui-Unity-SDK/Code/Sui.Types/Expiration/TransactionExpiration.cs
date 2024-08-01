//
//  TransactionExpiration.cs
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

namespace Sui.Types
{
    /// <summary>
    /// Represents a transaction expiration timestamp.
    /// </summary>
    public class TransactionExpiration : ISerializable
    {
        /// <summary>
        /// The type of transaction expiration.
        /// </summary>
        public ExpirationType Type { get; private set; }

        /// <summary>
        /// The private representation of the epoch value.
        /// </summary>
        private ulong? epoch;

        /// <summary>
        /// The public representation of the epoch value.
        /// It has a protected set function to change the type value to
        /// correspond with the value input.
        /// </summary>
        public ulong? Epoch
        {
            get => this.epoch;
            set
            {
                if (value == null)
                    this.Type = ExpirationType.None;
                else
                    this.Type = ExpirationType.Epoch;

                this.epoch = value;
            }
        }

        public TransactionExpiration(ExpirationType? type = null, ulong? epoch_value = null)
        {
            this.Type = type ?? ExpirationType.None;
            this.Epoch = epoch_value;
        }

        public void Serialize(Serialization serializer)
        {
            switch (this.Type)
            {
                case ExpirationType.None:
                    serializer.SerializeU8(0);
                    break;
                case ExpirationType.Epoch:
                    serializer.SerializeU8(1);
                    serializer.SerializeU64((ulong)this.Epoch);
                    break;
            }
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8().Value;

            switch (type)
            {
                case 0:
                    return new TransactionExpiration(ExpirationType.None);
                case 1:
                    return new TransactionExpiration(ExpirationType.Epoch, deserializer.DeserializeU64().Value);
                default:
                    return new SuiError(0, "Unable to deserialize TransactionExpiration", null);
            }
        }
    }
}