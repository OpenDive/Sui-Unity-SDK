//
//  SuiStructTag.cs
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
using Sui.Accounts;
using Sui.Utilities;
using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;

namespace Sui.Types
{
    [JsonConverter(typeof(SuiStructTagTypeConverter))]
    public class SuiStructTag : StructTag<SerializableTypeTag>
    {
        internal SuiStructTag(ErrorBase error) => this.Error = error;

        public SuiStructTag(AccountAddress address, string module, string name, List<SerializableTypeTag> type_arguments)
        {
            this.Address = address;
            this.Module = module;
            this.Name = name;
            this.TypeArguments = type_arguments;
        }

        public SuiStructTag(string address, string module, string name, List<SerializableTypeTag> type_arguments)
        {
            this.Address = AccountAddress.FromHex(address);
            this.Module = module;
            this.Name = name;
            this.TypeArguments = type_arguments;
        }

        public SuiStructTag(string sui_struct_tag)
        {
            SuiStructTag result = SuiStructTag.FromStr(sui_struct_tag);
            this.Address = result.Address;
            this.Module = result.Module;
            this.Name = result.Name;
            this.TypeArguments = result.TypeArguments;
        }

        public override void Serialize(Serialization serializer)
        {
            this.Address.Serialize(serializer);
            serializer.Serialize(this.Module);
            serializer.Serialize(this.Name);

            Sequence type_arguments = new Sequence(this.TypeArguments.ToArray());
            serializer.Serialize(type_arguments);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SuiStructTag
            (
                (AccountAddress)AccountAddress.Deserialize(deserializer),
                deserializer.DeserializeString().Value,
                deserializer.DeserializeString().Value,
                new List<SerializableTypeTag>()  // TODO: Implement Deserializer for TypeArguments
            );
        }

        public static SuiStructTag FromStr(string type_tag)
        {
            try
            {
                string[] struct_tag_components = SuiStructTag.FromStr_(type_tag);

                string name = "";
                int index = 0;
                SerializableTypeTag nested_value = null;

                while (index < struct_tag_components[2].Length)
                {
                    char letter = struct_tag_components[2][index];
                    index += 1;

                    if (letter == '<')
                    {
                        nested_value = new SerializableTypeTag
                        (
                            string.Join("::", struct_tag_components[2..struct_tag_components.Length])[index..]
                        );
                        break;
                    }
                    else if (letter == '>')
                        break;
                    else
                        name += letter;
                }

                AccountAddress address = AccountAddress.FromHex(struct_tag_components[0]);

                return new SuiStructTag
                (
                    address,
                    struct_tag_components[1],
                    name,
                    nested_value != null ?
                        new List<SerializableTypeTag> { nested_value } :
                        new List<SerializableTypeTag> { }
                );
            }
            catch
            {
                return new SuiStructTag(new SuiError(0, "Unable to initialize SuiStructTag from string.", type_tag));
            }
        }

        public override bool Equals(object other)
        {
            if (other is not SuiStructTag)
                this.SetError<bool, SuiError>(false, "Compared object is not a SuiStructTag.", other);

            SuiStructTag other_sui_struct_tag = (SuiStructTag)other;

            return
                this.Address.KeyBytes.SequenceEqual(other_sui_struct_tag.Address.KeyBytes) &&
                this.Module == other_sui_struct_tag.Module &&
                this.Name == other_sui_struct_tag.Name &&
                this.TypeArguments.SequenceEqual(other_sui_struct_tag.TypeArguments);
        }

        public override int GetHashCode() => base.GetHashCode();
    }
}