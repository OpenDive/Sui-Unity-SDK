//
//  SerializableTypeTag.cs
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

using Sui.Accounts;
using Sui.Utilities;
using Sui.Types;

namespace OpenDive.BCS
{
    /// <summary>
    /// An object that contains the type tag associated with the BCS value.
    /// </summary>
    public class SerializableTypeTag : ReturnBase, ISerializable
    {
        /// <summary>
        /// The Move type of the value.
        /// </summary>
        public TypeTag Type { get; set; }

        /// <summary>
        /// The serializable value.
        /// </summary>
        public ISerializable Value { get; set; }

        public SerializableTypeTag(ISerializable value)
        {
            this.Value = value;

            if (value.GetType() == typeof(SuiStructTag))
                this.Type = TypeTag.STRUCT;
            else if (value.GetType() == typeof(AccountAddress))
                this.Type = TypeTag.ACCOUNT_ADDRESS;
            else
                this.SetError<BcsError>("Unable to initialize SerializableTypeTag with ISerializable value.", value);
        }

        public SerializableTypeTag(string string_value)
        {
            SuiStructTag struct_tag = SuiStructTag.FromStr(string_value);

            if (struct_tag != null)
            {
                this.Value = struct_tag;
                this.Type = TypeTag.STRUCT;
            }
            else
            {
                AccountAddress account_address = AccountAddress.FromHex(string_value);

                if (account_address != null)
                {
                    this.Value = account_address;
                    this.Type = TypeTag.ACCOUNT_ADDRESS;
                    return;
                }

                this.Value = null;

                if (string_value == "bool")
                    this.Type = TypeTag.BOOL;
                else if (string_value == "u8")
                    this.Type = TypeTag.U8;
                else if (string_value == "u16")
                    this.Type = TypeTag.U16;
                else if (string_value == "u32")
                    this.Type = TypeTag.U32;
                else if (string_value == "u64")
                    this.Type = TypeTag.U64;
                else if (string_value == "u128")
                    this.Type = TypeTag.U128;
                else if (string_value == "u256")
                    this.Type = TypeTag.U256;
                else
                    this.SetError<BcsError>("Unable to initialize SerializableTypeTag with string value.", string_value);
            }
        }

        public SerializableTypeTag(TypeTag type, ISerializable value = null)
        {
            this.Type = type;
            this.Value = value;
        }

        public override bool Equals(object other)
        {
            if (other is not SerializableTypeTag)
                this.SetError<bool, BcsError>(false, "Compared object is not a SerializableTypeTag.", other);

            SerializableTypeTag other_serializable_type_tag = (SerializableTypeTag)other;

            return
                this.Type == other_serializable_type_tag.Type &&
                this.Value.Equals(other_serializable_type_tag.Value);
        }

        public override int GetHashCode() => base.GetHashCode();

        public override string ToString()
        {
            switch (this.Type)
            {
                case TypeTag.ACCOUNT_ADDRESS:
                    return "address";
                case TypeTag.STRUCT:
                    return this.Value.ToString();
                default:
                    return this.Type.ToString().ToLower();
            }
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8((byte)this.Type);
            this.Value.Serialize(serializer);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            TypeTag variant = (TypeTag)deserializer.DeserializeUleb128();
            ISerializable ser_value;

            if (variant == TypeTag.BOOL)
                ser_value = new Bool(deserializer.DeserializeBool());
            else if (variant == TypeTag.U8)
                ser_value = deserializer.DeserializeU8();
            else if (variant == TypeTag.U16)
                ser_value = deserializer.DeserializeU16();
            else if (variant == TypeTag.U32)
                ser_value = deserializer.DeserializeU32();
            else if (variant == TypeTag.U64)
                ser_value = deserializer.DeserializeU64();
            else if (variant == TypeTag.U128)
                ser_value = deserializer.DeserializeU128();
            else if (variant == TypeTag.U256)
                ser_value = deserializer.DeserializeU256();
            else if (variant == TypeTag.ACCOUNT_ADDRESS)
                ser_value = (AccountAddress)AccountAddress.Deserialize(deserializer);
            else if (variant == TypeTag.STRUCT)
                ser_value = (SuiStructTag)SuiStructTag.Deserialize(deserializer);

            // TODO: Implement below deserialization types.
            else if (variant == TypeTag.SIGNER)
                return new BcsError(0, "Unable to deserialize SerialziableTypeTag for Signer.", null);
            else if (variant == TypeTag.VECTOR)
                return new BcsError(0, "Unable to deserialize SerialziableTypeTag for Vector.", null);

            else
                return new BcsError(0, "Unable to deserialize SerialziableTypeTag.", null);

            return new SerializableTypeTag(variant, ser_value);
        }
    }
}