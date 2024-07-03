using System;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Models;
using UnityEngine;

namespace Sui.Transactions
{
    public class Serializer
    {
        // TODO: Marcus: Convert types below from StandardStruct to use SuiStructTag
        private class StandardStruct
        {
            public AccountAddress Address { get; }
            public string Module { get; }
            public string Name { get; }

            public StandardStruct(string address, string module, string name)
            {
                this.Address = AccountAddress.FromHex(address);
                this.Module = module;
                this.Name = name;
            }
        }

        static private string stdAsciiModuleName = "ascii";
        static private string stdAsciiStructName = "String";

        static private string stdUtf8ModuleName = "string";
        static private string stdUtf8StructName = "String";

        static private string stdOptionModuleName = "option";
        static private string stdOptionStructName = "Option";

        static private StandardStruct resolvedSuiId => new StandardStruct("0x2", "object", "ID");
        static private StandardStruct resolvedAsciiStr => new StandardStruct("0x1", stdAsciiModuleName, stdAsciiStructName);
        static private StandardStruct resolvedUtf8Str => new StandardStruct("0x1", stdUtf8ModuleName, stdUtf8StructName);
        static private StandardStruct resolvedStdOption => new StandardStruct("0x1", stdOptionModuleName, stdOptionStructName);

        static public SuiMoveNormalziedTypeStruct ExtractStructType(SuiMoveNormalizedType normalizedType)
        {
            switch (normalizedType.Type)
            {
                case SuiMoveNormalizedTypeSerializationType.Reference:
                    SuiMoveNormalizedTypeReference reference = (SuiMoveNormalizedTypeReference)normalizedType.NormalizedType;
                    switch(reference.Reference.Type)
                    {
                        case SuiMoveNormalizedTypeSerializationType.Struct:
                            return (SuiMoveNormalziedTypeStruct)reference.Reference.NormalizedType;
                        default:
                            return null;
                    }
                case SuiMoveNormalizedTypeSerializationType.MutableReference:
                    SuiMoveNormalizedTypeMutableReference mutableReference = (SuiMoveNormalizedTypeMutableReference)normalizedType.NormalizedType;
                    switch (mutableReference.MutableReference.Type)
                    {
                        case SuiMoveNormalizedTypeSerializationType.Struct:
                            return (SuiMoveNormalziedTypeStruct)mutableReference.MutableReference.NormalizedType;
                        default:
                            return null;
                    }
                default:
                    return null;
            }
        }

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

        static public Type GetPureNormalizedTypeType(SuiMoveNormalizedType normalizedType, ISerializable argVal)
        {
            List<string> allowedTypes = new List<string>() { "Address", "Bool", "U8", "U16", "U32", "U64", "U128", "U256" };

            switch(normalizedType.Type)
            {
                case SuiMoveNormalizedTypeSerializationType.String:
                    SuiMoveNormalizedTypeString stringType = (SuiMoveNormalizedTypeString)normalizedType.NormalizedType;

                    if (allowedTypes.Contains(stringType.Value))
                    {
                        List<string> allowedNumTypes = new List<string>() { "U8", "U16", "U32", "U64", "U128", "U256" };
                        if (allowedNumTypes.Contains(stringType.Value))
                        {
                            return Type.GetType(stringType.Value);
                        }
                        else if (stringType.Value == "Bool")
                        {
                            return typeof(Bool);
                        }
                        else if (stringType.Value == "Address")
                        {
                            return typeof(AccountAddress);
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }

                    throw new Exception($"Unknown pure normalized type {stringType.Value}");
                case SuiMoveNormalizedTypeSerializationType.Vector:
                    SuiMoveNormalizedTypeVector vectorType = (SuiMoveNormalizedTypeVector)normalizedType.NormalizedType;

                    if (argVal == null || argVal.GetType() == typeof(string))
                    {
                        Type type = GetPureNormalizedTypeType(vectorType.Vector, argVal);
                        if (type == typeof(U8))
                        {
                            return typeof(BString);
                        }
                    }

                    if (argVal != null && !(argVal.GetType().IsArray))
                    {
                        throw new Exception($"Expect {argVal} to be an array, received {argVal.GetType()}");
                    }

                    Type innerType = GetPureNormalizedTypeType(vectorType.Vector, argVal);

                    if (innerType == null)
                        break;

                    Type listType = typeof(List<>).MakeGenericType(innerType);
                    return listType;
                case SuiMoveNormalizedTypeSerializationType.Struct:
                    SuiMoveNormalziedTypeStruct structType = (SuiMoveNormalziedTypeStruct)normalizedType.NormalizedType;

                    if (IsSameStruct(structType, resolvedAsciiStr) || IsSameStruct(structType, resolvedUtf8Str))
                    {
                        return typeof(BString);
                    }
                    else if (IsSameStruct(structType, resolvedSuiId))
                    {
                        return typeof(AccountAddress);
                    }
                    else if (IsSameStruct(structType, resolvedStdOption))
                    {
                        if (structType.Struct.TypeArguments.Length == 0)
                            throw new Exception("Unable to unwrap Struct");
                        SuiMoveNormalizedTypeVector option_to_vec = new SuiMoveNormalizedTypeVector(structType.Struct.TypeArguments[0]);
                        return GetPureNormalizedTypeType(new SuiMoveNormalizedType(option_to_vec, SuiMoveNormalizedTypeSerializationType.Vector), argVal);
                    }
                    else
                    {
                        throw new Exception("Unable to unwrap Struct");
                    }
                default:
                    return null;
            }

            throw new Exception("Unable to unwrap Struct");
        }


        static public string GetPureNormalizedType(SuiMoveNormalizedType normalizedType, ISerializable argVal)
        {
            List<string> allowedTypes = new List<string>() { "Address", "Bool", "U8", "U16", "U32", "U64", "U128", "U256" };
            List<string> allowedNumTypes = new List<string>() { "U8", "U16", "U32", "U64", "U128", "U256" };

            switch (normalizedType.Type)
            {
                case SuiMoveNormalizedTypeSerializationType.String:
                    SuiMoveNormalizedTypeString stringType = (SuiMoveNormalizedTypeString)normalizedType.NormalizedType;

                    if (allowedTypes.Contains(stringType.Value))
                    {
                        if (allowedNumTypes.Contains(stringType.Value))
                        {
                            return stringType.Value;
                        }
                        else if (stringType.Value == "Bool")
                        {
                            return "bool";
                        }
                        else if (stringType.Value == "Address")
                        {
                            return "address";
                        }
                        else
                        {
                            throw new NotSupportedException();
                        }
                    }

                    throw new Exception($"Unknown pure normalized type {stringType.Value}");
                case SuiMoveNormalizedTypeSerializationType.Vector:
                    SuiMoveNormalizedTypeVector vectorType = (SuiMoveNormalizedTypeVector)normalizedType.NormalizedType;

                    if(vectorType.Vector.Type == SuiMoveNormalizedTypeSerializationType.String)
                    {
                        if (argVal.GetType() == typeof(BString) && ((SuiMoveNormalizedTypeString)vectorType.Vector.NormalizedType).Value == "U8")
                            return "string";
                    }

                    string innerType = GetPureNormalizedType(vectorType.Vector, argVal);

                    if (innerType == null)
                        return null;

                    return $"vector<{innerType}>";
                case SuiMoveNormalizedTypeSerializationType.Struct:
                    SuiMoveNormalziedTypeStruct structType = (SuiMoveNormalziedTypeStruct)normalizedType.NormalizedType;

                    if (IsSameStruct(structType, resolvedAsciiStr))
                        return "string";

                    if (IsSameStruct(structType, resolvedUtf8Str))
                        return "utf8string";

                    if (IsSameStruct(structType, resolvedSuiId))
                        return "address";

                    if (IsSameStruct(structType, resolvedStdOption))
                    {
                        if (structType.Struct.TypeArguments.Length == 0)
                            throw new Exception("Type argument is empty");

                        SuiMoveNormalizedTypeVector option_to_vec = new SuiMoveNormalizedTypeVector(structType.Struct.TypeArguments[0]);
                        return GetPureNormalizedType(new SuiMoveNormalizedType(option_to_vec, SuiMoveNormalizedTypeSerializationType.Vector), argVal);
                    }

                    break;
                default:
                    return null;
            }

            return null;
        }

        private static bool IsSameStruct(SuiMoveNormalziedTypeStruct lhs, StandardStruct rhs)
        {
            return
                lhs.Struct.StructTag.address == rhs.Address &&
                lhs.Struct.StructTag.module == rhs.Module &&
                lhs.Struct.StructTag.name == rhs.Name;
        }
    }
}

