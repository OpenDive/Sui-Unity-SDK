using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Crypto.Digests;
using Sui.Types;
using UnityEngine;

namespace Sui.Utilities
{
    public static class Utils
    {
        public readonly static SuiStructTag SuiCoinStruct = new SuiStructTag("0x2::sui::SUI");

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

        public static string ByteArrayToString(this byte[] input)
        {
            return string.Join(", ", input);
        }

        public static bool IsValidSuiAddress(string address)
        {
            if (Utils.NormalizeSuiAddress(address) == null)
                return false;

            if (Regex.IsMatch(address, @"^(0x)?[0-9a-fA-F]{32,64}$") == false)
                return false;

            return true;
        }

        public static bool AreValidSuiAddresses(string[] addresses)
            => addresses.ToList().All(address => Utils.IsValidSuiAddress(address));

        public static bool AreValidSuiAddresses(List<string> addresses)
            => addresses.All(address => Utils.IsValidSuiAddress(address));

        public static bool IsValidTransactionDigest(string digest)
        {
            if (Utilities.Base58Encoder.IsValidEncoding(digest) == false)
                return false;

            NBitcoin.DataEncoders.Base58Encoder base58_encoder = new NBitcoin.DataEncoders.Base58Encoder();
            byte[] digest_data = base58_encoder.DecodeData(digest);

            return digest_data.Count() == 32;
        }

        public static bool AreValidTransactionDigests(string[] digests)
            => digests.ToList().All(digest => Utils.IsValidTransactionDigest(digest)) &&
               digests.Distinct().Count() == digests.Count();

        public static bool AreValidTransactionDigests(List<string> digests)
            => digests.All(digest => Utils.IsValidTransactionDigest(digest)) &&
               digests.Distinct().Count() == digests.Count();

        public static bool IsValidHexKey(string privateKey)
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
            return Convert.TryFromBase64String(base64, buffer, out _);
        }

        public static IEnumerator WaitForSecondsCoroutine(int seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        /// <summary>
        /// Generates a Blake2b hash of typed data as a base64 string.
        /// </summary>
        /// <param name="typeTag">type tag (e.g. TransactionData, SenderSignedData)</param>
        /// <param name="data">data to hash</param>
        /// <returns></returns>
        public static byte[] HashTypedData(string typeTag, byte[] data)
        {
            byte[] typeTagBytes = Encoding.ASCII.GetBytes(typeTag + "::");
            byte[] dataWithTag = new byte[typeTagBytes.Length + data.Length];
            Array.Copy(typeTagBytes, dataWithTag, typeTagBytes.Length);
            Array.Copy(data, 0, dataWithTag, typeTagBytes.Length, data.Length);

            // BLAKE2b hash
            byte[] result = new byte[32];
            Blake2bDigest blake2b = new(256);
            blake2b.BlockUpdate(dataWithTag, 0, dataWithTag.Length);
            blake2b.DoFinal(result, 0);

            return result;
        }

        public static string NormalizeSuiAddress(string address, bool forceAdd0x = false)
        {
            // If the address starts with "0x", remove it
            if (address.StartsWith("0x") && !forceAdd0x)
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