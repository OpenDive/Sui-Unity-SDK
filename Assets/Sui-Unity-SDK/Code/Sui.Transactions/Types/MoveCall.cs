using System;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Transactions.Types.Arguments;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Types
{
    /// <summary>
    ///
    /// <code>
    ///     Executes a Move call. Returns whatever the Sui Move call returns.
    ///     txb.moveCall({ target, arguments, typeArguments  })
    ///
    ///     Example: 
    ///     txb.moveCall({ target: '0x2::devnet_nft::mint', arguments: [txb.pure(name), txb.pure(description), txb.pure(image)] }
    /// 
    ///     // Split a coin object off of the gas object:
    ///     const [coin] = txb.splitCoins(txb.gas, [txb.pure(100)]);
    ///     // Transfer the resulting coin object:
    ///     txb.transferObjects([coin], txb.pure(address));
    /// 
    ///     // Destructuring (preferred, as it gives you logical local names):
    ///     const [nft1, nft2] = txb.moveCall({ target: "0x2::nft::mint_many" });
    ///     txb.transferObjects([nft1, nft2], txb.pure(address));
    ///
    ///     //Array indexes:
    ///     const mintMany = txb.moveCall({ target: "0x2::nft::mint_many" });
    ///     txb.transferObjects([mintMany[0], mintMany[1]], txb.pure(address));
    /// </code> 
    ///
    /// https://github.com/MystenLabs/sui/blob/main/doc/src/build/prog-trans-ts-sdk.md
    /// </summary>
    public class MoveCall : ITransaction, ISerializable
    {
        public Kind Kind
        {
            get => Kind.MoveCall;
        }

        /// <summary>
        /// The module id that contains the target function
        /// Represents the following:
        /// address_hex::module::name
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
        public ITransactionArgument[] Arguments { get; private set; }

        /// <summary>
        /// Create a MoveCall transaction
        /// </summary>
        /// <param name="target"></param>
        /// <param name="typeArguments"></param>
        /// <param name="arguments">
        ///     These will be either `Result` or `TransactionBlockInput`:
        ///     
        ///     We should check whether it's not a 'Result'
        ///     and convert it to `TransactionBlockInput`.
        /// </param>
        public MoveCall(
            SuiStructTag target,
            ISerializableTag[] typeArguments = null,
            ITransactionArgument[] arguments = null)
        {
            Target          = target;
            TypeArguments   = new TagSequence(typeArguments);
            Arguments       = arguments;
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