//
//  Utils.cs
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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Sui.Types;

namespace Sui.Utilities
{
    public static class Utils
    {
        /// <summary>
        /// Represents the SUI coin struct tag.
        /// </summary>
        public readonly static SuiStructTag SuiCoinStruct = new SuiStructTag("0x2::sui::SUI");

        /// <summary>
        /// The Base 58 characters.
        /// </summary>
        private static readonly char[] pszBase58 = "123456789ABCDEFGHJKLMNPQRSTUVWXYZabcdefghijkmnopqrstuvwxyz".ToCharArray();

        /// <summary>
        /// Converts a hexadecimal string to an array of bytes.
        /// The input string can start with a `0x` or not.
        /// </summary>
        /// <param name="input">Valid hexadecimal string</param>
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
        /// Fast check if the string to know if base58 str
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsValidEncoding(string str)
        {
            bool maybeb58 = true;
            if (maybeb58)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (!pszBase58.Contains(str[i]))
                    {
                        maybeb58 = false;
                        break;
                    }
                }
            }
            return maybeb58 && str.Count() > 0;
        }

        /// <summary>
        /// Determines whether or not an inputted address is a valid Sui hexadecimal address.
        /// </summary>
        /// <param name="address">The inputted address.</param>
        /// <returns>A bool representing the validity of the address as a Sui hexadecimal address.</returns>
        public static bool IsValidSuiAddress(string address)
        {
            if (Utils.NormalizeSuiAddress(address) == null)
                return false;

            if (Regex.IsMatch(address, @"^(0x)?[0-9a-fA-F]{32,64}$") == false)
                return false;

            return true;
        }

        /// <summary>
        /// Determines if a set of addresses is valid or not.
        /// </summary>
        /// <param name="addresses">The address array.</param>
        /// <returns>A bool representing whether the array of addresses are valid Sui Addresses.</returns>
        public static bool AreValidSuiAddresses(string[] addresses)
            => addresses.ToList().All(address => Utils.IsValidSuiAddress(address));

        /// <summary>
        /// Determines if a set of addresses is valid or not.
        /// </summary>
        /// <param name="addresses">The address array.</param>
        /// <returns>A bool representing whether the array of addresses are valid Sui Addresses.</returns>
        public static bool AreValidSuiAddresses(List<string> addresses)
            => addresses.All(address => Utils.IsValidSuiAddress(address));

        /// <summary>
        /// Determines whether if an inputted digest is a valid Sui transaction digest.
        /// </summary>
        /// <param name="digest">The inputted transaction digest.</param>
        /// <returns>A bool representing the validity of the inputted transaction digest as a Sui transaction.</returns>
        public static bool IsValidTransactionDigest(string digest)
        {
            if (Utils.IsValidEncoding(digest) == false)
                return false;

            NBitcoin.DataEncoders.Base58Encoder base58_encoder = new NBitcoin.DataEncoders.Base58Encoder();
            byte[] digest_data = base58_encoder.DecodeData(digest);

            return digest_data.Count() == 32;
        }

        /// <summary>
        /// Determines whether if inputted digests are valid Sui transaction digests.
        /// </summary>
        /// <param name="digests">The inputted transaction digests.</param>
        /// <returns>A bool representing the validity of the inputted transaction digests as Sui transactions.</returns>
        public static bool AreValidTransactionDigests(string[] digests)
            => digests.ToList().All(digest => Utils.IsValidTransactionDigest(digest)) &&
               digests.Distinct().Count() == digests.Count();

        /// <summary>
        /// Determines whether if inputted digests are valid Sui transaction digests.
        /// </summary>
        /// <param name="digests">The inputted transaction digests.</param>
        /// <returns>A bool representing the validity of the inputted transaction digests as Sui transactions.</returns>
        public static bool AreValidTransactionDigests(List<string> digests)
            => digests.All(digest => Utils.IsValidTransactionDigest(digest)) &&
               digests.Distinct().Count() == digests.Count();

        /// <summary>
        /// Determines whether an inputted string is a valid hexadecimal value.
        /// </summary>
        /// <param name="private_key">The inputted hexadecimal string.</param>
        /// <returns>A bool representing whether the inputted string is a valid hexadecimal value or not.</returns>
        public static bool IsValidHexKey(string private_key)
        {
            if (private_key[0..2].Equals("0x"))
                private_key = private_key[2..];

            string pattern = @"[a-fA-F0-9]{64}$";
            Regex rg = new Regex(pattern);
            return rg.IsMatch(private_key);
        }

        /// <summary>
        /// Check if the inputted string is a valid Base 64 string.
        /// </summary>
        /// <param name="base64">The inputted Base 64 string</param>
        /// <returns>A bool representing whether the string is a valid Base 64 string.</returns>
        public static bool IsBase64String(string base64)
        {
            Span<byte> buffer = new(new byte[base64.Length]);
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        /// <summary>
        /// Normalizes an inputted address.
        /// Normalization meaning the address is lengthed to exactly 64 bytes with padded zeros on the left.
        /// </summary>
        /// <param name="address">The inputted address.</param>
        /// <param name="force_add_0x">Whether to add in the additional "0x" at the beginning.</param>
        /// <returns>A string of the normalized Sui address.</returns>
        public static string NormalizeSuiAddress(string address, bool force_add_0x = false)
        {
            // If the address starts with "0x", remove it
            if (address.StartsWith("0x") && !force_add_0x)
                address = address[2..];

            // Ensure the address is not longer than the desired length
            if (address.Length > 64)
                return null;

            // Pad the address with zeros to reach 64 characters
            string paddedAddress = address.PadLeft(64, '0');

            // Return the normalized address with "0x" prefix
            return "0x" + paddedAddress;
        }
    }
}