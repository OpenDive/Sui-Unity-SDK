//
//  SuiMoveNormalizedType.cs
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
using OpenDive.BCS;
using Sui.Utilities;
using System.Collections.Generic;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// A class representing the Normalized Type of the given parameter.
    /// </summary>
    [JsonConverter(typeof(NormalizedTypeConverter))]
    public class SuiMoveNormalizedType : ReturnBase, ISerializable
    {
        /// <summary>
        /// The Normalized Type's value.
        /// </summary>
        public ISuiMoveNormalizedType NormalizedType { get; internal set; }

        /// <summary>
        /// The type of Normalized Type.
        /// </summary>
        public SuiMoveNormalizedTypeSerializationType Type { get; internal set; }

        public SuiMoveNormalizedType
        (
            ISuiMoveNormalizedType normalized_type,
            SuiMoveNormalizedTypeSerializationType type
        )
        {
            this.NormalizedType = normalized_type;
            this.Type = type;
        }

        public SuiMoveNormalizedType(SuiError error)
        {
            this.Error = error;
        }

        /// <summary>
        /// Converts a string value to a Normalized Type.
        /// </summary>
        /// <param name="value">The normalized value as a string.</param>
        /// <returns>A `SuiMoveNormalziedType` value, or null if it's not in the allowed types.</returns>
        public static SuiMoveNormalizedType FromStr(string value)
        {
            List<string> allowed_types = new List<string>() { "Address", "Bool", "U8", "U16", "U32", "U64", "U128", "U256", "Signer" };

            if (allowed_types.Exists(str => str == value) == false)
                return null;

            return new SuiMoveNormalizedType
            (
                new SuiMoveNormalizedTypeString(value),
                SuiMoveNormalizedTypeSerializationType.String
            );
        }

        public void Serialize(Serialization serializer)
        {
            switch (this.Type)
            {
                case SuiMoveNormalizedTypeSerializationType.String:
                    ((SuiMoveNormalizedTypeString)this.NormalizedType).Serialize(serializer);
                    break;
                case SuiMoveNormalizedTypeSerializationType.TypeParameter:
                    serializer.SerializeU8(9);
                    serializer.Serialize((SuiMoveNormalziedTypeParameterType)this.NormalizedType);
                    break;
                case SuiMoveNormalizedTypeSerializationType.Reference:
                    serializer.SerializeU8(10);
                    serializer.Serialize((SuiMoveNormalizedTypeReference)this.NormalizedType);
                    break;
                case SuiMoveNormalizedTypeSerializationType.MutableReference:
                    serializer.SerializeU8(11);
                    serializer.Serialize((SuiMoveNormalizedTypeMutableReference)this.NormalizedType);
                    break;
                case SuiMoveNormalizedTypeSerializationType.Vector:
                    serializer.SerializeU8(12);
                    serializer.Serialize((SuiMoveNormalizedTypeVector)this.NormalizedType);
                    break;
                case SuiMoveNormalizedTypeSerializationType.Struct:
                    serializer.SerializeU8(13);
                    serializer.Serialize((SuiMoveNormalziedTypeStruct)this.NormalizedType);
                    break;
            }
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8().Value;

            ISuiMoveNormalizedType normalized_type;
            SuiMoveNormalizedTypeSerializationType type_value;

            switch (type)
            {
                case 0:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("Bool");
                    break;
                case 1:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U8");
                    break;
                case 2:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U16");
                    break;
                case 3:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U32");
                    break;
                case 4:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U64");
                    break;
                case 5:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U128");
                    break;
                case 6:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("U256");
                    break;
                case 7:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("Address");
                    break;
                case 8:
                    type_value = SuiMoveNormalizedTypeSerializationType.String;
                    normalized_type = new SuiMoveNormalizedTypeString("Signer");
                    break;
                case 9:
                    type_value = SuiMoveNormalizedTypeSerializationType.TypeParameter;
                    normalized_type = (SuiMoveNormalziedTypeParameterType)SuiMoveNormalziedTypeParameterType.Deserialize(deserializer);
                    break;
                case 10:
                    type_value = SuiMoveNormalizedTypeSerializationType.Reference;
                    normalized_type = (SuiMoveNormalizedTypeReference)SuiMoveNormalizedTypeReference.Deserialize(deserializer);
                    break;
                case 11:
                    type_value = SuiMoveNormalizedTypeSerializationType.MutableReference;
                    normalized_type = (SuiMoveNormalizedTypeMutableReference)SuiMoveNormalizedTypeMutableReference.Deserialize(deserializer);
                    break;
                case 12:
                    type_value = SuiMoveNormalizedTypeSerializationType.Vector;
                    normalized_type = (SuiMoveNormalizedTypeVector)SuiMoveNormalizedTypeVector.Deserialize(deserializer);
                    break;
                case 13:
                    type_value = SuiMoveNormalizedTypeSerializationType.Struct;
                    normalized_type = (SuiMoveNormalziedTypeStruct)SuiMoveNormalziedTypeStruct.Deserialize(deserializer);
                    break;
                default:
                    return new SuiError(0, "Unable to deserialize SuiMoveNormalizedType.", null);
            }

            return new SuiMoveNormalizedType(normalized_type, type_value);
        }
    }
}