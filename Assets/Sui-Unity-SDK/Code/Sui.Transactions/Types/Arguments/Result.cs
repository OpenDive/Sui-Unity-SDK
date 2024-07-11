using System;
using System.Collections;
using System.Collections.Generic;
using OpenDive.BCS;
using Sui.Utilities;
using UnityEngine;

namespace Sui.Transactions.Types.Arguments
{
    public class TransactionResult : IEnumerable<ITransactionArgument>, IEnumerator<ITransactionArgument>
    {
        private int _position = -1;

        public ITransactionArgument Current
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

        public IEnumerator<ITransactionArgument> GetEnumerator()
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
        private List<ITransactionArgument> _nestedResults;

        public ITransactionArgument TransactionArgument;

        public TransactionResult(ushort index, ushort? amount = null)
        {
            TransactionArgument = new Result(index);
            _count = amount ?? 1;
            _nestedResults = new List<ITransactionArgument>();
        }

        public ITransactionArgument NestedResultFor(ushort resultIndex)
        {
            int index = resultIndex + 1;
            if (index < _nestedResults.Count)
            {
                return _nestedResults[index];
            }

            if (TransactionArgument.Kind == Kind.Result)
            {
                var result = ((Result)TransactionArgument).Index;
                var nestedResult = new NestedResult(result, resultIndex);
                _nestedResults.Add(nestedResult);
                return nestedResult;
            }

            return null;
        }
    }

    /// <summary>
    /// The result of another transaction (from `ProgrammableTransactionBlock` transactions)
    /// </summary>
    public class Result : ITransactionArgument
    {
        Kind ITransactionArgument.Kind => Kind.Result;

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

        public static Result Deserialize(Deserialization deserializer)
        {
            return new Result(deserializer.DeserializeU16());
        }
    }
}