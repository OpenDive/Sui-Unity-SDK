using UnityEngine;

namespace Sui.Rpc.Models
{
    public class MoveNormalizedType
    {

    }

    public class NormalizedMoveFunction
    {
        public MoveVisibility Visibility;
        public bool IsEntry;
        public MoveAbilitySet[] TypeParameters;

    }

    public enum MoveVisibility
    {
        Private,
        Public,
        Friend
    }

    public class MoveAbilitySet
    {
        public string[] Abilities;
    }
}