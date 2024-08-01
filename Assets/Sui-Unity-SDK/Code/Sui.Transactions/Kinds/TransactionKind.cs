//
//  TransactionKind.cs
//  Sui-Unity-SDK
//
//  Copyright (c) 2024 OpenDive
//
//  Permission is hereby granted, free of charge, to any person obtaining a copy
//  of this software and associated documentation files (the "Software"), to deal
//  in the Software without restriction, including without limitation the rights
//  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//  copies of the Software, and to permit persons to whom the Software is
//  furnished to do so, subject to the following conditions:
//
//  The above copyright notice and this permission notice shall be included in
//  all copies or substantial portions of the Software.
//
//  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
//  THE SOFTWARE.
//

using Newtonsoft.Json;
using OpenDive.BCS;
using Sui.Utilities;
using System;

namespace Sui.Transactions
{
    /// <summary>
    /// Represents a Sui transaction and its type
    /// (e.g., Programmable Transaction,
    /// Genesis, Change Epoch, Consensus Commit Prologue).
    /// </summary>
    [JsonConverter(typeof(TransactionKindConverter))]
    public class TransactionKind : ReturnBase, ISerializable
    {
        /// <summary>
        /// The transaction's type.
        /// </summary>
        public TransactionKindType Type { get; internal set; }

        /// <summary>
        /// The internal representation of a transaction.
        /// </summary>
        private ITransactionKind transaction;

        /// <summary>
        /// The public representation of a transaction.
        /// 
        /// This has a protected set function to change the enum's
        /// type in accordance with the value's type.
        /// </summary>
        public ITransactionKind Transaction
        {
            get => this.transaction;
            set
            {
                if (value.GetType() == typeof(ProgrammableTransaction))
                    this.Type = TransactionKindType.ProgrammableTransaction;
                else if (value.GetType() == typeof(SuiChangeEpoch))
                    this.Type = TransactionKindType.ChangeEpoch;
                else if (value.GetType() == typeof(Genesis))
                    this.Type = TransactionKindType.Genesis;
                else if (value.GetType() == typeof(SuiConsensusCommitPrologue))
                    this.Type = TransactionKindType.ConsensusCommitPrologue;
                else
                    throw new Exception("Unable to set Transaction Kind");

                this.transaction = value;
            }
        }

        public TransactionKind
        (
            TransactionKindType type,
            ITransactionKind transaction
        )
        {
            this.Type = type;
            this.transaction = transaction;
        }

        public TransactionKind(SuiError error)
        {
            this.Error = error;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU8((byte)this.Type);
            serializer.Serialize(this.Transaction);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            byte type = deserializer.DeserializeU8().Value;
            switch (type)
            {
                case 0:
                    return new TransactionKind
                    (
                        TransactionKindType.ProgrammableTransaction,
                        (ProgrammableTransaction)ProgrammableTransaction.Deserialize(deserializer)
                    );
                case 1:
                    return new TransactionKind
                    (
                        TransactionKindType.ChangeEpoch,
                        (SuiChangeEpoch)SuiChangeEpoch.Deserialize(deserializer)
                    );
                case 2:
                    return new TransactionKind
                    (
                        TransactionKindType.Genesis,
                        (Genesis)Genesis.Deserialize(deserializer)
                    );
                case 3:
                    return new TransactionKind
                    (
                        TransactionKindType.ConsensusCommitPrologue,
                        (SuiConsensusCommitPrologue)SuiConsensusCommitPrologue.Deserialize(deserializer)
                    );
                default:
                    return new SuiError(0, "Unable to deserialize TransactionBlockKind", null);
            }
        }
    }
}