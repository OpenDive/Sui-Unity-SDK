using System.Collections.Generic;
using System.Linq;

namespace Sui.Utilities
{
    public static class ListExtensions
    {
        public static List<List<T>> Chunked<T>(this List<T> source, int size)
        {
            return Enumerable.Range(0, (source.Count + size - 1) / size)
                             .Select(i => source.Skip(i * size).Take(size).ToList())
                             .ToList();
        }
    }
}