using System;
using System.Text.RegularExpressions;

namespace Sui.Utils
{
    public static class Utils
    {
        /// <summary>
        /// Converts a hexadecimal string to an array of bytes.
        /// The input string can start with a `0x` or not.
        /// </summary>
        /// <param name="input"></param> Valid hexadecimal string
        /// <returns>Byte array representation of hexadecimal string</returns>
        public static byte[] HexStringToByteArray(this string input)
        {
            // Catch if a "0x" string is passed
            if (input.Substring(0, 2).Equals("0x"))
                input = input[2..];

            var outputLength = input.Length / 2;
            var output = new byte[outputLength];
            var numeral = new char[2];
            for (int i = 0; i < outputLength; i++)
            {
                input.CopyTo(i * 2, numeral, 0, 2);
                output[i] = Convert.ToByte(new string(numeral), 16);
            }
            return output;
        }

        /// <summary>
        /// Check if it's a valid hex address.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns>true if is a valid hex address, false otherwise.</returns>
        public static bool IsValidHexAddress(string walletAddress)
        {
            if (walletAddress[0..2].Equals("0x"))
                walletAddress = walletAddress[2..];

            string pattern = @"[a-fA-F0-9]{64}$";
            Regex rg = new Regex(pattern);
            return rg.IsMatch(walletAddress);
        }
    }
}