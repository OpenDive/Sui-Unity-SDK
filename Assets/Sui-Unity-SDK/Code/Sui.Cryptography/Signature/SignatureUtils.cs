//
//  SignatureUtils.cs
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
using Chaos.NaCl;
using System.Linq;
using Sui.Rpc.Client;
using Sui.Utilities;
using static Sui.Cryptography.SignatureUtils;

namespace Sui.Cryptography
{
    /// <summary>
    /// Implements signature definitions from this codebase:
    /// https://github.com/MystenLabs/sui/blob/main/sdk/typescript/src/cryptography/signature.ts
    /// </summary>
    public static class SignatureUtils
    {
        /// <summary>
        /// Signature schemes supported by Sui.
        /// </summary>
        public enum SignatureScheme
        {
            ED25519,
            Secp256k1,
            Secp256r1,
            MultiSig,
            Zk
        }

        /// <summary>
        /// Types of inten scope flags.
        /// An intent is a domain separator.
        /// </summary>
        public enum IntentScope
        {
            TransactionData     = 0,
            TransactionEffects  = 1,
            CheckpointSummary   = 2,
            PersonalMessage     = 3,
        }

        /// <summary>
        /// The app id.
        /// </summary>
        public enum AppId
        {
            Sui = 0
        }

        /// <summary>
        /// The intent version.
        /// </summary>
        public enum IntentVersion
        {
            V0 = 0
        }

        /// <summary>
        /// Generates an `Intent` based on the provided `IntentScope`.
        /// </summary>
        /// <param name="scope">An `IntentScope` instance defining the scope of the intent.</param>
        /// <returns>An `Intent` instance representing the generated intent.</returns>
        public static byte[] CreateIntentWithScope (IntentScope scope)
            => new byte[] { (byte)scope, (byte)IntentVersion.V0, (byte)AppId.Sui };

        /// <summary>
        /// Appends the provided message to the intent data and returns the combined `byte[]`.
        /// </summary>
        /// <param name="scope">An `IntentScope` instance defining the scope of the intent.</param>
        /// <param name="message">A `byte` array instance representing the message to be appended to the intent data.</param>
        /// <returns>A `byte` array instance representing the combined intent and message.</returns>
        public static byte[] CreateMessageWithIntent(IntentScope scope, byte[] message)
        {
            byte[] intent = SignatureUtils.CreateIntentWithScope(scope);
            byte[] intent_message = new byte[intent.Length + message.Length];

            Array.Copy(intent, intent_message, intent.Length);
            Array.Copy(message, 0, intent_message, intent.Length, message.Length);

            return intent_message;
        }

        // TODO: Implement the following functions
        public static string ToSerializedSignature(SignatureBase signature, PublicKeyBase pub_key)
            => throw new NotImplementedException();

        public static SignatureBase ToSerializedSignature(string serializedSignature)
            => throw new NotImplementedException();

        /// <summary>
        /// Create a serialized signature from:
        /// signature scheme, signature, and public key.
        /// </summary>
        /// <param name="serialized_signature_input">The `SerializedSignatureInput` object that represents the inputted signature values.</param>
        /// <returns>The serialized signature as a string.</returns>
        public static SuiResult<string> ToSerializedSignature(SerializeSignatureInput serialized_signature_input)
        {
            SuiPublicKeyBase public_key = serialized_signature_input.PublicKey;
            byte[] signature = serialized_signature_input.Signature;
            SignatureScheme signature_scheme = serialized_signature_input.SignatureScheme;

            if (public_key == null)
                return new SuiResult<string>(null, new SuiError(0, "Public Key is null", null));

            byte[] public_key_bytes = public_key.KeyBytes;
            byte[] serialized_signature = new byte[1 + signature.Length + public_key.KeyLength];

            serialized_signature[0] = SignatureSchemeToFlag.GetFlag(signature_scheme);
            Array.Copy(signature, 0, serialized_signature, 1, signature.Length);
            Array.Copy(public_key_bytes, 0, serialized_signature, 1 + signature.Length, public_key_bytes.Length);

            return new SuiResult<string>(CryptoBytes.ToBase64String(serialized_signature));
        }

        /// <summary>
        /// Decodes a serialized signature into its constituent components:
        /// the signature scheme, the actual signature, and the public key
        /// </summary>
        /// <param name="serialize_signature">Sui signature as a base64 string.</param>
        /// <returns>A `ParsedSignatureOutput` object that represents the parsed signature.</returns>
        public static SuiResult<ParsedSignatureOutput> ParseSerializedSignature(string serialize_signature)
        {
            byte[] bytes = Convert.FromBase64String(serialize_signature);
            SignatureScheme signature_scheme = SignatureFlagToScheme.GetScheme(bytes[0]);

            if (signature_scheme == SignatureScheme.MultiSig)
                return new SuiResult<ParsedSignatureOutput>
                (
                    null,
                    new SuiError
                    (
                        0,
                        "Unable to parse a multi-signature. (not implemented yet)",
                        serialize_signature
                    )
                );

            if (signature_scheme == SignatureScheme.Zk)
                return new SuiResult<ParsedSignatureOutput>
                (
                    null,
                    new SuiError
                    (
                        0,
                        "Unable to parse a zk signature. (not implemented yet)",
                        serialize_signature
                    )
                );

            SuiResult<int> size_result = SignatureSchemeToSize.GetSize(signature_scheme);

            if (size_result.Error != null)
                return new SuiResult<ParsedSignatureOutput>(null, (SuiError)size_result.Error);

            int size = size_result.Result;

            // TODO: Verifiy is this is correct. See TS implementation
            byte[] signature = bytes.Skip(1).Take(bytes.Length - 1 - size).ToArray();
            byte[] public_key = bytes.Skip(1 + signature.Length).ToArray();

            return new SuiResult<ParsedSignatureOutput>
            (
                new ParsedSignatureOutput
                (
                    serialize_signature,
                    bytes,
                    signature_scheme,
                    signature,
                    public_key
                )
            );
        }
    }

    /// <summary>
    /// Utility class to map a signature schem to the corresponding byte flag Sui uses.
    /// </summary>
    public static class SignatureSchemeToFlag
    {
        /// A dictionary mapping the names of signature schemes to their associated flags.
        private static readonly Dictionary<SignatureScheme, byte> _signature_scheme_to_flag = new
            Dictionary<SignatureScheme, byte>
        {
            { SignatureScheme.ED25519,      0x00 },
            { SignatureScheme.Secp256k1,    0x01 },
            { SignatureScheme.Secp256r1,    0x02 },
            { SignatureScheme.MultiSig,     0x03 },
            { SignatureScheme.Zk,           0x05 }
        };

        /// <summary>
        /// Represents the flag for the Ed25519 signature scheme.
        /// </summary>
        public static byte ED25519
            => SignatureSchemeToFlag._signature_scheme_to_flag[SignatureScheme.ED25519];

        /// <summary>
        /// Represents the flag for the SECP256K1 signature scheme.
        /// </summary>
        public static byte Secp256k1
            => SignatureSchemeToFlag._signature_scheme_to_flag[SignatureScheme.Secp256k1];

        /// <summary>
        /// Represents the flag for the SECP256R1 (NIST-P256) signature scheme.
        /// </summary>
        public static byte Secp256r1
            => SignatureSchemeToFlag._signature_scheme_to_flag[SignatureScheme.Secp256r1];

        /// <summary>
        /// Represents the flag for the multi-signature scheme.
        /// </summary>
        public static byte MultiSig
            => SignatureSchemeToFlag._signature_scheme_to_flag[SignatureScheme.MultiSig];

        /// <summary>
        /// Represents the flag for the zkLogin signature scheme.
        /// </summary>
        public static byte Zk
            => SignatureSchemeToFlag._signature_scheme_to_flag[SignatureScheme.Zk];

        /// <summary>
        /// Returns the byte flag that matches the signature scheme.
        /// </summary>
        /// <param name="signature_scheme">The signature scheme.</param>
        /// <returns></returns>
        public static byte GetFlag(SignatureScheme signature_scheme)
            => SignatureSchemeToFlag._signature_scheme_to_flag[signature_scheme];
    }

    /// <summary>
    /// Utility class to map a signature scheme to it's signature byte size.
    /// Presently the Sui TypeScript SDK doesn't support Multisig and Zk signature-scheme-to-size
    /// </summary>
    public static class SignatureSchemeToSize
    {
        /// A dictionary mapping the names of signature schemes to their associated byte size.
        private static readonly Dictionary<SignatureScheme, int> _signature_scheme_to_size =
            new Dictionary<SignatureScheme, int>
        {
            { SignatureScheme.ED25519,      32 },
            { SignatureScheme.Secp256k1,    33 },
            { SignatureScheme.Secp256r1,    33 },
            { SignatureScheme.MultiSig,     32 },
            { SignatureScheme.Zk,           32 }
        };

        /// <summary>
        /// Represents the size of the ED25519 signature in bytes.
        /// </summary>
        public static int ED25519
            => SignatureSchemeToSize._signature_scheme_to_size[SignatureScheme.ED25519];

        /// <summary>
        /// Represents the size of the SECP256K1 signature in bytes.
        /// </summary>
        public static int Secp256k1
            => SignatureSchemeToSize._signature_scheme_to_size[SignatureScheme.Secp256k1];

        /// <summary>
        /// Represents the size of the SECP256R1 (NIST-P256) signature in bytes.
        /// </summary>
        public static int Secp256r1
            => SignatureSchemeToSize._signature_scheme_to_size[SignatureScheme.Secp256r1];

        /// <summary>
        /// Represents the size of the multi-signature signature in bytes.
        /// </summary>
        public static int MultiSig
            => SignatureSchemeToSize._signature_scheme_to_size[SignatureScheme.MultiSig];

        /// <summary>
        /// Represents the size of the zkLogin signature in bytes.
        /// </summary>
        public static int Zk
            => SignatureSchemeToSize._signature_scheme_to_size[SignatureScheme.Zk];

        /// <summary>
        /// Returns the size of signature based on particular signature scheme.
        /// </summary>
        /// <param name="signature_scheme">The signature scheme.</param>
        /// <returns>The `int` value representing the size of the signature scheme.</returns>
        public static SuiResult<int> GetSize(SignatureScheme signature_scheme)
            =>  SignatureSchemeToSize._signature_scheme_to_size.ContainsKey(signature_scheme) == false ?
                new SuiResult<int>(-1, new SuiError(0, "Unsupported signature scheme", signature_scheme)) :
                new SuiResult<int>(SignatureSchemeToSize._signature_scheme_to_size[signature_scheme], null);
    }

    /// <summary>
    /// Utility class to map a signature byte flag to it's signature scheme name.
    /// </summary>
    public static class SignatureFlagToScheme
    {
        /// A dictionary mapping the signature byte flags to their corresponding signature scheme name.
        public static readonly Dictionary<byte, SignatureScheme> _signature_flag_to_scheme =
            new Dictionary<byte, SignatureScheme>
        {
            { 0x00, SignatureScheme.ED25519 },
            { 0x01, SignatureScheme.Secp256k1 },
            { 0x02, SignatureScheme.Secp256r1 },
            { 0x03, SignatureScheme.MultiSig },
            { 0x05, SignatureScheme.Zk }
        };

        /// <summary>
        /// Represents the ED25519 Signature Scheme enum value.
        /// </summary>
        public static SignatureScheme FLAG_0x00
            => SignatureFlagToScheme._signature_flag_to_scheme[0x00];

        /// <summary>
        /// Represents the SECP256K1 Signature Scheme enum value.
        /// </summary>
        public static SignatureScheme FLAG_0x01
            => SignatureFlagToScheme._signature_flag_to_scheme[0x01];

        /// <summary>
        /// Represents the SECP256R1 (NIST-P256) Signature Scheme enum value.
        /// </summary>
        public static SignatureScheme FLAG_0x02
            => SignatureFlagToScheme._signature_flag_to_scheme[0x02];

        /// <summary>
        /// Represents the multi-signature Signature Scheme enum value.
        /// </summary>
        public static SignatureScheme FLAG_0x03
            => SignatureFlagToScheme._signature_flag_to_scheme[0x03];

        /// <summary>
        /// Represents the zkLogin Signature Scheme enum value.
        /// </summary>
        public static SignatureScheme FLAG_0x05
            => SignatureFlagToScheme._signature_flag_to_scheme[0x05];

        /// <summary>
        /// Returns a signature scheme name from a byte flag.
        /// </summary>
        /// <param name="flag_byte">The byte flag.</param>
        /// <returns>The `SignatureScheme` object.</returns>
        public static SignatureScheme GetScheme(byte flag_byte)
            => SignatureFlagToScheme._signature_flag_to_scheme[flag_byte];
    }
}