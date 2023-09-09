using System;
using System.Collections.Generic;

namespace Sui.Transactions.TransactionBlock
{
    public class ProtocolConfig 
    {
        public Dictionary<string, ProtocolConfigValue> Attributes { get; private set; }
        public Dictionary<string, bool> FeatureFlags { get; private set; }
        public string MaxSupportedProtocolVersion { get; private set; }
        public string MinSupportedProtocolVersion { get; private set; }
        public string ProtocolVersion { get; private set; }

        /// <summary>
        /// TODO: Update so that the values can be null, or request developer to pass null
        /// </summary>
        /// <param name="attributes"></param>
        /// <param name="featureFlags"></param>
        /// <param name="maxSupportedProtocolVersion"></param>
        /// <param name="minSupportedProtocolVersion"></param>
        /// <param name="MinSupportedProtocolVersion"></param>
        /// <param name="ProtocolVersion"></param>
        public ProtocolConfig(Dictionary<string, ProtocolConfigValue> attributes,
            Dictionary<string, bool> featureFlags, string maxSupportedProtocolVersion,
            string minSupportedProtocolVersion, string protocolVersion)
        {
            Attributes = attributes;
            FeatureFlags = featureFlags;
            MaxSupportedProtocolVersion = maxSupportedProtocolVersion;
            MinSupportedProtocolVersion = minSupportedProtocolVersion;
            ProtocolVersion = protocolVersion;
        }
    }

    public class ProtocolConfigValue
    {
        public enum DataType
        {
            U32,
            U64,
            F64
        }

        /// <summary>
        /// TODO: Look into whether we need to update the get to return "u32", "u64", "f64"
        /// </summary>
        public DataType ValueDataType { get; private set; }
        public string Data { get; private set; }

        public ProtocolConfigValue(DataType dataType, string data)
        {
            ValueDataType = dataType;
            Data = data;
        }
    }
}