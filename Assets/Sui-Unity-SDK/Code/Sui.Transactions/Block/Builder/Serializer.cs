using System;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Models;

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

        static private StandardStruct resolvedSuiId = new StandardStruct("0x2", "object", "ID");
        static private StandardStruct resolvedAsciiStr = new StandardStruct("0x1", stdAsciiModuleName, stdAsciiStructName);
        static private StandardStruct resolvedUtf8Str = new StandardStruct("0x1", stdUtf8ModuleName, stdUtf8StructName);
        static private StandardStruct resolvedStdOption = new StandardStruct("0x1", stdOptionModuleName, stdOptionStructName);

        static public ISuiMoveNormalizedType ExtractStructType(ISuiMoveNormalizedType normalizedType)
        {
            if (normalizedType as SuiMoveNormalizedTypeReference != null)
            {
                SuiMoveNormalizedTypeReference reference = (SuiMoveNormalizedTypeReference)normalizedType;
                return reference.Reference;
            }

            if (normalizedType as SuiMoveNormalizedTypeMutableReference != null)
            {
                SuiMoveNormalizedTypeMutableReference mutableReference = (SuiMoveNormalizedTypeMutableReference)normalizedType;
                return mutableReference.MutableReference;
            }

            return null;
        }

        static public string GetPureNormalizedType(ISuiMoveNormalizedType normalizedType, ISerializable argVal)
        {
            List<string> allowedTypes = new List<string>() { "Address", "Bool", "U8", "U16", "U32", "U64", "U128", "U256" };

            if (normalizedType as SuiMoveNormalizedTypeString != null)
            {
                SuiMoveNormalizedTypeString stringType = (SuiMoveNormalizedTypeString)normalizedType;

                if (allowedTypes.Contains(stringType.Value))
                {
                    if (stringType.Value == "Address")
                    {
                        return "AccountAddress";
                    }

                    return stringType.Value;
                }

                throw new Exception($"Unknown pure normalized type {stringType.Value}");
            }

            if (normalizedType as SuiMoveNormalizedTypeVector != null)
            {
                do
                {
                    SuiMoveNormalizedTypeVector vectorType = (SuiMoveNormalizedTypeVector)normalizedType;

                    if (argVal == null || argVal.GetType() == typeof(string))
                    {
                        if (GetPureNormalizedType(vectorType.Vector, argVal) == "U8")
                        {
                            return "string";
                        }
                    }

                    if (argVal != null && !(argVal.GetType().IsArray))
                    {
                        throw new Exception($"Expect {argVal} to be an array, received {argVal.GetType()}");
                    }

                    string innerType = GetPureNormalizedType(vectorType.Vector, argVal);

                    if (innerType == null)
                        break;

                    return $"vector<{innerType}>";
                }
                while (false);
            }

            if (normalizedType as SuiMoveNormalziedTypeStruct != null)
            {
                do
                {
                    SuiMoveNormalziedTypeStruct structType = (SuiMoveNormalziedTypeStruct)normalizedType;

                    if (IsSameStruct(structType.Struct, resolvedAsciiStr) || IsSameStruct(structType.Struct, resolvedUtf8Str))
                    {
                        return "string";
                    }
                    else if (IsSameStruct(structType.Struct, resolvedSuiId))
                    {
                        return "AccountAddress";
                    }
                    else if (IsSameStruct(structType.Struct, resolvedStdOption))
                    {
                        //SuiMoveNormalizedTypeVector vectorType = new SuiMoveNormalizedTypeVector(structType.Struct.typeArgs[0]); TODO: Marcus: Implement case for option struct
                    }
                }
                while (false);
            }

            return null;
        }

        private static bool IsSameStruct(SuiStructTag lhs, StandardStruct rhs)
        {
            return
                lhs.address == rhs.Address &&
                lhs.module == rhs.Module &&
                lhs.name == rhs.Name;
        }
    }
}

