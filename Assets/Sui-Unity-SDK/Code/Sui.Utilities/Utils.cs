using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Sui.Utilities
{
    public static class Utils
    {
        public static bool Equals(this byte[] lhs, byte[] rhs)
        {
            if (lhs.Length != rhs.Length) return false;
#if !DOTNET35
            for(int i = 0; i < lhs.Length; i++)
            {
                if (lhs[i] != rhs[i]) return false;
            }
            return true;
#else
            return lhs.SequenceEqual(rhs);
#endif
        }

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


        public static string ToString(this byte[] input)
        {
            return string.Join(", ", input);
        }

        /// <summary>
        /// Check if it's a valid hex address.
        /// </summary>
        /// <param name="walletAddress"></param>
        /// <returns>true if is a valid hex address, false otherwise.</returns>
        //public static bool IsValidHexAddress(string walletAddress)
        //{
        //    if (walletAddress[0..2].Equals("0x"))
        //        walletAddress = walletAddress[2..];

        //    string pattern = @"[a-fA-F0-9]{64}$";
        //    Regex rg = new Regex(pattern);
        //    return rg.IsMatch(walletAddress);
        //}

        public static bool IsValidEd25519HexKey(string privateKey)
        {
            if (privateKey[0..2].Equals("0x"))
                privateKey = privateKey[2..];

            string pattern = @"[a-fA-F0-9]{64}$";
            Regex rg = new Regex(pattern);
            return rg.IsMatch(privateKey);
        }

        /// <summary>
        /// Check if it's a valid base64 string
        /// </summary>
        /// <param name="base64">Base64 string</param>
        /// <returns></returns>
        public static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out int bytesParsed);
        }

        /// <summary>
        /// Print a byte array into a readeable string
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static string ToReadableString(this byte[] input)
        {
            return string.Join(", ", input);
        }
    }
}