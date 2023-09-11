using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Builder.TransactionObjects
{
    public class MoveCallTransaction : ITransactionType, ISerializable
    {
        public string Kind { get => "MoveCall"; }

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
        public StructTag Target { get; private set; }

        /// <summary>
        ///
        /// new TagSequence(new ISerializableTag[] { new StructTag(AccountAddress.FromHex("0x4"), "token", "Token", new ISerializableTag[0]) })
        /// </summary>
        public TagSequence TypeArguments { get; private set; }

        /// <summary>
        /// The sequence of arguments
        /// </summary>
        public Sequence Arguments { get; private set; }


        public MoveCallTransaction(StructTag target, ISerializableTag[] typeArguments, ISerializable[] arguments)
        {
            //ModuleId = moduleId;
            //Function = function;
            Target = target;
            TypeArguments = new TagSequence(typeArguments);
            Arguments = new Sequence(arguments);
        }

        public string EncodeTransaction()
        {
            throw new System.NotImplementedException();
        }

        public void Serialize(Serialization serializer)
        {
            // Check for kind
            serializer.SerializeU8(0);
            Target.Serialize(serializer);
            TypeArguments.Serialize(serializer);
            //Arguments.Serialize(serializer);
            // TODO: This fixes the extra bytes -- using ISerializable[] instead of sequence
            // 2,4,99,97,112,121,4,67,97,112,121,0,1,2,
            // 2,4,99,97,112,121,4,67,97,112,121,
            serializer.Serialize((ISerializable[])Arguments.GetValue());

            Debug.Log(" === MoveCallTransaction ::: ");
            Serialization ser = new Serialization();
            ser.SerializeU8(0);
            Target.Serialize(ser);
            TypeArguments.Serialize(ser);
            Arguments.Serialize(ser);
            Debug.Log(ser.GetBytes().ByteArrayToString());
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