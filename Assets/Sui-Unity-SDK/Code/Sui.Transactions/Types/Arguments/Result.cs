using System;
using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Types.Arguments
{
    public class TransactionResult : IEnumerable<SuiTransactionArgument>, IEnumerator<SuiTransactionArgument>
    {
        private int _position = -1;

        public SuiTransactionArgument Current
        {
            get
            {
                return NestedResultFor((ushort)(_count - _position - 1));
            }
        }

        object IEnumerator.Current => Current;

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public IEnumerator<SuiTransactionArgument> GetEnumerator()
        {
            return this;
        }

        public bool MoveNext()
        {
            _position++;
            return (_position < _count);
        }

        public void Reset()
        {
            _position = -1;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private ushort _count;
        private List<SuiTransactionArgument> _nestedResults;

        public SuiTransactionArgument TransactionArgument;

        public TransactionResult(ushort index, ushort? amount = null)
        {
            TransactionArgument = new SuiTransactionArgument(TransactionArgumentKind.Result, new Result(index));
            _count = amount ?? 1;
            _nestedResults = new List<SuiTransactionArgument>();
        }

        public SuiTransactionArgument NestedResultFor(ushort resultIndex)
        {
            int index = resultIndex + 1;
            if (index < _nestedResults.Count)
            {
                return _nestedResults[index];
            }

            if (TransactionArgument.Kind == TransactionArgumentKind.Result)
            {
                int result = ((Result)TransactionArgument.TransactionArgument).Index;
                NestedResult nestedResult = new NestedResult(result, resultIndex);
                SuiTransactionArgument argument = new SuiTransactionArgument(TransactionArgumentKind.NestedResult, nestedResult);
                _nestedResults.Add(argument);
                return argument;
            }

            return null;
        }
    }

    /// <summary>
    /// The result of another transaction (from `ProgrammableTransactionBlock` transactions)
    /// </summary>
    public class Result : ITransactionArgument
    {
        public int Index { get; private set; }

        /// <summary>
        /// Represents a result from a given transaction.
        /// The index is the location of the result within the
        /// </summary>
        /// <param name="index"></param>
        public Result(int index)
        {
            Index = index;
        }

        public void Serialize(Serialization serializer)
        {
            serializer.SerializeU16((ushort)Index);
        }

        public static ISerializable Deserialize(Deserialization deserializer)
        {
            return new Result(deserializer.DeserializeU16());
        }
    }
}