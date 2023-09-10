using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using UnityEngine;

/// <summary>
/// TODO: Consider renaming to TransactionBlockInput
/// TODO: Missing implementation
/// </summary>
public class Input : ITransactionArgument
{
    public int Value;
    public Input(int value)
    {
        Value = value;
    }
    public void Serialize(Serialization serializer)
    {
        serializer.SerializeU16((ushort)Value);
    }
}
