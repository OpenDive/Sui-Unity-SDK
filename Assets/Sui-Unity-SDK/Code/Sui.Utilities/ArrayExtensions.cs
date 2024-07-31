namespace Sui.Utilities
{
    public static class ArrayExtensions
    {
        /// <summary>
        /// Print a byte array into a readeable string
        /// </summary>
        /// <param name="input">An input array of any object.</param>
        /// <returns>A string of the array's representation.</returns>
        public static string ToReadableString<T>(this T[] input) => $"[{string.Join(", ", input)}]";
    }
}