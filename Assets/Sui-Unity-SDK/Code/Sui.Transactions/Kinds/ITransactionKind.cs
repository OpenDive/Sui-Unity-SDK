using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using UnityEngine;

public interface ITransactionKind : ISerializable
{
    public enum Kind
    {
        ProgrammableTransaction,
        ChangeEpoch,
        Genesis,
        ConsensusCommitPrologue
    }
}
