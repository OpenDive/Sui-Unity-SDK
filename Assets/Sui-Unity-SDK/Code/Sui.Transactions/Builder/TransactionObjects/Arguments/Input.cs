using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Utilities;
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
        // TODO: Ask Marcus if Input has an enum value
        // It seems like there should be some extra bytes
        //serializer.SerializeU8(0);
        serializer.SerializeU8(1);
        serializer.SerializeU16((ushort)Value);

        Serialization ser = new Serialization();
        //ser.SerializeU8(0);
        ser.SerializeU8(1);
        ser.SerializeU16((ushort)Value);

        Debug.Log("==== Input");
        Debug.Log(ser.GetBytes().ByteArrayToString());
    }
}
