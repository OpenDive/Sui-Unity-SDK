using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Sui.Rpc.Models
{
    /// <summary>
    /// Represents  protocol config table for the given version number.
    /// If the version number is not specified, If none is specified,
    /// the node uses the version of the latest epoch it has processed.
    /// <code>
    ///
    /// </code>
    /// </summary>
    public class ProtocolConfig
    {
        [JsonProperty("minSupportedProtocolVersion")]
        public string MinSupportedProtocolVersion { get; set; }
        [JsonProperty("maxSupportedProtocolVersion")]
        public string MaxSupportedProtocolVersion { get; set; }
        [JsonProperty("protocolVersion")]
        public string ProtocolVersion { get; set; }
        [JsonProperty("featureFlags")]
        public FeatureFlags FeatureFlags { get; set; }
        [JsonProperty("attributes")]
        public Dictionary<string, AttributeValue> Attributes { get; set; }
    }

    public class FeatureFlags
    {
        [JsonProperty("advance_epoch_start_time_in_safe_mode")]
        public bool AdvanceEpochStartTimeInSafeMode { get; set; }
        [JsonProperty("advance_to_highest_supported_protocol_version")]
        public bool AdvanceToHighestSupportedProtocolVersion { get; set; }
        [JsonProperty("ban_entry_init")]
        public bool BanEntryInit { get; set; }
        [JsonProperty("commit_root_state_digest")]
        public bool CommitRootStateDigest { get; set; }
        [JsonProperty("consensus_order_end_of_epoch_last")]
        public bool ConsensusOrderEndOfEpochLast { get; set; }
        [JsonProperty("disable_invariant_violation_check_in_swap_loc")]
        public bool DisableInvariantViolationCheckInSwapLoc { get; set; }
        [JsonProperty("disallow_adding_abilities_on_upgrade")]
        public bool DisallowAddingAbilitiesOnUpgrade { get; set; }
        [JsonProperty("disallow_change_struct_type_params_on_upgrade")]
        public bool DisallowChangeStructTypeParamsOnUpgrade { get; set; }
        [JsonProperty("enable_jwk_consensus_updates")]
        public bool EnableJwkConsensusUpdates { get; set; }
        [JsonProperty("end_of_epoch_transaction_supported")]
        public bool EndOfEpochTransactionSupported { get; set; }
        [JsonProperty("loaded_child_object_format")]
        public bool LoadedChildObjectFormat { get; set; }
        [JsonProperty("loaded_child_object_format_type")]
        public bool LoadedChildObjectFormatType { get; set; }
        [JsonProperty("loaded_child_objects_fixed")]
        public bool LoadedChildObjectsFixed { get; set; }
        [JsonProperty("missing_type_is_compatibility_error")]
        public bool MissingTypeIsCompatibilityError { get; set; }
        [JsonProperty("narwhal_new_leader_election_schedule")]
        public bool NarwhalNewLeaderElectionSchedule { get; set; }
        [JsonProperty("narwhal_versioned_metadata")]
        public bool NarwhalVersionedMetadata { get; set; }
        [JsonProperty("no_extraneous_module_bytes")]
        public bool NoExtraneousModuleBytes { get; set; }
        [JsonProperty("package_digest_hash_module")]
        public bool PackageDigestHashModule { get; set; }
        [JsonProperty("package_upgrades")]
        public bool PackageUpgrades { get; set; }
        [JsonProperty("scoring_decision_with_validity_cutoff")]
        public bool ScoringDecisionWithValidityCutoff { get; set; }
        [JsonProperty("simple_conservation_checks")]
        public bool SimpleConservationChecks { get; set; }
        [JsonProperty("simplified_unwrap_then_delete")]
        public bool SimplifiedUnwrapThenDelete { get; set; }
        [JsonProperty("txn_base_cost_as_multiplier")]
        public bool TxnBaseCostAsMultiplier { get; set; }
        [JsonProperty("upgraded_multisig_supported")]
        public bool UpgradedMultisigSupported { get; set; }
        [JsonProperty("zklogin_auth")]
        public bool ZkloginAuth { get; set; }
    }

    public class AttributeValue
    {
        [JsonProperty("u64")]
        public string U64 { get; set; }

        [JsonProperty("u32")]
        public string U32 { get; set; }

        [JsonProperty("null")]
        public string Null { get; set; }
    }

}