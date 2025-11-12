//
//  PureCallArg.cs
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
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Accounts;

namespace Sui.Types
{
    /// <summary>
    /// A pure value takes in U8, U256, BString, AccountAddress, etc
    /// </summary>
    public class PureCallArg : ICallArg
    {
        /// <summary>
        /// The raw bytes that represent the pure call argument.
        /// </summary>
        public byte[] Value { get; set; }

        public PureCallArg(byte[] value)
        {
            this.Value = value;
        }

        public PureCallArg(JToken value, string value_type)
        {
            Serialization ser = new Serialization();
            SuiStructTag struct_tag = SuiStructTag.FromStr(value.Value<string>());

            if (struct_tag != null)
                struct_tag.Serialize(ser);
            else
            {
                if (value_type == "bool")
                    ser.SerializeBool(value.Value<bool>());
                else if (value_type == "u8")
                    ser.SerializeU8(value.Value<byte>());
                else if (value_type == "u16")
                    ser.SerializeU16(value.Value<ushort>());
                else if (value_type == "u32")
                    ser.SerializeU32(value.Value<uint>());
                else if (value_type == "u64")
                    ser.SerializeU64(value.Value<ulong>());
                else if (value_type == "u128")
                    ser.SerializeU128(BigInteger.Parse(value.Value<string>()));
                else if (value_type == "u256")
                    ser.SerializeU256(BigInteger.Parse(value.Value<string>()));
                else
                {
                    AccountAddress account_address = AccountAddress.FromHex(value.Value<string>());
                    if (account_address != null)
                        account_address.Serialize(ser);
                    else
                        ser.SerializeString(value.Value<string>());
                }
            }

            this.Value = ser.GetBytes();
        }

        public PureCallArg(ISerializable value)
        {
            Serialization ser = new Serialization();
            ser.Serialize(value);
            this.Value = ser.GetBytes();
        }

        public void Serialize(Serialization serializer)
            => serializer.Serialize(this.Value);

        public static ISerializable Deserialize(Deserialization deserializer)
            => new PureCallArg
               (
                   deserializer.ToBytes()
               );
    }
}