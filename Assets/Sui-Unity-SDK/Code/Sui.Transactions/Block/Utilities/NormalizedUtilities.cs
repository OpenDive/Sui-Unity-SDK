//
//  NormalizedUtilities.cs
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
using OpenDive.BCS;
using Sui.Rpc.Models;
using Sui.Types;
using Sui.Utilities;

namespace Sui.Transactions
{
    /// <summary>
    /// Utility class used to return information about a Sui Normalized Type.
    /// </summary>
    public class NormalizedUtilities
    {
        /// <summary>
        /// Represents the module name where the standard ASCII `String` struct is declared.
        /// </summary>
        static private string StandardASCIIModuleName = "ascii";

        /// <summary>
        /// Represents the standard name for the `String` struct in ASCII.
        /// </summary>
        static private string StandardASCIIStructName = "String";

        /// <summary>
        /// Represents the module name where the standard UTF-8 `String` struct is declared.
        /// </summary>
        static private string StandardUTF8ModuleName = "string";

        /// <summary>
        /// Represents the standard name for the `String` struct in UTF-8.
        /// </summary>
        static private string StandardUTF8StructName = "String";

        /// <summary>
        /// Represents the module name where the standard `Option` struct is declared.
        /// </summary>
        static private string StandardOptionModuleName = "option";

        /// <summary>
        /// Represents the standard name for the `Option` struct.
        /// </summary>
        static private string StandardOptionStructName = "Option";

        /// <summary>
        /// Represents the resolved Sui ID struct.
        /// </summary>
        static private SuiStructTag ResolvedSuiID => new SuiStructTag("0x2::object::ID");

        /// <summary>
        /// Represents the resolved ASCII String struct.
        /// </summary>
        static private SuiStructTag ResolvedASCIIString => new SuiStructTag($"0x1::{StandardASCIIModuleName}::{StandardASCIIStructName}");

        /// <summary>
        /// Represents the resolved UTF8 String struct.
        /// </summary>
        static private SuiStructTag ResolvedUTF8String => new SuiStructTag($"0x1::{StandardUTF8ModuleName}::{StandardUTF8StructName}");

        /// <summary>
        /// Represents the resolved standard option struct.
        /// </summary>
        static private SuiStructTag ResolvedStandardOption => new SuiStructTag($"0x1::{StandardOptionModuleName}::{StandardOptionStructName}");

        /// <summary>
        /// Function to extract structure tags from the normalized type.
        /// </summary>
        /// <param name="normalized_type">The normalized type value.</param>
        /// <returns>The structure type if it exists, otherwise returns `null`.</returns>
        static public SuiMoveNormalziedTypeStruct ExtractStructType(SuiMoveNormalizedType normalized_type)
        {
            switch (normalized_type.Type)
            {
                case SuiMoveNormalizedTypeSerializationType.Reference:
                    SuiMoveNormalizedTypeReference reference = (SuiMoveNormalizedTypeReference)normalized_type.NormalizedType;
                    switch(reference.Reference.Type)
                    {
                        case SuiMoveNormalizedTypeSerializationType.Struct:
                            return (SuiMoveNormalziedTypeStruct)reference.Reference.NormalizedType;
                        default:
                            return null;
                    }
                case SuiMoveNormalizedTypeSerializationType.MutableReference:
                    SuiMoveNormalizedTypeMutableReference mutable_reference = (SuiMoveNormalizedTypeMutableReference)normalized_type.NormalizedType;
                    switch (mutable_reference.MutableReference.Type)
                    {
                        case SuiMoveNormalizedTypeSerializationType.Struct:
                            return (SuiMoveNormalziedTypeStruct)mutable_reference.MutableReference.NormalizedType;
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

        /// <summary>
        /// Function to extract mutable references from the normalized type.
        /// </summary>
        /// <param name="normalized_type">The normalized type value.</param>
        /// <returns>The mutable reference type if it exists, otherwise returns `null`.</returns>
        static public SuiMoveNormalizedType ExtractMutableReference(SuiMoveNormalizedType normalized_type)
        {
            switch (normalized_type.Type)
            {
                case SuiMoveNormalizedTypeSerializationType.MutableReference:
                    return ((SuiMoveNormalizedTypeMutableReference)normalized_type.NormalizedType).MutableReference;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Function to determine the pure serialization type based on the argument value.
        /// </summary>
        /// <param name="normalized_type">The normalized type value.</param>
        /// <param name="arg_val">The `ISerializable` value of the argument.</param>
        /// <returns>A `string` representation of the pure serialization type if it exists, otherwise returns `null`.</returns>
        static public SuiResult<string> GetPureNormalizedType(SuiMoveNormalizedType normalized_type, ISerializable arg_val)
        {
            List<string> allowed_types = new List<string>() { "Address", "Bool", "U8", "U16", "U32", "U64", "U128", "U256" };
            List<string> allowed_num_types = new List<string>() { "U8", "U16", "U32", "U64", "U128", "U256" };

            switch (normalized_type.Type)
            {
                case SuiMoveNormalizedTypeSerializationType.String:
                    SuiMoveNormalizedTypeString string_type = (SuiMoveNormalizedTypeString)normalized_type.NormalizedType;

                    if (allowed_types.Contains(string_type.Value))
                    {
                        if (allowed_num_types.Contains(string_type.Value))
                            return new SuiResult<string>(string_type.Value);
                        else if (string_type.Value == "Bool")
                            return new SuiResult<string>("bool");
                        else if (string_type.Value == "Address")
                            return new SuiResult<string>("address");
                        else
                            return new SuiResult<string>(null, new SuiError(0, $"String type {string_type} is not allowed.", null));
                    }

                    return new SuiResult<string>(null, new SuiError(0, $"Unknown pure normalized type {string_type.Value}", null));
                case SuiMoveNormalizedTypeSerializationType.Vector:
                    SuiMoveNormalizedTypeVector vector_type = (SuiMoveNormalizedTypeVector)normalized_type.NormalizedType;

                    if(vector_type.Vector.Type == SuiMoveNormalizedTypeSerializationType.String)
                        if (arg_val.GetType() == typeof(BString) && ((SuiMoveNormalizedTypeString)vector_type.Vector.NormalizedType).Value == "U8")
                            return new SuiResult<string>("string");

                    SuiResult<string> inner_type = GetPureNormalizedType(vector_type.Vector, arg_val);

                    if (inner_type == null)
                        return null;

                    if (inner_type.Error != null)
                        return inner_type;

                    return new SuiResult<string>($"vector<{inner_type.Result}>");
                case SuiMoveNormalizedTypeSerializationType.Struct:
                    SuiMoveNormalziedTypeStruct struct_type = (SuiMoveNormalziedTypeStruct)normalized_type.NormalizedType;

                    if (struct_type.Equals(ResolvedASCIIString))
                        return new SuiResult<string>("string");

                    if (struct_type.Equals(ResolvedUTF8String))
                        return new SuiResult<string>("utf8string");

                    if (struct_type.Equals(ResolvedSuiID))
                        return new SuiResult<string>("address");

                    if (struct_type.Equals(ResolvedStandardOption))
                    {
                        if (struct_type.Struct.TypeArguments.Count == 0)
                            return new SuiResult<string>(null, new SuiError(0, "Type argument is empty", null));

                        SuiMoveNormalizedTypeVector option_to_vec = new SuiMoveNormalizedTypeVector(struct_type.Struct.TypeArguments[0]);
                        return GetPureNormalizedType(new SuiMoveNormalizedType(option_to_vec, SuiMoveNormalizedTypeSerializationType.Vector), arg_val);
                    }

                    break;
            }

            return null;
        }
    }
}

