using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using UnityEngine;

namespace Sui.Transactions.Types.Arguments
{
    public interface ITransactionArgument : ISerializable
    {
        public enum Type
        {
            GasCoin,
            Input,
            Result,
            NestedResult
        }
    }

}