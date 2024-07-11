using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using UnityEngine;

namespace Sui.Transactions.Kinds
{
    public enum SuiTransactionKindType
    {
        ProgrammableTransaction,
        ChangeEpoch,
        Genesis,
        ConsensusCommitPrologue
    }

    public interface ITransactionKind : ISerializable
    {
        public SuiTransactionKindType Type { get; }
    }
}
