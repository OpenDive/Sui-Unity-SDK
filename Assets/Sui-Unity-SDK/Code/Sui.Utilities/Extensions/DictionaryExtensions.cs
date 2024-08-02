using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sui.Utilities
{
    public static class DictionaryExtensions
    {
        public static string GetValuesAsString<TKey, TValue>(this Dictionary<TKey, TValue> dictionary)
        {
            StringBuilder sb = new StringBuilder();

            foreach (var kvp in dictionary)
            {
                string valueString;

                if (kvp.Value is byte[] byteArray)
                {
                    valueString = BitConverter.ToString(byteArray).Replace("-", " ");
                }
                else if (kvp.Value is IEnumerable<byte> byteEnumerable)
                {
                    valueString = BitConverter.ToString(byteEnumerable.ToArray()).Replace("-", " ");
                }
                else
                {
                    valueString = kvp.Value?.ToString() ?? "null";
                }

                sb.AppendLine($"{kvp.Key}: {valueString}");
            }

            return sb.ToString();
        }
    }
}