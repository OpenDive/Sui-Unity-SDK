using System;
using OpenDive.BCS;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Types
{
    /// <summary>
    /// Indicates the expiration time for a transaction.
    /// This can be a null values, an object with property `None` or
    /// an object with `Epoch` property.
    /// 
    /// TODO: Create a pull request in Sui GitHub to fix type in documentation
    /// https://github.com/MystenLabs/sui/blob/948be00ce391e300b17cca9b74c2fc3981762b87/sdk/typescript/src/bcs/index.ts#L102C4-L102C54
    /// <code>
    /// export type TransactionExpiration = { None: null } | { Epoch: number };
    /// </code>
    /// </summary>
    public class TransactionExpiration : ISerializable
    {
        private bool None { get; set; }
        private int Epoch { get; set; }

        public TransactionExpiration()
        {
            None = true;
        }

        /// <summary>
        /// TODO: Inquire on validation for epoch number
        /// </summary>
        /// <param name="epoch"></param>
        public TransactionExpiration(int epoch)
        {
            None = false;
            Epoch = epoch;
        }

        public void Serialize(Serialization serializer)
        {
            if (None == true)
                serializer.SerializeU8(0); // Nothing
            else
            {
                serializer.SerializeU8(1);
                serializer.SerializeU64((ulong)Epoch);
            }

            Debug.Log("TransactionExpiration ::: ");
            Debug.Log(serializer.GetBytes().ByteArrayToString());
        }

        public static ISerializable Deserialize(Deserialization deserialization)
        {
            throw new NotImplementedException();
        }
    }
}