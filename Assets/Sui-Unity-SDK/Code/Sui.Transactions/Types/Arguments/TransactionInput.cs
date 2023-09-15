using System;
using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Types.Arguments
{
    /// <summary>
    /// A transaction input represents anything from the list of `inputs`
    /// in the programmable transaction block,
    /// e.g. `pure` BCS value such as BString, U63, AccountAddress
    /// or a Sui object, e.g. ImmOrOwned Object Ref, Shared Object Ref
    /// https://github.com/MystenLabs/sui/blob/main/sdk/typescript/src/builder/Transactions.ts#L29
    /// </summary>
    public class TransactionInput : ITransactionArgument
    {
        public ITransactionArgument.Type Kind => ITransactionArgument.Type.Input;

        /// <summary>
        /// The index of 
        /// </summary>
        public int Index;
        public ISerializable Value; // Only used to prepare the transaction

        /// <summary>
        /// Creates a TransactionInput object.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="value">The value inside the TransactionInput
        /// 
        /// </param>
        public TransactionInput(int index, ISerializable value = null)
        {
            Index = index;
            Value = value;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU32AsUleb128((uint)ITransactionArgument.Type.Input);
            serializer.SerializeU16(Convert.ToUInt16(Index));

            Serialization ser = new Serialization();
            ser.SerializeU32AsUleb128((uint)ITransactionArgument.Type.Input);
            ser.SerializeU16(Convert.ToUInt16(Index));

            Debug.Log("==== Input");
            Debug.Log(ser.GetBytes().ByteArrayToString());
        }
    }

}