//
//  SuiMoveNormalizedStructType.cs
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
using Sui.Rpc.Models;
using Sui.Utilities;
using System.Collections.Generic;
using System.Linq;
using OpenDive.BCS;

namespace Sui.Types
{
    [JsonConverter(typeof(SuiMoveNormalizedStructTypeConverter))]
    public class SuiMoveNormalizedStructType : StructTag<SuiMoveNormalizedType>
    {
        internal SuiMoveNormalizedStructType(ErrorBase error) => this.Error = error;

        public SuiMoveNormalizedStructType(string address, string module, string name, List<SuiMoveNormalizedType> type_arguments)
        {
            this.Address = AccountAddress.FromHex(address);
            this.Module = module;
            this.Name = name;
            this.TypeArguments = type_arguments;
        }

        public SuiMoveNormalizedStructType(AccountAddress address, string module, string name, List<SuiMoveNormalizedType> type_arguments)
        {
            this.Address = address;
            this.Module = module;
            this.Name = name;
            this.TypeArguments = type_arguments;
        }

        public static SuiMoveNormalizedStructType FromStr(string type_tag)
        {
            try
            {
                string[] struct_tag_components = SuiMoveNormalizedStructType.FromStr_(type_tag);

                string name = "";
                int index = 0;
                SuiMoveNormalizedType nested_value = null;

                while (index < struct_tag_components[2].Length)
                {
                    char letter = struct_tag_components[2][index];
                    index += 1;

                    if (letter == '<')
                    {
                        nested_value = SuiMoveNormalizedType.FromStr
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

                return new SuiMoveNormalizedStructType
                (
                    address,
                    struct_tag_components[1],
                    name,
                    nested_value != null ?
                        new List<SuiMoveNormalizedType> { nested_value } :
                        new List<SuiMoveNormalizedType> { }
                );
            }
            catch
            {
                return new SuiMoveNormalizedStructType(new SuiError(0, "Unable to initialize SuiMoveNormalizedStructType from string.", type_tag));
            }
        }

        public override bool Equals(object other)
        {
            if (other is not SuiMoveNormalizedStructType)
                this.SetError<bool, SuiError>(false, "Compared object is not a SuiMoveNormalizedStructType.", other);

            SuiMoveNormalizedStructType other_normalized_struct_tag = (SuiMoveNormalizedStructType)other;

            return
                this.Address.KeyBytes.SequenceEqual(other_normalized_struct_tag.Address.KeyBytes) &&
                this.Module == other_normalized_struct_tag.Module &&
                this.Name == other_normalized_struct_tag.Name &&
                this.TypeArguments.SequenceEqual(other_normalized_struct_tag.TypeArguments);
        }

        public override int GetHashCode() => base.GetHashCode();

        public override void Serialize(Serialization serializer)
        {
            serializer.Serialize(this.Address);
            serializer.Serialize(this.Module);
            serializer.Serialize(this.Name);

            if (this.TypeArguments.Count != 0)
            {
                Sequence type_arguments = new Sequence(this.TypeArguments.ToArray());
                serializer.Serialize(type_arguments);
            }
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new SuiMoveNormalizedStructType
            (
                (AccountAddress)AccountAddress.Deserialize(deserializer),
                deserializer.DeserializeString().Value,
                deserializer.DeserializeString().Value,
                new List<SuiMoveNormalizedType> { }  // TODO: Implement deserializer for fetching Type Arguments.
            );
        }
    }
}