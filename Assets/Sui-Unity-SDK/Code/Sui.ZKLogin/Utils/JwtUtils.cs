using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace Sui.ZKLogin
{
    /// <summary>
    /// Represents a claim with a base64URL encoded value and its position indicator
    /// </summary>
    [Serializable]
    public class Claim
    {
        /// <summary>
        /// The base64URL encoded value of the claim
        /// </summary>
        public string value;

        /// <summary>
        /// The position indicator modulo 4, used for decoding
        /// </summary>
        public int indexMod4;
    }

    /// <summary>
    /// TODO: Add tests for JWT Utils
    /// </summary>
    public class JwtUtils
    {
        /// <summary>
        /// The standard base64URL character set used for encoding/decoding
        /// </summary>
        private static readonly string Base64UrlCharacterSet = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-_";

        /// <summary>
        /// Converts a single base64URL character to its 6-bit binary representation
        /// </summary>
        /// <param name="base64UrlChar">A single character from the base64URL character set</param>
        /// <returns>Array of 6 bits representing the character</returns>
        /// <exception cref="ArgumentException">Thrown when the input is not a valid base64URL character</exception>
        private static int[] Base64UrlCharTo6Bits(string base64UrlChar)
        {
            if (base64UrlChar.Length != 1)
                throw new ArgumentException("Invalid base64Url character: " + base64UrlChar);

            int index = Base64UrlCharacterSet.IndexOf(base64UrlChar);
            if (index == -1)
                throw new ArgumentException("Invalid base64Url character: " + base64UrlChar);

            // Convert the index to a 6-bit binary representation
            string binaryString = Convert.ToString(index, 2).PadLeft(6, '0');
            int[] bits = new int[6];
            for (int i = 0; i < 6; i++)
                bits[i] = binaryString[i] == '1' ? 1 : 0;
            return bits;
        }

        /// <summary>
        /// Converts a base64URL encoded string to a bit vector
        /// </summary>
        /// <param name="base64UrlString">The base64URL encoded string</param>
        /// <returns>Array of bits representing the entire string</returns>
        private static int[] Base64UrlStringToBitVector(string base64UrlString)
        {
            List<int> bitVector = new List<int>();
            for (int i = 0; i < base64UrlString.Length; i++)
            {
                string base64UrlChar = base64UrlString[i].ToString();
                int[] bits = Base64UrlCharTo6Bits(base64UrlChar);
                bitVector.AddRange(bits);
            }
            return bitVector.ToArray();
        }

        /// <summary>
        /// Encodes the input byte array or string to a Base64 URL-safe string.
        /// </summary>
        /// <param name="input">The input as a string.</param>
        /// <returns>A Base64 URL-safe encoded string.</returns>
        public static string Base64UrlEncode(string input)
        {
            byte[] data = Encoding.UTF8.GetBytes(input);
            return Base64UrlEncode(data);
        }

        /// <summary>
        /// Encodes the input byte array or string to a Base64 URL-safe string.
        /// </summary>
        /// <param name="input">The input as a byte array.</param>
        /// <returns>A Base64 URL-safe encoded string.</returns>
        public static string Base64UrlEncode(byte[] input)
        {
            string base64 = Convert.ToBase64String(input);
            return base64
                .Replace('+', '-')
                .Replace('/', '_')
                .TrimEnd('=');
        }

        /// <summary>
        /// Decodes a base64URL encoded string starting from a specific position
        /// </summary>
        /// <param name="s">The base64URL encoded string</param>
        /// <param name="i">The starting position for decoding</param>
        /// <returns>The decoded UTF8 string</returns>
        /// <exception cref="ArgumentException">Thrown when the input is not properly formatted or positioned</exception>
        private static string DecodeBase64URL(string s, int i)
        {
            if (s.Length < 2)
                throw new ArgumentException($"Input (s = {s}) is not tightly packed because s.length < 2");

            List<int> bits = new List<int>(Base64UrlStringToBitVector(s));

            // Handle the first character offset
            int firstCharOffset = i % 4;
            switch (firstCharOffset)
            {
                case 1:
                    bits.RemoveRange(0, 2);
                    break;
                case 2:
                    bits.RemoveRange(0, 4);
                    break;
                case 3:
                    throw new ArgumentException($"Input (s = {s}) is not tightly packed because i%4 = 3 (i = {i})");
            }

            // Handle the last character offset
            int lastCharOffset = (i + s.Length - 1) % 4;
            switch (lastCharOffset)
            {
                case 2:
                    bits.RemoveRange(bits.Count - 2, 2); // Remove last 2 bits
                    break;
                case 1:
                    bits.RemoveRange(bits.Count - 4, 4); // Remove last 4 bits
                    break;
                case 0:
                    throw new ArgumentException($"Input (s = {s}) is not tightly packed because (i + s.length - 1)%4 = 0 (i = {i})");
            }

            if (bits.Count % 8 != 0)
                throw new Exception("We should never reach here...");

            // Convert bit groups of 8 to bytes
            byte[] bytes = new byte[bits.Count / 8];
            for (int byteIndex = 0; byteIndex < bytes.Length; byteIndex++)
            {
                string bitChunk = string.Join("", bits.GetRange(byteIndex * 8, 8));
                bytes[byteIndex] = Convert.ToByte(bitChunk, 2);
            }

            return Encoding.UTF8.GetString(bytes);
        }

        [Serializable]
        private class ClaimWrapper
        {
            public string key;
            public string value;
        }


        /// <summary>
        /// Verifies and extracts the key-value pair from a claim string
        /// </summary>
        /// <param name="claim">The claim string to verify</param>
        /// <returns>A tuple containing the key and value from the claim</returns>
        /// <exception cref="ArgumentException">Thrown when the claim format is invalid</exception>
        //private static (string key, string value) VerifyExtendedClaim(string claim)
        //{
        //    // Verify claim ends with either '}' or ','
        //    if (!(claim.EndsWith("}") || claim.EndsWith(",")))
        //        throw new ArgumentException("Invalid claim");

        //    // Parse the claim as JSON
        //    Debug.Log("CLAIM: " + claim);
        //    string jsonStr = "{" + claim.Substring(0, claim.Length - 1) + "}";
        //    Debug.Log("JSON STR: " + jsonStr);
        //    var json = JsonUtility.FromJson<Dictionary<string, object>>(jsonStr);

        //    // Verify the claim contains exactly one key-value pair
        //    if (json.Count != 1)
        //        throw new ArgumentException("Invalid claim");

        //    // Return the first (and only) key-value pair
        //    foreach (var kvp in json)
        //        return (kvp.Key, kvp.Value.ToString());

        //    throw new ArgumentException("Invalid claim");
        //}

        private static (string key, string value) VerifyExtendedClaim(string claim)
        {
            Debug.Log($"1. Input claim string: {claim}");

            if (!(claim.EndsWith("}") || claim.EndsWith(",")))
            {
                Debug.Log($"2. Claim doesn't end with }} or ,");
                throw new ArgumentException("Invalid claim");
            }

            // Parse the claim as JSON
            string jsonStr = "{" + claim.Substring(0, claim.Length - 1) + "}";
            Debug.Log($"3. Final JSON string to parse: {jsonStr}");

            try
            {
                var wrapper = JsonUtility.FromJson<ClaimWrapper>(jsonStr);
                Debug.Log($"4. Parsed JSON wrapper: {wrapper?.key}");

                if (string.IsNullOrEmpty(wrapper?.key))
                    throw new ArgumentException("Claim must contain exactly one key-value pair");

                // Split the key-value pair
                var parts = wrapper.key.Split(new[] { ':' }, 2);
                if (parts.Length != 2)
                    throw new ArgumentException("Invalid claim format");

                return (parts[0].Trim('"'), parts[1].Trim());
            }
            catch (Exception ex)
            {
                Debug.LogError($"5. JSON parsing error: {ex.Message}");
                throw new ArgumentException("Invalid JSON in claim", ex);
            }
        }

        /// <summary>
        /// Extracts and deserializes a claim value
        /// </summary>
        /// <typeparam name="T">The type to deserialize the claim value to</typeparam>
        /// <param name="claim">The claim object containing the encoded value and position</param>
        /// <param name="claimName">The expected name of the claim</param>
        /// <returns>The deserialized claim value</returns>
        /// <exception cref="ArgumentException">Thrown when the claim name doesn't match or the claim is invalid</exception>
        //public static T ExtractClaimValue<T>(Claim claim, string claimName)
        //{
        //    string extendedClaim = DecodeBase64URL(claim.value, claim.indexMod4);
        //    Debug.Log("EXTENDED CLAIM: " + extendedClaim);
        //    var (name, value) = VerifyExtendedClaim(extendedClaim);

        //    if (name != claimName)
        //        throw new ArgumentException($"Invalid field name: found {name} expected {claimName}");

        //    return JsonUtility.FromJson<T>(value);
        //}

        public static T ExtractClaimValue<T>(Claim claim, string claimName) where T : class
        {
            if (claim == null)
                throw new ArgumentNullException(nameof(claim));
            if (string.IsNullOrEmpty(claimName))
                throw new ArgumentException("Claim name cannot be null or empty", nameof(claimName));

            string extendedClaim = DecodeBase64URL(claim.value, claim.indexMod4);
            var (name, value) = VerifyExtendedClaim(extendedClaim);

            if (name != claimName)
                throw new ArgumentException($"Invalid field name: found {name} expected {claimName}");

            T result = JsonUtility.FromJson<T>(value);
            if (result == null)
                throw new JwtDecodingException($"Failed to deserialize claim value to type {typeof(T).Name}");

            return result;
        }
    }

    public class JwtDecodingException : Exception
    {
        public JwtDecodingException(string message) : base(message) { }
        public JwtDecodingException(string message, Exception inner) : base(message, inner) { }
    }
}