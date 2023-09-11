using System.Collections;
using System.Collections.Generic;
using Sui.Accounts;
using Sui.BCS;
using Sui.Transactions.Builder.TransactionObjects;
using UnityEngine;

namespace Sui.Transactions.Builder
{
    /// <summary>
    /// Represents the input and output of the TransactionBlockDataBuilder
    /// In the TypeScript SDK, this would be `SerializedTransactionDataBuilder`
    /// </summary>
    public class TransactionBlockData
    {
        public int Version { get; private set; }
        public AccountAddress Sender { get; private set; }
        public TransactionExpiration Expiration { get; private set; }
        public GasConfig GasConfig { get; private set; }
        public TransactionBlockInput[] Inputs { get; private set; }
        public ITransactionType[] Transactions { get; private set; }

        public TransactionBlockData(int version, AccountAddress sender,
            TransactionExpiration expiration, GasConfig gasConfig,
            TransactionBlockInput[] inputs, ITransactionType[] transactions)
        {
            this.Version = version;
            this.Sender = sender;
            this.Expiration = expiration;
            this.GasConfig = gasConfig;
            this.Inputs = inputs;
            this.Transactions = transactions;
        }
    }
}