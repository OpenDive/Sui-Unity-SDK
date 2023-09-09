using System.Collections.Generic;

namespace Sui.Transactions.TransactionBlock
{
    public class BuilidOptions
    {
        // public SuiClient Client { get; private set; } // TODO: Implement Sui Client
        public bool OnlyTransactionKind { get; private set; }
        public ProtocolConfig ProtocolConfigArg { get; private set; }
        public Limits LimitsArg { get; private set; }

        public enum Limits
        {
            MaxTxGas,
            MaxGasObjects,
            MaxTxSizeBytes,
            MaxPureArgumentSize
        }

        public static string MaxTxGas { get => "max_tx_gas"; }

        public BuilidOptions(bool onlyTransactionKind, ProtocolConfig protocolConfig, Limits limits)
        {
            OnlyTransactionKind = onlyTransactionKind;
            ProtocolConfigArg = protocolConfig;
            LimitsArg = limits;
        }
    }

    public class Limits
    {
        public int MaxTxGasValue { get; private set; }
        public int MaxGasObjectsValue { get; private set; }
        public int MaxTxSizeBytesValue { get; private set; }
        public int MaxPureArgumentSizeValue { get; private set; }

        public static string MaxTxGasKey = "max_tx_gas";
        public static string MaxGasObjectsKey = "max_tx_gas";
        public static string MaxTxSizeBytesKey = "max_tx_gas";
        public static string MaxPureArgumentSizeKey = "max_tx_gas";

        //public Dictionary<BuilidOptions.Limits, string> _limitKeys = new()
        //{
        //    { BuilidOptions.Limits.MaxTxGas, "max_tx_gas" },
        //    // The maximum number of gas objects that can be selected for one transaction.
        //    { BuilidOptions.Limits.MaxGasObjects, "max_gas_payment_objects" },
        //    // The maximum size (in bytes) that the transaction can be.
        //    { BuilidOptions.Limits.MaxTxSizeBytes,  "max_tx_size_bytes" },
        //    // The maximum size (in bytes) that pure arguments can be.
        //    { BuilidOptions.Limits.MaxPureArgumentSize, "max_pure_argument_size" }
        //};

        /// <summary>
        /// Defines the limit configurations. Define as null is we are configuring.
        /// </summary>
        /// <param name="maxTxGas"></param>
        /// <param name="maxGasObjects"></param>
        /// <param name="maxTxSizeBytes"></param>
        /// <param name="maxPureArgumentSize"></param>
        public Limits(int maxTxGas, int maxGasObjects, int maxTxSizeBytes, int maxPureArgumentSize)
        {
            MaxTxGasValue = maxTxGas;
            MaxGasObjectsValue = maxGasObjects;
            MaxTxSizeBytesValue = maxTxSizeBytes;
            MaxPureArgumentSizeValue = maxPureArgumentSize;
        }

        public static string GetKey(BuilidOptions.Limits limit)
        {
            return limit switch
            {
                BuilidOptions.Limits.MaxTxGas => MaxTxSizeBytesKey,
                BuilidOptions.Limits.MaxGasObjects => MaxGasObjectsKey,
                BuilidOptions.Limits.MaxTxSizeBytes => MaxTxSizeBytesKey,
                BuilidOptions.Limits.MaxPureArgumentSize => MaxPureArgumentSizeKey,
                _ => throw new KeyNotFoundException(),
            };
        }
    }
}