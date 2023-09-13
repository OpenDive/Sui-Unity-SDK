using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Types.Arguments;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Types
{
    //public class NormalizedStructTag : StructTag
    //{
    //    public NormalizedStructTag(AccountAddress address, string module, string name, ISerializableTag[] typeArgs) : base()
    //    {
    //        this.address = address;
    //        this.module = module;
    //        this.name = name;
    //        this.typeArgs = typeArgs;
    //    }
    //}

    public class MoveCall : ITransaction, ISerializable
    {
        public ITransaction.Kind Kind
        {
            get => ITransaction.Kind.MoveCall;
        }

        /// <summary>
        /// The module id that contains the target function
        /// Represents the following:
        /// address_hex::module::name
        ///
        /// new ModuleId(AccountAddress.FromHex("0x4"), "aptos_token"), "burn"
        /// </summary>
        public ModuleId ModuleId { get; private set; }

        /// <summary>
        /// The target function name
        /// </summary>
        public string Function { get; private set; }

        /// <summary>
        /// The target
        /// </summary>
        public SuiStructTag Target { get; private set; }

        /// <summary>
        ///
        /// new TagSequence(new ISerializableTag[] { new StructTag(AccountAddress.FromHex("0x4"), "token", "Token", new ISerializableTag[0]) })
        /// </summary>
        public TagSequence TypeArguments { get; private set; }

        /// <summary>
        /// The sequence of arguments
        /// </summary>
        public ISerializable[] Arguments { get; private set; }


        public MoveCall(SuiStructTag target,
            ISerializableTag[] typeArguments, ITransactionArgument[] arguments)
        {
            //ModuleId = moduleId;
            //Function = function;
            Target = target;
            TypeArguments = new TagSequence(typeArguments);
            Arguments = arguments;
        }

        public string EncodeTransaction()
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(Serialization serializer)
        {
            // Check for kind
            serializer.SerializeU8(0);
            serializer.Serialize(Target);
            serializer.Serialize(TypeArguments);
            serializer.Serialize(Arguments);

            Serialization ser = new Serialization();
            ser.SerializeU8(0);
            Debug.Log(" === MoveCallTransaction ::: 1 :: " + ser.GetBytes().ByteArrayToString());
            Target.Serialize(ser);
            Debug.Log(" === MoveCallTransaction ::: 2 :: " + ser.GetBytes().ByteArrayToString());
            ser.Serialize((ISerializableTag[])TypeArguments.GetValue());
            Debug.Log(" === MoveCallTransaction ::: 3 :: " + ser.GetBytes().ByteArrayToString());
            ser.Serialize(Arguments);
            Debug.Log(" === MoveCallTransaction ::: 4 :: " + ser.GetBytes().ByteArrayToString());
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            BString target = BString.Deserialize(deserializer);
            TagSequence typeArguments = TagSequence.Deserialize(deserializer);
            Sequence arguments = Sequence.Deserialize(deserializer);

            string targetStr = (string)target.GetValue();
            string[] split = targetStr.Split("::");
            ModuleId moduleId = new ModuleId(AccountAddress.FromHex(split[0]), split[1]);
            string function = split[2];

            //return new MoveCallTransaction(
            //    moduleId,
            //    function,
            //    (ISerializableTag[])typeArguments.GetValue(),
            //    (ISerializable[])arguments.GetValue()
            //);

            throw new NotImplementedException();
        }
    }
}