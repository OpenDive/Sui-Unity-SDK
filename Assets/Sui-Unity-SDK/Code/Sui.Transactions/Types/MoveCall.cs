using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenDive.BCS;
using Sui.Accounts;
using Sui.Rpc.Models;
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
        /// <summary>
        /// The target
        /// </summary>
        public SuiMoveNormalizedStructType Target { get; private set; }

        /// <summary>
        ///
        /// new TagSequence(new ISerializableTag[] { new StructTag(AccountAddress.FromHex("0x4"), "token", "Token", new ISerializableTag[0]) })
        /// </summary>
        public SerializableTypeTag[] TypeArguments { get; private set; }

        /// <summary>
        /// The sequence of arguments
        /// </summary>
        public SuiTransactionArgument[] Arguments { get; private set; }

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
            SuiMoveNormalizedStructType target,
            SerializableTypeTag[] typeArguments = null,
            SuiTransactionArgument[] arguments = null)
        {
            Target          = target;
            TypeArguments   = typeArguments;
            Arguments       = arguments;
        }

        public MoveCall(JToken input)
        {
            this.Target = input.ToObject<SuiMoveNormalizedStructType>();
            this.TypeArguments = new SerializableTypeTag[] { };

            List<SuiTransactionArgument> arguments = new List<SuiTransactionArgument>();
            foreach (JToken arg in (JArray)input["arguments"])
                arguments.Add(arg.ToObject<SuiTransactionArgument>());

            this.Arguments = arguments.ToArray();
        }

        public void Serialize(Serialization serializer)
        {
            serializer.Serialize(Target);
            serializer.Serialize(TypeArguments);
            serializer.Serialize(Arguments);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            SuiMoveNormalizedStructType target = (SuiMoveNormalizedStructType)SuiMoveNormalizedStructType.Deserialize(deserializer);
            SerializableTypeTag[] type_arguments = deserializer.DeserializeSequence(typeof(SerializableTypeTag)).Values.Cast<SerializableTypeTag>().ToArray();
            SuiTransactionArgument[] arguments = deserializer.DeserializeSequence(typeof(SuiTransactionArgument)).Values.Cast<SuiTransactionArgument>().ToArray();

            return new MoveCall(
                target,
                type_arguments,
                arguments
            );
        }
    }
}